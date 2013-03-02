using System.Drawing;

namespace Hacking
{
    public class Layout
    {
        // Matches the background-images, all resources are scaled relativ to fit in the window keeping the ratio.
        // Would be better to save it in a resource, but this has to be enough for now.
        private readonly Size ResourceGameSize = new Size(2048, 1572);
        private readonly Size CodeAreaResourceSize = new Size(1594, 994);
        private readonly Point CodeAreaResourceOffset = new Point(212, 374);
        private readonly Size CodeBlockResourceSize = new Size(400, 200);

        private Size windowSize;
        private Size gameSize;
        private Point gameOffset;
        private double scale;

        public Layout(Size windowSize_)
        {
            windowSize = windowSize_;

            // I'm pretty sure this can be done simpler o.O
            gameSize = new Size(windowSize.Width, windowSize.Height);

            double gameRatio = (double)ResourceGameSize.Width / (double)ResourceGameSize.Height;
            double windowRatio = (double)windowSize.Width / (double)windowSize.Height;

            if (gameRatio > windowRatio)
            {
                gameSize.Height = (int)(gameSize.Width / gameRatio);
            }
            else
            {
                gameSize.Width = (int)(gameSize.Height * gameRatio);
            }

            scale = (double)gameSize.Width / (double)ResourceGameSize.Width;

            gameOffset = new Point((windowSize.Width - gameSize.Width) / 2, (windowSize.Height - windowSize.Height) / 2);
        }

        public Size WindowSize { get { return windowSize; } }
        public double Scale { get { return scale; } }
        public Point GameOffset { get { return gameOffset; } }

        public Rectangle CodeArea
        {
            get
            {
                Point offset = CodeAreaResourceOffset.Scaled(scale);
                offset.X += GameOffset.X;
                offset.Y += GameOffset.Y;
                return new Rectangle(offset, CodeAreaResourceSize.Scaled(scale));
            }
        }

        public Size CodeBlockSize
        {
            get { return CodeBlockResourceSize.Scaled(scale); }
        }

        public int CodeBlockColumnCount { get { return 4; } } // TODO: Calc this instead of hardcoding
        public int CodeBlockRowCount { get { return 6; } } // TODO: Calc this instead of hardcoding

        public Point BlockIndicator
        {
            get
            {
                Point position = new Point(585, 144).Scaled(scale);
                position.X += GameOffset.X;
                position.Y += GameOffset.Y;
                return position;
            }
        }
        public Point LevelIndicator
        {
            get
            {
                Point position = new Point(1700, 190).Scaled(scale);
                position.X += GameOffset.X;
                position.Y += GameOffset.Y;
                return position;
            }
        }
    }
}
