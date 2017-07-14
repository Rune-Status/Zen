namespace Zen.Game.Msg.Impl
{
    public class InterfaceTextMessage : IMessage
    {
        public InterfaceTextMessage(int id, int slot, string text)
        {
            Id = id;
            Slot = slot;
            Text = text;
        }

        public int Id { get; }
        public int Slot { get; }
        public string Text { get; }
    }
}