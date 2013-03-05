using System;
using System.IO;
using System.Net;
using System.Windows;
using SerialIO;

namespace Inkognitor
{
    public partial class App : Application
    {
        private const int CommandPort = 13135; // a = 1, c = 3, m = 13, e = 5 :o)

        private MainWindow window = new MainWindow();
        private Logger logger = new Logger();
        private Files files = new Files();
        private CommandDispatcher commandInterface = new CommandDispatcher(new CommandServer(IPAddress.Any, CommandPort));
        private IMode[] modes;
        private int currentMode = 0;
        private ArduinoConnector arduino;

        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                arduino = new ArduinoConnector();
            }
            catch (IOException exception)
            {
                MessageBox.Show(String.Format("Failed to connect to Arduino: {0}\n\n" +
                                              "Inkognitor will start, but no hardware " +
                                              "events can be received", exception.Message),
                    "Arduino-connection failed",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            modes = new IMode[] {
                new BrokenMode(window, arduino, logger, files),
                new MainMode(window, arduino, logger, files),
                new WaitMode(window, logger, files),
                new MaintainanceMode(window, logger, files),
                new HackingMode(window, arduino, logger, files),
                new EndMode(window, logger, files)
            };

            window.Closed += HandleWindowClosed;

            foreach (IMode mode in modes)
            {
                mode.ModeFinished += HandleModeFinished;
                commandInterface.AddListener(mode);
            }
            commandInterface.AddListener(this);
            (commandInterface.Provider as CommandServer).Start();

            modes[currentMode].Enter();

            window.Show();
        }

        [CommandListener("next_mode", Description="Enter the next mode")]
        private void NextMode()
        {
            if (currentMode < modes.Length - 1)
            {
                SetMode(currentMode + 1);
            }
        }

        [CommandListener("prev_mode", Description = "Enter the previous mode")]
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
                modes[currentMode].Enter();
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
