using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExileCore;
using ExileCore.PoEMemory.Elements;
using ExileCore.PoEMemory.Elements.InventoryElements;
using ExileCore.PoEMemory.MemoryObjects;

namespace Recipe.Model
{
    public class ChaosRecipe : IRecipe
    {
        private readonly GameController _gameController;
        private readonly Settings _settings;
        public List<RecipeItem> CurrentSet;
        private List<TabIndexItemTypes> _tabIndexItemTypes;

        private StashElement _stashElement =>
            _gameController.Game.IngameState.IngameUi.StashElement;

        public ChaosRecipe(GameController gameController, Settings settings)
        {
            _gameController = gameController;
            _settings = settings;

            _settings.Amulets.OnValueChanged += UpdateTabIndexItemTypes;
            _settings.Belts.OnValueChanged += UpdateTabIndexItemTypes;
            _settings.Boots.OnValueChanged += UpdateTabIndexItemTypes;
            _settings.Gloves.OnValueChanged += UpdateTabIndexItemTypes;
            _settings.Helmets.OnValueChanged += UpdateTabIndexItemTypes;
            _settings.Rings.OnValueChanged += UpdateTabIndexItemTypes;
            _settings.Weapons.OnValueChanged += UpdateTabIndexItemTypes;
        }

        private void UpdateTabIndexItemTypes(object sender, int i)
        {
            _tabIndexItemTypes = new List<TabIndexItemTypes>
            {
                new TabIndexItemTypes(ItemType.Amulet, _settings.Amulets.Value),
                new TabIndexItemTypes(ItemType.Belt, _settings.Belts.Value),
                new TabIndexItemTypes(ItemType.Gloves, _settings.Gloves.Value),
                new TabIndexItemTypes(ItemType.Helmet, _settings.Helmets.Value),
                new TabIndexItemTypes(ItemType.OneHandedWeapon,
                    _settings.Weapons.Value),
                new TabIndexItemTypes(ItemType.TwoHandedWeapon,
                    _settings.Weapons.Value),
                new TabIndexItemTypes(ItemType.Ring, _settings.Rings.Value),
            };
        }

        public IEnumerator Go()
        {
            if (InInventory())
            {
                SellSetToVendor();
                yield break;
            }

            if (!CanFit())
            {
                yield break;
            }

            if (!AvailableInStash())
            {
                yield break;
            }

            yield return PickUpFromStash();
            yield return SellSetToVendor();
        }

        private IEnumerator SellSetToVendor()
        {
            throw new System.NotImplementedException();
        }

        public bool InInventory()
        {
            throw new System.NotImplementedException();
        }

        public bool CanFit()
        {
            throw new System.NotImplementedException();
        }

        public bool AvailableInStash()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator PickUpFromStash()
        {
            var orderedRecipeSet = CurrentSet.OrderBy(x => x.ItemType);
            foreach (var recipeItem in orderedRecipeSet)
            {
                var stashIndex = GetStashIndexOfRecipeItem(recipeItem);
                // GoToTab(stashIndex)
                var inventoryItems = GetStashInventory(
                        stashIndex).VisibleInventoryItems.ToList();
                var inventoryItem =
                    FindRecipeItemInStashTab(recipeItem, inventoryItems);
            }

            throw new System.NotImplementedException();
        }

        private NormalInventoryItem FindRecipeItemInStashTab(
            RecipeItem recipeItem, List<NormalInventoryItem> inventoryItems)
        {
            var inventoryItem = inventoryItems.Find(x =>
                Equals(x, recipeItem.NormalInventoryItem));
            if (inventoryItem == null)
            {
                throw new Exception("Meme");
            }

            return inventoryItem;
        }

        private Inventory GetStashInventory(int stashIndex)
        {
            var inventory = _stashElement.GetStashInventoryByIndex(stashIndex);
            if (inventory == null)
            {
                throw new Exception("Inventory is null");
            }

            return inventory;
        }

        private int GetStashIndexOfRecipeItem(RecipeItem recipeItem)
        {
            var stashIndex =
                _tabIndexItemTypes.FirstOrDefault(x =>
                    x.ItemType == recipeItem.ItemType);
            if (stashIndex == null)
            {
                throw new Exception(
                    "Couldn't find stash index of item type.");
            }

            return stashIndex.Index;
        }
    }
}