using System;
using System.IO;

namespace Inkognitor
{
    public class Logger
    {
        private LogFile chatLog = new LogFile("Chat.log");

        public LogFile ChatLog { get { return chatLog; } }

        public class LogFile
        {
            private StreamWriter streamWriter;

            public LogFile(string filename)
            {
                streamWriter = new StreamWriter(filename, true);
            }

            public void Log(string message, params object[] args)
            {
                message = String.Format(message, args);
                string date = DateTime.Now.ToShortDateString();
                string time = DateTime.Now.ToLongTimeString();
                streamWriter.WriteLine("{0} {1} {2}", date, time, message);
                streamWriter.Flush();
            }
        }
    }
}
