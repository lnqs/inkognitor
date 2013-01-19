using System;
using System.Drawing;
using System.IO;
using SdlDotNet.Graphics;
using SdlDotNet.Graphics.Sprites;

namespace Hacking
{
    public class CodeBlock : Sprite
    {
        public static readonly int Personalities = 10; // has to match the number of images
        public static readonly int PersonalityError = int.MaxValue;

        private static readonly string BlockImageFile = "code{0:D}.png";
        private static readonly string ErrorBlockImageFile = "code_error.png";

        private static Surface[] surfaces = new Surface[Personalities];
        private static Surface errorSurface;

        private int personality;

        static CodeBlock()
        {
            string filename = Path.Combine(HackingMode.ResourceDirectory, ErrorBlockImageFile);
            errorSurface = new Surface(filename);

            for (int i = 0; i < Surfaces.Length; i++)
            {
                filename = String.Format(Path.Combine(HackingMode.ResourceDirectory, BlockImageFile), i);
                surfaces[i] = new Surface(filename);
            }
        }

        public CodeBlock(Size size)
        {
            // TODO: Resize the surface
        }

        public static Surface[] Surfaces { get { return surfaces; } }
        public static Surface ErrorSurface { get { return errorSurface; } }

        public int Personality
        {
            get { return personality; }
            set
            {
                personality = value;
                Surface = value == PersonalityError ? ErrorSurface : Surfaces[value];
            }
        }
    }
}
