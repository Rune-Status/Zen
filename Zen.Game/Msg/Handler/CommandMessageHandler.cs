using NLog;
using Zen.Game.Model;
using Zen.Game.Msg.Impl;

namespace Zen.Game.Msg.Handler
{
    public class CommandMessageHandler : MessageHandler<CommandMessage>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public override void Handle(Player player, CommandMessage message)
        {
            var name = message.Name;
            var arguments = message.Arguments;

            // TODO Handle Command
        }
    }
}