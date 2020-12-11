using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExileCore;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.Elements.InventoryElements;
using ExileCore.PoEMemory.MemoryObjects;
using Recipe.Model;

namespace Recipe.Controller
{
    public class ChaosRecipe : Recipe
    {
        private GameController _gameController;
        private Settings _settings;
        private List<TabIndexItemTypes> _tabIndexItemTypes;
        //private List<NormalInventoryItem> Amulets =>

        public ChaosRecipe(GameController gameController, Settings settings) :
            base(gameController, settings)
        {
            settings.Amulets.OnValueChanged += UpdateTabIndexItemTypes;
            settings.Belts.OnValueChanged += UpdateTabIndexItemTypes;
            settings.Boots.OnValueChanged += UpdateTabIndexItemTypes;
            settings.Gloves.OnValueChanged += UpdateTabIndexItemTypes;
            settings.Helmets.OnValueChanged += UpdateTabIndexItemTypes;
            settings.Rings.OnValueChanged += UpdateTabIndexItemTypes;
            settings.Weapons.OnValueChanged += UpdateTabIndexItemTypes;
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
                new TabIndexItemTypes(ItemType.Ring, _settings.Rings.Value)
            };
        }

        private IEnumerator SellSetToVendor()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Checks if the currently loaded "_currentSet" is present in our inventory
        ///     and ready to be sold.
        /// </summary>
        /// <returns></returns>
        public bool InInventory()
        {
            var items = PlayerInventory.VisibleInventoryItems;
            foreach (var recipeItem in CurrentSet)
                if (!items.Contains(recipeItem.NormalInventoryItem))
                    return false;

            return true;
        }

        /// <summary>
        ///     Assuming we allow only the most space efficient Set for now.
        ///     Calculate the required inventory space (as of patch 3.12 it's 29
        ///     Cells) and check if we can numerically fit this set.
        /// </summary>
        /// <returns></returns>
        public bool CanFit(IEnumerable<NormalInventoryItem> inventoryItems)
        {
            /*
             * TODO(Find a library for a fitting algorithm that can check if the
             * set can fit in geometrical and not just by numerical free space;
             * im too lazy to implement that myself)
            */
            //2x(3x1 or 3x2)+3x(2x2)+1x(2x1)+3x(1x1): 2x1hand/1x2Hand,Body,Boots+Helm+Gloves,Belt,2xRings+1+Amulet
            const byte minSetSize = 29;
            var inventoryCells = 5 * 12; //5 rows, 12 columns
            foreach (var item in inventoryItems)
            {
                inventoryCells -= item.ItemWidth * item.ItemHeight;
                if (inventoryCells <= minSetSize) return false;
            }

            return true;
        }

        public bool AvailableInStash()
        {
            //var lowLvl = NormalInventoryItem.Item.GetComponent<Mods>().ItemLevel < 75;    throw new NotImplementedException();
        }

        private RecipeItem FinditemInStash(
            IEnumerable<NormalInventoryItem> inventoryItems)
        {
            return null;
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

            throw new NotImplementedException();
        }

        private static NormalInventoryItem FindRecipeItemInStashTab(
            RecipeItem recipeItem, List<NormalInventoryItem> inventoryItems)
        {
            var inventoryItem = inventoryItems.Find(x =>
                Equals(x, recipeItem.NormalInventoryItem));
            if (inventoryItem == null) throw new Exception("Meme");

            return inventoryItem;
        }

        private Inventory GetStashInventory(int stashIndex)
        {
            var inventory = StashElement.GetStashInventoryByIndex(stashIndex);
            if (inventory == null) throw new Exception("Inventory is null");

            return inventory;
        }

        private int GetStashIndexOfRecipeItem(RecipeItem recipeItem)
        {
            var stashIndex =
                _tabIndexItemTypes.FirstOrDefault(x =>
                    x.ItemType == recipeItem.ItemType);
            if (stashIndex == null)
                throw new Exception(
                    "Couldn't find stash index of item type.");

            return stashIndex.Index;
        }
    }
}