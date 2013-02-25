using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using Hacking;
using SdlDotNet.Core;

namespace Inkognitor
{
    public class HackingMode : IMode
    {
        private HackingGame hackingGame;
        private Thread gameThread;

        bool isActive = false;
        bool sdlWindowVisible = true;

        public HackingMode()
        {
            hackingGame = new HackingGame(
                    (int)SystemParameters.PrimaryScreenWidth,
                    (int)SystemParameters.PrimaryScreenHeight);
            hackingGame.Tick += HandleTick;
            gameThread = new Thread(hackingGame.Run);
            gameThread.Start();
        }

        public event ModeFinishedHandler ModeFinished;

        public void Enter(MainWindow window)
        {
            isActive = true;
            hackingGame.Reset();
        }

        public void Exit()
        {
            isActive = false;
        }

        private void HandleTick(object sender, TickEventArgs e)
        {
            // We need to set this here, since we cannot know when the window is created elsewhere :/
            if (sdlWindowVisible && !isActive)
            {
                ShowWindow(hackingGame.WindowHandle.ToInt32(), 0);
                sdlWindowVisible = false;
            }
            else if (!sdlWindowVisible && isActive)
            {
                ShowWindow(hackingGame.WindowHandle.ToInt32(), 9);
                sdlWindowVisible = true;
            }

            if (!isActive)
            {
                Thread.Sleep(500);
            }
        }

        [DllImport("User32")]
        private static extern int ShowWindow(int hwnd, int nCmdShow);
    }
}
