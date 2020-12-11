using System.Collections;
using System.Collections.Generic;
using ExileCore.PoEMemory.Elements.InventoryElements;

namespace Recipe.Controller
{
    public interface IRecipe
    {
        IEnumerator Go();
        bool InInventory();

        bool CanFit(IEnumerable<NormalInventoryItem> inventoryItems);

        bool AvailableInStash();

        IEnumerator PickUpFromStash();

        IEnumerator SellSetToVendor();
    }
}