using DotNetty.Buffers;

namespace Zen.Fs
{
    public class Archive
    {
        private Archive(int size)
        {
            Entries = new IByteBuffer[size];
        }

        public IByteBuffer[] Entries { get; set; }
        public int Size => Entries.Length;

        public static Archive Decode(IByteBuffer buffer, int size)
        {
            var archive = new Archive(size);

            buffer.SetReaderIndex(buffer.Capacity - 1);
            var chunks = buffer.ReadByte() & 0xFF;

            var chunkSizes = new int[chunks, size];
            var sizes = new int[size];
            buffer.SetReaderIndex(buffer.Capacity - 1 - chunks * size * 4);
            for (var chunk = 0; chunk < chunks; chunk++)
            {
                var chunkSize = 0;
                for (var id = 0; id < size; id++)
                {
                    var delta = buffer.ReadInt();
                    chunkSize += delta;

                    chunkSizes[chunk, id] = chunkSize;
                    sizes[id] += chunkSize;
                }
            }

            for (var id = 0; id < size; id++)
                archive.Entries[id] = Unpooled.Buffer(sizes[id]);

            buffer.ResetReaderIndex();
            for (var chunk = 0; chunk < chunks; chunk++)
            for (var id = 0; id < size; id++)
            {
                var chunkSize = chunkSizes[chunk, id];

                var temp = new byte[chunkSize];
                buffer.ReadBytes(temp);

                archive.Entries[id].WriteBytes(temp);
            }

            for (var id = 0; id < size; id++)
                archive.Entries[id].ResetReaderIndex();

            return archive;
        }
    }
}