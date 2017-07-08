using Zen.Game.Model;

namespace Zen.Game.Msg.Impl
{
    public class InterfaceSlottedItemsMessage : Message
    {
        public InterfaceSlottedItemsMessage(int id, int slot, int type, SlottedItem[] items)
        {
            Id = id;
            Slot = slot;
            Type = type;
            Items = items;
        }

        public int Id { get; }
        public int Slot { get; }
        public int Type { get; }
        public SlottedItem[] Items { get; }
    }
}