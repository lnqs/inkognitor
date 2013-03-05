using System;

namespace Inkognitor
{
    public abstract class MainWindowMode : IMode
    {
        protected MainWindow window;
        protected Logger logger;
        protected Files files;
        protected string defaultText;

        public MainWindowMode(MainWindow window_, Logger logger_, Files files_)
        {
            window = window_;
            logger = logger_;
            files = files_;
            defaultText = files.DefaultText;
        }

        public event ModeFinishedHandler ModeFinished;

        public abstract string Name { get; }
        public virtual string DefaultText { get { return defaultText; } }

        public virtual void Enter()
        {
            window.TextEntered += HandleUserInput;
            window.textBlock.Text = DefaultText;
        }

        public virtual void Exit()
        {
            window.TextEntered -= HandleUserInput;
        }

        public virtual void Quit() { }

        protected void FireFinishedEvent()
        {
            ModeFinished.Invoke(this, EventArgs.Empty);
        }

        protected abstract void HandleUserInput(object sender, MainWindow.TextEnteredEventArgs e);
    }
}
