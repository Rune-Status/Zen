using Zen.Game.Model.Player;
using Zen.Game.Msg.Impl;

namespace Zen.Game.Msg.Handler
{
    public class ChatMessageHandler : MessageHandler<ChatMessage>
    {
        public override void Handle(Player player, ChatMessage message)
        {
            player.UpdateChatMessage(message);
        }
    }
}