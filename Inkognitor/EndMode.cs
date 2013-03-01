using System;
using System.IO;
using System.Threading;

namespace Inkognitor
{
    class EndMode : MainWindowMode, IDisposable
    {
        private Personality<MemoryStream> personality = new Personality<MemoryStream>();
        private PrintingTimer printingTimer = new PrintingTimer();

        public override string Name { get { return "End"; } }

        public override void Enter(MainWindow window_, Files files)
        {
            base.Enter(window_, files);
            window.textBlock.Text = files.EndPreText;
            printingTimer.Start(window.textBlock, files.EndDelay, 1, ".", (sender, e) => {
                window.textBlock.Text += "\n" + files.EndPostText;
                personality.Say(files.EndPostText);
            });
        }

        protected override void HandleUserInput(object sender, MainWindow.TextEnteredEventArgs e) { }

        public void Dispose()
        {
            personality.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
