using Zen.Game.Model;

namespace Zen.Game.Msg.Impl
{
    public class InterfaceOpenMessage : IMessage
    {
        public InterfaceOpenMessage(int id, int bitpackedId, bool transparent)
        {
            Id = id;
            BitpackedId = bitpackedId;
            Transparent = transparent;
        }

        public bool Transparent { get; }
        public int BitpackedId { get; }
        public int Id { get; }
    }
}