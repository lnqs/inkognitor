using System.Globalization;
using System.IO;
using System.Media;
using System.Speech.AudioFormat;
using System.Speech.Synthesis;
using AIMLbot;

namespace Inkognitor
{
    class Personality
    {
        private static readonly string BotSettingsFile = "Bot/Settings.xml";
        private static readonly string UserName = "Benutzer";
        private static readonly SpeechAudioFormatInfo AudioFormat
                = new SpeechAudioFormatInfo(44100, AudioBitsPerSample.Sixteen, AudioChannel.Stereo);

        private SoundPlayer soundPlayer = new SoundPlayer();
        private SpeechSynthesizer synthesizer = new SpeechSynthesizer();
        private Bot bot = new Bot();

        public Personality()
        {
            Broken = true;

            synthesizer.SelectVoiceByHints(VoiceGender.Male, VoiceAge.Senior, 0, new CultureInfo("de-DE"));
            synthesizer.SetOutputToNull();

            bot.loadSettings(BotSettingsFile);
            bot.loadAIMLFromFiles();
        }

        [CommandListener("synthesizer_broken", Description="get/sets if the synthesizer is broken")]
        public bool Broken { get; set; }

        public void Respond(string text)
        {
            MemoryStream stream = new MemoryStream();

            if (Broken)
            {
                stream = new DataCorruptingWaveMemoryStream();
            }
            else
            {
                stream = new MemoryStream();
            }

            synthesizer.SetOutputToWaveStream(stream);

            Result response = bot.Chat(text, UserName);
            synthesizer.Speak(response.Output);

            stream.Seek(0, SeekOrigin.Begin);
            soundPlayer.Stream = stream;
            soundPlayer.PlaySync();
            soundPlayer.Stream = null;
            synthesizer.SetOutputToNull();
        }
    }
}
