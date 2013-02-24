using System;

namespace Inkognitor
{
    public abstract class MainWindowMode : IMode
    {
        protected MainWindow window;

        public event ModeFinishedHandler ModeFinished;

        public virtual void Enter(MainWindow window_)
        {
            window = window_;
            window.TextEntered += HandleUserInput;
            window.Show();
        }

        public virtual void Exit()
        {
            window.TextEntered -= HandleUserInput;
            window.Hide();
        }

        protected void FireFinishedEvent()
        {
            ModeFinished.Invoke(this, EventArgs.Empty);
        }

        protected abstract void HandleUserInput(object sender, MainWindow.TextEnteredEventArgs e);
    }
}
