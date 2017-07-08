using System;
using DotNetty.Buffers;
using Zen.Util;

namespace Zen.Fs
{
    public class Index
    {
        public const int Length = 6;

        private Index(int size, int sector)
        {
            Size = size;
            Sector = sector;
        }

        public int Sector { get; }
        public int Size { get; }

        public static Index Decode(IByteBuffer buffer)
        {
            if (buffer.ReadableBytes != Length)
                throw new ArgumentException();

            var size = buffer.ReadTriByte();
            var sector = buffer.ReadTriByte();

            return new Index(size, sector);
        }
    }
}