using SdlDotNet.Core;
using SdlDotNet.Graphics;

namespace Hacking
{
    public class HackingMode
    {
        private static readonly string WindowName = "Inkognitor";
        private static readonly int WindowWidth = 800;
        private static readonly int WindowHeight = 600;

        InformationArea informationArea;

        public void start()
        {
            Video.SetVideoMode(WindowWidth, WindowHeight);
            Video.WindowCaption = WindowName;

            Events.Tick += HandleTick;
            Events.Quit += HandleQuit;
            Events.Run();
        }

        private void HandleTick(object sender, TickEventArgs args)
        {
        }

        private void HandleQuit(object sender, QuitEventArgs e)
        {
            Events.QuitApplication();
        }
    }
}
