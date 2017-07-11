using System.Linq;
using NLog;
using Zen.Game.Model;
using Zen.Game.Plugin;
using Zen.Game.Update;
using Zen.Shared;

namespace Zen.Game
{
    public class GameWorld
    {
        public static readonly GameWorld Instance = new GameWorld();
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly MobList<Player> _players = new MobList<Player>(GameConstants.MaxPlayers);

        private readonly PlayerUpdater _updater;

        private GameWorld()
        {
            _updater = new PlayerUpdater(this);
        }

        public PluginRepository Repository { get; } = new PluginRepository();

        public bool AddPlayer(Player player)
        {
            if (player == null || _players.Contains(player))
                return false;

            _players.Add(player);

            var slotId = _players.IndexOf(player);

            Logger.Info(
                slotId != -1
                    ? $"Registered Player [Username={player.Username}, Id={player.Id}, Total Online={_players.Count}]"
                    : $"Could not register Player [Username={player.Username}, Id={player.Id}]");
            return true;
        }

        public void RemovePlayer(Player player)
        {
            if (!_players.Contains(player))
                return;
            if (!_players.Remove(player))
                return;

            Logger.Info(
                $"Unregistered Player [Username={player.Username}, Id={player.Id}, Total Online={_players.Count}]");
        }

        public Player GetPlayer(string username)
        {
            return _players.FirstOrDefault(player => player != null && player.Username.Equals(username));
        }

        public void Tick()
        {
            foreach (var player in _players)
                player.Session?.ProcessMessageQueue();

            _updater.Tick();
        }

        public MobList<Player> GetPlayers()
        {
            return _players;
        }
    }
}