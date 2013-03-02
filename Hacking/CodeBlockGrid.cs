using System;
using System.Drawing;
using SdlDotNet.Core;
using SdlDotNet.Graphics;

namespace Hacking
{
    public class CodeBlockGrid : RotateableGrid<CodeBlock>, IDisposable
    {
        private float pixelOffset = 0.0f;
        private Size pixelSize;
        private Size blockPixelSize;

        private bool disposed = false;

        public CodeBlockGrid(int width, int height,
                Size pixelSize_, Size blockPixelSize_,
                CodeBlockPersonalities personalities) : base(width, height)
        {
            pixelSize = pixelSize_;
            blockPixelSize = blockPixelSize_;

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    this[x, y] = new CodeBlock(personalities);
                }
            }
        }

        ~CodeBlockGrid()
        {
            Dispose();
        }

        public delegate void RotatedEventHandler(object sender, EventArgs e);
        public event RotatedEventHandler Rotated;

        public Size PixelSize { get { return pixelSize; } }
        public Size BlockPixelSize { get { return blockPixelSize; } }
        public int PixelOffset { get { return (int)pixelOffset; } }
        public float ScrollingSpeed { get; set; }

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

        public void Dispose()
        {
            if (!disposed)
            {
                foreach (CodeBlock member in this)
                {
                    member.Dispose();
                }

                disposed = true;
            }

            GC.SuppressFinalize(this);
        }
    }
}
