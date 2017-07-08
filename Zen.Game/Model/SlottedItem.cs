namespace Zen.Game.Model
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