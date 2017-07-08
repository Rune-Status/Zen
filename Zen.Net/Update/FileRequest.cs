namespace Zen.Net.Update
{
    public class FileRequest
    {
        public FileRequest(bool priority, int type, int file)
        {
            Priority = priority;
            Type = type;
            File = file;
        }

        public bool Priority { get; }
        public int Type { get; }
        public int File { get; }
    }
}