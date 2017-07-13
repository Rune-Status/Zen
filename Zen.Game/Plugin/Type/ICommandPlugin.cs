using Zen.Game.Model;

namespace Zen.Game.Plugin.Type
{
    public interface ICommandPlugin : IPlugin
    {
        void OnCommand(Player player, string keyword, string[] arguments);
    }
}