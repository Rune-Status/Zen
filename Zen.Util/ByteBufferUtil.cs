using System.IO;
using System.Text;
using DotNetty.Buffers;
using ICSharpCode.SharpZipLib.Checksums;
using Zen.Util.Crypto;

namespace Zen.Util
{
    public static class ByteBufferUtil
    {
        private static readonly char[] Characters =
        {
            '\u20AC', '\0', '\u201A', '\u0192', '\u201E', '\u2026',
            '\u2020', '\u2021', '\u02C6', '\u2030', '\u0160', '\u2039',
            '\u0152', '\0', '\u017D', '\0', '\0', '\u2018', '\u2019',
            '\u201C', '\u201D', '\u2022', '\u2013', '\u2014', '\u02DC',
            '\u2122', '\u0161', '\u203A', '\u0153', '\0', '\u017E', '\u0178'
        };

        public static int GetCrcChecksum(IByteBuffer buffer)
        {
            var checksum = new Crc32();
            for (var i = 0; i < buffer.Capacity; i++)
                checksum.Update(buffer.GetByte(i));
            return (int) checksum.Value;
        }

        public static void WriteWorldListString(this IByteBuffer buffer, string str)
        {
            buffer.WriteByte(0);
            buffer.WriteBytes(Encoding.GetEncoding("ISO-8859-1").GetBytes(str));
            buffer.WriteByte(0);
        }

        public static byte[] GetWhirlpoolDigest(IByteBuffer buf)
        {
            var bytes = new byte[buf.Capacity];
            buf.ReadBytes(bytes);
            return Whirlpool.Crypt(bytes, 0, bytes.Length);
        }

        public static IByteBuffer ReadFully(this FileStream stream, long ptr, int count)
        {
            var data = new byte[count];

            stream.Position = ptr;
            stream.Read(data, 0, count);

            return Unpooled.WrappedBuffer(data);
        }

        public static string ReadJagexString(this IByteBuffer buffer)
        {
            var builder = new StringBuilder();

            int code;
            while ((code = buffer.ReadByte() & 0xFF) != 0)
                if (code >= 127 && code < 160)
                {
                    var character = Characters[code - 128];
                    if (character != 0)
                        builder.Append(character);
                }
                else
                {
                    builder.Append((char) code);
                }

            return builder.ToString();
        }

        public static string ReadString(this IByteBuffer buffer)
        {
            buffer.MarkReaderIndex();

            var len = 0;
            while ((buffer.ReadByte() & 0xFF) != 0)
                len++;

            buffer.ResetReaderIndex();

            var bytes = new byte[len];
            buffer.ReadBytes(bytes);
            buffer.SetReaderIndex(buffer.ReaderIndex + 1);
            return Encoding.GetEncoding("ISO-8859-1").GetString(bytes);
        }

        public static void WriteSmart(this IByteBuffer buffer, int value)
        {
            if (value < 128)
                buffer.WriteByte(value);
            else
                buffer.WriteShort(32768 + value);
        }

        public static int ReadTriByte(this IByteBuffer buffer) => ((buffer.ReadByte() & 0xff) << 16) |
                                                                  ((buffer.ReadByte() & 0xff) << 8) |
                                                                  (buffer.ReadByte() & 0xff);

        public static int ReadUnsignedByte(this IByteBuffer buffer) => buffer.ReadByte() & 0xFF;
    }
}