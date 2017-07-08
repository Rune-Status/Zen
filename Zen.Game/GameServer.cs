﻿using NLog;
using Zen.Fs;
using Zen.Fs.Definition;
using Zen.Game.IO;
using Zen.Game.Msg;

namespace Zen.Game
{
    public class GameServer
    {
        public const int Version = 530;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public GameServer()
        {
            _logger.Info("Starting Zen..");
            Cache = new Cache(FileStore.Open(@"../Data/Cache"));

            var keyTable = LandscapeKeyTable.Open(@"../Data/Landscape/");
            Repository = new MessageRepository(keyTable);

            ItemDefinition.Load(Cache);
        }

        public MessageRepository Repository { get; }
        public Cache Cache { get; }
        public GameWorld World { get; } = GameWorld.Instance;
    }
}