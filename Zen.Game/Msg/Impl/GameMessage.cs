namespace Zen.Game.Msg.Impl
{
    public class GameMessage : IMessage
    {
        public GameMessage(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }
}