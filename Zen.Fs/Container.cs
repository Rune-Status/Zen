using System.IO;
using DotNetty.Buffers;
using Zen.Util;
using Zen.Util.Crypto;

namespace Zen.Fs
{
    public class Container
    {
        public const int CompressionNone = 0;
        public const int CompressionBzip2 = 1;
        public const int CompressionGzip = 2;

        public Container(int type, IByteBuffer data, int version = -1)
        {
            Type = type;
            Data = data;
            Version = version;
        }

        public int Version { get; }
        public IByteBuffer Data { get; }
        public int Type { get; }

        public bool Versioned => Version != -1;

        public static Container Decode(IByteBuffer buffer, int[] keys = null)
        {
            var type = buffer.ReadByte() & 0xFF;
            var length = buffer.ReadInt();

            if (keys != null && keys[0] != 0 && keys[1] != 0 && keys[2] != 0 && keys[3] != 0)
                Xtea.Decipher(buffer, 5, length + (type == CompressionNone ? 5 : 9), keys);

            int version;
            if (type == CompressionNone)
            {
                var temp = new byte[length];
                buffer.ReadBytes(temp);
                var data = Unpooled.WrappedBuffer(temp);

                version = -1;
                if (buffer.ReadableBytes >= 2)
                    version = buffer.ReadShort();

                return new Container(type, data, version);
            }

            var uncompressedLength = buffer.ReadInt();

            var compressed = new byte[length];
            buffer.ReadBytes(compressed);

            byte[] uncompressed;
            switch (type)
            {
                case CompressionBzip2:
                    uncompressed = CompressionUtil.Bunzip2(compressed);
                    break;
                case CompressionGzip:
                    uncompressed = CompressionUtil.Gunzip(compressed);
                    break;
                default:
                    throw new IOException("Invalid compression type");
            }

            if (uncompressed.Length != uncompressedLength)
                throw new IOException("Length mismatch");

            version = -1;
            if (buffer.ReadableBytes >= 2)
                version = buffer.ReadShort();

            return new Container(type, Unpooled.WrappedBuffer(uncompressed), version);
        }

        public IByteBuffer Encode()
        {
            var data = Data;

            var bytes = new byte[data.Capacity];
            data.MarkReaderIndex();
            data.ReadBytes(bytes);
            data.ResetReaderIndex();

            byte[] compressed;
            switch (Type)
            {
                case CompressionNone:
                    compressed = bytes;
                    break;
                case CompressionGzip:
                    compressed = CompressionUtil.Gzip(bytes);
                    break;
                case CompressionBzip2:
                    compressed = CompressionUtil.Bzip2(bytes);
                    break;
                default:
                    throw new IOException("Invalid compression type");
            }

            var header = 5 + (Type == CompressionNone ? 0 : 4) + (Versioned ? 2 : 0);
            var buf = Unpooled.Buffer(header + compressed.Length);

            buf.WriteByte((byte) Type);
            buf.WriteInt(compressed.Length);

            if (Type != CompressionNone)
                buf.WriteInt(data.Capacity);

            buf.WriteBytes(compressed);
            if (Versioned)
                buf.WriteShort(Version);

            buf.ResetReaderIndex();
            return buf;
        }
    }
}