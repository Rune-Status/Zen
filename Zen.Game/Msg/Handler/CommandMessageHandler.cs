using NLog;
using Org.BouncyCastle.Security;
using Zen.Fs.Definition;
using Zen.Game.Model;
using Zen.Game.Msg.Impl;

namespace Zen.Game.Msg.Handler
{
    public class CommandMessageHandler : MessageHandler<CommandMessage>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public override void Handle(Player player, CommandMessage message)
        {
            var keyword = message.Name;
            var arguments = message.Arguments;

            if (!GameWorld.Instance.Repository.OnCommand(player, keyword, arguments))
                Logger.Debug($"Unhandled Command (keyword={keyword})");
        }
    }
}