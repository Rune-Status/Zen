namespace Zen.Net.Update
{
    public class UpdateVersionMessage
    {
        public UpdateVersionMessage(int version)
        {
            Version = version;
        }

        public int Version { get; }
    }
}