using System;
using System.Net;
using System.Windows;

// TODO: The pathes to the resources need unification
namespace Inkognitor
{
    public partial class App : Application
    {
        static private readonly int CommandPort = 13135; // a = 1, c = 3, m = 13, e = 5 :o)

        private MainWindow window = new MainWindow();
        private CommandDispatcher commandInterface = new CommandDispatcher(new CommandServer(IPAddress.Any, CommandPort));
        private IMode[] modes = new IMode[] { new BrokenMode(), new MainMode(), new MaintainanceMode(), new HackingMode() };
        private int currentMode = 0;

        protected override void OnStartup(StartupEventArgs e)
        {
            foreach (IMode mode in modes)
            {
                mode.ModeFinished += HandleModeFinished;
                commandInterface.AddListener(mode);
            }
            commandInterface.AddListener(this);
            (commandInterface.Provider as CommandServer).Start();

            currentMode = 2; // TODO
            modes[currentMode].Enter(window);
        }

        [CommandListener("next_mode", Description="Enter the next mode")]
        private void NextMode()
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                if (currentMode < modes.Length - 1)
                {
                    modes[currentMode].Exit();
                    currentMode += 1;
                    modes[currentMode].Enter(window);
                }
            }));
        }

        private void HandleModeFinished(object sender, EventArgs e)
        {
            NextMode();
        }
    }
}
