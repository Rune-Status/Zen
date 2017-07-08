using DotNetty.Transport.Channels;
using Zen.Game;
using Zen.Net.Service;

namespace Zen.Net
{
    public abstract class Session
    {
        protected Session(ServiceManager serviceManager, GameServer server, IChannel channel)
        {
            ServiceManager = serviceManager;
            Server = server;
            Channel = channel;
        }

        public GameServer Server { get; }
        protected ServiceManager ServiceManager { get; }
        protected IChannel Channel { get; }

        public abstract void MessageReceived(object message);
        public abstract void Unregister();
    }
}