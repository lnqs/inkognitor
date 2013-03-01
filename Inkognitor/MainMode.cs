using System;
using System.IO;

namespace Inkognitor
{
    public class MainMode : MainWindowMode, IDisposable
    {
        private Personality<MemoryStream> personality = new Personality<MemoryStream>();
        private Files files;

        public override string Name { get { return "Main"; } }

        public override void Enter(MainWindow window, Files files_)
        {
            files = files_;
            base.Enter(window, files_);
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
                }
                catch (Files.ElementException)
                {
                    window.textBlock.Text = files.UnknownToken;
                    personality.Say(files.UnknownToken);
                }
            }
            else
            {
                personality.Respond(e.Text);
            }
        }

        public void Dispose()
        {
            personality.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
