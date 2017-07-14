using Zen.Game.Model;

namespace Zen.Game.Msg.Impl
{
    public class InterfaceItemsMessage : IMessage
    {
        public InterfaceItemsMessage(int id, int slot, int type, Item[] items)
        {
            Id = id;
            Slot = slot;
            Type = type;
            Items = items;
        }

        public int Id { get; }
        public int Slot { get; }
        public int Type { get; }
        public Item[] Items { get; }
    }
}