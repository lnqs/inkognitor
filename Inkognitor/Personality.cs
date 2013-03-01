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
        private const string UserName = "Benutzer";
        private readonly SpeechAudioFormatInfo AudioFormat
                = new SpeechAudioFormatInfo(44100, AudioBitsPerSample.Sixteen, AudioChannel.Stereo);

        private SoundPlayer soundPlayer = new SoundPlayer();
        private SpeechSynthesizer synthesizer = new SpeechSynthesizer();
        private Bot bot = new Bot();

        public Personality()
        {
            synthesizer.SelectVoiceByHints(VoiceGender.Male, VoiceAge.Senior, 0, new CultureInfo("de-DE"));
            synthesizer.SetOutputToNull();

            bot.loadSettings(BotSettingsFile);
            bot.loadAIMLFromFiles();
        }

        public string Respond(string text)
        {
            Result response = bot.Chat(text, UserName);
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
