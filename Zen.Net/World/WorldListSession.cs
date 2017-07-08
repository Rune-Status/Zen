using DotNetty.Transport.Channels;
using Zen.Game;
using Zen.Net.Service;

namespace Zen.Net.World
{
    public class WorldListSession : Session
    {
        private static readonly Country[] Countries =
        {
            new Country(Country.FlagMexico, "Mexico")
        };

        public WorldListSession(ServiceManager serviceManager, GameServer server, IChannel channel) : base(
            serviceManager, server, channel)
        {
            /* Empty. */
        }

        public override void MessageReceived(object message)
        {
            var worlds = new[]
                {new WorldEntry(1, WorldEntry.FlagMembers | WorldEntry.FlagHighlight, 0, "-", "127.0.0.1")};
            var players = new[] {0};
            Channel.WriteAndFlushAsync(new WorldListMessage(unchecked((int) 0xDEADBEEF), Countries, worlds, players))
                .ContinueWith(delegate { Channel.CloseAsync(); });
        }

        public override void Unregister()
        {
            /* Empty */
        }
    }
}