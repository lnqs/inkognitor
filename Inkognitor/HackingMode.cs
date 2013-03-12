using System;
using System.Threading;
using System.Windows;
using Hacking;
using SerialIO;

namespace Inkognitor
{
    public class HackingMode : IMode, IDisposable
    {
        private const int WinLevel = 6;

        private MainWindow mainWindow;
        private HackingGame hackingGame;
        private Thread hackingGameThread;
        private SwitchGame switchGame;
        private Thread switchGameThread;

        public HackingMode(MainWindow window, ArduinoConnector arduino, Logger logger, Files files)
        {
            mainWindow = window;

            hackingGame = new HackingGame(
                    (int)SystemParameters.PrimaryScreenWidth,
                    (int)SystemParameters.PrimaryScreenHeight,
                    false, false, false);
            hackingGame.LevelChanged += HandleLevelChanged;

            hackingGameThread = new Thread(hackingGame.Run);
            hackingGameThread.Name = "HackingGame";
            hackingGameThread.Start();

            hackingGame.Suspend();
            hackingGame.HideWindow();

            if (arduino != null)
            {
                switchGame = new SwitchGame(arduino, WinLevel);
                switchGame.MistakeMade += HandleSwitchGameMistakeMade;
            }
        }

        public event ModeFinishedHandler ModeFinished;

        public string Name { get { return "Hacking"; } }

        public void Enter()
        {
            hackingGame.Reset();
            hackingGame.Resume();
            hackingGame.ShowWindow();

            if (switchGame != null)
            {
                switchGameThread = new Thread(switchGame.Run);
                switchGameThread.Name = "SwitchGame";
                switchGameThread.Start();
            }

            mainWindow.Hide();
        }

        public void Exit()
        {
            mainWindow.Show();

            if (switchGame != null)
            {
                switchGame.Stop();
                switchGameThread.Join();
            }

            hackingGame.Suspend();
            hackingGame.HideWindow();
        }

        public void Quit()
        {
            hackingGame.Quit();
            hackingGameThread.Join();
        }

        private void HandleLevelChanged(object sender, HackingGame.LevelChangedEventArgs e)
        {
            if (e.Level == WinLevel)
            {
                ModeFinished.Invoke(this, EventArgs.Empty);
            }
            else
            {
                switchGame.Level = e.Level;
            }
        }

        private void HandleSwitchGameMistakeMade(object sender, EventArgs e)
        {
            hackingGame.ForceError("Neuronetzhandsteuerungsmisskonfiguration!"); // TODO: Shouldn't be hardcoded
        }

        public void Dispose()
        {
            hackingGame.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
