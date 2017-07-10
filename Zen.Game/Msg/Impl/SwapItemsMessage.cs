namespace Zen.Game.Msg.Impl
{
    public class SwapItemsMessage : Message
    {
        public SwapItemsMessage(int id, int slot, int source, int destination, int type)
        {
            Id = id;
            Slot = slot;
            Source = source;
            Destination = destination;
            Type = type;
        }

        public int Id { get; }
        public int Slot { get; }
        public int Source { get; }
        public int Destination { get; }
        public int Type { get; }
    }
}