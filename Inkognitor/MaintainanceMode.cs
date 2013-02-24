using System.Collections.Generic;

namespace Inkognitor
{
    public class MaintainanceMode : MainWindowMode
    {
        static readonly string TextFile = "GUI/MaintainanceText.xml";

        private Dictionary<string, CommandHandler> commandHandlers;

        private PrintingTimer printingTimer = new PrintingTimer();

        private string defaultText;

        public MaintainanceMode()
        {
            commandHandlers = new Dictionary<string, CommandHandler>() {
                { "BEFEHLE", Handle_BEFEHLE },
                { "KERNTEST", Handle_TEST },
                { "RING1TEST", Handle_TEST },
                { "RING2TEST", Handle_TEST },
                { "RING3TEST", Handle_TEST },
                { "NEUROSUPPRESSIONSKONFIGURATION", Handle_NEUROSUPPRESSIONSKONFIGURATION },
                { "NEUINITIALISIERUNG", Handle_NEUINITIALISIERUNG },
            };
        }

        private delegate void CommandHandler();

        public override void Enter(MainWindow window_)
        {
            base.Enter(window_);
            DisplayTextXMLDocument xml = new DisplayTextXMLDocument(TextFile);
            window.titleLabel.Content = xml.Title;
            defaultText = xml.Text;
            window.textBlock.Text = defaultText;
        }

        protected override void HandleUserInput(object sender, MainWindow.TextEnteredEventArgs e)
        {
            if (!printingTimer.IsEnabled)
            {
                string command = e.Text.ToUpper();

                if (commandHandlers.ContainsKey(command))
                {
                    commandHandlers[command]();
                }
                else
                {
                    Handle_CommandNotFound();
                }
            }
        }

        private void Handle_BEFEHLE()
        {
            window.textBlock.Text = defaultText;
        }

        private void Handle_TEST()
        {
            window.textBlock.Text = "\n    - Test eingeleitet -\n    Bitte warten";
            printingTimer.Start(window.textBlock, 60, 1, ".",
                    (sender, e) => window.textBlock.Text += "\n\n    Test erfolgreich");
        }

        private void Handle_NEUROSUPPRESSIONSKONFIGURATION()
        {
            window.textBlock.Text = "\n    - Neurosuppressionskonfiguration wird gestartet -\n    Bitte warten";
            printingTimer.Start(window.textBlock, 5 * 120, 1, ".",
                    (sender, e) => { FireFinishedEvent(); });
        }

        private void Handle_NEUINITIALISIERUNG()
        {
            window.textBlock.Text = "\n    - Neuinitialisierung gestartet -\n    Bitte warten";
            printingTimer.Start(window.textBlock, 5 * 60, 1, ".",
                    (sender, e) => window.textBlock.Text += "\n\n    Test erfolgreich");
        }

        private void Handle_CommandNotFound()
        {
            window.textBlock.Text = defaultText;
            window.textBlock.Text = "\n    - Unbekannter Befehl -\n    'BEFEHLE' fuer Befehlsliste";
        }
    }
}
