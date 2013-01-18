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

        public CodeBlockGrid(int columnCount, int rowCount, Size pixelSize_)
            : base(columnCount, rowCount)
        {
            pixelSize = pixelSize_;
            blockPixelSize = new Size(pixelSize.Width / Rows, pixelSize.Height / (Columns - 1));

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    this[i, j] = new CodeBlock(BlockPixelSize);
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

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    CodeBlock block = this[i, j];
                    block.X = i * BlockPixelSize.Width;
                    block.Y = j * BlockPixelSize.Height - PixelOffset;
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
                destination.X = destinationPoint.X + block.Position.X;
                destination.Y = destinationPoint.Y + block.Position.Y;

                surface.Blit(block.Surface, destination, block.CalcClippingRectangle());
            }

            return new Rectangle(destinationPoint.X, destinationPoint.Y, codeBlockGrid.PixelSize.Width, codeBlockGrid.PixelSize.Height);
        }

    }
}
