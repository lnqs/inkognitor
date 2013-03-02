using System.Drawing;
using SdlDotNet.Graphics;
using SdlDotNet.Graphics.Sprites;

namespace Hacking
{
    public class Cursor : Sprite
    {
        private const string Filename = "Resources/Game/Cursor.png";

        public Cursor(Size size)
        {
            using (Surface sourceSurface = new Surface(Filename))
            {
                Surface = sourceSurface.CreateStretchedSurface(size);
            }

            GridX = 0;
            GridY = 1;
        }

        public int GridX { get; set; }
        public int GridY { get; set; }
    }
}
