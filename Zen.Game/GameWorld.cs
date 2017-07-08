using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using Zen.Game.Model;
using Zen.Game.Update;

namespace Zen.Game
{
    public class GameWorld
    {
        public static readonly GameWorld Instance = new GameWorld();
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly Dictionary<short, Player> _players = new Dictionary<short, Player>();

        private readonly bool[] _slots = new bool[2500];
        private readonly PlayerUpdater _updater;

        private GameWorld()
        {
            _updater = new PlayerUpdater(this);
            _slots[0] = true;
        }

        public Dictionary<short, Player>.ValueCollection Players => _players.Values;

        public bool AddPlayer(Player player)
        {
            short slotId;
            if ((slotId = ReserveSlot()) == -1 || _players.ContainsKey(slotId))
                return false;

            player.Id = slotId;
            _players[slotId] = player;

            Logger.Info($"Registered Player [Username={player.Username}, Id={player.Id}]");
            return true;
        }

        public void RemovePlayer(Player player)
        {
            var slotId = player.Id;
            if (!_players.ContainsKey(slotId)) return;
            if (!_players.Remove(slotId)) return;

            _slots[slotId] = false;
            Logger.Info($"Unregistered Player [Username={player.Username}, Online={_players.Count}]");
        }

        public Player GetPlayer(string username) => _players.Values.FirstOrDefault(
            player => player.Username.Equals(username, StringComparison.InvariantCultureIgnoreCase));

        private short ReserveSlot()
        {
            for (short id = 1; id < _slots.Length; id++)
            {
                if (_slots[id]) continue;

                _slots[id] = true;
                return id;
            }
            return -1;
        }

        public void Tick()
        {
            foreach (var player in Players)
                player.Session?.ProcessMessageQueue();

            _updater.Tick();
        }
    }
}