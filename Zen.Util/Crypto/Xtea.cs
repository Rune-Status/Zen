using System;
using DotNetty.Buffers;

namespace Zen.Util.Crypto
{
    public class Xtea
    {
        public const uint GoldenRatio = 0x9E3779B9;
        public const int Rounds = 32;

        public static void Decipher(IByteBuffer buffer, int start, int end, int[] key)
        {
            if (key.Length != 4)
                throw new ArgumentException();

            var numQuads = (end - start) / 8;
            for (var i = 0; i < numQuads; i++)
            {
                var sum = unchecked(GoldenRatio * Rounds);
                var v0 = buffer.GetInt(start + i * 8);
                var v1 = buffer.GetInt(start + i * 8 + 4);
                for (var j = 0; j < Rounds; j++)
                {
                    v1 -= (int) ((((v0 << 4) ^ (int) ((uint) v0 >> 5)) + v0) ^ (sum + key[(sum >> 11) & 3]));
                    sum -= GoldenRatio;
                    v0 -= (int) ((((v1 << 4) ^ (int) ((uint) v1 >> 5)) + v1) ^ (sum + key[sum & 3]));
                }
                buffer.SetInt(start + i * 8, v0);
                buffer.SetInt(start + i * 8 + 4, v1);
            }
        }
    }
}