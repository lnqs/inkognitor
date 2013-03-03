using System;
using AIMLbot;

namespace BotTest
{
    class Program
    {
        private static readonly string BotSettingsFile = "Resources/Bot/Settings.xml";
        private static readonly string UserID = "Benutzer";

        static void Main(string[] args)
        {
            Bot bot = new Bot();
            bot.loadSettings(BotSettingsFile);
            bot.loadAIMLFromFiles();

            User user = new User(UserID, bot);
            
            while (true)
            {
                Console.Write(">> ");
                string input = Console.ReadLine();

                if (input == null)
                {
                    break;
                }

                Request request = new Request(input, user, bot);
                Result response = bot.Chat(request);
                Console.WriteLine("<< {0}", response.Output);
            }
        }
    }
}
