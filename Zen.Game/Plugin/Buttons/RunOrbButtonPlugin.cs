using Zen.Game.Model;
using Zen.Game.Plugin.Type;

namespace Zen.Game.Plugin.Buttons
{
    public class RunOrbButtonPlugin : IButtonPlugin
    {
        public void Initiate(PluginRepository repository)
        {
            repository.Register(this, Orbs.Energy);
        }

        public void OnButtonClick(Player player, int id, int slot, int parameter)
        {
            if (slot == 1)
                player.Settings.ToggleRunning();
        }
    }
}