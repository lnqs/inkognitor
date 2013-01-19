using System.Drawing;

namespace Hacking
{
    public static class Layout
    {
        public static readonly Size WindowSize = new Size(800, 600);

        public static readonly Rectangle InformationArea = new Rectangle(
                0, 0,
                WindowSize.Width, WindowSize.Height / 4);
        public static readonly Rectangle CodeArea = new Rectangle(
                0, InformationArea.Height,
                WindowSize.Width, WindowSize.Height - InformationArea.Height);

        public static readonly Point LevelIndicatorPosition = new Point(InformationArea.Width - 50, 0);
        public static readonly Point CodeBlockIndicatorPosition = new Point(0, 0);

        public static readonly int CodeBlockColumnCount = 4;
        public static readonly int CodeBlockRowCount = 6;
        public static readonly Size CodeBlockSize = new Size(CodeArea.Width / CodeBlockColumnCount, CodeArea.Height / CodeBlockRowCount);
    }
}
