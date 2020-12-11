using System.Collections;
using System.Collections.Generic;
using ExileCore;
using ExileCore.PoEMemory.Elements;
using ExileCore.PoEMemory.Elements.InventoryElements;
using ExileCore.PoEMemory.MemoryObjects;
using Recipe.Model;

namespace Recipe.Controller
{
    public class Recipe : IRecipe
    {
        private readonly GameController _gameController;
        private readonly Settings _settings;

        public Inventory PlayerInventory =>
            _gameController.Game.IngameState.IngameUi.InventoryPanel[
                ExileCore.Shared.Enums.InventoryIndex.PlayerInventory];

        public StashElement StashElement =>
            _gameController.Game.IngameState.IngameUi.StashElement;

        public List<RecipeItem> CurrentSet;

        public Recipe(GameController gameController, Settings settings)
        {
            _settings = settings;
            _gameController = gameController;
        }

        public IEnumerator Go()
        {
            if (InInventory())
            {
                SellSetToVendor();
                yield break;
            }

            if (!CanFit(PlayerInventory.VisibleInventoryItems)) yield break;

            if (!AvailableInStash()) yield break;

            yield return PickUpFromStash();
            yield return SellSetToVendor();
        }

        public bool InInventory()
        {
            throw new System.NotImplementedException();
        }

        public bool CanFit(IEnumerable<NormalInventoryItem> inventoryItems)
        {
            /* The player inventory will look like the following
             * 1 0 0 0 0 0 0 0 0 0 0 0
             * 1 0 0 0 0 0 0 0 0 0 0 0
             * 1 0 0 0 0 0 0 0 0 0 0 0
             * 0 0 0 0 0 0 0 0 0 0 0 0
             * 0 0 0 0 0 0 0 0 0 0 0 0
             */
            var x = _gameController.Game.IngameState.ServerData
                .PlayerInventories[0].Inventory.Hash;
            return false;
        }

        public bool AvailableInStash()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator PickUpFromStash()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator SellSetToVendor()
        {
            throw new System.NotImplementedException();
        }
    }
}