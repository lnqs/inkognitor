using System;

namespace Inkognitor
{
    public class BrokenMode : MainWindowMode, IDisposable
    {
        private Personality<DataCorruptingWaveMemoryStream> personality
                = new Personality<DataCorruptingWaveMemoryStream>();

        public override string Name { get { return "Broken"; } }
        public override string DefaultText { get { return RandomReplace(defaultText); } }

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

        public void Dispose()
        {
            personality.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
