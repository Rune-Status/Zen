namespace Zen.Game.Model
{
    public class ContainerAppearanceListener : IContainerListener
    {
        private readonly Player _player;

        public ContainerAppearanceListener(Player player)
        {
            _player = player;
        }

        public void ItemChanged(Container container, int slot, Item item) => _player.Appearance.ResetTicketId();
        public void ItemsChanged(Container container) => _player.Appearance.ResetTicketId();

        public void CapacityExceeded(Container container)
        {
            /* empty. */
        }
    }
}