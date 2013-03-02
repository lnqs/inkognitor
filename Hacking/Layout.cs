using System;
using System.Drawing;
using System.Xml;

namespace Hacking
{
    public class Layout
    {
        private const string Filename = "Resources/Game/Layout.xml";

        private XmlDocument document = new XmlDocument();
        private Size windowSize;
        private double scale;
        private Point offset;

        public Layout(Size windowSize_)
        {
            windowSize = windowSize_;
            document.Load(Filename);

            // I'm pretty sure this can be done simpler o.O
            Size resourceGameSize = NodeToSize("/Game");
            Size gameSize = new Size(windowSize.Width, windowSize.Height);

            double gameRatio = (double)resourceGameSize.Width / (double)resourceGameSize.Height;
            double windowRatio = (double)windowSize.Width / (double)windowSize.Height;
 
            if (gameRatio > windowRatio)
            {
                gameSize.Height = (int)(gameSize.Width / gameRatio);
            }
            else
            {
                gameSize.Width = (int)(gameSize.Height * gameRatio);
            }

            scale = (double)gameSize.Width / (double)resourceGameSize.Width;
            offset = new Point((windowSize.Width - gameSize.Width) / 2, (windowSize.Height - windowSize.Height) / 2);
        }

        public double Scale { get { return scale; } }
        public Point Offset { get { return offset; } }
        public Size WindowSize { get { return windowSize; } }
        public Rectangle Game { get { return NodeToRectangle("/Game").Scaled(scale).Translated(offset); } }
        public Rectangle BlockIndicator { get { return NodeToRectangle("/Game/BlockIndicator").Scaled(scale).Translated(offset); } }
        public Rectangle LevelIndicator { get { return NodeToRectangle("/Game/LevelIndicator").Scaled(scale).Translated(offset); } }
        public Rectangle CodeArea { get { return NodeToRectangle("/Game/CodeArea").Scaled(scale).Translated(offset); } }
        public Size CodeBlockCount { get { return NodeToSize("/Game/CodeArea/Grid"); } }
        public Size CodeBlockSize { get { return NodeToSize("/Game/CodeArea/Block").Scaled(scale); } }

        private Point NodeToPoint(string path)
        {
            XmlNode node = document.SelectSingleNode(path);
            return new Point(
                Int32.Parse(node.Attributes["x"].InnerText),
                Int32.Parse(node.Attributes["y"].InnerText));
        }

        private Size NodeToSize(string path)
        {
            XmlNode node = document.SelectSingleNode(path);
            return new Size(
                Int32.Parse(node.Attributes["width"].InnerText),
                Int32.Parse(node.Attributes["height"].InnerText));
        }

        private Rectangle NodeToRectangle(string path)
        {
            XmlNode node = document.SelectSingleNode(path);
            return new Rectangle(NodeToPoint(path), NodeToSize(path));
        }
    }
}
