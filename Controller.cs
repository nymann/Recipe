/*
 * This module is the main entrypoint of the application, it's called Controller
 * since the controller in the MVC pattern is responsible for accepting user
 * input (is hotkey pressed?) and converting that to a Model command
 * (_chaosRecipe.Go).
 */

using System.Diagnostics;
using System.Windows.Forms;
using ExileCore;
using ExileCore.Shared;
using Recipe.Model;

namespace Recipe
{
    public class Controller : BaseSettingsPlugin<Settings>
    {
        private const string CoroutineName = "Recipe Main Routine";
        private readonly Stopwatch _debugTimer = new Stopwatch();
        private Coroutine _coroutineWorker;
        private IRecipe _chaosRecipe;

        public Controller()
        {
            Name = "Recipe";
        }

        public override bool Initialise()
        {
            Settings.StartHotkey.OnValueChanged += () =>
            {
                Input.RegisterKey(Settings.StartHotkey);
            };
            _chaosRecipe = new ChaosRecipe(GameController, Settings);
            return true;
        }

        public override Job Tick()
        {
            // test if anything can even happen right now
            if (Settings.StartHotkey.PressedOnce())
            {
                if (_coroutineWorker == null)
                {
                    _coroutineWorker = new Coroutine(_chaosRecipe.Go(), this,
                        "ChaosRecipe");
                    Core.ParallelRunner.Run(_coroutineWorker);
                    return null;
                }

                /* if a Procedure (chaos recipe) is already running and the
                 * hotkey is pressed before finished, Pause the procedure. 
                 * "Im not sure if this works but will try, probably needs
                 * more to work like that"
                 */
                if (_coroutineWorker.Running)
                {
                    _coroutineWorker.Pause();
                    //TODO(must find better place for this)
                    Input.KeyUp(Keys.LControlKey);
                    return null;
                }

                Input.KeyDown(Keys.LControlKey);
                _coroutineWorker.Resume();
            }

            if (_coroutineWorker != null && _coroutineWorker.Running &&
                _debugTimer.ElapsedMilliseconds > 15000)
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