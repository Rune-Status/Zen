using System;
using DotNetty.Buffers;
using Zen.Util;

namespace Zen.Fs
{
    public class Sector
    {
        public const int HeaderLength = 8;
        public const int DataLength = 512;
        public const int Length = HeaderLength + DataLength;

        private Sector(int type, int id, int chunk, int nextSector, byte[] data)
        {
            Type = type;
            Id = id;
            Chunk = chunk;
            NextSector = nextSector;
            Data = data;
        }

        public byte[] Data { get; }
        public int NextSector { get; }
        public int Chunk { get; }
        public int Id { get; }
        public int Type { get; }

        public static Sector Decode(IByteBuffer buffer)
        {
            if (buffer.ReadableBytes != Length)
                throw new ArgumentException();

            var id = buffer.ReadUnsignedShort();
            var chunk = buffer.ReadUnsignedShort();
            var nextSector = buffer.ReadTriByte();
            var type = buffer.ReadByte() & 0xFF;
            var data = new byte[DataLength];
            buffer.ReadBytes(data);

            return new Sector(type, id, chunk, nextSector, data);
        }
    }
}