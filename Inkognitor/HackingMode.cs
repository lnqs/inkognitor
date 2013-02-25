using System;
using System.Runtime.InteropServices;
using System.Threading;
using Hacking;
using SdlDotNet.Core;

namespace Inkognitor
{
    public class HackingMode : IMode
    {
        private HackingGame hackingGame = new HackingGame();
        private Thread gameThread;
        Mutex suspendMutex = new Mutex();

        public HackingMode()
        {
            hackingGame.Tick += HandleTick;
            suspendMutex.WaitOne();
            gameThread = new Thread(hackingGame.Run);
            gameThread.Start();
        }

        public event ModeFinishedHandler ModeFinished;

        public void Enter(MainWindow window)
        {
            suspendMutex.ReleaseMutex();
        }

        public void Exit()
        {
            suspendMutex.WaitOne();
        }

        private void HandleTick(object sender, TickEventArgs e)
        {
            suspendMutex.WaitOne();
            suspendMutex.ReleaseMutex();
        }
    }
}
