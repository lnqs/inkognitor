namespace Inkognitor
{
    public class BrokenMode : IMode
    {
        private MainWindow window;
        private Personality<DataCorruptingWaveMemoryStream> personality
                = new Personality<DataCorruptingWaveMemoryStream>();

        public void Enter(MainWindow window_)
        {
            window = window_;
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
