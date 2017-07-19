using Zen.Game.Model.Player;
using Zen.Game.Msg.Impl;

namespace Zen.Game.Msg.Handler
{
    public class InterfaceClosedMessageHandler : MessageHandler<InterfaceClosedMessage>
    {
        public override void Handle(Player player, InterfaceClosedMessage message)
        {
            player.StopActions(true);
        }
    }
}