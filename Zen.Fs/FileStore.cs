using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DotNetty.Buffers;
using Zen.Util;

namespace Zen.Fs
{
    public class FileStore : IDisposable
    {
        private readonly FileStream _dataStream;
        private readonly List<FileStream> _indexStreams;
        private readonly FileStream _metaStream;

        private FileStore(FileStream dataStream, List<FileStream> indexStreams, FileStream metaStream)
        {
            _dataStream = dataStream;
            _indexStreams = indexStreams;
            _metaStream = metaStream;
        }

        public int TypeCount => _indexStreams.Count;

        public void Dispose()
        {
            _dataStream.Close();
            _indexStreams.ForEach(stream => stream.Close());
            _metaStream.Close();
        }

        public static FileStore Open(string root)
        {
            var data = $"{root}/main_file_cache.dat2";
            if (!File.Exists(data))
                throw new FileNotFoundException();

            var dataStream = new FileStream(data, FileMode.Open, FileAccess.Read);

            var indexStreams = Enumerable.Range(0, 254)
                .Select(id => $"{root}/main_file_cache.idx{id}")
                .TakeWhile(File.Exists)
                .Select(index => new FileStream(index, FileMode.Open, FileAccess.Read))
                .ToList();

            if (indexStreams.Count == 0)
                throw new FileNotFoundException();

            var meta = $"{root}/main_file_cache.idx255";
            if (!File.Exists(meta))
                throw new FileNotFoundException();

            var metaStream = new FileStream(meta, FileMode.Open, FileAccess.Read);
            return new FileStore(dataStream, indexStreams, metaStream);
        }

        public IByteBuffer Read(int type, int id)
        {
            if ((type < 0 || type >= _indexStreams.Count) && type != 255)
                throw new FileNotFoundException();

            var indexStream = type == 255
                ? _metaStream
                : _indexStreams[type];

            var ptr = (long) id * Index.Length;
            if (ptr < 0 || ptr >= indexStream.Length)
                throw new FileNotFoundException();

            var buffer = indexStream.ReadFully(ptr, Index.Length);
            var index = Index.Decode(buffer);

            var data = Unpooled.Buffer(index.Size);

            int chunk = 0, remaining = index.Size;
            ptr = (long) index.Sector * Sector.Length;

            do
            {
                buffer = _dataStream.ReadFully(ptr, Sector.Length);
                var sector = Sector.Decode(buffer);

                if (remaining > Sector.DataLength)
                {
                    data.WriteBytes(sector.Data, 0, Sector.DataLength);
                    remaining -= Sector.DataLength;

                    if (sector.Type != type)
                        throw new IOException("File type mismatch.");

                    if (sector.Id != id)
                        throw new IOException("File id mismatch.");

                    if (sector.Chunk != chunk++)
                        throw new IOException("Chunk mismatch.");

                    ptr = (long) sector.NextSector * Sector.Length;
                }
                else
                {
                    data.WriteBytes(sector.Data, 0, remaining);
                    remaining = 0;
                }
            } while (remaining > 0);

            data.ResetReaderIndex();
            return data;
        }

        public int GetFileCount(int type)
        {
            if ((type < 0 || type >= _indexStreams.Count) & (type != 255))
                throw new FileNotFoundException();

            return type == 255
                ? (int) (_metaStream.Length / Index.Length)
                : (int) (_indexStreams[type].Length / Index.Length);
        }
    }
}