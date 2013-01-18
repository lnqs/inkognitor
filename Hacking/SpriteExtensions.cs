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
            Rectangle rectangle = new Rectangle();
            rectangle.X = sprite.X < 0 ? sprite.X : 0;
            rectangle.Y = sprite.Y < 0 ? sprite.Y : 0;
            rectangle.Width = sprite.X < 0 ? sprite.Width - sprite.X : sprite.Width;
            rectangle.Height = sprite.Y < 0 ? sprite.Height - sprite.Y : sprite.Height;
            return rectangle;
        }
    }
}
