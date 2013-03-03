using System;
using System.Globalization;
using System.IO;
using System.Media;
using System.Speech.AudioFormat;
using System.Speech.Synthesis;
using AIMLbot;

namespace Inkognitor
{
    class Personality<BufferType> : IDisposable where BufferType : MemoryStream, new()
    {
        private const string BotSettingsFile = "Resources/Bot/Settings.xml";
        private const string UserID = "Benutzer";
        private readonly SpeechAudioFormatInfo AudioFormat
                = new SpeechAudioFormatInfo(44100, AudioBitsPerSample.Sixteen, AudioChannel.Stereo);

        private SoundPlayer soundPlayer = new SoundPlayer();
        private SpeechSynthesizer synthesizer = new SpeechSynthesizer();
        private Bot bot;
        private User user;

        public Personality()
        {
            synthesizer.SelectVoiceByHints(VoiceGender.Male, VoiceAge.Senior, 0, new CultureInfo("de-DE"));
            synthesizer.SetOutputToNull();

            bot = new Bot();
            user = new User(UserID, bot);
            bot.loadSettings(BotSettingsFile);
            bot.loadAIMLFromFiles();
        }

        public string Respond(string text)
        {
            // Yep, this sucks hard. But for some reason the Substitutions.xml doesn't work
            // as I expected and there's almost no time left till Kettensturm :/
            text = text
                .Replace("ä", "ae")
                .Replace("ö", "oe")
                .Replace("ü", "ue")
                .Replace("Ä", "Ae")
                .Replace("Ö", "Oe")
                .Replace("Ü", "Ue")
                .Replace("ß", "ss");

            Result response = bot.Chat(new Request(text, user, bot));
            Say(response.Output);
            return response.Output;
        }

        public void Say(string text)
        {
            using (MemoryStream stream = new BufferType())
            {
                synthesizer.SetOutputToWaveStream(stream);
                synthesizer.Speak(text);

                stream.Seek(0, SeekOrigin.Begin);
                soundPlayer.Stream = stream;
                soundPlayer.PlaySync();
                soundPlayer.Stream = null;
                synthesizer.SetOutputToNull();
            }
        }

        public void Dispose()
        {
            soundPlayer.Dispose();
            synthesizer.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
