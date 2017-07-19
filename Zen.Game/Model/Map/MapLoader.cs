using System;
using System.Collections.Generic;
using Zen.Fs;
using Zen.Game.IO;
using Zen.Game.Model.Object;
using Zen.Util;

namespace Zen.Game.Model.Map
{
    public class MapLoader
    {
        public const int RoofFlag = 0x4;
        public const int FlagClip = 0x1;
        public const int BridgeFlag = 0x2;

        private readonly List<MapListener> _listeners = new List<MapListener>();
        private readonly Cache _cache;
        private readonly LandscapeKeyTable _keyTable;
        private readonly bool[,] _loaded = new bool[256, 256];
        private readonly ReferenceTable _referenceTable;

        public MapLoader(Cache cache, LandscapeKeyTable keyTable)
        {
            _cache = cache;
            _keyTable = keyTable;
            _referenceTable = ReferenceTable.Decode(Container.Decode(_cache.Store.Read(255, 5)).Data);
        }

        public void Load(int mapX, int mapY)
        {
            for (var x = mapX - 4; x <= mapX + 4 && x < _loaded.Length; x++)
            {
                if (x < 0) continue;

                for (var y = mapY - 4; y <= mapY + 4 && y < _loaded.Length; y++)
                {
                    if (y < 0) continue;
                    if (_loaded[x, y]) continue;

                    var landscapeId = _referenceTable.GetEntryId($"l{x}_{y}");
                    if (landscapeId != -1)
                        ReadLandscape(x, y, landscapeId);

                    var mapId = _referenceTable.GetEntryId($"m{x}_{y}");
                    if (mapId != -1)
                        ReadMap(x, y, mapId);

                    _loaded[x, y] = true;
                }
            }
        }

        private void ReadMap(int x, int y, int id)
        {
            var buffer = _cache.Read(5, id).Data;

            for (var plane = 0; plane < 4; plane++)
            {
                for (var localX = 0; localX < 64; localX++)
                {
                    for (var localY = 0; localY < 64; localY++)
                    {
                        var position = new Position(x * 64 + localX, y * 64 + localY, plane);
                        var flags = 0;

                        for (;;)
                        {
                            var config = buffer.ReadUnsignedByte();

                            if (config == 0)
                            {
                                foreach (var listener in _listeners)
                                    listener.OnTileDecode(flags, position);
                                break;
                            }

                            if (config == 1)
                            {
                                buffer.ReadUnsignedByte();

                                foreach (var listener in _listeners)
                                    listener.OnTileDecode(flags, position);
                                break;
                            }

                            if (config <= 49)
                                buffer.ReadUnsignedByte();
                            else if (config <= 81)
                                flags = config - 49;
                        }
                    }
                }
            }
        }

        private void ReadLandscape(int x, int y, int landscapeId)
        {
            var buffer = _cache.Store.Read(5, landscapeId);

            var keys = _keyTable.GetKeys(x, y);
            try
            {
                buffer = Container.Decode(buffer, keys).Data;
            }
            catch (Exception)
            {
                /*
                 * Ignore, we can't decrypt the landscape (incorrect keys).
                 */
            }

            var id = -1;
            int deltaId;

            while ((deltaId = buffer.ReadSmart()) != 0)
            {
                id += deltaId;

                var pos = 0;
                int deltaPos;

                while ((deltaPos = buffer.ReadSmart()) != 0)
                {
                    pos += deltaPos - 1;

                    var localX = (pos >> 6) & 0x3F;
                    var localY = pos & 0x3F;
                    var height = (pos >> 12) & 0x3;

                    var temp = buffer.ReadUnsignedByte();
                    var type = temp >> 2;
                    var rotation = temp & 0x3;

                    var position = new Position(x * 64 + localX, y * 64 + localY, height);
                    foreach (var listener in _listeners)
                        listener.OnObjectDecode(id, rotation, (ObjectType) type, position);
                }
            }
        }

        public void AddListener(MapListener listener) => _listeners.Add(listener);
    }
}