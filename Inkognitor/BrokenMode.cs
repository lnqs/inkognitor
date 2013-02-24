using System;

namespace Inkognitor
{
    public class BrokenMode : IMode
    {
        static readonly string DefaultTextFile = "GUI/DefaultText.xml";

        private MainWindow window;
        private Personality<DataCorruptingWaveMemoryStream> personality
                = new Personality<DataCorruptingWaveMemoryStream>();

        public void Enter(MainWindow window_)
        {
            window = window_;

            DisplayTextXMLDocument xml = new DisplayTextXMLDocument(DefaultTextFile);
            window.titleLabel.Content = RandomReplace(xml.Title);
            window.textBlock.Text = RandomReplace(xml.Text);

            window.TextEntered += HandleUserInput;
            window.Show();
        }

        public void Exit()
        {
            window.TextEntered -= HandleUserInput;
            window.Hide();
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

        private void HandleUserInput(object sender, MainWindow.TextEnteredEventArgs e)
        {
            personality.Respond(e.Text);
        }
    }
}
