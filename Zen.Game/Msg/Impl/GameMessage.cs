namespace Zen.Game.Msg.Impl
{
    public class GameMessage : Message
    {
        public GameMessage(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }
}