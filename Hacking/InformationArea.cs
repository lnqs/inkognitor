using System;
using System.Drawing;
using System.IO;
using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Graphics.Sprites;

namespace Hacking
{
    public class InformationArea
    {
        private TextSprite levelDisplay = new TextSprite(new SdlDotNet.Graphics.Font("Resources/GUI/Font.ttf", 40));

        public InformationArea(Point levelPosition, Point codeBlockPosition)
        {
            LevelPosition = levelPosition;
            CodeBlockPosition = codeBlockPosition;
        }

        public TextSprite DisplayedLevel { get { return levelDisplay; } set { levelDisplay = value; } }
        public Surface DisplayedCodeBlock { get; set; }
        public Point LevelPosition { get; set; }
        public Point CodeBlockPosition { get; set; }

        public void Update(TickEventArgs e) { }
    }

    public static class InformationAreaSurfaceExtensions
    {
        public static Rectangle Blit(this Surface surface, InformationArea informationArea, Point destinationPoint)
        {
            Point destination = new Point();

            destination.X = informationArea.LevelPosition.X + destinationPoint.X;
            destination.Y = informationArea.LevelPosition.Y + destinationPoint.Y;
            Rectangle clippedLevel = surface.Blit(informationArea.DisplayedLevel, destination);

            destination.X = informationArea.CodeBlockPosition.X + destinationPoint.X;
            destination.Y = informationArea.CodeBlockPosition.Y + destinationPoint.Y;
            Rectangle clippedBlock = surface.Blit(informationArea.DisplayedCodeBlock, destination);

            return Rectangle.Union(clippedLevel, clippedBlock);
        }
    }
}
