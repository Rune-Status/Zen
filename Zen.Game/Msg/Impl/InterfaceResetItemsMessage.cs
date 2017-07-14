namespace Zen.Game.Msg.Impl
{
    public class InterfaceResetItemsMessage : IMessage
    {
        public InterfaceResetItemsMessage(int id, int slot)
        {
            Id = id;
            Slot = slot;
        }

        public int Id { get; }
        public int Slot { get; }
    }
}