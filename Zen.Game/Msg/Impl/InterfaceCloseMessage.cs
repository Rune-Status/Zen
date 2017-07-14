namespace Zen.Game.Msg.Impl
{
    public class InterfaceCloseMessage : Message
    {
        public InterfaceCloseMessage(int bitpackedId)
        {
            BitpackedId = bitpackedId;
        }

        public int BitpackedId { get; }
    }
}