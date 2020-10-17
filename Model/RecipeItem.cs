using ExileCore.PoEMemory.Elements.InventoryElements;

namespace Recipe.Model
{
    public class RecipeItem
    {
        public NormalInventoryItem NormalInventoryItem;
        public ItemType ItemType;
        public bool IsLowLevel;

        public RecipeItem(ItemType itemType,
            NormalInventoryItem normalInventoryItem, bool isLowLevel)
        {
            ItemType = itemType;
            NormalInventoryItem = normalInventoryItem;
            IsLowLevel = isLowLevel;
        }
    }
}