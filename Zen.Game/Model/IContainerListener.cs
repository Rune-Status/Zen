namespace Zen.Game.Model
{
    public interface IContainerListener
    {
        void ItemChanged(Container container, int slot, Item item);

        void ItemsChanged(Container container);

        void CapacityExceeded(Container container);
    }
}