using System;
using System.IO;
using DotNetty.Buffers;
using Org.BouncyCastle.Math;
using Zen.Util;
using Zen.Util.Crypto;

namespace Zen.Fs
{
    public class ChecksumTable
    {
        private readonly Entry[] _entries;

        public ChecksumTable(int size)
        {
            _entries = new Entry[size];
        }

        public IByteBuffer Encode(bool whirlpool = false, BigInteger modulus = null, BigInteger privateKey = null)
        {
            using (var stream = new MemoryStream())
            {
                if (whirlpool)
                    stream.WriteByte((byte) _entries.Length);

                foreach (var entry in _entries)
                {
                    stream.WriteInt(entry.Crc);
                    stream.WriteInt(entry.Version);
                    if (whirlpool)
                        stream.Write(entry.Whirlpool);
                }

                if (whirlpool)
                {
                    var bytes = stream.ToArray();
                    var temp = Unpooled.Buffer(65);
                    temp.WriteByte(0);
                    temp.WriteBytes(Whirlpool.Crypt(bytes, 0, bytes.Length));
                    temp.ResetReaderIndex();

                    if (modulus != null && privateKey != null)
                        temp = Rsa.Crypt(temp, modulus, privateKey);

                    bytes = new byte[temp.Capacity];
                    temp.ReadBytes(bytes);
                    stream.Write(bytes);
                }

                var output = stream.ToArray();
                return Unpooled.WrappedBuffer(output);
            }
        }

        public Entry GetEntry(int id)
        {
            if (id < 0 || id >= _entries.Length)
                throw new IndexOutOfRangeException();

            return _entries[id];
        }

        public void SetEntry(int id, Entry entry)
        {
            if (id < 0 || id >= _entries.Length)
                throw new IndexOutOfRangeException();

            _entries[id] = entry;
        }

        public class Entry
        {
            public Entry(int crc, int version, byte[] whirlpool)
            {
                if (whirlpool.Length != 64)
                    throw new ArgumentException();

                Crc = crc;
                Version = version;
                Whirlpool = whirlpool;
            }

            public int Crc { get; }
            public int Version { get; }
            public byte[] Whirlpool { get; }
        }
    }
}