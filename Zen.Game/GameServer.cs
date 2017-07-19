using NLog;
using Zen.Fs;
using Zen.Game.Definition;
using Zen.Game.IO;
using Zen.Game.Model.Map;
using Zen.Game.Model.Object;
using Zen.Game.Model.Player;
using Zen.Game.Msg;
using Zen.Shared;

namespace Zen.Game
{
    public class GameServer
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public GameServer()
        {
            _logger.Info($"Starting {GameConstants.Title}..");
            Cache = new Cache(FileStore.Open(GameConstants.CacheDirectory));

            var keyTable = LandscapeKeyTable.Open(GameConstants.LandscapeDirectory);
            Repository = new MessageRepository(keyTable);

            ItemDefinition.Load(Cache);
            EquipmentDefinition.Load();
            EnumDefinition.Load(Cache);
            ObjectDefinition.Load(Cache);

            var mapLoader = new MapLoader(Cache, keyTable);
            mapLoader.AddListener(new GroundObjectPopulator(World.GroundObjects));
            mapLoader.AddListener(new MapDataListener(World.TraversalMap));

            World.MapLoader = mapLoader;
        }

        public MessageRepository Repository { get; }
        public Cache Cache { get; }
        public GameWorld World { get; } = GameWorld.Instance;
    }
}