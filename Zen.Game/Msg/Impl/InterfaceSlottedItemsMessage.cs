using Zen.Game.Model.Item;

namespace Zen.Game.Msg.Impl
{
    public class InterfaceSlottedItemsMessage : IMessage
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