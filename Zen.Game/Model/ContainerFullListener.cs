namespace Zen.Game.Model
{
    public class ContainerFullListener : IContainerListener
    {
        private readonly string _name;
        private readonly Player _player;

        public ContainerFullListener(Player player, string name)
        {
            _player = player;
            _name = name;
        }

        public void ItemChanged(Container container, int slot, Item item)
        {
            /* ignore. */
        }

        public void ItemsChanged(Container container)
        {
            /* ignore. */
        }

        public void CapacityExceeded(Container container) => _player.SendGameMessage($"Not enough {_name} space.");
    }
}