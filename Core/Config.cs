using System.Windows.Forms;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;
using ExileCore.Shared.Attributes;

namespace Recipe.Core
{
    public class Config : ISettings
    {
        public ToggleNode Enable { get; set; }
        [Menu("Hotkey")]
        public HotkeyNode Hotkey { get; set; }
        public Config()
        {
            Enable = new ToggleNode(false);
            Hotkey = Keys.F5;
        }
    }
}