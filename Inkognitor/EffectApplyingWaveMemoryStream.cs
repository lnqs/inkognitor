using System;
using System.IO;

namespace Inkognitor
{
    public abstract class EffectApplyingWaveMemoryStream : MemoryStream
    {
        private const int HeaderSize = 45;

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (Position < HeaderSize)
            {
                int toWrite = Math.Min(count, HeaderSize - (int)Position);

                base.Write(buffer, offset, toWrite);

                offset += toWrite;
                count -= toWrite;
            }

            if (count > 0)
            {
                byte[] modified = new byte[count];
                Buffer.BlockCopy(buffer, offset, modified, 0, count);

                ApplyEffect(modified);

                base.Write(modified, 0, modified.Length);
            }
        }

        protected abstract void ApplyEffect(byte[] buffer);
    }
}
