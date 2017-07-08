using Zen.Game.Msg.Impl;

namespace Zen.Game.Model
{
    public class Inventory : ItemContainer
    {
        private readonly Player _player;

        public const int Capacity = 28;

        public Inventory(Player player) : base(Capacity)
        {
            _player = player;
        }

        public override void FireItemChanged(int slot, Item item)
        {
            var items = new[] {new SlottedItem(slot, item)};
            _player.Send(new InterfaceSlottedItemsMessage(149, 0, 93, items));
        }

        public override void FireItemsChanged()
        {
            if (Empty)
                _player.Send(new InterfaceResetItemsMessage(149, 0));
            else
            {
                var items = ToArray();
                _player.Send(new InterfaceItemsMessage(149, 0, 93, items));
            }
        }

        public override void FireCapacityExceeded()
        {
            _player.SendGameMessage("You don't have enough inventory space to hold that itemn.");
        }
    }
}