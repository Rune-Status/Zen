using NLog;
using Zen.Game.Model;
using Zen.Game.Plugin.Type;

namespace Zen.Game.Plugin.Buttons
{
    public class SettingsButtonPlugin : IButtonPlugin
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public void Initiate(PluginRepository repository)
        {
            repository.Register(this, InterfaceSet.Interfaces.Settings);
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
                    player.InterfaceSet.OpenInterface(InterfaceSet.Interfaces.DisplaySettings);
                    break;
                case 18:
                    player.InterfaceSet.OpenInterface(InterfaceSet.Interfaces.AudioSettings);
                    break;
                default:
                    Logger.Debug($"Unhandled Settings Click (Slot={slot}).");
                    break;
            }
        }
    }
}