using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;

namespace Recipe.Core
{
    public class Config : ISettings
    {
        public ToggleNode Enable { get; set; }

        public Config()
        {
            Enable = new ToggleNode(false);
        }
    }
}