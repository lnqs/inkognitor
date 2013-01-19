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

        private static readonly string WindowName = "Inkognitor";
        private static readonly int MaxErrorsPerCodeBlockRow = 3;
        private static readonly float ErrorCodeBlockProbability = 0.1f;

        private InformationArea informationArea = new InformationArea(
                Layout.LevelIndicatorPosition, Layout.CodeBlockIndicatorPosition);

        private Random random = new Random();

        private CodeBlockGrid codeBlocks;
        private Cursor cursor;

        private int level = 1;

        private int searchedCodeBlock = 0;

        public void start()
        {
            codeBlocks = new CodeBlockGrid(Layout.CodeBlockColumnCount, Layout.CodeBlockRowCount, Layout.CodeArea.Size);
            cursor = new Cursor(Layout.CodeBlockSize);
            InitializeCodeBlocks();
            informationArea.DisplayedCodeBlock = CodeBlock.Surfaces[searchedCodeBlock];

            Video.SetVideoMode(Layout.WindowSize.Width, Layout.WindowSize.Height);
            Video.WindowCaption = WindowName;

            Events.Tick += HandleTick;
            Events.KeyboardDown += HandleKeyboardDown;
            Events.Quit += HandleQuit;

            codeBlocks.Rotated += HandleCodeBlocksRotated;

            Events.Run();
        }

        private void HandleTick(object sender, TickEventArgs e)
        {
            informationArea.Update(e);
            codeBlocks.Update(e);
            cursor.Position = codeBlocks[cursor.GridX, cursor.GridY].Position;
            cursor.X += Layout.CodeArea.X;
            cursor.Y += Layout.CodeArea.Y;

            Video.Screen.Blit(informationArea, Layout.InformationArea.Location);
            Video.Screen.Blit(codeBlocks, Layout.CodeArea.Location);
            Video.Screen.Blit(cursor.Surface, cursor.Position, cursor.CalcClippingRectangle());

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

            InitializeCodeBlockRow(Layout.CodeBlockRowCount - 1);
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
                informationArea.DisplayedLevel.Text = level.ToString();
                searchedCodeBlock = random.Next(CodeBlock.Personalities);
                informationArea.DisplayedCodeBlock = CodeBlock.Surfaces[searchedCodeBlock];
            }
        }

        private void CheckErrorCodeFound()
        {
            if (codeBlocks[cursor.GridX, cursor.GridY].Personality == CodeBlock.PersonalityError)
            {
                level -= 1;
                informationArea.DisplayedLevel.Text = level.ToString();
                searchedCodeBlock = random.Next(CodeBlock.Personalities);
                informationArea.DisplayedCodeBlock = CodeBlock.Surfaces[searchedCodeBlock];
                InitializeCodeBlocks();
            }
        }
    }
}
