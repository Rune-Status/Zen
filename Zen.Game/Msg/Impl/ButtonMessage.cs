namespace Zen.Game.Msg.Impl
{
    public class ButtonMessage : IMessage
    {
        public ButtonMessage(int id, int slot, int parameter)
        {
            Id = id;
            Slot = slot;
            Parameter = parameter;
        }

        public int Parameter { get; }
        public int Slot { get; }
        public int Id { get; }
    }
}