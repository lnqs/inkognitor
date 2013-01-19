namespace Inkognitor
{
    class DataCorruptingWaveMemoryStream : EffectApplyingWaveMemoryStream
    {
        protected override void ApplyEffect(byte[] buffer)
        {
            for (int i = 1; i < buffer.Length; i++)
            {
                buffer[i] += buffer[i - 1];
            }
        }
    }
}
