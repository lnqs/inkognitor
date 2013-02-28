using System;
using System.Drawing;
using SdlDotNet.Graphics;

namespace Hacking
{
    public class CodeBlockPersonalities : IDisposable
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
            using (Surface sourceSurface = new Surface(filename))
            {
                errorSurface = sourceSurface.CreateStretchedSurface(size);
            }

            for (int i = 0; i < Surfaces.Length; i++)
            {
                filename = String.Format(BlockImageFile, i);
                using (Surface sourceSurface = new Surface(filename))
                {
                    surfaces[i] = sourceSurface.CreateStretchedSurface(size);
                }
            }
        }

        public void Dispose()
        {
            foreach (Surface surface in surfaces)
            {
                surface.Dispose();
            }
            errorSurface.Dispose();
            GC.SuppressFinalize(this);
        }

        public Surface[] Surfaces { get { return surfaces; } }
        public Surface ErrorSurface { get { return errorSurface; } }
    }
}
