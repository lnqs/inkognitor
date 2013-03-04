using System;
using System.IO;

namespace Inkognitor
{
    public class MainMode : MainWindowMode, IDisposable
    {
        private Personality<MemoryStream> personality = new Personality<MemoryStream>();
        private Files files;

        public MainMode()
        {
            BotMayAnswer = true;
        }
        
        [CommandListener("bot_may_answer", Description = "Gets/Sets if the bot answers to input")]
        public bool BotMayAnswer { get; set; }

        public override string Name { get { return "Main"; } }

        public override void Enter(MainWindow window, Logger logger, Files files_)
        {
            files = files_;
            base.Enter(window, logger, files_);
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
            else if (BotMayAnswer)
            {
                string response = personality.Respond(e.Text);
                logger.ChatLog.Log("User: {0}", e.Text);
                logger.ChatLog.Log("Inkognitor: {0}", response);
            }
        }

        [CommandListener("say", Description = "Outputs text via the speech-synthesizer")]
        private void Say(string text)
        {
            personality.Say(text);
        }

        public void Dispose()
        {
            personality.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
