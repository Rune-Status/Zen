namespace Zen.Game.Model.Item
{
    public class ContainerFullListener : IContainerListener
    {
        private readonly string _name;
        private readonly Player.Player _player;

        public ContainerFullListener(Player.Player player, string name)
        {
            _player = player;
            _name = name;
        }

        public void ItemChanged(ItemContainer container, int slot, Item item)
        {
            /* ignore. */
        }

        public void ItemsChanged(ItemContainer container)
        {
            /* ignore. */
        }

        public void CapacityExceeded(ItemContainer container) => _player.SendGameMessage($"Not enough {_name} space.");
    }
}