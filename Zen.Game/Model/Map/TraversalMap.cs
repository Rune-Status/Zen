using System.Linq;
using Zen.Game.Model.Object;
using static Zen.Game.Model.Map.Flag;

namespace Zen.Game.Model.Map
{
    public class TraversalMap
    {
        public const int Size = 256;
        public const int RegionSize = 64;
        public const int MaximumPlane = 4;

        private class Region
        {
            private readonly Tile[,] _tiles;

            public Region()
            {
                _tiles = new Tile[MaximumPlane, RegionSize * RegionSize];
                for (var plane = 0; plane < MaximumPlane; plane++)
                {
                    for (var id = 0; id < RegionSize * RegionSize; id++)
                    {
                        _tiles[plane, id] = new Tile();
                    }
                }
            }

            public Tile GetTile(int plane, int x, int y) => _tiles[plane, x + y * RegionSize];
        }

        private readonly Region[] _regions = new Region[Size * Size];

        public void InitializeRegion(int x, int y)
        {
            int regionX = x >> 6, regionY = y >> 6;
            _regions[regionX + regionY * Size] = new Region();
        }

        private void Set(int plane, int x, int y, params Flag[] flags)
        {
            var flag = flags.Aggregate(0, (current, f) => current | (int) f);

            int regionX = x >> 6, regionY = y >> 6;
            var region = _regions[regionX + regionY * Size];

            region?.GetTile(plane, x & 0x3f, y & 0x3f).Set(flag);
        }

        private void Unset(int plane, int x, int y, params Flag[] flags)
        {
            var flag = flags.Aggregate(0, (current, f) => current | (int) f);

            int regionX = x >> 6, regionY = y >> 6;
            var region = _regions[regionX + regionY * Size];

            region?.GetTile(plane, x & 0x3f, y & 0x3f).Unset(flag);
        }

        public void MarkBlocked(int plane, int x, int y)
        {
            int regionX = x >> 6, regionY = y >> 6;
            int localX = x & 0x3f, localY = y & 0x3f;

            var region = _regions[regionX + regionY * Size];
            if (region == null) return;

            var modifiedPlane = plane;
            if ((region.GetTile(1, localX, localY).Flags & (int) Bridge) != 0)
                modifiedPlane--;

            region.GetTile(modifiedPlane, x & 0x3f, y & 0x3f).Set((int) Blocked);
        }

        public bool RegionInitialized(int x, int y)
        {
            int regionX = x >> 6, regionY = y >> 6;
            return _regions[regionX + regionY * Size] != null;
        }

        public void MarkWall(int rotation, int plane, int x, int y, ObjectType type, bool impenetrable)
        {
            switch (type)
            {
                case ObjectType.StraightWall:
                    if (rotation == ObjectOrientation.West)
                    {
                        Set(plane, x, y, WallWest);
                        Set(plane, x - 1, y, WallEast);
                        if (impenetrable)
                        {
                            Set(plane, x, y, ImpenetrableWallWest);
                            Set(plane, x - 1, y, ImpenetrableWallEast);
                        }
                    }
                    if (rotation == ObjectOrientation.North)
                    {
                        Set(plane, x, y, WallNorth);
                        Set(plane, x, y + 1, WallSouth);
                        if (impenetrable)
                        {
                            Set(plane, x, y, ImpenetrableWallNorth);
                            Set(plane, x, y + 1, ImpenetrableWallSouth);
                        }
                    }
                    if (rotation == ObjectOrientation.East)
                    {
                        Set(plane, x, y, WallEast);
                        Set(plane, x + 1, y, WallWest);
                        if (impenetrable)
                        {
                            Set(plane, x, y, ImpenetrableWallEast);
                            Set(plane, x + 1, y, ImpenetrableWallWest);
                        }
                    }
                    if (rotation == ObjectOrientation.South)
                    {
                        Set(plane, x, y, WallSouth);
                        Set(plane, x, y - 1, WallNorth);
                        if (impenetrable)
                        {
                            Set(plane, x, y, ImpenetrableWallSouth);
                            Set(plane, x, y - 1, ImpenetrableWallNorth);
                        }
                    }
                    break;
                case ObjectType.Type2:
                    if (rotation == ObjectOrientation.West)
                    {
                        Set(plane, x, y, WallWest, WallNorth);
                        Set(plane, x - 1, y, WallEast);
                        Set(plane, x, y + 1, WallSouth);
                        if (impenetrable)
                        {
                            Set(plane, x, y, ImpenetrableWallWest, ImpenetrableWallNorth);
                            Set(plane, x - 1, y, ImpenetrableWallEast);
                            Set(plane, x, y + 1, ImpenetrableWallSouth);
                        }
                    }
                    if (rotation == ObjectOrientation.North)
                    {
                        Set(plane, x, y, WallEast, WallNorth);
                        Set(plane, x, y + 1, WallSouth);
                        Set(plane, x + 1, y, WallWest);
                        if (impenetrable)
                        {
                            Set(plane, x, y, ImpenetrableWallEast, ImpenetrableWallNorth);
                            Set(plane, x, y + 1, ImpenetrableWallSouth);
                            Set(plane, x + 1, y, ImpenetrableWallWest);
                        }
                    }
                    if (rotation == ObjectOrientation.East)
                    {
                        Set(plane, x, y, WallEast, WallSouth);
                        Set(plane, x + 1, y, WallWest);
                        Set(plane, x, y - 1, WallNorth);
                        if (impenetrable)
                        {
                            Set(plane, x, y, ImpenetrableWallEast, ImpenetrableWallSouth);
                            Set(plane, x + 1, y, ImpenetrableWallWest);
                            Set(plane, x, y - 1, ImpenetrableWallNorth);
                        }
                    }
                    if (rotation == ObjectOrientation.South)
                    {
                        Set(plane, x, y, WallWest, WallSouth);
                        Set(plane, x - 1, y, WallEast);
                        Set(plane, x, y - 1, WallNorth);
                        if (impenetrable)
                        {
                            Set(plane, x, y, ImpenetrableWallWest, ImpenetrableWallSouth);
                            Set(plane, x - 1, y, ImpenetrableWallEast);
                            Set(plane, x, y - 1, ImpenetrableWallNorth);
                        }
                    }
                    break;

                case ObjectType.Type1:
                case ObjectType.Type3:
                    if (rotation == ObjectOrientation.West)
                    {
                        Set(plane, x, y, WallNorthWest);
                        Set(plane, x - 1, y + 1, WallSouthEast);
                        if (impenetrable)
                        {
                            Set(plane, x, y, ImpenetrableWallNorthWest);
                            Set(plane, x - 1, y + 1, ImpenetrableWallSouthEast);
                        }
                    }
                    if (rotation == ObjectOrientation.North)
                    {
                        Set(plane, x, y, WallNorthEast);
                        Set(plane, x + 1, y + 1, WallSouthWest);
                        if (impenetrable)
                        {
                            Set(plane, x, y, ImpenetrableWallNorthEast);
                            Set(plane, x + 1, y + 1, ImpenetrableWallSouthWest);
                        }
                    }
                    if (rotation == ObjectOrientation.East)
                    {
                        Set(plane, x, y, WallSouthEast);
                        Set(plane, x + 1, y - 1, WallNorthWest);
                        if (impenetrable)
                        {
                            Set(plane, x, y, ImpenetrableWallSouthEast);
                            Set(plane, x + 1, y - 1, ImpenetrableWallNorthWest);
                        }
                    }
                    if (rotation == ObjectOrientation.South)
                    {
                        Set(plane, x, y, WallSouthWest);
                        Set(plane, x - 1, y - 1, WallNorthEast);
                        if (impenetrable)
                        {
                            Set(plane, x, y, ImpenetrableWallSouthWest);
                            Set(plane, x - 1, y - 1, ImpenetrableWallNorthEast);
                        }
                    }
                    break;
            }
        }

        public void MarkOccupant(int plane, int x, int y, int width, int length, bool impenetrable)
        {
            for (var offsetX = 0; offsetX < width; offsetX++)
            {
                for (var offsetY = 0; offsetY < length; offsetY++)
                {
                    Set(plane, x + offsetX, y + offsetY, Occupant);
                    if (impenetrable)
                    {
                        Set(plane, x + offsetX, y + offsetY, ImpenetrableOccupant);
                    }
                }
            }
        }

        public void UnmarkWall(int rotation, int plane, int x, int y, ObjectType type, bool impenetrable)
        {
            switch (type)
            {
                case ObjectType.StraightWall:
                    if (rotation == ObjectOrientation.West)
                    {
                        Unset(plane, x, y, WallWest);
                        Unset(plane, x - 1, y, WallEast);
                        if (impenetrable)
                        {
                            Unset(plane, x, y, ImpenetrableWallWest);
                            Unset(plane, x - 1, y, ImpenetrableWallEast);
                        }
                    }
                    if (rotation == ObjectOrientation.North)
                    {
                        Unset(plane, x, y, WallNorth);
                        Unset(plane, x, y + 1, WallSouth);
                        if (impenetrable)
                        {
                            Unset(plane, x, y, ImpenetrableWallNorth);
                            Unset(plane, x, y + 1, ImpenetrableWallSouth);
                        }
                    }
                    if (rotation == ObjectOrientation.East)
                    {
                        Unset(plane, x, y, WallEast);
                        Unset(plane, x + 1, y, WallWest);
                        if (impenetrable)
                        {
                            Unset(plane, x, y, ImpenetrableWallEast);
                            Unset(plane, x + 1, y, ImpenetrableWallWest);
                        }
                    }
                    if (rotation == ObjectOrientation.South)
                    {
                        Unset(plane, x, y, WallSouth);
                        Unset(plane, x, y - 1, WallNorth);
                        if (impenetrable)
                        {
                            Unset(plane, x, y, ImpenetrableWallSouth);
                            Unset(plane, x, y - 1, ImpenetrableWallNorth);
                        }
                    }
                    break;

                case ObjectType.Type2:
                    if (rotation == ObjectOrientation.West)
                    {
                        Unset(plane, x, y, WallWest, WallNorth);
                        Unset(plane, x - 1, y, WallEast);
                        Unset(plane, x, y + 1, WallSouth);
                        if (impenetrable)
                        {
                            Unset(plane, x, y, ImpenetrableWallWest, ImpenetrableWallNorth);
                            Unset(plane, x - 1, y, ImpenetrableWallEast);
                            Unset(plane, x, y + 1, ImpenetrableWallSouth);
                        }
                    }
                    if (rotation == ObjectOrientation.North)
                    {
                        Unset(plane, x, y, WallEast, WallNorth);
                        Unset(plane, x, y + 1, WallSouth);
                        Unset(plane, x + 1, y, WallWest);
                        if (impenetrable)
                        {
                            Unset(plane, x, y, ImpenetrableWallEast, ImpenetrableWallNorth);
                            Unset(plane, x, y + 1, ImpenetrableWallSouth);
                            Unset(plane, x + 1, y, ImpenetrableWallWest);
                        }
                    }
                    if (rotation == ObjectOrientation.East)
                    {
                        Unset(plane, x, y, WallEast, WallSouth);
                        Unset(plane, x + 1, y, WallWest);
                        Unset(plane, x, y - 1, WallNorth);
                        if (impenetrable)
                        {
                            Unset(plane, x, y, ImpenetrableWallEast, ImpenetrableWallSouth);
                            Unset(plane, x + 1, y, ImpenetrableWallWest);
                            Unset(plane, x, y - 1, ImpenetrableWallNorth);
                        }
                    }
                    if (rotation == ObjectOrientation.South)
                    {
                        Unset(plane, x, y, WallEast, WallSouth);
                        Unset(plane, x, y - 1, WallWest);
                        Unset(plane, x - 1, y, WallNorth);
                        if (impenetrable)
                        {
                            Unset(plane, x, y, ImpenetrableWallEast, ImpenetrableWallSouth);
                            Unset(plane, x, y - 1, ImpenetrableWallWest);
                            Unset(plane, x - 1, y, ImpenetrableWallNorth);
                        }
                    }
                    break;

                case ObjectType.Type1:
                case ObjectType.Type3:
                    if (rotation == ObjectOrientation.West)
                    {
                        Unset(plane, x, y, WallNorthWest);
                        Unset(plane, x - 1, y + 1, WallSouthEast);
                        if (impenetrable)
                        {
                            Unset(plane, x, y, ImpenetrableWallNorthWest);
                            Unset(plane, x - 1, y + 1, ImpenetrableWallSouthEast);
                        }
                    }
                    if (rotation == ObjectOrientation.North)
                    {
                        Unset(plane, x, y, WallNorthEast);
                        Unset(plane, x + 1, y + 1, WallSouthWest);
                        if (impenetrable)
                        {
                            Unset(plane, x, y, ImpenetrableWallNorthEast);
                            Unset(plane, x + 1, y + 1, ImpenetrableWallSouthWest);
                        }
                    }
                    if (rotation == ObjectOrientation.East)
                    {
                        Unset(plane, x, y, WallSouthEast);
                        Unset(plane, x + 1, y - 1, WallNorthWest);
                        if (impenetrable)
                        {
                            Unset(plane, x, y, ImpenetrableWallSouthEast);
                            Unset(plane, x + 1, y - 1, ImpenetrableWallNorthWest);
                        }
                    }
                    if (rotation == ObjectOrientation.South)
                    {
                        Unset(plane, x, y, WallSouthWest);
                        Unset(plane, x - 1, y - 1, WallNorthEast);
                        if (impenetrable)
                        {
                            Unset(plane, x, y, ImpenetrableWallSouthWest);
                            Unset(plane, x - 1, y - 1, ImpenetrableWallNorthEast);
                        }
                    }
                    break;
            }
        }

        public void UnmarkOccupant(int plane, int x, int y, int width, int length, bool impenetrable)
        {
            for (var offsetX = 0; offsetX < width; offsetX++)
            {
                for (var offsetY = 0; offsetY < length; offsetY++)
                {
                    Unset(plane, x + offsetX, y + offsetY, Occupant);
                    if (impenetrable)
                    {
                        Unset(plane, x + offsetX, y + offsetY, ImpenetrableOccupant);
                    }
                }
            }
        }

        public bool ShouldModifyPlane(int x, int y)
        {
            int regionX = x >> 6, regionY = y >> 6;
            int localX = x & 0x3f, localY = y & 0x3f;
            var region = _regions[regionX + regionY * Size];

            return region != null && region.GetTile(1, localX, localY).IsActive((int) Bridge);
        }

        public void MarkBridge(int plane, int x, int y) => Set(plane, x, y, Bridge);
    }
}