﻿using DotNetty.Transport.Channels;
using Zen.Game;
using Zen.Game.Model.Player;
using Zen.Game.Msg;
using Zen.Game.Msg.Impl;
using Zen.Net.Service;
using Zen.Shared;

namespace Zen.Net.Game
{
    public class GameSession : Session
    {
        private readonly LoginService _loginService;
        private readonly Player _player;
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
            var msg = message as IMessage;
            if (msg == null)
                return;

            _playerSession.Enqueue(msg);
        }

        public void Init(int displayMode)
        {
            _player.Session = _playerSession;

            _player.Send(new RegionChangeMessage(_player));
            _player.InterfaceSet.OnLogin(displayMode);
            _player.SendGameMessage($"Welcome to {GameConstants.Title}.");

            _player.SkillSet.RefreshAll();
            _player.Inventory.Refresh();
            _player.Equipment.Refresh();
            _player.Settings.RefreshAll();
        }

        public override void Unregister()
        {
            _loginService.AddLogoutRequest(_player);
        }
    }
}