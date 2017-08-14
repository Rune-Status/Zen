using Zen.Game.Model.Player;
using Zen.Game.Plugin.Type;

namespace Zen.Game.Plugin.Buttons
{
    public class WelcomeScreenButtonPlugin : IButtonPlugin
    {
        public void Initiate(PluginRepository repository)
        {
            repository.Register(this, Interface.WelcomeScreen);
        }

        public void OnButtonClick(Player player, int id, int slot, int parameter)
        {
            if (slot != 140)
                return;

            player.InterfaceSet.OpenGameFrame();
        }
    }
}