using System;
using System.Drawing;
using System.IO;
using SdlDotNet.Graphics;
using SdlDotNet.Graphics.Sprites;

namespace Hacking
{
    public class CodeBlock : Sprite
    {
        private static readonly string BlockImageFile = "code{0:D}.png";
        private static readonly string ErrorBlockImageFile = "code_error.png";
        public static readonly int Personalities = 10; // has to match the number of images
        public static readonly int PersonalityError = int.MaxValue;
        public static Surface[] Surfaces = new Surface[Personalities];
        private static Surface ErrorSurface;

        static CodeBlock()
        {
            string filename = Path.Combine(HackingMode.ResourceDirectory, ErrorBlockImageFile);
            ErrorSurface = new Surface(filename);

            for (int i = 0; i < Surfaces.Length; i++)
            {
                filename = String.Format(Path.Combine(HackingMode.ResourceDirectory, BlockImageFile), i);
                Surfaces[i] = new Surface(filename);
            }
        }

        public int Personality
        {
            get { return personality; }
            set
            {
                personality = value;
                Surface = value == PersonalityError ? ErrorSurface : Surfaces[value];
            }
        }

        private int personality;

        public CodeBlock(Size size)
        {
            // TODO: Resize the surface
        }
    }
}
