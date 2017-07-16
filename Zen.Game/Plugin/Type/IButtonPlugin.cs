using Zen.Game.Model.Player;

namespace Zen.Game.Plugin.Type
{
    public interface IButtonPlugin : IPlugin
    {
        void OnButtonClick(Player player, int id, int slot, int parameter);
    }
}