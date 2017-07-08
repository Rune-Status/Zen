using System;

namespace Zen.Util.Crypto
{
    public class Whirlpool
    {
        protected internal const int R = 10;

        private const string sbox =
            "\u1823\uc6E8\u87B8\u014F\u36A6\ud2F5\u796F\u9152" +
            "\u60Bc\u9B8E\uA30c\u7B35\u1dE0\ud7c2\u2E4B\uFE57" +
            "\u1577\u37E5\u9FF0\u4AdA\u58c9\u290A\uB1A0\u6B85" +
            "\uBd5d\u10F4\ucB3E\u0567\uE427\u418B\uA77d\u95d8" +
            "\uFBEE\u7c66\udd17\u479E\ucA2d\uBF07\uAd5A\u8333" +
            "\u6302\uAA71\uc819\u49d9\uF2E3\u5B88\u9A26\u32B0" +
            "\uE90F\ud580\uBEcd\u3448\uFF7A\u905F\u2068\u1AAE" +
            "\uB454\u9322\u64F1\u7312\u4008\uc3Ec\udBA1\u8d3d" +
            "\u9700\ucF2B\u7682\ud61B\uB5AF\u6A50\u45F3\u30EF" +
            "\u3F55\uA2EA\u65BA\u2Fc0\udE1c\uFd4d\u9275\u068A" +
            "\uB2E6\u0E1F\u62d4\uA896\uF9c5\u2559\u8472\u394c" +
            "\u5E78\u388c\ud1A5\uE261\uB321\u9c1E\u43c7\uFc04" +
            "\u5199\u6d0d\uFAdF\u7E24\u3BAB\ucE11\u8F4E\uB7EB" +
            "\u3c81\u94F7\uB913\u2cd3\uE76E\uc403\u5644\u7FA9" +
            "\u2ABB\uc153\udc0B\u9d6c\u3174\uF646\uAc89\u14E1" +
            "\u163A\u6909\u70B6\ud0Ed\ucc42\u98A4\u285c\uF886";

        private static readonly long[,] C = new long[8, 256];
        private static readonly long[] Rc = new long[R + 1];

        protected internal byte[] BitLength = new byte[32];
        protected internal long[] Block = new long[8];
        protected internal byte[] Buffer = new byte[64];

        protected internal int BufferBits;
        protected internal int BufferPos;

        protected internal long[] Hash = new long[8];

        protected internal long[] K = new long[8];
        protected internal long[] L = new long[8];
        protected internal long[] State = new long[8];

        static Whirlpool()
        {
            for (var x = 0; x < 256; x++)
            {
                var c = sbox[x / 2];
                long v1 = (x & 1) == 0 ? (int) ((uint) c >> 8) : c & 0xff;
                var v2 = v1 << 1;
                if (v2 >= 0x100L)
                    v2 ^= 0x11dL;

                var v4 = v2 << 1;
                if (v4 >= 0x100L)
                    v4 ^= 0x11dL;

                var v5 = v4 ^ v1;
                var v8 = v4 << 1;
                if (v8 >= 0x100L)
                    v8 ^= 0x11dL;

                var v9 = v8 ^ v1;
                C[0, x] =
                    (v1 << 56) | (v1 << 48) | (v4 << 40) | (v1 << 32) |
                    (v8 << 24) | (v5 << 16) | (v2 << 8) | v9;

                for (var t = 1; t < 8; t++)
                    C[t, x] = (long) ((ulong) C[t - 1, x] >> 8) | (C[t - 1, x] << 56);
            }

            Rc[0] = 0L;
            for (var r = 1; r <= R; r++)
            {
                var i = 8 * (r - 1);
                Rc[r] =
                    (C[0, i] & unchecked((long) 0xff00000000000000L)) ^
                    (C[1, i + 1] & 0x00ff000000000000L) ^
                    (C[2, i + 2] & 0x0000ff0000000000L) ^
                    (C[3, i + 3] & 0x000000ff00000000L) ^
                    (C[4, i + 4] & 0x00000000ff000000L) ^
                    (C[5, i + 5] & 0x0000000000ff0000L) ^
                    (C[6, i + 6] & 0x000000000000ff00L) ^
                    (C[7, i + 7] & 0x00000000000000ffL);
            }
        }

        public static byte[] Crypt(byte[] data, int off, int len)
        {
            byte[] source;
            if (off <= 0)
            {
                source = data;
            }
            else
            {
                source = new byte[len];
                for (var i = 0; i < len; i++)
                    source[i] = data[off + i];
            }
            var whirlpool = new Whirlpool();
            whirlpool.NessieInit();
            whirlpool.NessieAdd(source, len * 8);
            var digest = new byte[64];
            whirlpool.NessieFinalize(digest);
            return digest;
        }

        protected internal virtual void ProcessBuffer()
        {
            for (int i = 0,
                    j = 0;
                i < 8;
                i++, j += 8)
                Block[i] =
                    ((long) Buffer[j] << 56) ^
                    ((Buffer[j + 1] & 0xffL) << 48) ^
                    ((Buffer[j + 2] & 0xffL) << 40) ^
                    ((Buffer[j + 3] & 0xffL) << 32) ^
                    ((Buffer[j + 4] & 0xffL) << 24) ^
                    ((Buffer[j + 5] & 0xffL) << 16) ^
                    ((Buffer[j + 6] & 0xffL) << 8) ^
                    (Buffer[j + 7] & 0xffL);

            for (var i = 0; i < 8; i++)
                State[i] = Block[i] ^ (K[i] = Hash[i]);

            for (var r = 1; r <= R; r++)
            {
                for (var i = 0; i < 8; i++)
                {
                    L[i] = 0L;
                    for (int t = 0, s = 56; t < 8; t++, s -= 8)
                        L[i] ^= C[t, (int) (long) ((ulong) K[(i - t) & 7] >> s) & 0xff];
                }

                for (var i = 0; i < 8; i++)
                    K[i] = L[i];
                K[0] ^= Rc[r];

                for (var i = 0; i < 8; i++)
                {
                    L[i] = K[i];
                    for (int t = 0, s = 56; t < 8; t++, s -= 8)
                        L[i] ^= C[t, (int) (long) ((ulong) State[(i - t) & 7] >> s) & 0xff];
                }

                for (var i = 0; i < 8; i++)
                    State[i] = L[i];
            }

            for (var i = 0; i < 8; i++)
                Hash[i] ^= State[i] ^ Block[i];
        }

        public virtual void NessieInit()
        {
            for (var id = 0; id < BitLength.Length; id++)
                BitLength[id] = 0;
            BufferBits = BufferPos = 0;
            Buffer[0] = 0;
            for (var id = 0; id < Hash.Length; id++)
                Hash[id] = 0L;
        }

        public virtual void NessieAdd(byte[] source, long sourceBits)
        {
            var sourcePos = 0;
            var sourceGap = (8 - ((int) sourceBits & 7)) & 7;
            var bufferRem = BufferBits & 7;
            int b;
            var value = sourceBits;
            for (int i = 31,
                    carry = 0;
                i >= 0;
                i--)
            {
                carry += (BitLength[i] & 0xff) + ((int) value & 0xff);
                BitLength[i] = (byte) carry;
                carry = (int) ((uint) carry >> 8);
                value = (long) ((ulong) value >> 8);
            }
            while (sourceBits > 8)
            {
                b = ((source[sourcePos] << sourceGap) & 0xff) |
                    (int) ((uint) (source[sourcePos + 1] & 0xff) >> (8 - sourceGap));
                if (b < 0 || b >= 256)
                    throw new Exception("LOGIC ERROR");
                Buffer[BufferPos++] |= (byte) (int) ((uint) b >> bufferRem);
                BufferBits += 8 - bufferRem;
                if (BufferBits == 512)
                {
                    ProcessBuffer();
                    BufferBits = BufferPos = 0;
                }
                Buffer[BufferPos] = unchecked((byte) ((b << (8 - bufferRem)) & 0xff));
                BufferBits += bufferRem;
                sourceBits -= 8;
                sourcePos++;
            }
            if (sourceBits > 0)
            {
                b = (source[sourcePos] << sourceGap) & 0xff;
                Buffer[BufferPos] |= (byte) (int) ((uint) b >> bufferRem);
            }
            else
            {
                b = 0;
            }
            if (bufferRem + sourceBits < 8)
            {
                BufferBits += (int) sourceBits;
            }
            else
            {
                BufferPos++;
                BufferBits += 8 - bufferRem;
                sourceBits -= 8 - bufferRem;
                if (BufferBits == 512)
                {
                    ProcessBuffer();
                    BufferBits = BufferPos = 0;
                }
                Buffer[BufferPos] = unchecked((byte) ((b << (8 - bufferRem)) & 0xff));
                BufferBits += (int) sourceBits;
            }
        }

        public virtual void NessieFinalize(byte[] digest)
        {
            Buffer[BufferPos] |= (byte) (int) ((uint) 0x80 >> (BufferBits & 7));
            BufferPos++;
            if (BufferPos > 32)
            {
                while (BufferPos < 64)
                    Buffer[BufferPos++] = 0;
                ProcessBuffer();
                BufferPos = 0;
            }
            while (BufferPos < 32)
                Buffer[BufferPos++] = 0;
            Array.Copy(BitLength, 0, Buffer, 32, 32);
            ProcessBuffer();
            for (int i = 0,
                    j = 0;
                i < 8;
                i++, j += 8)
            {
                var h = Hash[i];
                digest[j] = (byte) (long) ((ulong) h >> 56);
                digest[j + 1] = (byte) (long) ((ulong) h >> 48);
                digest[j + 2] = (byte) (long) ((ulong) h >> 40);
                digest[j + 3] = (byte) (long) ((ulong) h >> 32);
                digest[j + 4] = (byte) (long) ((ulong) h >> 24);
                digest[j + 5] = (byte) (long) ((ulong) h >> 16);
                digest[j + 6] = (byte) (long) ((ulong) h >> 8);
                digest[j + 7] = (byte) h;
            }
        }

        public virtual void NessieAdd(string source)
        {
            if (source.Length > 0)
            {
                var data = new byte[source.Length];
                for (var i = 0; i < source.Length; i++)
                    data[i] = (byte) source[i];
                NessieAdd(data, 8 * data.Length);
            }
        }
    }
}