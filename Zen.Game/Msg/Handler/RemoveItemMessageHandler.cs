using Zen.Game.Model;
using Zen.Game.Msg.Impl;

namespace Zen.Game.Msg.Handler
{
    public class RemoveItemMessageHandler : MessageHandler<RemoveItemMessage>
    {
        public override void Handle(Player player, RemoveItemMessage message)
        {
            if (message.Id != InterfaceSet.Interfaces.Equipment || message.Slot != 28) return;
            var item = player.Equipment.Get(message.ItemSlot);
            if (item == null || item.Id != message.ItemId)
                return;

            Equipment.Unequip(player, message.ItemSlot);
        }
    }
}