using System;
using System.Threading;
using System.Windows;
using Hacking;
using SdlDotNet.Core;

namespace Inkognitor
{
    public class HackingMode : IMode, IDisposable
    {
        private const int WinLevel = 5;

        private HackingGame hackingGame;
        private Thread gameThread;

        public HackingMode()
        {
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

        public void Enter(MainWindow window, Files files)
        {
            hackingGame.Reset();
            hackingGame.Resume();
            hackingGame.ShowWindow();
        }

        public void Exit()
        {
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
