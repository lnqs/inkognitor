using System;

namespace Inkognitor
{
    public class BrokenMode : MainWindowMode
    {
        static readonly string TextFile = "GUI/DefaultText.xml";

        private Personality<DataCorruptingWaveMemoryStream> personality
                = new Personality<DataCorruptingWaveMemoryStream>();

        public override void Enter(MainWindow window_)
        {
            base.Enter(window_);
            DisplayTextXMLDocument xml = new DisplayTextXMLDocument(TextFile);
            window.titleLabel.Content = RandomReplace(xml.Title);
            window.textBlock.Text = RandomReplace(xml.Text);
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
            personality.Respond(e.Text);
        }
    }
}
