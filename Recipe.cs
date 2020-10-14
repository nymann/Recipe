using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using ExileCore;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.Elements.InventoryElements;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared;
using ExileCore.Shared.Enums;
using SharpDX;
using Stack = ExileCore.PoEMemory.Components.Stack;

namespace Recipe
{
    public class Recipe : BaseSettingsPlugin<Settings.Config>
    {
        const int MaxShownSidebarStashTabs = 31;
        public int _stashCount;
        private const string CoroutineName = "Recipe Main Routine";
        private readonly Stopwatch _debugTimer = new Stopwatch();
        private Coroutine _coroutineWorker;
        
        public Recipe()
        {
            Name = "Recipe";
        }

        public override bool Initialise()
        {
            Settings.Enable.OnValueChanged += (sender, enabled) =>
            {
                if (enabled)
                {
                    // Setup
                    DebugWindow.LogMsg($"{Name}: enabled", 5);
                    _stashCount = GameController.IngameState.IngameUi.StashElement.AllStashNames.Count;
                }
                else
                {
                    // Teardown
                    DebugWindow.LogMsg($"{Name}: disabled", 5);
                }
            };
            Settings.StartHotkey.OnValueChanged += () => { Input.RegisterKey(Settings.StartHotkey); };

            return true;
        }
        public override Job Tick()
        {
            //test if anything can even happen right now
            if (!CheckRequirements()) return null;
            if (Settings.StartHotkey.PressedOnce())
            {
                if(_coroutineWorker == null)
                {
                    _coroutineWorker = new Coroutine(ChaosRecipe(), this, "ChaosRecipe");
                    Core.ParallelRunner.Run(_coroutineWorker);
                }
                else 
                {
                    //if a Procedure(chaos recipe) is already running and the hotkey is pressed before finished, Pause the procedure. "Im not sure if this works but will try, probably needs more to work like that"
                    if (_coroutineWorker.Running) 
                    {
                        _coroutineWorker.Pause();
                        Input.KeyUp(Keys.LControlKey); //must find better place for this
                    }
                    else
                    {
                        Input.KeyDown(Keys.LControlKey);
                        _coroutineWorker.Resume();
                    }
                }
            }

            /*
            var uiTabsOpened = GameController.Game.IngameState.IngameUi.InventoryPanel.IsVisible &&
                               GameController.Game.IngameState.IngameUi.StashElement.IsVisibleLocal;*/

            /*if (!uiTabsOpened && _coroutineWorker != null && !_coroutineWorker.IsDone)
            {
                _coroutineWorker = ExileCore.Core.ParallelRunner.FindByName(CoroutineName);
                _coroutineWorker?.Done();
            }*/
            if (_coroutineWorker != null && _coroutineWorker.Running && _debugTimer.ElapsedMilliseconds > 15000)
            {
                _coroutineWorker?.Done();
                _debugTimer.Restart();
                _debugTimer.Stop();
                Input.KeyUp(Keys.LControlKey);
            }
            */
        }

        private bool CheckRequirements()
        {
            //ToDo: ideally a check if we even have a full set available for vendoring -> Setting: minimum Sets for vendoring. Only vendor recipes when >=2 e.g. Sets are available
            var uiTabsOpened = GameController.Game.IngameState.IngameUi.InventoryPanel.IsVisible &&
                               GameController.Game.IngameState.IngameUi.StashElement.IsVisibleLocal;
            return (GameController.Area.CurrentArea.IsHideout || GameController.Area.CurrentArea.IsTown) && uiTabsOpened;
        }

        public override void Render()
        {
            //use Render only for actual rendering stuff
            //otherwise we use Tick()
            
        }

        private IEnumerator ChaosRecipe()
        {
            _debugTimer.Restart();
            var uiTabsOpened = GameController.Game.IngameState.IngameUi.InventoryPanel.IsVisible &&
                               GameController.Game.IngameState.IngameUi.StashElement.IsVisibleLocal;
            if (uiTabsOpened)
            {
                DebugWindow.LogMsg("Getting item from stash", 5, Color.Green);
                var twoSets = Settings.TwoSetsAtOnce.Value ? 2 : 1;
                yield return new WaitTime(5  +Settings.ExtraDelay);   //if i have to check what the delay function even does, the code is not more readable imo
                yield return SwitchToTabAndGrab(Settings.BodyArmor.Value, twoSets);
                yield return SwitchToTabAndGrab(Settings.Weapons.Value, 2 * twoSets, true);
                yield return SwitchToTabAndGrab(Settings.Helmets.Value, twoSets);
                yield return SwitchToTabAndGrab(Settings.Gloves.Value, twoSets);
                yield return SwitchToTabAndGrab(Settings.Boots.Value, twoSets);
                yield return SwitchToTabAndGrab(Settings.Rings.Value, 2 * twoSets);
                yield return SwitchToTabAndGrab(Settings.Amulets.Value, twoSets);
            }
            else
            {
                DebugWindow.LogMsg("Selling items to vendor", 5, Color.Gold);
                yield return SellSetToVendor();
            }

            yield return Delay(30);

            _coroutineWorker = ExileCore.Core.ParallelRunner.FindByName(CoroutineName);
            _coroutineWorker?.Done();
            Input.KeyUp(Keys.LControlKey);
            _debugTimer.Restart();
            _debugTimer.Stop();
        }

        private IEnumerator SwitchToTabAndGrab(int tabIndex, int grabCount = 1, bool isWeapon = false)
        {
            yield return Delay(5);
            yield return SwitchToTabViaDropdownMenu(tabIndex);
            yield return Delay(5);
            var visibleStash = GameController.IngameState.IngameUi.StashElement.VisibleStash.Address;
            var stash = GameController.IngameState.IngameUi.StashElement.GetStashInventoryByIndex(tabIndex).Address;
            if (visibleStash != stash)
            {
                DebugWindow.LogMsg("Stash was not the requested.");
                yield return Delay(20);
            }

            if (isWeapon)
            {
                yield return GetWeaponsFromStash();
            }
            else
            {
                yield return GrabRandomItemFromVisibleStash(grabCount);
            }
        }

        private IEnumerator GrabRandomItemFromVisibleStash(int grabCount = 1)
        {
            var visibleStash = GameController.IngameState.IngameUi.StashElement.VisibleStash;
            var visibleInventoryItems = visibleStash?.VisibleInventoryItems;
            for (var i = 0; i < grabCount; i++)
            {
                var visibleInventoryItem = visibleInventoryItems?[i];
                Input.KeyDown(Keys.LControlKey);
                yield return Delay(10);
                if (visibleInventoryItem == null)
                {
                    DebugWindow.LogMsg("Visible inventory item was null. This should not happen.");
                    Input.KeyUp(Keys.LControlKey);
                    yield break;
                }

                yield return ClickElement(visibleInventoryItem.GetClientRect().Center);
            }

            Input.KeyUp(Keys.LControlKey);
        }

        private IEnumerator GetWeaponsFromStash()
        {
            var visibleStash = GameController.IngameState.IngameUi.StashElement.VisibleStash;
            var visibleInventoryItems = visibleStash?.VisibleInventoryItems;
            var setsToGrab = Settings.TwoSetsAtOnce.Value ? 2 : 1;
            var items = GetBowFromInventory(visibleInventoryItems, setsToGrab);
            if (items.Count != setsToGrab)
            {
                var numberOfOneHandsToGrab = (setsToGrab - items.Count) * 2;
                var oneHandedWeapons = GetOneHandFromInventory(visibleInventoryItems, numberOfOneHandsToGrab);
                if (oneHandedWeapons.Count != numberOfOneHandsToGrab)
                {
                    DebugWindow.LogMsg("Not enough weapons to fulfill this request.");
                    yield break;
                }
                items.AddRange(oneHandedWeapons);
            }
            

            foreach (var visibleInventoryItem in items)
            {
                Input.KeyDown(Keys.LControlKey);
                yield return Delay(10);
                yield return ClickElement(visibleInventoryItem.GetClientRect().Center);
            }

            Input.KeyUp(Keys.LControlKey);
        }

        private List<NormalInventoryItem> GetBowFromInventory(IEnumerable<NormalInventoryItem> inventoryItems, int count = 1)
        {
            var a = new List<NormalInventoryItem>();
            foreach (var x in inventoryItems)
            {
                if (GameController.Game.Files.BaseItemTypes.Translate(x.Item.Path).ClassName != "Bow") continue;

                a.Add(x);
                if (a.Count == count) return a;
            }

            return a;
        }

        private List<NormalInventoryItem> GetOneHandFromInventory(IEnumerable<NormalInventoryItem> inventoryItems, int count = 2)
        {
            var a = new List<NormalInventoryItem>();
            foreach (var x in inventoryItems)
            {
                if (x.ItemWidth != 1 || x.ItemHeight != 3) continue;

                a.Add(x);
                if (a.Count == count) return a;
            }

            return a;
        }


        private bool DropDownMenuIsVisible()
        {
            return GameController.Game.IngameState.IngameUi.StashElement.ViewAllStashPanel.IsVisible;
        }

        private IEnumerator OpenDropDownMenu()
        {
            var button = GameController.Game.IngameState.IngameUi.StashElement.ViewAllStashButton.GetClientRect();
            yield return ClickElement(button.Center);
            while (!DropDownMenuIsVisible())
            {
                yield return Delay(1);
            }
        }

        private IEnumerator ClickElement(Vector2 pos, MouseButtons mouseButton = MouseButtons.Left)
        {
            MoveMouseToElement(pos);
            yield return Delay(20);
            yield return Click(mouseButton);
        }

        private IEnumerator Click(MouseButtons mouseButton = MouseButtons.Left)
        {
            Input.Click(mouseButton);
            yield return Delay();
        }

        private void MoveMouseToElement(Vector2 pos)
        {
            Input.SetCursorPos(pos + GameController.Window.GetWindowRectangle().TopLeft);
        }

        private IEnumerator Delay(int ms = 0)
        {
            yield return new WaitTime(Settings.ExtraDelay.Value + ms);
        }

        private static bool StashLabelIsClickable(int index)
        {
            return index + 1 < MaxShownSidebarStashTabs;
        }

        private bool SliderPresent()
        {
            return _stashCount > MaxShownSidebarStashTabs;
        }

        private static void VerticalScroll(bool scrollUp, int clicks)
        {
            const int wheelDelta = 120;
            if (scrollUp)
                WinApi.mouse_event(Input.MOUSE_EVENT_WHEEL, 0, 0, clicks * wheelDelta, 0);
            else
                WinApi.mouse_event(Input.MOUSE_EVENT_WHEEL, 0, 0, -(clicks * wheelDelta), 0);
        }

        private IEnumerator ClickDropDownMenuStashTabLabel(int tabIndex)
        {
            var dropdownMenu = GameController.Game.IngameState.IngameUi.StashElement.ViewAllStashPanel;
            var stashTabLabels = dropdownMenu.GetChildAtIndex(1);
            _stashCount = (int) GameController.Game.IngameState.IngameUi.StashElement.TotalStashes;

            //if the stash tab index we want to visit is less or equal to 30, then we scroll all the way to the top.
            //scroll amount (clicks) should always be (stash_tab_count - 31);
            //TODO(if the guy has more than 31*2 tabs and wants to visit stash tab 32 fx, then we need to scroll all the way up (or down) and then scroll 13 clicks after.)

            var clickable = StashLabelIsClickable(tabIndex);
            // we want to go to stash 32 (index 31).
            // 44 - 31 = 13
            // 31 + 45 - 44 = 30
            // MaxShownSideBarStashTabs + _stashCount - tabIndex = index
            var index = clickable ? tabIndex : tabIndex - (_stashCount - 1 - (MaxShownSidebarStashTabs - 1));
            var pos = stashTabLabels.GetChildAtIndex(index).GetClientRect().Center;
            MoveMouseToElement(pos);
            if (SliderPresent())
            {
                var clicks = _stashCount - MaxShownSidebarStashTabs;
                yield return Delay(10);
                VerticalScroll(scrollUp: clickable, clicks: clicks);
                yield return Delay(3);
            }

            DebugWindow.LogMsg($"Stashie: Moving to tab '{tabIndex}'.", 3, Color.LightGray);
            yield return Click();
        }

        private IEnumerator SwitchToTabViaDropdownMenu(int tabIndex)
        {
            Input.KeyUp(Keys.LControlKey);
            if (!DropDownMenuIsVisible())
            {
                yield return OpenDropDownMenu();
            }

            yield return ClickDropDownMenuStashTabLabel(tabIndex);
        }


        public IEnumerator SellSetToVendor()
        {
            var npcTradingWindow = GameController.Game.IngameState.IngameUi?.SellWindow;

            if (npcTradingWindow == null || !npcTradingWindow.IsVisible)
            {
                DebugWindow.LogMsg("Error: npcTradingWindow is not visible (opened)!", 5);
                yield break;
            }

            var items = GameController.Game.IngameState.IngameUi.InventoryPanel[InventoryIndex.PlayerInventory]
                .VisibleInventoryItems.Where(x => x.Item.GetComponent<Mods>()?.ItemRarity == ItemRarity.Rare);
            Input.KeyDown(Keys.LControlKey);
            yield return Delay(3);
            foreach (var item in items)
            {
                yield return ClickElement(item.GetClientRect().Center);
            }

            yield return Delay(3);
            Input.KeyUp(Keys.LControlKey);


            yield return Delay(50);
            if (!VendorOfferUsChaos(npcTradingWindow))
            {
                yield break;
            }

            DebugWindow.LogMsg("Vendor is offering us Regal or Chaos, accept.");
            yield return ClickElement(npcTradingWindow.AcceptButton.GetClientRect().Center);
        }

        public bool VendorOfferUsChaos(SellWindow npcTradingWindow)
        {
            var item = npcTradingWindow.OtherOffer.Children.Skip(1).FirstOrDefault()?.AsObject<NormalInventoryItem>()
                .Item;
            if (item == null)
            {
                return false;
            }

            var itemName = GameController.Files.BaseItemTypes.Translate(item.Path).BaseName;
            var constant = Settings.TwoSetsAtOnce.Value ? 2 : 1;
            return itemName == "Chaos Orb" && item.GetComponent<Stack>().Size == constant * 2;
        }
    }
}