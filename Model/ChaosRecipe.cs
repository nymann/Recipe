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
            throw new System.NotImplementedException();
        }
    }
}