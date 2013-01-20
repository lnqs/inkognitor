using System.Net;
using System.Windows;
using Hacking;

namespace Inkognitor
{
    public partial class App : Application
    {
        static private readonly int CommandPort = 13135; // a = 1, c = 3, m = 13, e = 5 :o)

        private MainWindow window = new MainWindow();
        private Personality personality = new Personality();
        private HackingMode hackingMode = new HackingMode();
        private CommandDispatcher commandInterface = new CommandDispatcher(new CommandServer(IPAddress.Any, CommandPort));

        protected override void OnStartup(StartupEventArgs e)
        {
            (commandInterface.Provider as CommandServer).Start();
            commandInterface.AddListener(personality);

            window.Show();
            window.TextEntered += HandleUserInput;
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
