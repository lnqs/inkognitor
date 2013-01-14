using System.Drawing;
using SdlDotNet.Core;
using SdlDotNet.Graphics;

namespace Hacking
{
    public class HackingMode
    {
        public static readonly string ResourceDirectory = "Resources";

        private static readonly Size WindowSize = new Size(800, 600);
        private static readonly string WindowName = "Inkognitor";

        private InformationArea informationArea;
        private CodeArea codeArea;

        public void start()
        {
            Video.SetVideoMode(WindowSize.Width, WindowSize.Height);
            Video.WindowCaption = WindowName;

            informationArea = new InformationArea(new Rectangle(0, 0, WindowSize.Width, (int)(WindowSize.Height * 0.1875f)));
            codeArea = new CodeArea(new Rectangle(0, informationArea.Height, WindowSize.Width, WindowSize.Height - informationArea.Height));

            Events.Tick += HandleTick;
            Events.Quit += HandleQuit;
            Events.Run();
        }

        private void HandleTick(object sender, TickEventArgs args)
        {
            informationArea.Update(args);
            codeArea.Update(args);

            Video.Screen.Blit(informationArea, new Point(0, 0));
            Video.Screen.Blit(codeArea, new Point(0, informationArea.Height));

            Video.Screen.Update();
        }

        private void HandleQuit(object sender, QuitEventArgs e)
        {
            Events.QuitApplication();
        }
    }
}
