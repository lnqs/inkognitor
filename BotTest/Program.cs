using System;
using AIMLbot;

namespace BotTest
{
    class Program
    {
        private static readonly string BotSettingsFile = "Resources/Bot/Settings.xml";
        private static readonly string UserName = "Benutzer";

        static void Main(string[] args)
        {
            Bot bot = new Bot();
            bot.loadSettings(BotSettingsFile);
            bot.loadAIMLFromFiles();
            
            while (true)
            {
                Console.Write(">> ");
                string input = Console.ReadLine();

                if (input == null)
                {
                    break;
                }

                Result response = bot.Chat(input, UserName);
                Console.WriteLine("<< {0}", response.Output);
            }
        }
    }
}
