namespace Zen.Game.Model.Item
{
    public interface IContainerListener
    {
        void ItemChanged(ItemContainer container, int slot, Item item);

        void ItemsChanged(ItemContainer container);

        void CapacityExceeded(ItemContainer container);
    }
}