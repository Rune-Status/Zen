using DotNetty.Buffers;

namespace Zen.Net.Update
{
    public class FileResponse
    {
        public FileResponse(bool priority, int type, int file, IByteBuffer container)
        {
            Priority = priority;
            Type = type;
            File = file;
            Container = container;
        }

        public IByteBuffer Container { get; }
        public int File { get; set; }
        public int Type { get; }
        public bool Priority { get; }
    }
}