namespace Zen.Game.Model.Item
{
    public class ContainerAppearanceListener : IContainerListener
    {
        private readonly Player.Player _player;

        public ContainerAppearanceListener(Player.Player player)
        {
            _player = player;
        }

        public void ItemChanged(ItemContainer container, int slot, Item item) => _player.Appearance.ResetTicketId();
        public void ItemsChanged(ItemContainer container) => _player.Appearance.ResetTicketId();

        public void CapacityExceeded(ItemContainer container)
        {
            /* empty. */
        }
    }
}