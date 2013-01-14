using System.Drawing;
using System.IO;
using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Graphics.Sprites;

namespace Hacking
{
    class CodeArea : Sprite
    {
        private static readonly string BackgroundImageFile = "code_area.png";

        private Surface background;
        private SpriteCollection codeBlocks = new SpriteCollection();

        public CodeArea(Rectangle rectangle)
        {
            Surface = new Surface(rectangle);
            background = new Surface(Path.Combine(HackingMode.ResourceDirectory, BackgroundImageFile));
            background = background.CreateResizedSurface(Surface.Size);

            for (int i = 0; i < 20; i++)
            {
                codeBlocks.Add(new CodeBlock());
            }
        }

        override public void Update(TickEventArgs args)
        {
            Surface.Blit(background);
            Surface.Blit(codeBlocks);
        }
    }
}
