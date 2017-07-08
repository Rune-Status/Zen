namespace Zen.Net.Update
{
    public class UpdateEncryptionMessage
    {
        public UpdateEncryptionMessage(int key)
        {
            Key = key;
        }

        public int Key { get; }
    }
}