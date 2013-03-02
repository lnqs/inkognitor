using System.Drawing;
using SdlDotNet.Graphics.Sprites;

namespace Hacking
{
    public static class Extensions
    {
        public static Point Scaled(this Point point, double scale)
        {
            return new Point((int)(point.X * scale), (int)(point.Y * scale));
        }

        public static Size Scaled(this Size size, double scale)
        {
            return new Size((int)(size.Width * scale), (int)(size.Height * scale));
        }
    }
}
