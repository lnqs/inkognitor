using System.Drawing;
using System.IO;
using SdlDotNet.Graphics;
using SdlDotNet.Graphics.Sprites;

namespace Hacking
{
    public class Cursor : Sprite
    {
        private const string ImageFile = "Resources/Game/CodeError.png";

        public Cursor(Size size)
        {
            string filename = ImageFile;
            Surface = new Surface(filename).CreateStretchedSurface(size);

            GridX = 0;
            GridY = 1;
        }

        public int GridX { get; set; }
        public int GridY { get; set; }
    }
}
