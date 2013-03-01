using System;

namespace Inkognitor
{
    public delegate void ModeFinishedHandler(object sender, EventArgs e);

    public interface IMode
    {
        event ModeFinishedHandler ModeFinished;

        void Enter(MainWindow window, Logger logger, Files files);
        void Exit();
        void Quit();
    }
}
