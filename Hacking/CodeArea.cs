using System;
using System.Drawing;
using SdlDotNet.Core;
using SdlDotNet.Graphics;

namespace Hacking
{
    public class CodeArea : IDisposable
    {
        private CodeBlockPersonalities blockPersonalities;
        private CodeBlockGrid blocks;
        private Cursor cursor;
        private Rectangle rectangle;
        private Surface surface;

        private Random random = new Random();

        public CodeArea(int columnCount, int rowCount, Rectangle areaRectangle, Size blockSize)
        {
            Size blockPixelSize = new Size(
                    areaRectangle.Width / columnCount, areaRectangle.Height / (rowCount - 1));
            blockPersonalities = new CodeBlockPersonalities(blockPixelSize);
            rectangle = areaRectangle;
            surface = new Surface(areaRectangle);
            blocks = new CodeBlockGrid(columnCount, rowCount, rectangle.Size, blockPixelSize, blockPersonalities);

            cursor = new Cursor(blockSize);

            for (int i = 0; i < blocks.Height; i++)
            {
                InitializeCodeBlockRow(i);
            }
            blocks.Rotated += HandleCodeBlocksRotated;
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
        public CodeBlockGrid CodeBlocks { get { return blocks; } }
        public Cursor Cursor { get { return cursor; } }
        public int MaxErrorsPerCodeBlockRow { get; set; }
        public float ErrorCodeBlockProbability { get; set; }
        public float ScrollingSpeed { get { return blocks.ScrollingSpeed; } set { blocks.ScrollingSpeed = value; } }
        public CodeBlockPersonalities BlockPersonalities { get { return blockPersonalities; } }
        public Rectangle Area { get { return rectangle; } }
        public Surface Surface { get { return surface; } }

        public void Update(TickEventArgs e)
        {
            blocks.Update(e);
            cursor.Position = blocks[cursor.GridX, cursor.GridY].Position;

            surface.Fill(Color.Black); // No transparency here, it's too slow
            foreach (CodeBlock block in blocks)
            {
                surface.Blit(block.Surface, block.Position);
            }
            surface.Blit(Cursor.Surface, Cursor.Position);
        }

        public void MoveCursor(Direction direction)
        {
            switch (direction)
            {
                case Direction.Left:
                    Cursor.GridX = Math.Max(Cursor.GridX - 1, 0);
                    break;
                case Direction.Right:
                    Cursor.GridX = Math.Min(Cursor.GridX + 1, blocks.Width - 1);
                    break;
                case Direction.Up:
                    Cursor.GridY = Math.Max(Cursor.GridY - 1, 1);
                    break;
                case Direction.Down:
                    Cursor.GridY = Math.Min(Cursor.GridY + 1, blocks.Height - 2);
                    break;
            }

            CheckErrorTouched();
        }

        public void SetRandomSearchedBlock()
        {
            SearchedCodeBlock = random.Next(CodeBlockPersonalities.Personalities);
        }

        public void CheckBlockFound()
        {
            if (blocks[cursor.GridX, cursor.GridY].Personality == SearchedCodeBlock)
            {
                if (SearchedBlockFound != null)
                {
                    SearchedBlockFound.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private void CheckErrorTouched()
        {
            if (blocks[cursor.GridX, cursor.GridY].Personality == CodeBlockPersonalities.PersonalityError)
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

            for (int i = 0; i < blocks.Width; i++)
            {
                CodeBlock block = blocks[i, row];

                if (errors < MaxErrorsPerCodeBlockRow && random.NextDouble() < ErrorCodeBlockProbability)
                {
                    block.Personality = CodeBlockPersonalities.PersonalityError;
                    errors += 1;
                }
                else
                {
                    block.Personality = random.Next(CodeBlockPersonalities.Personalities);
                }
            }
        }

        private void HandleCodeBlocksRotated(object sender, EventArgs e)
        {
            if (cursor.GridY > 1)
            {
                cursor.GridY -= 1;
            }

            InitializeCodeBlockRow(blocks.Height - 1);
            CheckErrorTouched();
        }

        public void Dispose()
        {
            blockPersonalities.Dispose();
            blocks.Dispose();
            cursor.Dispose();
            surface.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
