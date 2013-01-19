using System;
using System.IO;

namespace Inkognitor
{
    class DataCorruptingWaveMemoryStream : MemoryStream
    {
        private static readonly int HeaderSize = 45;

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
                byte[] broken = new byte[count];
                Buffer.BlockCopy(buffer, offset, broken, 0, count);

                for (int i = 1; i < broken.Length; i++)
                {
                    broken[i] += broken[i - 1];
                }

                base.Write(broken, 0, broken.Length);
            }
        }
    }
}
