using System;

namespace Inkognitor
{
    public class WaitMode : MainWindowMode
    {
        private Files files;
        private PrintingTimer printingTimer = new PrintingTimer();

        public override string Name { get { return "Wait"; } }

        [CommandListener("maintainance_may_start", Description = "Gets/Sets if maintainance-mode may start")]
        public bool MayProceed { get; set; }

        public override void Enter(MainWindow window, Logger logger, Files files_)
        {
            files = files_;
            base.Enter(window, logger, files_);

            MayProceed = false;
            StartTimer();
        }

        public override void Exit()
        {
            printingTimer.Stop();
        }

        private void StartTimer()
        {
            window.textBlock.Text = files.WaitPreText;
            printingTimer.Start(window.textBlock, files.WaitDelay, 1, ".", HandleTimerFinished);
        }

        private void HandleTimerFinished(object sender, EventArgs e)
        {
            window.textBlock.Text += "\n" + files.EndPostText;

            if (MayProceed)
            {
                FireFinishedEvent();
            }
            else
            {
                StartTimer();
            }
        }

        protected override void HandleUserInput(object sender, MainWindow.TextEnteredEventArgs e) { }
    }
}
