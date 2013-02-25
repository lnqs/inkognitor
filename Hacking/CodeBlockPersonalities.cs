using System;
using System.Drawing;
using System.IO;
using SdlDotNet.Graphics;

namespace Hacking
{
    public class CodeBlockPersonalities
    {
        public static readonly int Personalities = 10; // has to match the number of images
        public static readonly int PersonalityError = int.MaxValue;

        private static readonly string BlockImageFile = "code{0:D}.png";
        private static readonly string ErrorBlockImageFile = "code_error.png";

        private Surface[] surfaces = new Surface[Personalities];
        private Surface errorSurface;

        public CodeBlockPersonalities(Size size)
        {
            string filename = Path.Combine(HackingGame.ResourceDirectory, ErrorBlockImageFile);
            errorSurface = new Surface(filename);

            for (int i = 0; i < Surfaces.Length; i++)
            {
                filename = String.Format(Path.Combine(HackingGame.ResourceDirectory, BlockImageFile), i);
                surfaces[i] = new Surface(filename).CreateStretchedSurface(size);
            }
        }

        public Surface[] Surfaces { get { return surfaces; } }
        public Surface ErrorSurface { get { return errorSurface; } }
    }
}
