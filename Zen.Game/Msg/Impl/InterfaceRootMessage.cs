namespace Zen.Game.Msg.Impl
{
    public class InterfaceRootMessage : Message
    {
        public InterfaceRootMessage(int id)
        {
            Id = id;
        }

        public int Id { get; }
    }
}