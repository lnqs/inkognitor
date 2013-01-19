using System.IO;

namespace Inkognitor
{
    class DataCorruptingWaveMemoryStream : MemoryStream
    {
        private static readonly int HeaderSize = 44;

        public override void Write(byte[] buffer, int offset, int count)
        {
            byte[] broken = new byte[buffer.Length];
            buffer.CopyTo(broken, 0);

            if (Position > HeaderSize)
            {
                for (int i = 1; i < broken.Length; i++)
                {
                    broken[i] += broken[i - 1];
                }
            }

            base.Write(broken, offset, count);
        }
    }
}
