using System;
using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Input;

namespace Hacking
{
    public class HackingGame
    {
        public static readonly string ResourceDirectory = "Resources";

        private static readonly string WindowName = "Inkognitor";
        private static readonly int LevelCount = 10;

        private Layout layout;

        private InformationArea informationArea;
        private CodeArea codeArea;

        private int level = 1;
        private Difficulty difficulty = new Difficulty(LevelCount);

        public HackingGame(int windowWidth, int windowHeight)
        {
            layout = new Layout(windowWidth, windowHeight);
            informationArea = new InformationArea(
                    layout.LevelIndicatorPosition, layout.CodeBlockIndicatorPosition);
            codeArea = new CodeArea(layout.CodeBlockColumnCount, layout.CodeBlockRowCount,
                    layout.CodeArea, layout.CodeBlockSize);

            Video.WindowCaption = WindowName;

            Events.Tick += HandleTick;
            Events.KeyboardDown += HandleKeyboardDown;
            Events.Quit += HandleQuit;

            codeArea.SearchedBlockFound += HandleSearchedBlockFound;
            codeArea.ErrorBlockTouched += HandleErrorBlockTouched;
        }

        public event EventHandler<TickEventArgs> Tick
        {
            add { Events.Tick += value; }
            remove { Events.Tick += value; }
        }

        public event EventHandler<QuitEventArgs> Quit
        {
            add { Events.Quit += value; }
            remove { Events.Quit += value; }
        }

        public IntPtr WindowHandle { get { return Video.WindowHandle; } }

        public void Run()
        {
            Video.SetVideoMode(layout.WindowSize.Width, layout.WindowSize.Height);
            Reset();
            Events.Run();
        }

        public void Reset()
        {
            SetLevel(1);
            codeArea.Cursor.GridX = 0;
            codeArea.Cursor.GridY = 1;
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

            Video.Screen.Blit(informationArea, layout.InformationArea.Location);
            Video.Screen.Blit(codeArea, layout.CodeArea.Location);

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
