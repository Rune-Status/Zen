namespace Zen.Game.Msg.Impl
{
    public class InterfaceCloseMessage : IMessage
    {
        public InterfaceCloseMessage(int bitpackedId)
        {
            BitpackedId = bitpackedId;
        }

        public int BitpackedId { get; }
    }
}