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
        private static readonly int CodeBlockColumns = 6;
        private static readonly int CodeBlockRows = 4;

        private CodeBlockGrid codeBlocks;
        private Cursor cursor;

        public void start()
        {
            codeBlocks = new CodeBlockGrid(CodeBlockColumns, CodeBlockRows, WindowSize);
            cursor = new Cursor(codeBlocks.BlockPixelSize);

            Video.SetVideoMode(WindowSize.Width, WindowSize.Height);
            Video.WindowCaption = WindowName;

            Events.Tick += HandleTick;
            Events.KeyboardDown += HandleKeyboardDown;
            Events.Quit += HandleQuit;

            Events.Run();
        }

        private void HandleTick(object sender, TickEventArgs args)
        {
            codeBlocks.Update(args);
            cursor.Position = codeBlocks[cursor.GridX, cursor.GridY].Position;

            Video.Screen.Blit(codeBlocks);
            Video.Screen.Blit(cursor);

            Video.Screen.Update();
        }

        private void HandleKeyboardDown(object sender, KeyboardEventArgs args)
        {
            switch (args.Key)
            {
                case Key.LeftArrow:
                    cursor.GridX = cursor.GridX > 0 ? cursor.GridX - 1 : cursor.GridX;
                    break;
                case Key.RightArrow:
                    cursor.GridX = cursor.GridX < codeBlocks.Rows - 1 ? cursor.GridX + 1 : cursor.GridX;
                    break;
                case Key.UpArrow:
                    cursor.GridY = cursor.GridY > 0 ? cursor.GridY - 1 : cursor.GridY;
                    break;
                case Key.DownArrow:
                    cursor.GridY = cursor.GridY < codeBlocks.Columns - 1 ? cursor.GridY + 1 : cursor.GridY;
                    break;
            }
        }

        private void HandleQuit(object sender, QuitEventArgs e)
        {
            Events.QuitApplication();
        }
    }
}
