using System;

namespace Inkognitor
{
    public delegate void ModeFinishedHandler(object sender, EventArgs e);

    public interface IMode
    {
        event ModeFinishedHandler ModeFinished;

        void Enter(MainWindow window);
        void Exit();
    }
}
