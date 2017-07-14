namespace Zen.Game.Msg.Impl
{
    public class DisplayMessage : IMessage
    {
        public DisplayMessage(int mode, int width, int height)
        {
            Mode = mode;
            Width = width;
            Height = height;
        }

        public int Mode { get; }
        public int Width { get; }
        public int Height { get; }
    }
}