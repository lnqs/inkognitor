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
        private Surface background;
        private Surface surface;

        private Random random = new Random();

        public CodeArea(Size blockCount, Rectangle area, Size blockSize, Surface background_)
        {
            blockPersonalities = new CodeBlockPersonalities(blockSize);
            background = background_;
            surface = new Surface(area);
            blocks = new CodeBlockGrid(blockCount.Width, blockCount.Height, area.Size, blockSize, blockPersonalities);

            cursor = new Cursor(blockSize);

            for (int i = 0; i < blocks.Height; i++)
            {
                InitializeCodeBlockRow(i);
            }
            blocks.Rotated += HandleCodeBlocksRotated;
        }

        public event EventHandler SearchedBlockFound;
        public event EventHandler WrongBlockSelected;
        public event EventHandler ErrorBlockTouched;

        public enum Direction
        {
            Left, Right, Up, Down
        }

        public int SearchedCodeBlock { get; set; }
        public Cursor Cursor { get { return cursor; } }
        public double ErrorCodeBlockProbability { get; set; }
        public double ScrollingSpeed { get { return blocks.ScrollingSpeed; } set { blocks.ScrollingSpeed = value; } }
        public CodeBlockPersonalities BlockPersonalities { get { return blockPersonalities; } }
        public Surface Surface { get { return surface; } }

        public void Update(TickEventArgs e)
        {
            blocks.Update(e);
            cursor.Position = blocks[cursor.GridX, cursor.GridY].Position;

            surface.Blit(background);
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
            else
            {
                if (WrongBlockSelected != null)
                {
                    WrongBlockSelected.Invoke(this, EventArgs.Empty);
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
            for (int i = 0; i < blocks.Width; i++)
            {
                CodeBlock block = blocks[i, row];

                bool mayBeError = true;
                for (int j = Math.Max(i - 1, 0); j <= Math.Min(i + 1, blocks.Width - 1); j++)
                {
                    for (int k = Math.Max(row - 1, 0); k <= Math.Min(row + 1, blocks.Height - 1); k++)
                    {
                        if (j == i && k == row)
                        {
                            continue;
                        }

                        if (blocks[j, k].Personality == CodeBlockPersonalities.PersonalityError)
                        {
                            mayBeError = false;
                            break;
                        }
                    }
                }

                if (mayBeError && random.NextDouble() < ErrorCodeBlockProbability)
                {
                    block.Personality = CodeBlockPersonalities.PersonalityError;
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
            background.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
