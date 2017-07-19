using System;
using Zen.Fs;
using Zen.Game.IO;
using Zen.Shared;

namespace Zen.Core.Tests
{
    public class CacheTests
    {
        public static void Main()
        {
            var cache = new Cache(FileStore.Open(GameConstants.CacheDirectory));
            var keyTable = LandscapeKeyTable.Open(GameConstants.LandscapeDirectory);

            /*
             * RegionX = 49, RegionY = 144
             * RegionX = 49, RegionY = 145
             */

            var table = ReferenceTable.Decode(Container.Decode(cache.Store.Read(255, 5)).Data);
            var landscapeId = table.GetEntryId($"l{49}_{144}");
            var buffer = cache.Store.Read(5, landscapeId);

            Console.Out.WriteLine("landscapeId = {0}", landscapeId);

            var keys = keyTable.GetKeys(49, 144);
            try
            {
                buffer = Container.Decode(buffer, keys).Data;
            }
            catch (Exception)
            {
                /* IGNORE. */
            }

            Console.Out.WriteLine(buffer.Capacity);

            Console.ReadLine();
        }
    }
}