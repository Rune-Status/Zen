namespace Zen.Game.Msg.Impl
{
    public class ChatMessage : IMessage
    {
        public ChatMessage(int color, int effects, string text)
        {
            Color = color;
            Effects = effects;
            Text = text;
        }

        public int Color { get; }
        public int Effects { get; }
        public string Text { get; }
    }
}