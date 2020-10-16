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
            throw new System.NotImplementedException();
        }
    }
}