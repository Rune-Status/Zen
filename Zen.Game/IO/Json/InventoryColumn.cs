using Zen.Game.Model;

namespace Zen.Game.IO.Json
{
    public class InventoryColumn : ItemColumn
    {
        public override void Load(dynamic playerObject, Player player)
        {
            var inventoryArray = playerObject.Inventory;

            foreach (var inventory in inventoryArray)
            {
                int slot = inventory.Slot;
                var item = new Item((int) inventory.Id, (int) inventory.Amount);

                player.Inventory.Set(slot, item, false);
            }
        }

        public override void Save(dynamic playerObject, Player player) => playerObject.Inventory = CreateArray(player);
        public override Container GetContainer(Player player) => player.Inventory;
    }
}