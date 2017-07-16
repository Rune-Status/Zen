using NLog;
using Zen.Game.Model.Player;
using Zen.Game.Msg.Impl;

namespace Zen.Game.Msg.Handler
{
    public class ButtonMessageHandler : MessageHandler<ButtonMessage>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public override void Handle(Player player, ButtonMessage message)
        {
            var id = message.Id;
            var slot = message.Slot;
            var parameter = message.Parameter;

            if (!GameWorld.Instance.Repository.OnButtonClick(player, id, slot, parameter))
                Logger.Debug($"Unhandled Button Click (Id={id}, Slot={slot}, Parameter={parameter}).");
        }
    }
}