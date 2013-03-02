using System.Drawing;

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

        public static Rectangle Scaled(this Rectangle rectangle, double scale)
        {
            return new Rectangle(rectangle.Location.Scaled(scale), rectangle.Size.Scaled(scale));
        }

        public static Point Translated(this Point point, Point offset)
        {
            return new Point(point.X + offset.X, point.Y + offset.Y);
        }

        public static Rectangle Translated(this Rectangle rectangle, Point offset)
        {
            return new Rectangle(rectangle.Location.Translated(offset), rectangle.Size);
        }

        public static Point NegativeTranslated(this Point point, Point offset)
        {
            return new Point(point.X - offset.X, point.Y - offset.Y);
        }

        public static Rectangle NegativeTranslated(this Rectangle rectangle, Point offset)
        {
            return new Rectangle(rectangle.Location.NegativeTranslated(offset), rectangle.Size);
        }

        public static Point Center(this Rectangle rectangle)
        {
            return new Point(rectangle.X + rectangle.Width / 2, rectangle.Y + rectangle.Height / 2);
        }
    }
}
