using NLog;
using Zen.Game.Model;
using Zen.Game.Update;
using Zen.Shared;

namespace Zen.Game
{
    public class GameWorld
    {
        public static readonly GameWorld Instance = new GameWorld();
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly EntityList<Player> _players = new EntityList<Player>(GameConstants.MaxPlayers);

        private readonly PlayerUpdater _updater;

        private GameWorld()
        {
            _updater = new PlayerUpdater(this);
        }

        public bool AddPlayer(Player player)
        {
            if (player == null ||_players.Contains(player))
                return false;

            _players.Add(player);
            var slotId = _players.IndexOf(player);
            player.Id = (short) slotId;

            if (slotId != -1)
            {
                Logger.Info($"Registered Player [Username={player.Username}, Id={player.Id}, Total Online={_players.Count}]");
            }
            else
            {
                Logger.Error($"Could not register Player [Username={player.Username}, Id={player.Id}]");

            }
            return true;
        }

        public void RemovePlayer(Player player)
        {
            var slotId = _players.IndexOf(player);
            player.Id = (short) slotId;

            if (!_players.Contains(player))
                return;
            if (!_players.Remove(player))
                return;

            Logger.Info($"Unregistered Player [Username={player.Username}, Id={player.Id}, Total Online={_players.Count}]");
        }

        public Player GetPlayer(string username)
        {
            foreach (var player in _players)
            {
                if (player != null && player.Username.Equals(username))
                {
                    return player;
                }
            }
            return null;
        }

        public void Tick()
        {
            foreach (var player in _players)
                player.Session?.ProcessMessageQueue();

            _updater.Tick();
        }

        public EntityList<Player> GetPlayers()
        {
            return _players;
        }
    }
}