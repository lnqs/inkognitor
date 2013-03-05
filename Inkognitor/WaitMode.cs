using System;

namespace Inkognitor
{
    public class WaitMode : MainWindowMode
    {
        private PrintingTimer printingTimer = new PrintingTimer();

        public WaitMode(MainWindow window, Logger logger, Files files) : base(window, logger, files)
        {
            MayProceed = false;
        }

        public override string Name { get { return "Wait"; } }

        [CommandListener("maintainance_may_start", Description = "Gets/Sets if maintainance-mode may start")]
        public bool MayProceed { get; set; }

        public override void Enter()
        {
            base.Enter();
            StartTimer();
        }

        public override void Exit()
        {
            StopTimer();
        }

        private void StartTimer()
        {
            window.textBlock.Text = files.WaitPreText;
            printingTimer.Start(window.textBlock, files.WaitDelay, 1, ".", HandleTimerFinished);
        }

        private void StopTimer()
        {
            printingTimer.Stop();
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
