using System.IO;
using System.Linq;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;
using Org.BouncyCastle.Security;
using Zen.Game;
using Zen.Game.Model;
using Zen.Net.Game;
using Zen.Net.Service;
using Zen.Shared;

namespace Zen.Net.Login
{
    public class LoginSession : Session
    {
        private static readonly SecureRandom Random = new SecureRandom();

        private readonly long _serverSessionKey = Random.NextLong();
        private readonly LoginService _service;
        private int _displayMode;

        public LoginSession(ServiceManager serviceManager, GameServer server, IChannel channel) : base(serviceManager,
            server, channel)
        {
            _service = serviceManager.LoginService;
            Init();
        }

        private void Init()
        {
            var buffer = Channel.Allocator.Buffer(8);
            buffer.WriteLong(_serverSessionKey);
            Channel.WriteAndFlushAsync(new LoginResponse(LoginConstants.StatusExchangeKeys, buffer));
        }

        public override void MessageReceived(object message)
        {
            var request = message as LoginRequest;
            if (request == null) return;

            if (request.ServerSessionKey != _serverSessionKey)
                throw new IOException("Server session key mismatch.");

            var versionMismatch = request.Version != GameConstants.Version;

            var table = Server.Cache.ChecksumTable;
            var crc = request.Crc;
            if (crc.Where((t, i) => table.GetEntry(i).Crc != t).Any())
                versionMismatch = true;

            if (versionMismatch)
            {
                SendLoginFailure(LoginConstants.StatusGameUpdated);
                return;
            }

            _displayMode = request.DisplayMode;
            _service.AddLoginRequest(this, request);
        }

        public override void Unregister()
        {
            /* Empty. */
        }

        public void SendLoginFailure(int status)
        {
            var response = new LoginResponse(status);
            Channel.WriteAndFlushAsync(response).ContinueWith(delegate { Channel.CloseAsync(); });
        }

        public void SendLoginSuccess(int status, Player player)
        {
            var buffer = Channel.Allocator.Buffer(11);
            buffer.WriteByte(player.Rights);
            buffer.WriteByte(0);
            buffer.WriteByte(0);
            buffer.WriteByte(0);
            buffer.WriteByte(0);
            buffer.WriteByte(0);
            buffer.WriteByte(0);
            buffer.WriteShort(player.Id);
            buffer.WriteByte(1);
            buffer.WriteByte(1);

            var pipeline = Channel.Pipeline;

            var session = new GameSession(ServiceManager, Server, Channel, player);
            var handler = pipeline.Get<GameChannelHandler>();
            handler.Session = session;

            pipeline.Remove<ReadTimeoutHandler>();

            var response = new LoginResponse(status, buffer);
            Channel.WriteAndFlushAsync(response);

            pipeline.AddFirst(
                new GameFrameEncoder(),
                new GameMessageEncoder(Server.Repository),
                new GameFrameDecoder(),
                new GameMessageDecoder(Server.Repository)
            );

            session.Init(_displayMode);
        }
    }
}