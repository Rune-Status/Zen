using System.Collections.Generic;
using DotNetty.Transport.Channels;
using Zen.Game;
using Zen.Game.Model;
using Zen.Game.Msg;
using Zen.Game.Msg.Impl;
using Zen.Net.Service;

namespace Zen.Net.Game
{
    public class GameSession : Session
    {
        private readonly Player _player;
        private readonly LoginService _loginService;
        private readonly PlayerSession _playerSession;

        public GameSession(ServiceManager serviceManager, GameServer server, IChannel channel, Player player) : base(
            serviceManager, server, channel)
        {
            _loginService = serviceManager.LoginService;
            _player = player;
            _playerSession = new PlayerSession(_player, channel, server.Repository);
        }

        public override void MessageReceived(object message)
        {
            var msg = message as Message;
            if (msg == null)
                return;

            _playerSession.Enqueue(msg);
        }

        public void Init(bool resizable)
        {
            _player.Session = _playerSession;

            _player.Send(new RegionChangeMessage(_player));
            _player.InterfaceSet.OnLogin(resizable);
            _player.SendGameMessage("Welcome to Zen.");

            _player.SkillSet.RefreshAll();
            _player.Inventory.Refresh();
            _player.Equipment.Refresh();
        }

        public override void Unregister()
        {
            _loginService.AddLogoutRequest(_player);
        }
    }
}