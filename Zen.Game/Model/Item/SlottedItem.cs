namespace Zen.Game.Model.Item
{
    public class SlottedItem
    {
        public SlottedItem(int slot, Item item)
        {
            Slot = slot;
            Item = item;
        }

        public int Slot { get; }
        public Item Item { get; }
    }
}