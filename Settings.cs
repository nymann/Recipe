using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ExileCore.Shared.Attributes;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;

namespace Recipe
{
    public class Settings: ISettings
    {
        public ToggleNode Enable { get; set; }
        
        [Menu("StartHotkey")]
        public HotkeyNode StartHotkey { get; set; }

        [Menu("Body Armor")]
        public RangeNode<int> BodyArmor { get; set; }

        [Menu("Weapons")]
        public RangeNode<int> Weapons { get; set; }

        [Menu("Helmets")]
        public RangeNode<int> Helmets { get; set; }

        [Menu("Boots")]
        public RangeNode<int> Boots { get; set; }

        [Menu("Gloves")]
        public RangeNode<int> Gloves { get; set; }

        [Menu("Belts")]
        public RangeNode<int> Belts { get; set; }

        [Menu("Rings")]
        public RangeNode<int> Rings { get; set; }

        [Menu("Amulets")]
        public RangeNode<int> Amulets { get; set; }

        [Menu("Two sets at once?")]

        public ToggleNode TwoSetsAtOnce { get; set; }

        [Menu("Extra Delay", "Is it going too fast? Then add a delay (in ms).")]
        public RangeNode<int> ExtraDelay { get; set; }
        public Settings()
        {
            Enable = new ToggleNode(false);
            StartHotkey = Keys.F5;
            TwoSetsAtOnce = new ToggleNode(true);

            ExtraDelay = new RangeNode<int>(0, 0, 2000);


            BodyArmor = new RangeNode<int>(1, 0,  40);
            Weapons = new RangeNode<int>(2, 0,  40);
            Helmets = new RangeNode<int>(3, 0,  40);
            Boots = new RangeNode<int>(4, 0, 40);
            Gloves = new RangeNode<int>(5, 0, 40);
            Belts = new RangeNode<int>(6, 0, 40);
            Rings = new RangeNode<int>(8, 0, 40);
            Amulets = new RangeNode<int>(11, 0, 40);

        }
    }
}