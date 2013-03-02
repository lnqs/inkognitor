using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Graphics.Sprites;
using SdlDotNet.Input;

namespace Hacking
{
    public class HackingGame : IDisposable
    {
        private const string WindowName = "Inkognitor";
        private const string BackgroundFile = "Resources/Game/Background.png";
        private const int LevelCount = 10;

        private Layout layout;
        private bool resizeable;
        private bool fullscreen;
        private bool frame;

        private Surface background;
        private CodeArea codeArea;
        private TextSprite levelDisplay = new TextSprite(new SdlDotNet.Graphics.Font("Resources/GUI/Font.ttf", 40));

        private int level = 1;
        private Difficulty difficulty = new Difficulty(LevelCount);

        private Mutex suspensionMutex = new Mutex();
        private Queue<Action> dispatcherQueue = new Queue<Action>();

        public HackingGame(int windowWidth, int windowHeight,
                bool resizeable_, bool fullscreen_, bool frame_)
        {
            layout = new Layout(new Size(windowWidth, windowHeight));
            resizeable = resizeable_;
            fullscreen = fullscreen_;
            frame = frame_;

            using (Surface sourceSurface = new Surface(BackgroundFile))
            {
                background = sourceSurface.CreateScaledSurface(layout.Scale);
            }

            codeArea = new CodeArea(layout.CodeBlockCount.Width, layout.CodeBlockCount.Height, layout.CodeArea, layout.CodeBlockSize);

            Video.WindowCaption = WindowName;

            Events.Tick += HandleTick;
            Events.KeyboardDown += HandleKeyboardDown;
            Events.Quit += HandleQuit;

            codeArea.SearchedBlockFound += HandleSearchedBlockFound;
            codeArea.ErrorBlockTouched += HandleErrorBlockTouched;
        }

        public event EventHandler<LevelChangedEventArgs> LevelChanged;

        public void Run()
        {
            Video.SetVideoMode(layout.WindowSize.Width,
                    layout.WindowSize.Height, resizeable, false, fullscreen, frame);
            Video.Screen.Fill(Color.Black);
            Video.Screen.Blit(background, layout.Game);

            if (!frame) // for some reason giving false to SetVideoMode doesn't work
            {
                SetWindowLong(Video.WindowHandle, -16, 0); // delete all styles
                SetWindowPos(Video.WindowHandle, IntPtr.Zero, 0, 0, 0, 0, 0x0045);
            }

            Reset();
            Events.Run();
        }

        public void Reset()
        {
            dispatcherQueue.Enqueue(() =>
            {
                SetLevel(1);
                codeArea.Cursor.GridX = 0;
                codeArea.Cursor.GridY = 1;
            });
        }

        public void Quit()
        {
            suspensionMutex.ReleaseMutex();

            dispatcherQueue.Enqueue(() =>
            {
                Events.QuitApplication();
            });
        }

        public void Suspend()
        {
            suspensionMutex.WaitOne();
        }

        public void Resume()
        {
            suspensionMutex.ReleaseMutex();
        }

        public void ShowWindow()
        {
            dispatcherQueue.Enqueue(() =>
            {
                ShowWindow(Video.WindowHandle, 9);
                SetForegroundWindow(Video.WindowHandle);
            });
        }

        public void HideWindow()
        {
            dispatcherQueue.Enqueue(() =>
            {
                ShowWindow(Video.WindowHandle, 0);
            });
        }

        private void SetLevel(int level_)
        {
            level = level_;
            difficulty.SetForLevel(level);
            codeArea.SetRandomSearchedBlock();
            codeArea.ErrorCodeBlockProbability = difficulty.ErrorCodeBlockProbability;
            codeArea.MaxErrorsPerCodeBlockRow = difficulty.MaxErrorsPerCodeBlockRow;
            codeArea.ScrollingSpeed = difficulty.ScrollingSpeed;
            levelDisplay.Text = level.ToString();

            try
            {
                LevelChanged.Invoke(this, new LevelChangedEventArgs(level));
            }
            catch (NullReferenceException) { }
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
            if (dispatcherQueue.Count > 0)
            {
                dispatcherQueue.Dequeue()();
            }

            // time-bound to give the framework the chance to update the window now and then
            if (suspensionMutex.WaitOne(500))
            {
                try
                {
                    // TODO: Optimize this. We only need to re-blit the areas where searched block
                    //       and level-indicator are drawn
                    Video.Screen.Blit(background, layout.Game);

                    codeArea.Update(e);
                    Video.Screen.Blit(codeArea.Surface, codeArea.Area);

                    Video.Screen.Blit(codeArea.BlockPersonalities.Surfaces[codeArea.SearchedCodeBlock], layout.BlockIndicator);
                    Video.Screen.Blit(levelDisplay, layout.LevelIndicator.Location);

                    Video.Screen.Update();
                }
                finally
                {
                    suspensionMutex.ReleaseMutex();
                }
            }
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

        public void Dispose()
        {
            codeArea.Dispose();
            levelDisplay.Dispose();
            GC.SuppressFinalize(this);
        }

        public class LevelChangedEventArgs : EventArgs
        {
            public LevelChangedEventArgs(int level)
            {
                Level = level;
            }

            public int Level { get; set; }
        }

        [DllImport("user32.dll")]
        private static extern int ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);
    }
}
