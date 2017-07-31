using System;
using Zen.Game.Model.Mob.Action.Impl;
using Zen.Game.Model.Player;
using Zen.Game.Plugin.Type;

namespace Zen.Game.Plugin.Commands
{
    public class PlayerActionTestPlugin : ICommandPlugin
    {

        public void Initiate(PluginRepository repository)
        {
            repository.Register(this, "actiontest");
        }

        public void OnCommand(Player player, string keyword, string[] arguments)
        {
            //player.StartAction(new PlayerAction(0, true, player));
        }
    }
}
