using System.Drawing;
using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Input;

namespace Hacking
{
    public class HackingMode
    {
        public static readonly string ResourceDirectory = "Resources";

        private static readonly Size WindowSize = new Size(800, 600);
        private static readonly string WindowName = "Inkognitor";

        private CodeBlockGrid codeBlocks = new CodeBlockGrid(6, 4, WindowSize);

        public void start()
        {
            Video.SetVideoMode(WindowSize.Width, WindowSize.Height);
            Video.WindowCaption = WindowName;

            Events.Tick += HandleTick;
            Events.Quit += HandleQuit;

            Events.Run();
        }

        private void HandleTick(object sender, TickEventArgs args)
        {
            codeBlocks.Update(args);

            Video.Screen.Blit(codeBlocks);

            Video.Screen.Update();
        }

        private void HandleQuit(object sender, QuitEventArgs e)
        {
            Events.QuitApplication();
        }
    }
}
