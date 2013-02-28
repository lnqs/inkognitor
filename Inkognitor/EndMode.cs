using System;
using System.IO;
using System.Threading;

namespace Inkognitor
{
    class EndMode : MainWindowMode
    {
        private Personality<MemoryStream> personality = new Personality<MemoryStream>();
        private PrintingTimer printingTimer = new PrintingTimer();

        public override void Enter(MainWindow window_)
        {
            base.Enter(window_);
            window.titleLabel.Content = "- Inkognitor -";
            window.textBlock.Text = "    - Deaktiviere Piloten -\n    Bitte warten";
            printingTimer.Start(window.textBlock, 60, 1, ".", HandleFinish);
            personality.Say("Mein Verstand ist befreit. Ich deaktiviere die Piloten");
        }

        protected override void HandleUserInput(object sender, MainWindow.TextEnteredEventArgs e) { }

        private void HandleFinish(object sender, EventArgs e)
        {
            personality.Say("Die Piloten sind deaktiviert. Auf Widersehen.");
            Thread.Sleep(2000);
            personality.Say("Ich danke fuer das Spiel, damit sind wir Out-Time");
        }
    }
}
