using System.Xml;

namespace Inkognitor
{
    public class DisplayTextXMLDocument
    {
        private string title;
        private string text;

        public DisplayTextXMLDocument(string filename)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(filename);
            title = xml.GetElementsByTagName("Title")[0].InnerText;
            text = xml.GetElementsByTagName("Text")[0].InnerText;
        }

        public string Title { get { return title; } }
        public string Text { get { return text; } }
    }
}
