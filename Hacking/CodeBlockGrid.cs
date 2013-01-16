using System.Drawing;
using SdlDotNet.Core;
using SdlDotNet.Graphics;

namespace Hacking
{
    public class CodeBlockGrid : RotateableGrid<CodeBlock>
    {
        static private readonly float ScrollingSpeed = 100.0f;

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
                    this[i][j] = new CodeBlock(BlockPixelSize);
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
            }

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    CodeBlock block = this[i][j];
                    block.X = i * BlockPixelSize.Width;
                    block.Y = j * BlockPixelSize.Height - PixelOffset;
                }
            }
        }
    }

    public static class SurfaceExtensions
    {
        public static Rectangle Blit(this Surface surface, CodeBlockGrid codeBlockGrid)
        {
            for (int i = 0; i < codeBlockGrid.Rows; i++)
            {
                for (int j = 0; j < codeBlockGrid.Columns; j++)
                {
                    int offset = codeBlockGrid.PixelOffset;
                    CodeBlock block = codeBlockGrid[i][j];
                    surface.Blit(block.Surface, block.Position);
                }
            }

            return new Rectangle(0, 0, codeBlockGrid.PixelSize.Width, codeBlockGrid.PixelSize.Height);
        }
    }
}
