using Zen.Game.Model;
using Zen.Game.Msg.Impl;

namespace Zen.Game.Msg.Handler
{
    public class DisplayMessageHandler : MessageHandler<DisplayMessage>
    {
        public override void Handle(Player player, DisplayMessage message)
        {
            var interfaceSet = player.InterfaceSet;
            var currentMode = interfaceSet.Mode;
            DisplayMode newMode;

            if (message.Mode == 0 || message.Mode == 1)
                newMode = DisplayMode.Fixed;
            else
                newMode = DisplayMode.Resizable;

            if (newMode == currentMode)
                return;

            interfaceSet.OnLogin(message.Mode);
            interfaceSet.OpenInterface(Interfaces.DisplaySettings);
        }
    }
}