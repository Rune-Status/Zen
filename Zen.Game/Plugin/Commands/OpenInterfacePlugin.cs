using Zen.Game.Model.Player;
using Zen.Game.Plugin.Type;

namespace Zen.Game.Plugin.Commands
{
    public class OpenInterfacePlugin : ICommandPlugin
    {

        public void Initiate(PluginRepository repository)
        {
            repository.Register(this, "interface");
        }

        public void OnCommand(Player player, string keyword, string[] arguments)
        {
            var interfaceId = int.Parse(arguments[0]);
            player.InterfaceSet.OpenInterface(interfaceId);
            player.SendGameMessage("Opened interface: " + interfaceId);
        }

    }
}