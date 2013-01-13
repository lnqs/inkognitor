using System.Globalization;
using System.Speech.Synthesis;
using AIMLbot;

namespace Inkognitor
{
    class Personality
    {
        private static readonly string BotSettingsFile = "Bot/Settings.xml";
        private static readonly string UserName = "Benutzer";

        private SpeechSynthesizer synthesizer = new SpeechSynthesizer();
        private Bot bot = new Bot();

        public Personality()
        {
            synthesizer.SelectVoiceByHints(VoiceGender.Male, VoiceAge.Senior, 0, new CultureInfo("de-DE"));

            bot.loadSettings(BotSettingsFile);
            bot.loadAIMLFromFiles();
        }

        public void Respond(string text)
        {
            Result response = bot.Chat(text, UserName);
            synthesizer.Speak(response.Output);
        }
    }
}
