using System.IO;

namespace Inkognitor
{
    public class MainMode : IMode
    {
        static readonly string DefaultTextFile = "GUI/DefaultText.xml";

        private MainWindow window;
        private Personality<MemoryStream> personality = new Personality<MemoryStream>();

        public void Enter(MainWindow window_)
        {
            window = window_;

            DisplayTextXMLDocument xml = new DisplayTextXMLDocument(DefaultTextFile);
            window.titleLabel.Content = xml.Title;
            window.textBlock.Text = xml.Text;

            window.TextEntered += HandleUserInput;
            window.Show();
        }

        public void Exit()
        {
            window.TextEntered -= HandleUserInput;
            window.Hide();
        }

        private void HandleUserInput(object sender, MainWindow.TextEnteredEventArgs e)
        {
            personality.Respond(e.Text);
        }
    }
}
