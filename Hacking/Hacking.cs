using System;
using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Input;

namespace Hacking
{
    public class HackingMode
    {
        public static readonly string ResourceDirectory = "Resources";

        private static readonly string WindowName = "Inkognitor";
        private static readonly int LevelCount = 10;

        private InformationArea informationArea = new InformationArea(
                Layout.LevelIndicatorPosition, Layout.CodeBlockIndicatorPosition);
        private CodeArea codeArea = new CodeArea(
                Layout.CodeBlockColumnCount, Layout.CodeBlockRowCount,
                Layout.CodeArea, Layout.CodeBlockSize);

        private int level = 1;
        private Difficulty difficulty = new Difficulty(LevelCount);

        public HackingMode()
        {
            Video.SetVideoMode(Layout.WindowSize.Width, Layout.WindowSize.Height);
            Video.WindowCaption = WindowName;

            Events.Tick += HandleTick;
            Events.KeyboardDown += HandleKeyboardDown;
            Events.Quit += HandleQuit;

            codeArea.SearchedBlockFound += HandleSearchedBlockFound;
            codeArea.ErrorBlockTouched += HandleErrorBlockTouched;
        }

        public void start()
        {
            SetLevel(1);
            Events.Run();
        }

        private void SetLevel(int level_)
        {
            level = level_;
            difficulty.SetForLevel(level);
            codeArea.SetRandomSearchedBlock();
            codeArea.ErrorCodeBlockProbability = difficulty.ErrorCodeBlockProbability;
            codeArea.MaxErrorsPerCodeBlockRow = difficulty.MaxErrorsPerCodeBlockRow;
            codeArea.ScrollingSpeed = difficulty.ScrollingSpeed;
            informationArea.DisplayedLevel.Text = level.ToString();
            informationArea.DisplayedCodeBlock = CodeBlock.Surfaces[codeArea.SearchedCodeBlock];
        }

        private void HandleSearchedBlockFound(object sender, EventArgs e)
        {
            SetLevel(level + 1);
        }

        private void HandleErrorBlockTouched(object sender, EventArgs e)
        {
            SetLevel(level - 1);
        }

        private void HandleTick(object sender, TickEventArgs e)
        {
            informationArea.Update(e);
            codeArea.Update(e);

            Video.Screen.Blit(informationArea, Layout.InformationArea.Location);
            Video.Screen.Blit(codeArea, Layout.CodeArea.Location);

            Video.Screen.Update();
        }

        private void HandleKeyboardDown(object sender, KeyboardEventArgs e)
        {
            switch (e.Key)
            {
                case Key.LeftArrow:
                    codeArea.MoveCursor(CodeArea.Direction.Left);
                    break;
                case Key.RightArrow:
                    codeArea.MoveCursor(CodeArea.Direction.Right);
                    break;
                case Key.UpArrow:
                    codeArea.MoveCursor(CodeArea.Direction.Up);
                    break;
                case Key.DownArrow:
                    codeArea.MoveCursor(CodeArea.Direction.Down);
                    break;
                case Key.Space:
                    codeArea.CheckBlockFound();
                    break;
            }
        }

        private void HandleQuit(object sender, QuitEventArgs e)
        {
            Events.QuitApplication();
        }
    }
}
