using System;
using System.Windows;
using Hacking;

namespace Inkognitor
{
    public partial class App : Application
    {
        private WebInterface webInterface = new WebInterface();
        private MainWindow window = new MainWindow();
        private Personality personality = new Personality();
        private HackingMode hackingMode = new HackingMode();

        protected override void OnStartup(StartupEventArgs e)
        {
            webInterface.Start();

            window.Show();
            window.Closed += HandleWindowClosed;
            window.TextEntered += HandleUserInput;
        }

        private void HandleWindowClosed(object sender, EventArgs e)
        {
            webInterface.Stop();
        }

        private void HandleUserInput(object sender, MainWindow.TextEnteredEventArgs e)
        {
            if (e.Text == "hack")
            {
                window.Hide();
                hackingMode.start();
                window.Show();
            }
            else
            {
                personality.Respond(e.Text);
            }
        }
    }
}
