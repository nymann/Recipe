using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using ExileCore;
using ExileCore.PoEMemory.Elements.InventoryElements;
using ExileCore.Shared;
using ExileCore.Shared.Enums;
using SharpDX;

namespace Recipe
{
    public class Recipe : BaseSettingsPlugin<Core.Config>
    {
        const int MaxShownSidebarStashTabs = 31;
        public int _stashCount = 45;
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
                }
                else
                {
                    // Teardown
                    DebugWindow.LogMsg($"{Name}: disabled", 5);
                }
            };
            Settings.Hotkey.OnValueChanged += () => { Input.RegisterKey(Settings.Hotkey); };

            return true;
        }

        public override void Render()
        {
            if (_coroutineWorker != null && _coroutineWorker.IsDone)
            {
                Input.KeyUp(Keys.LControlKey);
                _coroutineWorker = null;
            }

            var uiTabsOpened = GameController.Game.IngameState.IngameUi.InventoryPanel.IsVisible &&
                               GameController.Game.IngameState.IngameUi.StashElement.IsVisibleLocal;

            if (!uiTabsOpened && _coroutineWorker != null && !_coroutineWorker.IsDone)
            {
                _coroutineWorker = ExileCore.Core.ParallelRunner.FindByName(CoroutineName);
                _coroutineWorker?.Done();
            }

            if (_coroutineWorker != null && _coroutineWorker.Running && _debugTimer.ElapsedMilliseconds > 15000)
            {
                _coroutineWorker?.Done();
                _debugTimer.Restart();
                _debugTimer.Stop();
            }

            if (!Settings.Hotkey.PressedOnce()) return;
            _coroutineWorker = new Coroutine(ChaosRecipe(), this, CoroutineName);
            ExileCore.Core.ParallelRunner.Run(_coroutineWorker);
        }

        private IEnumerator ChaosRecipe()
        {
            _debugTimer.Restart();
            var uiTabsOpened = GameController.Game.IngameState.IngameUi.InventoryPanel.IsVisible &&
                               GameController.Game.IngameState.IngameUi.StashElement.IsVisibleLocal;
            Input.KeyDown(Keys.LControlKey);
            if (uiTabsOpened)
            {
                DebugWindow.LogMsg("Getting item from stash", 5, Color.Green);
                var constant = Settings.TwoSetsAtOnce.Value ? 2 : 1;
                yield return Delay(5);
                yield return SwitchToTabAndGrab(Settings.BodyArmor.Value, constant);
                yield return SwitchToTabAndGrab(Settings.Weapons.Value, 2 * constant);
                yield return SwitchToTabAndGrab(Settings.Helmets.Value, constant);
                yield return SwitchToTabAndGrab(Settings.Gloves.Value, constant);
                yield return SwitchToTabAndGrab(Settings.Boots.Value, constant);
                yield return SwitchToTabAndGrab(Settings.Rings.Value, 2 * constant);
                yield return SwitchToTabAndGrab(Settings.Amulets.Value, constant);
            }
            else
            {
                DebugWindow.LogMsg("Selling items to vendor", 5, Color.Gold);
                yield return SellSetToVendor();
            }

            _coroutineWorker = ExileCore.Core.ParallelRunner.FindByName(CoroutineName);
            _coroutineWorker?.Done();
            Input.KeyUp(Keys.LControlKey);
            _debugTimer.Restart();
            _debugTimer.Stop();
        }

        private IEnumerator SwitchToTabAndGrab(int tabIndex, int grabCount = 1)
        {
            yield return SwitchToTabViaDropdownMenu(tabIndex);
            yield return Delay(5);
            yield return GrabRandomItemFromVisibleStash(grabCount);
        }

        private IEnumerator GrabRandomItemFromVisibleStash(int grabCount = 1)
        {
            var visibleStash = GameController.IngameState.IngameUi.StashElement.VisibleStash;
            var visibleInventoryItems = visibleStash?.VisibleInventoryItems;
            for (var i = 0; i < grabCount; i++)
            {
                var visibleInventoryItem = visibleInventoryItems?[i];
                yield return Delay(10);
                if (visibleInventoryItem == null)
                {
                    yield break;
                }

                yield return ClickElement(visibleInventoryItem.GetClientRect().Center);
            }
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
                yield return Delay(3);
                VerticalScroll(scrollUp: clickable, clicks: clicks);
                yield return Delay(3);
            }

            DebugWindow.LogMsg($"Stashie: Moving to tab '{tabIndex}'.", 3, Color.LightGray);
            yield return Click();
        }

        private IEnumerator SwitchToTabViaDropdownMenu(int tabIndex)
        {
            if (!DropDownMenuIsVisible())
            {
                yield return OpenDropDownMenu();
            }

            yield return ClickDropDownMenuStashTabLabel(tabIndex);
        }


        public IEnumerator SellSetToVendor(int callCount = 1)
        {
            var npcTradingWindow = GameController.Game.IngameState.IngameUi.SellWindow;

            if (npcTradingWindow == null || !npcTradingWindow.IsVisible)
            {
                DebugWindow.LogMsg("Error: npcTradingWindow is not visible (opened)!", 5);
                yield break;
            }
            var items = GameController.Game.IngameState.IngameUi.InventoryPanel[InventoryIndex.PlayerInventory].VisibleInventoryItems;
            DebugWindow.LogMsg($"There should be 9 items in inventory there are '{items.Count}'");
            foreach (var normalInventoryItem in items)
            {
                DebugWindow.LogMsg("Clicking a normal inventory item.");
                yield return Delay(20);
                yield return ClickElement(normalInventoryItem.GetClientRect().Center);
                yield return Delay(20);
            }
            var playerOfferItems = npcTradingWindow.OtherOffer;
        }
    }
}