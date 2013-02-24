using System.IO;

namespace Inkognitor
{
    public class MainMode : IMode
    {
        private MainWindow window;
        private Personality<MemoryStream> personality = new Personality<MemoryStream>();

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
