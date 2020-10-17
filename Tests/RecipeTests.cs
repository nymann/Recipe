using System;
using System.Collections.Generic;
using NUnit.Framework;
using Recipe;
using Recipe.Model;

namespace Tests
{
    public class Tests
    {
        private ChaosRecipe _chaosRecipe;

        [SetUp]
        public void Setup()
        {
            _chaosRecipe = new ChaosRecipe(null, new Settings())
            {
                CurrentSet = MockRecipeItemSet()
            };
        }

        private List<RecipeItem> MockRecipeItemSet()
        {
            var items = new List<RecipeItem>
            {
                MockRecipeItem(ItemType.Amulet),
                MockRecipeItem(ItemType.Belt),
                MockRecipeItem(ItemType.Boots),
                MockRecipeItem(ItemType.BodyArmour),
                MockRecipeItem(ItemType.Gloves),
                MockRecipeItem(ItemType.Ring),
                MockRecipeItem(ItemType.OneHandedWeapon),
                MockRecipeItem(ItemType.OneHandedWeapon),
            };
            return items;
        }

        private RecipeItem MockRecipeItem(ItemType itemType)
        {
            NormalInventoryItem item;
            switch (itemType)
            {
                case ItemType.OneHandedWeapon:
                    item = new NormalInventoryItem(3, 1);
                    break;
                case ItemType.BodyArmour:
                    item = new NormalInventoryItem(3, 2);
                    break;
                case ItemType.Helmet:
                    item = new NormalInventoryItem(2, 2);
                    break;
                case ItemType.Gloves:
                    item = new NormalInventoryItem(2, 2);
                    break;
                case ItemType.Boots:
                    item = new NormalInventoryItem(2, 2);
                    break;
                case ItemType.Belt:
                    item = new NormalInventoryItem(1, 2);
                    break;
                case ItemType.Amulet:
                    item = new NormalInventoryItem(1, 1);
                    break;
                case ItemType.Ring:
                    item = new NormalInventoryItem(1, 1);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(itemType),
                        itemType, null);
            }

            return new RecipeItem(itemType, item, true);
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
    }
}