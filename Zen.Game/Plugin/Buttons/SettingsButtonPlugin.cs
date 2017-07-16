using Zen.Game.Model.Player;
using Zen.Game.Plugin.Type;

namespace Zen.Game.Plugin.Buttons
{
    public class SettingsButtonPlugin : IButtonPlugin
    {
        public void Initiate(PluginRepository repository)
        {
            repository.Register(this, Interface.Settings);
        }

        public void OnButtonClick(Player player, int id, int slot, int parameter)
        {
            var settings = player.Settings;
            switch (slot)
            {
                case 3:
                    settings.ToggleRunning();
                    break;
                case 4:
                    settings.ToggleChatFancy();
                    break;
                case 5:
                    settings.ToggleSplitPrivateChat();
                    break;
                case 6:
                    settings.ToggleTwoButtonMouse();
                    break;
                case 7:
                    settings.ToggleAcceptingAid();
                    break;
                case 16:
                    player.InterfaceSet.OpenInterface(Interface.DisplaySettings);
                    break;
                case 18:
                    player.InterfaceSet.OpenInterface(Interface.AudioSettings);
                    break;
            }
        }
    }
}