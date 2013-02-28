using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Input;

namespace Hacking
{
    public class HackingGame : IDisposable
    {
        private const string WindowName = "Inkognitor";
        private const int LevelCount = 10;

        private Layout layout;
        private bool resizeable;
        private bool fullscreen;
        private bool frame;

        private InformationArea informationArea;
        private CodeArea codeArea;

        private int level = 1;
        private Difficulty difficulty = new Difficulty(LevelCount);

        private Mutex suspensionMutex = new Mutex();
        private Queue<Action> dispatcherQueue = new Queue<Action>();

        public HackingGame(int windowWidth, int windowHeight,
                bool resizeable_, bool fullscreen_, bool frame_)
        {
            layout = new Layout(windowWidth, windowHeight);
            resizeable = resizeable_;
            fullscreen = fullscreen_;
            frame = frame_;

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

        public event EventHandler<LevelChangedEventArgs> LevelChanged;

        public void Run()
        {
            Video.SetVideoMode(layout.WindowSize.Width,
                    layout.WindowSize.Height, resizeable, false, fullscreen, frame);

            if (!frame) // for some reason giving false to SetVideoMode doesn't work
            {
                SetWindowLong(Video.WindowHandle, -16, 0); // delete all styles
                SetWindowPos(Video.WindowHandle, new IntPtr(0), 0, 0, 0, 0, 0x0045);
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
            informationArea.DisplayedLevel.Text = level.ToString();
            informationArea.DisplayedCodeBlock = codeArea.BlockPersonalities.Surfaces[codeArea.SearchedCodeBlock];

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
                    informationArea.Update(e);
                    codeArea.Update(e);

                    Video.Screen.Blit(informationArea, layout.InformationArea.Location);
                    Video.Screen.Blit(codeArea, layout.CodeArea.Location);

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
            informationArea.Dispose();
            codeArea.Dispose();
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
