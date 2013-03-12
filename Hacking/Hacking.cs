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
    // TODO: This class does too much stuff. Should be splitted up.
    public class HackingGame : IDisposable
    {
        private const string WindowName = "Inkognitor";
        private const string BackgroundFile = "Resources/Game/Background.png";
        private const string FontFile = "Resources/GUI/Font.ttf";
        private const double HitsLeftFontSize = 50.0f;
        private const double StatusFontSize = 25.0f;
        private const int HitsPerLevel = 10;
        private const int MaxErrors = 10;

        private Layout layout;
        private bool resizeable;
        private bool fullscreen;
        private bool frame;

        private Surface background;
        private CodeArea codeArea;
        private TextSprite hitsLeftDisplay;
        private TextSprite statusDisplay;

        private int level;
        private int hitsLeftThisLevel;
        private int errors;
        private Difficulty difficulty = new Difficulty();

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

            Surface codeAreaBackground = new Surface(layout.CodeArea);
            codeAreaBackground.Blit(background, new Point(0, 0), layout.CodeArea.NegativeTranslated(layout.Offset));
            codeArea = new CodeArea(layout.CodeBlockCount, layout.CodeArea, layout.CodeBlockSize, codeAreaBackground);

            hitsLeftDisplay = new TextSprite(new SdlDotNet.Graphics.Font(
                    FontFile, (int)(HitsLeftFontSize * layout.Scale)));
            hitsLeftDisplay.Position = layout.HitsLeftIndicator.Location;
            hitsLeftDisplay.Color = Color.Black;
            statusDisplay = new TextSprite(new SdlDotNet.Graphics.Font(
                    FontFile, (int)(StatusFontSize * layout.Scale)));

            Video.WindowCaption = WindowName;

            Events.Tick += HandleTick;
            Events.KeyboardDown += HandleKeyboardDown;
            Events.Quit += HandleQuit;

            codeArea.SearchedBlockFound += HandleSearchedBlockFound;
            codeArea.ErrorBlockTouched += HandleWrongBlockSelected;
            codeArea.WrongBlockSelected += HandleWrongBlockSelected;
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
            lock (dispatcherQueue)
            {
                dispatcherQueue.Enqueue(() =>
                {
                    SetLevel(1);
                    codeArea.Cursor.GridX = 0;
                    codeArea.Cursor.GridY = 1;
                    SetInfoText("");
                });
            }
        }

        public void Quit()
        {
            suspensionMutex.ReleaseMutex();

            lock (dispatcherQueue)
            {
                dispatcherQueue.Enqueue(() =>
                {
                    Events.QuitApplication();
                });
            }
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
            lock (dispatcherQueue)
            {
                dispatcherQueue.Enqueue(() =>
                {
                    ShowWindow(Video.WindowHandle, 9);
                    SetForegroundWindow(Video.WindowHandle);
                });
            }
        }

        public void HideWindow()
        {
            lock (dispatcherQueue)
            {
                dispatcherQueue.Enqueue(() =>
                {
                    ShowWindow(Video.WindowHandle, 0);
                });
            }
        }

        public void ForceError(string message)
        {
            lock (dispatcherQueue)
            {
                dispatcherQueue.Enqueue(() =>
                {
                    PerformError(message);
                });
            }
        }

        private void SetLevel(int level_)
        {
            level = Math.Max(level_, 1);
            difficulty.SetForLevel(level);

            codeArea.SetRandomSearchedBlock();
            codeArea.ErrorCodeBlockProbability = difficulty.ErrorCodeBlockProbability;
            codeArea.ScrollingSpeed = difficulty.ScrollingSpeed;

            hitsLeftThisLevel = HitsPerLevel;
            hitsLeftDisplay.Text = hitsLeftThisLevel.ToString();

            errors = 0;

            try
            {
                LevelChanged.Invoke(this, new LevelChangedEventArgs(level));
            }
            catch (NullReferenceException) { }
        }

        private void SetInfoText(string message, bool warning = false)
        {
            if (warning)
            {
                statusDisplay.Color = Color.Red;
            }
            else
            {
                statusDisplay.Color = Color.Black;
            }
            statusDisplay.Text = message;
            statusDisplay.Center = layout.StatusArea.Center();
        }

        private void HandleSearchedBlockFound(object sender, EventArgs e)
        {
            hitsLeftThisLevel--;
            hitsLeftDisplay.Text = hitsLeftThisLevel.ToString();
            codeArea.SetRandomSearchedBlock();
            SetInfoText("");

            if (hitsLeftThisLevel == 0)
            {
                SetInfoText(String.Format("Neurosuppressionsebene {0} entsperrt", level)); // TODO: Shouldn't be hardcoded
                SetLevel(level + 1);
            }
        }

        private void HandleWrongBlockSelected(object sender, EventArgs e)
        {
            PerformError("Fehler!"); // TODO: Shouldn't be hardcoded
        }

        private void PerformError(string message)
        {
            errors += 1;
            SetInfoText(message, true);

            if (errors > MaxErrors)
            {
                SetInfoText(String.Format("Neurosuppressionsebene {0} gesperrt", level), true); // TODO: Shouldn't be hardcoded
                SetLevel(level);
            }
        }

        private void HandleTick(object sender, TickEventArgs e)
        {
            DispatcherQueueWork();

            // time-bound to give the framework the chance to update the window now and then
            if (suspensionMutex.WaitOne(500))
            {
                try
                {
                    // Only redraw background where necessary to same some cycles
                    Video.Screen.Blit(background, layout.BlockIndicator.Location,
                        layout.BlockIndicator.NegativeTranslated(layout.Offset));
                    Video.Screen.Blit(background, layout.HitsLeftIndicator.Location,
                        layout.HitsLeftIndicator.NegativeTranslated(layout.Offset));
                    Video.Screen.Blit(background, layout.StatusArea.Location,
                        layout.StatusArea.NegativeTranslated(layout.Offset));

                    codeArea.Update(e);
                    Video.Screen.Blit(codeArea.Surface, layout.CodeArea);

                    Video.Screen.Blit(codeArea.BlockPersonalities.Surfaces[codeArea.SearchedCodeBlock], layout.BlockIndicator);
                    Video.Screen.Blit(hitsLeftDisplay);
                    Video.Screen.Blit(statusDisplay);

                    Video.Screen.Update();
                }
                finally
                {
                    suspensionMutex.ReleaseMutex();
                }
            }
        }

        private void DispatcherQueueWork()
        {
            while (true)
            {
                Action action = null;

                lock (dispatcherQueue)
                {
                    if (dispatcherQueue.Count > 0)
                    {
                        action = dispatcherQueue.Dequeue();
                    }
                    else
                    {
                        break;
                    }
                }

                action();
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
                case Key.Return:
                case Key.KeypadEnter:
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
            hitsLeftDisplay.Dispose();
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
