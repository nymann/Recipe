namespace Recipe.Model
{
    public class TabIndexItemTypes
    {
        public ItemType ItemType;
        public int Index;

        public TabIndexItemTypes(ItemType itemType, int index)
        {
            ItemType = itemType;
            Index = index;
        }
    }
}