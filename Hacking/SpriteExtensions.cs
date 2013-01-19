using System.Drawing;
using SdlDotNet.Graphics.Sprites;

namespace Hacking
{
    public static class SpriteExtensions
    {
        public static Rectangle CalcClippingRectangle(this Sprite sprite)
        {
            // SDL.net happily draws to negative destinations, so we have
            // this extension-method to calculate a source-rectagle for proper
            // clipping
            Rectangle rectangle = new Rectangle(0, 0, sprite.Width, sprite.Height);

            if (sprite.X < 0)
            {
                rectangle.X -= sprite.X;
                rectangle.Width += sprite.X;
            }

            if (sprite.Y < 0)
            {
                rectangle.Y -= sprite.Y;
                rectangle.Height += sprite.Y;
            }

            return rectangle;
        }
    }
}
