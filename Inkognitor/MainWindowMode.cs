using System;

namespace Inkognitor
{
    public abstract class MainWindowMode : IMode
    {
        protected MainWindow window;
        protected Logger logger;
        protected string defaultText;

        public event ModeFinishedHandler ModeFinished;

        public abstract string Name { get; }
        public virtual string DefaultText { get { return defaultText; } }

        public virtual void Enter(MainWindow window_, Logger logger_, Files files)
        {
            window = window_;
            window.TextEntered += HandleUserInput;

            logger = logger_;

            defaultText = files.DefaultText;

            window.textBlock.Text = DefaultText;

            window.Show();
        }

        public virtual void Exit()
        {
            window.TextEntered -= HandleUserInput;
            window.Hide();
        }

        public virtual void Quit() { }

        protected void FireFinishedEvent()
        {
            ModeFinished.Invoke(this, EventArgs.Empty);
        }

        protected abstract void HandleUserInput(object sender, MainWindow.TextEnteredEventArgs e);
    }
}
