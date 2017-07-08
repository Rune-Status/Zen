using NLog;
using Zen.Game.Model;
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

            // TODO Handle Buttons
        }
    }
}