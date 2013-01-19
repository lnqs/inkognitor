using System;
using System.Drawing;
using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Graphics.Sprites;
using SdlDotNet.Input;
using System.IO;

namespace Hacking
{
    // TODO: Too much stuff in this class, split it up
    public class HackingMode
    {
        public static readonly string ResourceDirectory = "Resources";

        private static readonly Size WindowSize = new Size(800, 600);
        private static readonly string WindowName = "Inkognitor";
        private static readonly int CodeBlockColumns = 4;
        private static readonly int CodeBlockRows = 6;
        private static readonly int MaxErrorsPerCodeBlockRow = 3;
        private static readonly float ErrorCodeBlockProbability = 0.1f;
        private static readonly float InfoAreaHeight = 0.25f;
        private static readonly Rectangle CodeAreaRectangle = new Rectangle(
                new Point(0, (int)(WindowSize.Height * InfoAreaHeight)),
                new Size(WindowSize.Width, (int)(WindowSize.Height * (1.0f - InfoAreaHeight))));
        private static readonly Point LevelDisplayPosition = new Point(WindowSize.Width - 50, 0);
        private static readonly Point SearchedCodeBlockDisplayPosition = new Point(0, 0);

        private Random random = new Random();
        SdlDotNet.Graphics.Font font = new SdlDotNet.Graphics.Font(
                Path.Combine(System.Environment.GetEnvironmentVariable("windir"), "fonts", "arial.ttf"), 40);

        private CodeBlockGrid codeBlocks;
        private Cursor cursor;

        private int level = 1;
        private TextSprite levelDisplaySprite;

        private int searchedCodeBlock = 0;

        public void start()
        {
            codeBlocks = new CodeBlockGrid(CodeBlockColumns, CodeBlockRows, CodeAreaRectangle.Size);
            cursor = new Cursor(codeBlocks.BlockPixelSize);
            InitializeCodeBlocks();

            levelDisplaySprite = new TextSprite(level.ToString(), font);

            Video.SetVideoMode(WindowSize.Width, WindowSize.Height);
            Video.WindowCaption = WindowName;

            Events.Tick += HandleTick;
            Events.KeyboardDown += HandleKeyboardDown;
            Events.Quit += HandleQuit;

            codeBlocks.Rotated += HandleCodeBlocksRotated;

            Events.Run();
        }

        private void HandleTick(object sender, TickEventArgs e)
        {
            codeBlocks.Update(e);
            cursor.Position = codeBlocks[cursor.GridX, cursor.GridY].Position;
            cursor.X += CodeAreaRectangle.X;
            cursor.Y += CodeAreaRectangle.Y;

            Video.Screen.Blit(codeBlocks, CodeAreaRectangle.Location);
            Video.Screen.Blit(cursor.Surface, cursor.Position, cursor.CalcClippingRectangle());
            Video.Screen.Blit(CodeBlock.Surfaces[searchedCodeBlock], SearchedCodeBlockDisplayPosition);
            Video.Screen.Blit(levelDisplaySprite, LevelDisplayPosition);

            Video.Screen.Update();
        }

        private void HandleKeyboardDown(object sender, KeyboardEventArgs e)
        {
            // TODO: refactor this method
            switch (e.Key)
            {
                case Key.LeftArrow:
                    cursor.GridX = cursor.GridX > 0 ? cursor.GridX - 1 : cursor.GridX;
                    CheckErrorCodeFound();
                    break;
                case Key.RightArrow:
                    cursor.GridX = cursor.GridX < codeBlocks.Width - 1 ? cursor.GridX + 1 : cursor.GridX;
                    CheckErrorCodeFound();
                    break;
                case Key.UpArrow:
                    cursor.GridY = cursor.GridY > 1 ? cursor.GridY - 1 : cursor.GridY;
                    CheckErrorCodeFound();
                    break;
                case Key.DownArrow:
                    cursor.GridY = cursor.GridY < codeBlocks.Height - 2 ? cursor.GridY + 1 : cursor.GridY;
                    CheckErrorCodeFound();
                    break;
                case Key.Space:
                    CheckCodeBlockFound();
                    break;
            }
        }

        private void HandleQuit(object sender, QuitEventArgs e)
        {
            Events.QuitApplication();
        }

        private void HandleCodeBlocksRotated(object sender, EventArgs e)
        {
            if (cursor.GridY > 1)
            {
                cursor.GridY -= 1;
            }

            CheckErrorCodeFound();

            InitializeCodeBlockRow(CodeBlockRows - 1);
        }

        private void InitializeCodeBlocks()
        {
            for (int i = 0; i < codeBlocks.Height; i++)
            {
                InitializeCodeBlockRow(i);
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

        private void CheckCodeBlockFound()
        {
            if (searchedCodeBlock == codeBlocks[cursor.GridX, cursor.GridY].Personality)
            {
                level += 1;
                levelDisplaySprite.Text = level.ToString();
                searchedCodeBlock = random.Next(CodeBlock.Personalities);
            }
        }

        private void CheckErrorCodeFound()
        {
            if (codeBlocks[cursor.GridX, cursor.GridY].Personality == CodeBlock.PersonalityError)
            {
                level -= 1;
                levelDisplaySprite.Text = level.ToString();
                searchedCodeBlock = random.Next(CodeBlock.Personalities);
                InitializeCodeBlocks();
            }
        }
    }
}
