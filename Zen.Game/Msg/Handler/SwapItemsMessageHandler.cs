using Zen.Game.Model;
using Zen.Game.Msg.Impl;

namespace Zen.Game.Msg.Handler
{
    public class SwapItemsMessageHandler : MessageHandler<SwapItemsMessage>
    {
        public override void Handle(Player player, SwapItemsMessage message)
        {
            if (message.Id == InterfaceSet.Interfaces.Inventory && message.Slot == 0)
                player.Inventory.Swap(message.Source, message.Destination);
        }
    }
}