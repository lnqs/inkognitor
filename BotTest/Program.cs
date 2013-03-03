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

                // Yep, this sucks hard. But for some reason the Substitutions.xml doesn't work
                // as I expected and there's almost no time left till Kettensturm :/
                input = input
                    .Replace("ä", "ae")
                    .Replace("ö", "oe")
                    .Replace("ü", "ue")
                    .Replace("Ä", "Ae")
                    .Replace("Ö", "Oe")
                    .Replace("Ü", "Ue")
                    .Replace("ß", "ss");

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
