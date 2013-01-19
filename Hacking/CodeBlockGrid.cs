using System;
using System.Drawing;
using SdlDotNet.Core;
using SdlDotNet.Graphics;

namespace Hacking
{
    public class CodeBlockGrid : RotateableGrid<CodeBlock>
    {
        static private readonly float ScrollingSpeed = 100.0f;

        public delegate void RotatedEventHandler(object sender, EventArgs e);
        public event RotatedEventHandler Rotated;

        public Size PixelSize { get { return pixelSize; } }
        public Size BlockPixelSize { get { return blockPixelSize; } }
        public int PixelOffset { get { return (int)pixelOffset; } }

        private float pixelOffset = 0.0f;
        private Size pixelSize;
        private Size blockPixelSize;

        public CodeBlockGrid(int width, int height, Size pixelSize_)
            : base(width, height)
        {
            pixelSize = pixelSize_;
            blockPixelSize = new Size(pixelSize.Width / Width, pixelSize.Height / (Height - 1));

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    this[x, y] = new CodeBlock(BlockPixelSize);
                }
            }
        }

        public void Update(TickEventArgs args)
        {
            // we're using the direct access to the float here for more accuracy
            pixelOffset += args.SecondsElapsed * ScrollingSpeed;

            if (pixelOffset > BlockPixelSize.Height)
            {
                rotate();
                pixelOffset = 0.0f;

                if (Rotated != null)
                {
                    Rotated.Invoke(this, EventArgs.Empty);
                }
            }

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    CodeBlock block = this[x, y];
                    block.X = x * BlockPixelSize.Width;
                    block.Y = y * BlockPixelSize.Height - PixelOffset;
                }
            }
        }
    }

    public static class CodeBlockGridSurfaceExtensions
    {
        public static Rectangle Blit(this Surface surface, CodeBlockGrid codeBlockGrid, Point destinationPoint)
        {
            foreach (CodeBlock block in codeBlockGrid)
            {
                int offset = codeBlockGrid.PixelOffset;

                Point destination = new Point();
                destination.X = Math.Max(block.Position.X + destinationPoint.X, destinationPoint.X);
                destination.Y = Math.Max(block.Position.Y + destinationPoint.Y, destinationPoint.Y);

                surface.Blit(block.Surface, destination, block.CalcClippingRectangle());
            }

            return new Rectangle(destinationPoint, codeBlockGrid.PixelSize);
        }
    }
}
