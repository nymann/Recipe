namespace Tests
{
    public class NormalInventoryItem : ExileCore.PoEMemory.Elements.
        InventoryElements.NormalInventoryItem
    {
        public override int ItemWidth { get; }
        public override int ItemHeight { get; }

        public NormalInventoryItem(int itemHeight, int itemWidth)
        {
            ItemHeight = itemHeight;
            ItemWidth = itemWidth;
        }
    }
}