using System.Linq;
using NLog;
using Zen.Game.Definition;
using Zen.Game.Model.Map;
using Zen.Game.Model.Mob;
using Zen.Game.Model.Npc;
using Zen.Game.Model.Object;
using Zen.Game.Model.Player;
using Zen.Game.Plugin;
using Zen.Game.Update;
using Zen.Shared;

namespace Zen.Game
{
    public class World
    {
        public static readonly World Instance = new World();
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly PlayerUpdater _updater;

        private World()
        {
            _updater = new PlayerUpdater(this);
            GroundObjects.AddListener(new ObjectDataListener(TraversalMap));
        }

        public MobList<Player> Players { get; } = new MobList<Player>(GameConstants.MaxPlayers);
        public MobList<Npc> Npcs { get; } = new MobList<Npc>(GameConstants.MaxNpcs);
        public PluginRepository Repository { get; } = new PluginRepository();
        public GroundObjectList GroundObjects { get; } = new GroundObjectList();
        public TraversalMap TraversalMap { get; } = new TraversalMap();

        public bool AddPlayer(Player player)
        {
            if (player == null || Players.Contains(player))
                return false;

            Players.Add(player);
            var slotId = Players.IndexOf(player);

            Logger.Info(
                slotId != -1
                    ? $"Registered Player [Username={player.Username}, Id={player.Id}, Total Online={Players.Count}]"
                    : $"Could not register Player [Username={player.Username}, Id={player.Id}]");
            return true;
        }

        public bool AddNpc(Npc npc)
        {
            if (npc == null || Npcs.Contains(npc))
                return false;

            Npcs.Add(npc);
            return Npcs.IndexOf(npc) != -1;
        }

        public void RemovePlayer(Player player)
        {
            if (!Players.Contains(player) || !Players.Remove(player))
                return;
            Logger.Info(
                $"Unregistered Player [Username={player.Username}, Id={player.Id}, Total Online={Players.Count}]");
        }

        public Player GetPlayer(string username) => Players.FirstOrDefault(
            player => player != null && player.Username.Equals(username));

        public void Tick()
        {
            foreach (var player in Players)
                player.Session?.ProcessMessageQueue();

            _updater.Tick();
        }
    }
}