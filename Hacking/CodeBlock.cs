using System;
using System.Drawing;
using System.IO;
using SdlDotNet.Graphics;
using SdlDotNet.Graphics.Sprites;

namespace Hacking
{
    public class CodeBlock : Sprite
    {
        private static Surface[] Surfaces = new Surface[10]; // has to match the number of images
        private static readonly string BlockImageFile = "code{0:D}.png";

        static CodeBlock()
        {
            for (int i = 0; i < Surfaces.Length; i++)
            {
                string filename = String.Format(Path.Combine(HackingMode.ResourceDirectory, BlockImageFile), i);
                Surfaces[i] = new Surface(filename);
            }
        }

        public CodeBlock(Size size)
        {
            Surface = Surfaces[1]; // TODO: Hardcoding the first sucks :o)
            // TODO: Also remember that we need to resize it
        }
    }
}
