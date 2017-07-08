using System;
using System.IO;
using Zen.Util;

namespace Zen.Fs
{
    public class Cache : IDisposable
    {
        public Cache(FileStore store)
        {
            Store = store;
            ChecksumTable = CreateChecksumTable();
        }

        public FileStore Store { get; }
        public ChecksumTable ChecksumTable { get; }

        public int TypeCount => Store.TypeCount;
        public void Dispose() => Store.Dispose();

        public ChecksumTable CreateChecksumTable()
        {
            var size = Store.TypeCount;
            var table = new ChecksumTable(size);

            for (var i = 0; i < size; i++)
            {
                var buf = Store.Read(255, i);

                var crc = 0;
                var version = 0;
                var whirlpool = new byte[64];

                if (buf.Capacity > 0)
                {
                    var reference = ReferenceTable.Decode(Container.Decode(buf).Data);
                    crc = ByteBufferUtil.GetCrcChecksum(buf);
                    version = reference.Version;
                    buf.ResetReaderIndex();
                    whirlpool = ByteBufferUtil.GetWhirlpoolDigest(buf);
                }

                table.SetEntry(i, new ChecksumTable.Entry(crc, version, whirlpool));
            }
            return table;
        }

        public Container Read(int type, int file)
        {
            if (type == 255)
                throw new IOException("Reference tables can only be read with the low level FileStore API!");

            return Container.Decode(Store.Read(type, file));
        }

        public int GetFileCount(int type) => Store.GetFileCount(type);
    }
}