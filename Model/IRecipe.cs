using System.Collections;

namespace Recipe.Model
{
    public interface IRecipe
    {
        IEnumerator Go();
        bool InInventory();

        bool CanFit();

        bool AvailableInStash();

        IEnumerator PickUpFromStash();
    }
}