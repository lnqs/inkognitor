using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;

namespace Inkognitor
{
    public class Files
    {
        private const string Filename = "Resources/Files/Files.xml";

        private XmlDocument document = new XmlDocument();

        public Files()
        {
            document.Load(Filename);
        }

        public string Prefix { get { return GetText("/Files/Prefix"); } }
        public string UnknownToken { get { return GetText("/Files/UnknownToken"); } }
        public string UnknownCommand { get { return GetText("/Files/UnknownCommand"); } }
        public string LoadingFile { get { return GetText("/Files/LoadingFile"); } }
        public string DefaultText { get { return GetText("/Files/DefaultText"); } }
        public string WaitPreText { get { return GetText("/Files/Wait/Pre"); } }
        public int WaitDelay { get { return Int32.Parse(GetText("/Files/Wait/Delay")); } }
        public string EndPreText { get { return GetText("/Files/End/Pre"); } }
        public int EndDelay { get { return Int32.Parse(GetText("/Files/End/Delay")); } }
        public string EndPostText { get { return GetText("/Files/End/Post"); } }

        public string[] MaintainanceCommandNames
        {
            get
            {
                XmlNodeList nodes = document.GetElementsByTagName("MaintainanceCommand");
                string[] commands = new string[nodes.Count];
                for (int i = 0; i < nodes.Count; i++)
                {
                    commands[i] = nodes[i].Attributes["Name"].InnerText.ToUpper();
                }

                return commands;
            }
        }

        public Dictionary<string, MaintainanceMode.Command> MaintainanceCommands
        {
            get
            {
                Dictionary<string, MaintainanceMode.Command> commands = new Dictionary<string, MaintainanceMode.Command>();

                XmlNodeList nodes = document.GetElementsByTagName("MaintainanceCommand");
                foreach (XmlNode node in nodes)
                {
                    string name = node.Attributes["Name"].InnerText.ToUpper();
                    string type = node.Attributes["Type"].InnerText;

                    if (type == "ShowCommands")
                    {
                        commands[name] = new MaintainanceMode.ShowCommandsCommand();
                    }
                    else if (type == "PrintDotted")
                    {
                        commands[name] = new MaintainanceMode.PrintDottetCommand(
                            CleanSpaces(node.SelectSingleNode("Pre").InnerText),
                            Int32.Parse(node.SelectSingleNode("Delay").InnerText),
                            CleanSpaces(node.SelectSingleNode("Post").InnerText));
                    }
                    else if (type == "NextMode")
                    {
                        commands[name] = new MaintainanceMode.NextModeCommand(
                            CleanSpaces(node.SelectSingleNode("Pre").InnerText),
                            Int32.Parse(node.SelectSingleNode("Delay").InnerText));
                    }
                }

                return commands;
            }
        }

        public string GetFile(string token)
        {
            XmlNodeList nodes = document.GetElementsByTagName("File");
            foreach (XmlNode node in nodes)
            {
                if (node.Attributes["Token"].InnerText.ToUpper() == token.ToUpper())
                {
                    return CleanSpaces(node.InnerText);
                }
            }

            throw new ElementException("No file for token " + token);
        }

        private string GetText(string path)
        {
            return CleanSpaces(document.SelectSingleNode(path).InnerText);
        }

        private string CleanSpaces(string text)
        {
            return Regex.Replace(text, " +", " ");
        }

        public class ElementException : Exception
        {
            public ElementException() : base() { }
            public ElementException(string message) : base(message) { }
            public ElementException(string message, Exception innerException) : base(message, innerException) { }
        }
    }
}
