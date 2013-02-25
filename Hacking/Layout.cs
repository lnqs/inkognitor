using System.Drawing;

namespace Hacking
{
    public class Layout
    {
        private Size windowSize;

        public Layout(int windowWidth, int windowHeight)
        {
            windowSize = new Size(windowWidth, windowHeight);
        }

        public Size WindowSize { get { return windowSize; } }

        public Rectangle InformationArea
        {
            get
            {
                return new Rectangle(0, 0, WindowSize.Width, WindowSize.Height / 4);
            }
        }

        public Rectangle CodeArea
        {
            get
            {
                return new Rectangle(0, InformationArea.Height, WindowSize.Width,
                        WindowSize.Height - InformationArea.Height);
            }
        }

        public Point LevelIndicatorPosition { get { return new Point(InformationArea.Width - 50, 0); } }
        public Point CodeBlockIndicatorPosition { get { return new Point(0, 0); } }

        public int CodeBlockColumnCount { get { return 4; } }
        public int CodeBlockRowCount { get { return 6; } }

        public Size CodeBlockSize
        {
            get
            {
                return new Size(CodeArea.Width / CodeBlockColumnCount,
                        CodeArea.Height / CodeBlockRowCount);
            }
        }
    }
}
