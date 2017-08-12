using System.Collections.Generic;
using Zen.Fs;
using Zen.Game.Definition;
using Zen.Game.IO;
using Zen.Game.Model.Object;
using Zen.Util;

namespace Zen.Game.Model.Map
{
    public class GameMap
    {
        public const int FlagClip = 0x1;
        public const int FlagBridge = 0x2;

        private readonly Cache _cache;
        private readonly ReferenceTable _table;
        private readonly LandscapeKeyTable _keyTable;
        private readonly GameMapListener _mapListener;

        private readonly bool[,] _parsed = new bool[256, 256];
        private readonly Dictionary<Position, MapTile> _tiles = new Dictionary<Position, MapTile>();

        public GameMap(Cache cache, LandscapeKeyTable keyTable, GameMapListener mapListener)
        {
            _cache = cache;
            _table = ReferenceTable.Decode(Container.Decode(cache.Store.Read(255, 5)).Data);
            _keyTable = keyTable;
            _mapListener = mapListener;
        }

        public void Parse(int mapX, int mapY)
        {
            for (var x = mapX - 4; x <= mapX + 4 && x < _parsed.Length; x++)
            {
                if (x < 0) continue;

                for (var y = mapY - 4; y <= mapY + 4 && y < _parsed.Length; y++)
                {
                    if (y < 0) continue;
                    if (_parsed[x, y]) continue;

                    var landscapeId = _table.GetEntryId($"l{x}_{y}");
                    if (landscapeId != -1)
                        ParseLandscape(x, y, landscapeId);

                    var mapId = _table.GetEntryId($"m{x}_{y}");
                    if (mapId != -1)
                        ParseMap(x, y, mapId);

                    _parsed[x, y] = true;
                }
            }
        }

        private void ParseMap(int x, int y, int id)
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
                                _mapListener.Decode(flags, position);
                                break;
                            }

                            if (config == 1)
                            {
                                buffer.ReadUnsignedByte();
                                _mapListener.Decode(flags, position);
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

        private void ParseLandscape(int x, int y, int landscapeId)
        {
            try
            {
                var keys = _keyTable.GetKeys(x, y);
                var buffer = Container.Decode(_cache.Store.Read(5, landscapeId), keys).Data;

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
                        var typeId = temp >> 2;
                        var rotation = temp & 0x3;

                        var position = new Position(x * 64 + localX, y * 64 + localY, height);
                        var type = ObjectType.ForId(typeId);

                        _mapListener.Decode(id, rotation, type, position);
                        AddObject(id, rotation, type, position);
                    }
                }
            }
            catch
            {
                /* Ignore */
            }
        }

        private void AddObject(int id, int rotation, ObjectType type, Position position)
        {
            var definition = ObjectDefinition.ForId(id);

            if (definition.Name == "null" || !ShouldForce(position) || definition.TransformObjects == null)
                return;

            var obj = new GameObject(position, id, rotation, type);
            var tile = _tiles.Get(position);

            if (tile == null)
            {
                tile = new MapTile();
                _tiles[position] = tile;
            }

            tile.Add(obj.Type.Group, obj, false);
        }

        private bool ShouldForce(Position pos) => pos.X >= 232 && pos.X < 247 && pos.Y >= 632 && pos.Y <= 639;
        public GameObject GetObject(int id, Position position) => _tiles.Get(position)?.Get(id);
    }
}