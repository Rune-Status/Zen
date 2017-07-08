using System;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Channels;
using Zen.Game;
using Zen.Net.Handshake;
using Zen.Net.Login;
using Zen.Net.Service;
using Zen.Net.Update;
using Zen.Net.World;

namespace Zen.Net
{
    public class GameChannelHandler : ChannelHandlerAdapter
    {
        private readonly ServiceManager _serviceManager;
        private readonly GameServer _server;

        public GameChannelHandler(ServiceManager serviceManager, GameServer server)
        {
            _serviceManager = serviceManager;
            _server = server;
        }

        public Session Session { get; set; }

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            try
            {
                if (Session != null)
                {
                    Session.MessageReceived(message);
                    return;
                }

                var handshake = message as HandshakeMessage;
                var channel = context.Channel;

                if (handshake == null) return;

                switch (handshake.Opcode)
                {
                    case HandshakeMessage.ServiceUpdate:
                        Session = new UpdateSession(_serviceManager, _server, channel);
                        break;
                    case HandshakeMessage.ServiceLogin:
                        Session = new LoginSession(_serviceManager, _server, channel);
                        break;
                    case HandshakeMessage.ServiceWorldList:
                        Session = new WorldListSession(_serviceManager, _server, channel);
                        break;
                    default:
                        throw new Exception("Invalid handshake id received.");
                }
            }
            finally
            {
                ReferenceCountUtil.Release(message);
            }
        }

        public override void ChannelUnregistered(IChannelHandlerContext context)
        {
            Session?.Unregister();
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            context.CloseAsync();
        }
    }
}