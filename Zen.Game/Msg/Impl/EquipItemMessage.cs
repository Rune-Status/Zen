namespace Zen.Game.Msg.Impl
{
    public class EquipItemMessage : IMessage
    {
        public EquipItemMessage(int id, int slot, int itemSlot, int itemId)
        {
            Id = id;
            Slot = slot;
            ItemSlot = itemSlot;
            ItemId = itemId;
        }

        public int Id { get; }
        public int Slot { get; }
        public int ItemSlot { get; }
        public int ItemId { get; }
    }
}