using System;
using System.Drawing;
using SdlDotNet.Core;
using SdlDotNet.Graphics;

namespace Hacking
{
    public class CodeArea
    {
        private CodeBlockGrid codeBlocks;
        private Cursor cursor;
        private Rectangle rectangle;

        private Random random = new Random();

        public CodeArea(int columnCount, int rowCount, Rectangle areaRectangle, Size blockSize)
        {
            rectangle = areaRectangle;
            codeBlocks = new CodeBlockGrid(columnCount, rowCount, rectangle.Size);

            cursor = new Cursor(blockSize);

            for (int i = 0; i < codeBlocks.Height; i++)
            {
                InitializeCodeBlockRow(i);
            }
            codeBlocks.Rotated += HandleCodeBlocksRotated;
        }

        public delegate void SearchedBlockFoundHandler(object sender, EventArgs e);
        public delegate void ErrorBlockTouchedHandler(object sender, EventArgs e);
        public event SearchedBlockFoundHandler SearchedBlockFound;
        public event ErrorBlockTouchedHandler ErrorBlockTouched;

        public enum Direction
        {
            Left, Right, Up, Down
        }

        public int SearchedCodeBlock { get; set; }
        public CodeBlockGrid CodeBlocks { get { return codeBlocks; } }
        public Cursor Cursor { get { return cursor; } }
        public int MaxErrorsPerCodeBlockRow { get; set; }
        public float ErrorCodeBlockProbability { get; set; }
        public float ScrollingSpeed { get { return codeBlocks.ScrollingSpeed; } set { codeBlocks.ScrollingSpeed = value; } }

        public void Update(TickEventArgs e)
        {
            codeBlocks.Update(e);
            cursor.Position = codeBlocks[cursor.GridX, cursor.GridY].Position;
            cursor.X += rectangle.X;
            cursor.Y += rectangle.Y;
        }

        public void MoveCursor(Direction direction)
        {
            switch (direction)
            {
                case Direction.Left:
                    Cursor.GridX = Math.Max(Cursor.GridX - 1, 0);
                    break;
                case Direction.Right:
                    Cursor.GridX = Math.Min(Cursor.GridX + 1, codeBlocks.Width - 1);
                    break;
                case Direction.Up:
                    Cursor.GridY = Math.Max(Cursor.GridY - 1, 1);
                    break;
                case Direction.Down:
                    Cursor.GridY = Math.Min(Cursor.GridY + 1, codeBlocks.Height - 2);
                    break;
            }

            CheckErrorTouched();
        }

        public void SetRandomSearchedBlock()
        {
            SearchedCodeBlock = random.Next(CodeBlock.Personalities);
        }

        public void CheckBlockFound()
        {
            if (codeBlocks[cursor.GridX, cursor.GridY].Personality == SearchedCodeBlock)
            {
                if (SearchedBlockFound != null)
                {
                    SearchedBlockFound.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private void CheckErrorTouched()
        {
            if (codeBlocks[cursor.GridX, cursor.GridY].Personality == CodeBlock.PersonalityError)
            {
                if (ErrorBlockTouched != null)
                {
                    ErrorBlockTouched.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private void InitializeCodeBlockRow(int row)
        {
            int errors = 0;

            for (int i = 0; i < codeBlocks.Width; i++)
            {
                CodeBlock block = codeBlocks[i, row];

                if (errors < MaxErrorsPerCodeBlockRow && random.NextDouble() < ErrorCodeBlockProbability)
                {
                    block.Personality = CodeBlock.PersonalityError;
                    errors += 1;
                }
                else
                {
                    block.Personality = random.Next(CodeBlock.Personalities);
                }
            }
        }

        private void HandleCodeBlocksRotated(object sender, EventArgs e)
        {
            if (cursor.GridY > 1)
            {
                cursor.GridY -= 1;
            }

            InitializeCodeBlockRow(codeBlocks.Height - 1);
            CheckErrorTouched();
        }
    }

    public static class CodeAreaSurfaceExtensions
    {
        public static Rectangle Blit(this Surface surface, CodeArea codeArea, Point destinationPoint)
        {
            Rectangle clippedBlocks = surface.Blit(codeArea.CodeBlocks, destinationPoint);
            Rectangle clippedCursor = surface.Blit(
                    codeArea.Cursor.Surface, codeArea.Cursor.Position, codeArea.Cursor.CalcClippingRectangle());

            return Rectangle.Union(clippedBlocks, clippedCursor);
        }
    }
}
