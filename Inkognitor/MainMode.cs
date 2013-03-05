using System;
using System.IO;
using SerialIO;

namespace Inkognitor
{
    public class MainMode : MainWindowMode, IDisposable
    {
        private Personality<MemoryStream> personality = new Personality<MemoryStream>();
        private ArduinoConnector arduino;

        public MainMode(MainWindow window, ArduinoConnector arduino_, Logger logger, Files files)
            : base(window, logger, files)
        {
            arduino = arduino_;
            BotMayAnswer = false;
        }
        
        [CommandListener("bot_may_answer", Description = "Gets/Sets if the bot answers to input")]
        public bool BotMayAnswer { get; set; }

        public override string Name { get { return "Main"; } }

        public override void Enter()
        {
            base.Enter();

            if (arduino != null)
            {
                arduino.KeysTurned += HandleKeysTurned;
            }
        }

        public override void Exit()
        {
            base.Exit();

            if (arduino != null)
            {
                arduino.KeysTurned -= HandleKeysTurned;
            }
        }

        protected override void HandleUserInput(object sender, MainWindow.TextEnteredEventArgs e)
        {
            if (e.Text.ToUpper().StartsWith(files.Prefix.ToUpper()))
            {
                string token = e.Text.Substring(files.Prefix.Length).Trim();

                try
                {
                    window.textBlock.Text = files.GetFile(token);
                    personality.Say(files.LoadingFile);
                    logger.ChatLog.Log("File-access: {0}", token);
                }
                catch (Files.ElementException)
                {
                    window.textBlock.Text = files.UnknownToken;
                    personality.Say(files.UnknownToken);
                    logger.ChatLog.Log("No such file: {0}", token);
                }
            }
            else
            {
                logger.ChatLog.Log("User: {0}", e.Text);
                if (BotMayAnswer)
                {
                    string response = personality.Respond(e.Text);
                    logger.ChatLog.Log("Inkognitor: {0}", response);
                }
            }
        }

        private void HandleKeysTurned(object sender, EventArgs e)
        {
            FireFinishedEvent();
        }

        [CommandListener("say", Description = "Outputs text via the speech-synthesizer")]
        private void Say(string text)
        {
            if (logger != null)
            {
                logger.ChatLog.Log("Inkognitor: (manual) {0}", text);
            }

            personality.Say(text);
        }

        public void Dispose()
        {
            personality.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
