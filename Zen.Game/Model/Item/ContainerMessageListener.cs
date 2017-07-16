using Zen.Game.Msg.Impl;

namespace Zen.Game.Model.Item
{
    public class ContainerMessageListener : IContainerListener
    {
        private readonly int _id, _slot, _type;
        private readonly Player.Player _player;

        public ContainerMessageListener(Player.Player player, int id, int slot, int type)
        {
            _player = player;
            _id = id;
            _slot = slot;
            _type = type;
        }

        public void ItemChanged(ItemContainer container, int slot, Item item)
        {
            var items = new[] {new SlottedItem(slot, item)};
            _player.Send(new InterfaceSlottedItemsMessage(_id, _slot, _type, items));
        }

        public void ItemsChanged(ItemContainer container)
        {
            if (container.Empty)
            {
                _player.Send(new InterfaceResetItemsMessage(_id, _slot));
            }
            else
            {
                var items = container.ToArray();
                _player.Send(new InterfaceItemsMessage(_id, _slot, _type, items));
            }
        }

        public void CapacityExceeded(ItemContainer container)
        {
            /* empty. */
        }
    }
}