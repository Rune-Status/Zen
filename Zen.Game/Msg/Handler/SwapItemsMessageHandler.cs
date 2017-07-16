using Zen.Game.Model.Player;
using Zen.Game.Msg.Impl;

namespace Zen.Game.Msg.Handler
{
    public class SwapItemsMessageHandler : MessageHandler<SwapItemsMessage>
    {
        public override void Handle(Player player, SwapItemsMessage message)
        {
            if (message.Id == Interface.Inventory && message.Slot == 0)
                player.Inventory.Swap(message.Source, message.Destination);
        }
    }
}