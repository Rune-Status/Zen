using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zen.Game.Model.Item;
using Zen.Game.Model.Player;
using Zen.Game.Plugin.Type;

namespace Zen.Game.Plugin.Commands
{
    public class SpawnItemPlugin : ICommandPlugin
    {

        public void Initiate(PluginRepository repository)
        {
            repository.Register(this, "item");
        }

        public void OnCommand(Player player, string keyword, string[] arguments)
        {
            var itemId = int.Parse(arguments[0]);
            var itemAmount = int.Parse(arguments[1]);
            player.Inventory.Add(new Item(itemId, itemAmount));
        }

    }
}
