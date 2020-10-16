using System.Diagnostics;
using System.Windows.Forms;
using ExileCore;
using ExileCore.Shared;
using Recipe.Model;

namespace Recipe
{
    public class Main : BaseSettingsPlugin<Settings>
    {
        private const string CoroutineName = "Recipe Main Routine";
        private readonly Stopwatch _debugTimer = new Stopwatch();
        private Coroutine _coroutineWorker;
        private readonly IRecipe _chaosRecipe = new ChaosRecipe();

        public Main()
        {
            Name = "Recipe";
        }

        public override bool Initialise()
        {
            Settings.StartHotkey.OnValueChanged += () => { Input.RegisterKey(Settings.StartHotkey); };

            return true;
        }

        public override Job Tick()
        {
            //test if anything can even happen right now
            if (Settings.StartHotkey.PressedOnce())
            {
                if (_coroutineWorker == null)
                {
                    _coroutineWorker = new Coroutine(_chaosRecipe.Go(), this, "ChaosRecipe");
                    Core.ParallelRunner.Run(_coroutineWorker);
                }
                else
                {
                    //if a Procedure(chaos recipe) is already running and the hotkey is pressed before finished, Pause the procedure. "Im not sure if this works but will try, probably needs more to work like that"
                    if (_coroutineWorker.Running)
                    {
                        _coroutineWorker.Pause();
                        Input.KeyUp(Keys.LControlKey); //must find better place for this
                    }
                    else
                    {
                        Input.KeyDown(Keys.LControlKey);
                        _coroutineWorker.Resume();
                    }
                }
            }

            if (_coroutineWorker != null && _coroutineWorker.Running && _debugTimer.ElapsedMilliseconds > 15000)
            {
                _coroutineWorker?.Done();
                _debugTimer.Restart();
                _debugTimer.Stop();
                Input.KeyUp(Keys.LControlKey);
            }

            return null;
        }
    }
}