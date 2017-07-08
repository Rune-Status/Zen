using System.IO;

namespace Zen.Util
{
    public static class StreamUtil
    {
        public static void WriteInt(this MemoryStream stream, int value)
        {
            stream.WriteByte((byte) (value >> 24));
            stream.WriteByte((byte) (value >> 16));
            stream.WriteByte((byte) (value >> 8));
            stream.WriteByte((byte) value);
        }

        public static void Write(this MemoryStream stream, byte[] buffer) => stream.Write(buffer, 0, buffer.Length);
    }
}