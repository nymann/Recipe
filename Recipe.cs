using ExileCore;

namespace Recipe
{
    public class Recipe : BaseSettingsPlugin<Core.Config>
    {
        public Recipe()
        {
            Name = "Recipe";
        }

        public override bool Initialise()
        {
            Settings.Enable.OnValueChanged += (sender, enabled) =>
            {
                if (enabled)
                {
                    // Setup
                    DebugWindow.LogMsg($"{Name}: enabled", 5);
                }
                else
                {
                    // Teardown
                    DebugWindow.LogMsg($"{Name}: disabled", 5);
                }
            };

            Settings.Hotkey.OnValueChanged += () => { Input.RegisterKey(Settings.Hotkey); };

            return true;
        }

        public override void Render()
        {
            if (Settings.Hotkey.PressedOnce())
            {
                DebugWindow.LogMsg($"{Name}: Hotkey pressed.");
            }
        }
    }
}