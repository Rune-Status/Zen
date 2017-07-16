using Zen.Game.Model.Player;
using Zen.Game.Msg.Impl;

namespace Zen.Game.Msg.Handler
{
    public class EquipItemMessageHandler : MessageHandler<EquipItemMessage>
    {
        public override void Handle(Player player, EquipItemMessage message)
        {
            if (message.Id != Interface.Inventory || message.Slot != 0) return;
            var item = player.Inventory.Get(message.ItemSlot);
            if (item == null || item.Id != message.ItemId)
                return;

            Equipment.Equip(player, message.ItemSlot);
        }
    }
}