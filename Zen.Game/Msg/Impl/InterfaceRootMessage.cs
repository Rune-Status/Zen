namespace Zen.Game.Msg.Impl
{
    public class InterfaceRootMessage : IMessage
    {
        public InterfaceRootMessage(int id)
        {
            Id = id;
        }

        public int Id { get; }
    }
}