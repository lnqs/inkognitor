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
        private Thread gameThread;
        private ArduinoConnector arduino;

        public HackingMode(MainWindow window, ArduinoConnector arduino_, Logger logger, Files files)
        {
            mainWindow = window;
            arduino = arduino_;

            hackingGame = new HackingGame(
                    (int)SystemParameters.PrimaryScreenWidth,
                    (int)SystemParameters.PrimaryScreenHeight,
                    false, false, false);
            hackingGame.LevelChanged += HandleLevelChanged;

            gameThread = new Thread(hackingGame.Run);
            gameThread.Start();

            hackingGame.Suspend();
            hackingGame.HideWindow();
        }

        public event ModeFinishedHandler ModeFinished;

        public string Name { get { return "Hacking"; } }

        public void Enter()
        {
            hackingGame.Reset();
            hackingGame.Resume();
            hackingGame.ShowWindow();
            mainWindow.Hide();
        }

        public void Exit()
        {
            mainWindow.Show();
            hackingGame.Suspend();
            hackingGame.HideWindow();
        }

        public void Quit()
        {
            hackingGame.Quit();
            gameThread.Join();
        }

        private void HandleLevelChanged(object sender, HackingGame.LevelChangedEventArgs e)
        {
            if (e.Level == WinLevel)
            {
                ModeFinished.Invoke(this, EventArgs.Empty);
            }
        }

        public void Dispose()
        {
            hackingGame.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
