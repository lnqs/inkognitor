using System;
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
        private static readonly string CodeBlockImageFile = "code{0:D}.png";
        private static readonly int CodeBlockImageCount = 10;
        private static readonly int CodeBlocksHorizontal = 4;
        private static readonly int CodeBlocksVertical = 6;

        private static readonly float ScrollingSpeed = 100.0f;

        private Random random = new Random();

        private Surface background;
        private Surface[] codeBlockSurfaces = new Surface[CodeBlockImageCount];

        private Sprite[,] codeBlocks = new Sprite[CodeBlocksHorizontal, CodeBlocksVertical];

        public CodeArea(Rectangle rectangle)
        {
            Surface = new Surface(rectangle);
            background = new Surface(Path.Combine(HackingMode.ResourceDirectory, BackgroundImageFile));
            background = background.CreateResizedSurface(Surface.Size);

            for (int i = 0; i < codeBlockSurfaces.Length; i++) // TODO: foreach-syntax?
            {
                string filename = String.Format(Path.Combine(HackingMode.ResourceDirectory, CodeBlockImageFile), i);
                // TODO:
                //codeBlockSurfaces[i] = new Surface(filename).CreateResizedSurface(
                //        new Size(rectangle.Width / CodeBlocksHorizontal, rectangle.Height / (CodeBlocksVertical - 1)));
                codeBlockSurfaces[i] = new Surface(filename);
            }

            for (int i = 0; i < CodeBlocksHorizontal; i++)
            {
                for (int j = 0; j < CodeBlocksVertical; j++)
                {
                    Sprite block = new Sprite();

                    block.Surface = codeBlockSurfaces[random.Next(10)];
                    block.Position = new Point(i * (rectangle.Width / CodeBlocksHorizontal), j * (rectangle.Height / (CodeBlocksVertical - 1)));

                    codeBlocks[i, j] = block;
                }
            }
        }

        override public void Update(TickEventArgs args)
        {
            Surface.Blit(background);

            for (int i = 0; i < CodeBlocksHorizontal; i++) // TODO: foreach-syntax?
            {
                for (int j = 0; j < CodeBlocksVertical; j++)
                {
                    Sprite block = codeBlocks[i, j];

                    block.Y -= (int)(args.SecondsElapsed * ScrollingSpeed);

                    if (block.Y + block.Height < 0)
                    {
                        block.Y = Surface.Height;
                    }

                    Surface.Blit(block);
                }
            }
        }
    }
}
