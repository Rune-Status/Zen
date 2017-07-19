namespace Zen.Game.Msg.Impl
{
    public class ObjectOptionOneMessage : IMessage
    {
        public ObjectOptionOneMessage(int x, int y, int id)
        {
            X = x;
            Y = y;
            Id = id;
        }

        public int Id { get; }
        public int Y { get; }
        public int X { get; }
    }
}
