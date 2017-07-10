using NLog;
using Zen.Fs;
using Zen.Fs.Definition;
using Zen.Game.IO;
using Zen.Game.Model;
using Zen.Game.Msg;
using Zen.Shared;

namespace Zen.Game
{
    public class GameServer
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public GameServer()
        {
            _logger.Info("Starting {0}..", GameConstants.Title);
            Cache = new Cache(FileStore.Open(GameConstants.CacheDirectory));

            var keyTable = LandscapeKeyTable.Open(GameConstants.LandscapeDirectory);
            Repository = new MessageRepository(keyTable);

            ItemDefinition.Load(Cache);
            EquipmentDefinition.Load();
        }

        public MessageRepository Repository { get; }
        public Cache Cache { get; }
        public GameWorld World { get; } = GameWorld.Instance;
    }
}