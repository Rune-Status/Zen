using Zen.Game.Model;
using Zen.Game.Msg.Impl;

namespace Zen.Game.Msg.Handler
{
    public class InterfaceClosedMessageHandler : MessageHandler<InterfaceClosedMessage>
    {
        public override void Handle(Player player, InterfaceClosedMessage message)
        {
            if (player.InterfaceSet.InterfaceOpen)
                player.InterfaceSet.RemoveOpenInterface();

            player.WalkingQueue.Reset();
        }
    }
}