using System.Drawing;
using System.IO;
using SdlDotNet.Graphics;
using SdlDotNet.Graphics.Sprites;

namespace Hacking
{
    public class Cursor : Sprite
    {
        private static readonly string ImageFile = "code_error.png";

        public int GridX { get; set; }
        public int GridY { get; set; }

        public Cursor(Size size)
        {
            string filename = Path.Combine(HackingMode.ResourceDirectory, ImageFile);
            Surface = new Surface(filename); // TODO: Resize

            GridX = 0;
            GridY = 1;
        }
    }
}
