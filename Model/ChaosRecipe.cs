using System.Collections;
using System.Collections.Generic;
using ExileCore;
using ExileCore.PoEMemory.Elements.InventoryElements;

namespace Recipe.Model
{
    public class ChaosRecipe : IRecipe
    {
        private GameController _gameController;
        public ChaosRecipe(GameController gameController)
        {
            _gameController = gameController;
        }
        private List<NormalInventoryItem> _currentSet;
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
        /// <summary>
        /// Checks if the currently loaded "_currentSet" is present in our inventory and ready to be sold.
        /// </summary>
        /// <returns></returns>
        public bool InInventory()
        {
            var inventoryItems = _gameController.Game.IngameState.IngameUi.InventoryPanel[ExileCore.Shared.Enums.InventoryIndex.PlayerInventory].VisibleInventoryItems;
            foreach (var recipeItem in _currentSet)
            {
                if (!inventoryItems.Contains(recipeItem)) return false;
            }
            return true;
        }
        /// <summary>
        /// Assuming we allow only the most space efficient Set for now. Calculate the required inventoryspace(as of patch 3.12 its 29 Cells)  and check if we can numerically fit this set.
        /// TODO: Find a library for a fitting algorithm that can check if the set can fit in geometrical and not just by numerical free space; im too lazy to implement that myself
        /// </summary>
        /// <returns></returns>
        public bool CanFit()
        {
            byte minSetSize = 29; //2x(3x1 or 3x2)+3x(2x2)+1x(2x1)+3x(1x1): 2x1hand/1x2Hand,Body,Boots+Helm+Gloves,Belt,2xRings+1+Amulet
            var inventoryCells = 5*12; //5 rows, 12 coolumns
            var inventoryItems = _gameController.Game.IngameState.IngameUi.InventoryPanel[ExileCore.Shared.Enums.InventoryIndex.PlayerInventory].VisibleInventoryItems;

            foreach(var item in inventoryItems)
            {
                inventoryCells -= item.ItemWidth * item.ItemHeight;
                if (inventoryCells <= minSetSize) return false;
            }
            return true;
        }

        public bool AvailableInStash()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator PickUpFromStash()
        {
            throw new System.NotImplementedException();
        }
    }
}