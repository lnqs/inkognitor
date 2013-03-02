using System;
using System.Net;
using System.Windows;

namespace Inkognitor
{
    public partial class App : Application
    {
        private const int CommandPort = 13135; // a = 1, c = 3, m = 13, e = 5 :o)

        private MainWindow window = new MainWindow();
        private Logger logger = new Logger();
        private Files files = new Files();
        private CommandDispatcher commandInterface = new CommandDispatcher(new CommandServer(IPAddress.Any, CommandPort));
        private IMode[] modes = new IMode[] { new BrokenMode(), new MainMode(), new WaitMode(),
                new MaintainanceMode(), new HackingMode(), new EndMode() };
        private int currentMode = 0;

        protected override void OnStartup(StartupEventArgs e)
        {
            window.Closed += HandleWindowClosed;

            foreach (IMode mode in modes)
            {
                mode.ModeFinished += HandleModeFinished;
                commandInterface.AddListener(mode);
            }
            commandInterface.AddListener(this);
            (commandInterface.Provider as CommandServer).Start();

            modes[currentMode].Enter(window, logger, files);
        }

        [CommandListener("next_mode", Description="Enter the next mode")]
        private void NextMode()
        {
            if (currentMode < modes.Length - 1)
            {
                SetMode(currentMode + 1);
            }
        }

        [CommandListener("prev_mode", Description = "Enter the revious mode")]
        private void PrevMode()
        {
            if (currentMode > 0)
            {
                SetMode(currentMode - 1);
            }
        }

        [CommandListener("get_mode_name", Description = "Returns the name of the current mode")]
        private string GetModeName()
        {
            return modes[currentMode].Name;
        }

        private void SetMode(int mode)
        {
            Dispatcher.Invoke((Action)(() =>
            {
                modes[currentMode].Exit();
                currentMode = mode;
                modes[currentMode].Enter(window, logger, files);
            }));
        }

        private void HandleModeFinished(object sender, EventArgs e)
        {
            NextMode();
        }

        private void HandleWindowClosed(object sender, EventArgs e)
        {
            foreach (IMode mode in modes)
            {
                mode.Quit();
            }
        }
    }
}
