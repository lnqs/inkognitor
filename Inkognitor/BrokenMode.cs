using System;
using SerialIO;

namespace Inkognitor
{
    public class BrokenMode : MainWindowMode, IDisposable
    {
        private Personality<DataCorruptingWaveMemoryStream> personality
                = new Personality<DataCorruptingWaveMemoryStream>();
        private ArduinoConnector arduino;
        private bool firstPatchCompleted = false;

        public BrokenMode(MainWindow window, ArduinoConnector arduino_, Logger logger, Files files) : base(window, logger, files)
        {
            arduino = arduino_;
        }

        public override string Name { get { return "Broken"; } }
        public override string DefaultText { get { return RandomReplace(defaultText); } }

        public override void Enter()
        {
            base.Enter();

            if (arduino != null)
            {
                arduino.PatchCompleted += HandlePatchCompleted;
            }
        }

        public override void Exit()
        {
            base.Exit();

            if (arduino != null)
            {
                arduino.PatchCompleted -= HandlePatchCompleted;
            }
        }

        private string RandomReplace(string text)
        {
            char[] output = text.ToCharArray();
            Random random = new Random();

            for (int i = 0; i < output.Length; i++)
            {
                if (random.NextDouble() > 0.6)
                {
                    output[i] = (char)random.Next(0x2500, 0x25AF);
                }
            }

            return new string(output);
        }

        protected override void HandleUserInput(object sender, MainWindow.TextEnteredEventArgs e)
        {
            string response = personality.Respond(e.Text);
            logger.ChatLog.Log("User: {0}", e.Text);
            logger.ChatLog.Log("Inkognitor: (broken) {0}", response);
        }

        private void HandlePatchCompleted(object sender, EventArgs e)
        {
            if (!firstPatchCompleted)
            {
                firstPatchCompleted = true;
            }
            else
            {
                FireFinishedEvent();
            }
        }

        public void Dispose()
        {
            personality.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
