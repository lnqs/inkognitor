using System;
using System.Drawing;
using SdlDotNet.Graphics;

namespace Hacking
{
    public class CodeBlockPersonalities
    {
        public const int Personalities = 10; // has to match the number of images
        public const int PersonalityError = int.MaxValue;

        private const string BlockImageFile = "Resources/Game/Code{0:D}.png";
        private const string ErrorBlockImageFile = "Resources/Game/CodeError.png";

        private Surface[] surfaces = new Surface[Personalities];
        private Surface errorSurface;

        public CodeBlockPersonalities(Size size)
        {
            string filename = ErrorBlockImageFile;
            errorSurface = new Surface(filename).CreateStretchedSurface(size);

            for (int i = 0; i < Surfaces.Length; i++)
            {
                filename = String.Format(BlockImageFile, i);
                surfaces[i] = new Surface(filename).CreateStretchedSurface(size);
            }
        }

        public Surface[] Surfaces { get { return surfaces; } }
        public Surface ErrorSurface { get { return errorSurface; } }
    }
}
