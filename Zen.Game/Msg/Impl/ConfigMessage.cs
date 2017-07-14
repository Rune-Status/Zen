namespace Zen.Game.Msg.Impl
{
    public class ConfigMessage : IMessage
    {
        public ConfigMessage(int id, int value)
        {
            Id = id;
            Value = value;
        }

        public int Id { get; }
        public int Value { get; }
    }
}