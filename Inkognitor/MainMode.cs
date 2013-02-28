using System.IO;

namespace Inkognitor
{
    public class MainMode : MainWindowMode
    {
        const string TextFile = "Resources/Files/DefaultText.xml";

        private Personality<MemoryStream> personality = new Personality<MemoryStream>();

        public override void Enter(MainWindow window_)
        {
            base.Enter(window_);
            DisplayTextXMLDocument xml = new DisplayTextXMLDocument(TextFile);
            window.titleLabel.Content = xml.Title;
            window.textBlock.Text = xml.Text;
        }

        protected override void HandleUserInput(object sender, MainWindow.TextEnteredEventArgs e)
        {
            personality.Respond(e.Text);
        }
    }
}
