using Zen.Game.Model;

namespace Zen.Game.IO.Json
{
    public class EquipmentColumn : ItemColumn
    {
        public override void Load(dynamic playerObject, Player player)
        {
            var equipmentArray = playerObject.Equipment;

            foreach (var equipment in equipmentArray)
            {
                int slot = equipment.Slot;
                var item = new Item((int) equipment.Id, (int) equipment.Amount);

                player.Equipment.Set(slot, item, false);
            }
        }

        public override void Save(dynamic playerObject, Player player) => playerObject.Equipment = CreateArray(player);
        public override Container GetContainer(Player player) => player.Equipment;
    }
}