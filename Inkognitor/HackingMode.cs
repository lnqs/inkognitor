using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using Hacking;
using SdlDotNet.Core;

namespace Inkognitor
{
    public class HackingMode : IMode
    {
        static readonly int WinLevel = 5;

        private HackingGame hackingGame;
        private Thread gameThread;

        bool isActive = false;
        bool initialization = true;

        public HackingMode()
        {
            hackingGame = new HackingGame(
                    (int)SystemParameters.PrimaryScreenWidth,
                    (int)SystemParameters.PrimaryScreenHeight,
                    false, false, false);
            hackingGame.Tick += HandleTick;
            gameThread = new Thread(hackingGame.Run);
            gameThread.Start();
        }

        public event ModeFinishedHandler ModeFinished;

        public void Enter(MainWindow window)
        {
            isActive = true;
            hackingGame.Reset();
            ShowSDLWindow();
            hackingGame.LevelChanged += HandleLevelChanged;
        }

        public void Exit()
        {
            hackingGame.LevelChanged -= HandleLevelChanged;
            isActive = false;
            HideSDLWindow();
        }

        private void ShowSDLWindow()
        {
            ShowWindow(hackingGame.WindowHandle, 9);
        }

        private void HideSDLWindow()
        {
            ShowWindow(hackingGame.WindowHandle, 0);
        }

        private void HandleTick(object sender, TickEventArgs e)
        {
            // We need to set this here, since we cannot know when the window is created elsewhere :/
            if (initialization)
            {
                SetWindowLong(hackingGame.WindowHandle, -16, 0); // delete all styles
                SetWindowPos(hackingGame.WindowHandle, new IntPtr(0), 0, 0, 0, 0, 0x0045);
                HideSDLWindow();
                initialization = false;
            }

            if (!isActive)
            {
                Thread.Sleep(500);
            }
        }

        private void HandleLevelChanged(object sender, HackingGame.LevelChangedEventArgs e)
        {
            if (e.Level == WinLevel)
            {
                ModeFinished.Invoke(this, EventArgs.Empty);
            }
        }

        [DllImport("user32.dll")]
        private static extern int ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);
    }
}
