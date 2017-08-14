using System;
using Zen.Game.Model.Object;
using static Zen.Game.Model.Map.TraversalMap.Tile;

namespace Zen.Game.Model.Map
{
    public class TraversalMap
    {
        public const int Size = 256;
        public const int RegionSize = 64;
        public const int MaximumPlane = 4;

        private readonly Region[] _regions = new Region[Size * Size];

        private void Set(int plane, int x, int y, int flag)
        {
            int regionX = x >> 6, regionY = y >> 6;
            var region = _regions[regionX + regionY * Size];

            region?.GetTile(plane, x & 0x3f, y & 0x3f).Set(flag);
        }

        public void MarkBridge(int plane, int x, int y) => Set(plane, x, y, Bridge);

        public void MarkBlocked(int plane, int x, int y)
        {
            int regionX = x >> 6, regionY = y >> 6;
            int localX = x & 0x3f, localY = y & 0x3f;

            var region = _regions[regionX + regionY * Size];
            if (region == null)
                return;

            var modifiedPlane = plane;
            if ((region.GetTile(1, localX, localY).Flags & Bridge) != 0)
                modifiedPlane = plane - 1;

            region.GetTile(modifiedPlane, x & 0x3f, y & 0x3f).Set(Blocked);
        }

        public bool RegionInitialized(int x, int y)
        {
            int regionX = x >> 6, regionY = y >> 6;
            return _regions[regionX + regionY * Size] != null;
        }

        public void InitializeRegion(int x, int y)
        {
            int regionX = x >> 6, regionY = y >> 6;
            _regions[regionX + regionY * Size] = new Region();
        }

        public void MarkWall(Rotation rotation, int plane, int x, int y, ObjectType type, bool impenetrable)
        {
            if (type.Equals(ObjectType.LengthwiseWall))
                switch (rotation)
                {
                    case Rotation.West:
                        Set(plane, x, y, WallWest);
                        Set(plane, x - 1, y, WallEast);
                        if (!impenetrable)
                            return;
                        Set(plane, x, y, ImpenetrableWallWest);
                        Set(plane, x - 1, y, ImpenetrableWallEast);
                        break;
                    case Rotation.North:
                        Set(plane, x, y, WallNorth);
                        Set(plane, x, y + 1, WallSouth);
                        if (!impenetrable)
                            return;
                        Set(plane, x, y, ImpenetrableWallNorth);
                        Set(plane, x, y + 1, ImpenetrableWallSouth);
                        break;
                    case Rotation.East:
                        Set(plane, x, y, WallEast);
                        Set(plane, x + 1, y, WallWest);
                        if (!impenetrable)
                            return;
                        Set(plane, x, y, ImpenetrableWallEast);
                        Set(plane, x + 1, y, ImpenetrableWallWest);
                        break;
                    case Rotation.South:
                        Set(plane, x, y, WallSouth);
                        Set(plane, x, y - 1, WallNorth);
                        if (!impenetrable)
                            return;
                        Set(plane, x, y, ImpenetrableWallSouth);
                        Set(plane, x, y - 1, ImpenetrableWallNorth);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(rotation), rotation, null);
                }
            else if (type.Equals(ObjectType.WallCorner))
                switch (rotation)
                {
                    case Rotation.West:
                        Set(plane, x, y, WallWest | WallNorth);
                        Set(plane, x - 1, y, WallEast);
                        Set(plane, x, y + 1, WallSouth);
                        if (!impenetrable)
                            return;
                        Set(plane, x, y, ImpenetrableWallWest | ImpenetrableWallNorth);
                        Set(plane, x - 1, y, ImpenetrableWallEast);
                        Set(plane, x, y + 1, ImpenetrableWallSouth);
                        break;
                    case Rotation.North:
                        Set(plane, x, y, WallEast | WallNorth);
                        Set(plane, x, y + 1, WallSouth);
                        Set(plane, x + 1, y, WallWest);
                        if (!impenetrable)
                            return;
                        Set(plane, x, y, ImpenetrableWallEast | ImpenetrableWallNorth);
                        Set(plane, x, y + 1, ImpenetrableWallSouth);
                        Set(plane, x + 1, y, ImpenetrableWallWest);
                        break;
                    case Rotation.East:
                        Set(plane, x, y, WallEast | WallSouth);
                        Set(plane, x + 1, y, WallWest);
                        Set(plane, x, y - 1, WallNorth);
                        if (!impenetrable)
                            return;
                        Set(plane, x, y, ImpenetrableWallEast | ImpenetrableWallSouth);
                        Set(plane, x + 1, y, ImpenetrableWallWest);
                        Set(plane, x, y - 1, ImpenetrableWallNorth);
                        break;
                    case Rotation.South:
                        Set(plane, x, y, WallWest | WallSouth);
                        Set(plane, x - 1, y, WallEast);
                        Set(plane, x, y - 1, WallNorth);
                        if (!impenetrable)
                            return;
                        Set(plane, x, y, ImpenetrableWallWest | ImpenetrableWallSouth);
                        Set(plane, x - 1, y, ImpenetrableWallEast);
                        Set(plane, x, y - 1, ImpenetrableWallNorth);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(rotation), rotation, null);
                }
            else if (type.Equals(ObjectType.TriangularCorner) || type.Equals(ObjectType.RectangularCorner))
                switch (rotation)
                {
                    case Rotation.West:
                        Set(plane, x, y, WallNorthWest);
                        Set(plane, x - 1, y + 1, WallSouthEast);
                        if (!impenetrable)
                            return;
                        Set(plane, x, y, ImpenetrableWallNorthWest);
                        Set(plane, x - 1, y + 1, ImpenetrableWallSouthEast);
                        break;
                    case Rotation.North:
                        Set(plane, x, y, WallNorthEast);
                        Set(plane, x + 1, y + 1, WallSouthWest);
                        if (!impenetrable)
                            return;
                        Set(plane, x, y, ImpenetrableWallNorthEast);
                        Set(plane, x + 1, y + 1, ImpenetrableWallSouthWest);
                        break;
                    case Rotation.East:
                        Set(plane, x, y, WallSouthEast);
                        Set(plane, x + 1, y - 1, WallNorthWest);
                        if (!impenetrable)
                            return;
                        Set(plane, x, y, ImpenetrableWallSouthEast);
                        Set(plane, x + 1, y - 1, ImpenetrableWallNorthWest);
                        break;
                    case Rotation.South:
                        Set(plane, x, y, WallSouthWest);
                        Set(plane, x - 1, y - 1, WallNorthEast);
                        if (!impenetrable)
                            return;
                        Set(plane, x, y, ImpenetrableWallSouthWest);
                        Set(plane, x - 1, y - 1, ImpenetrableWallNorthEast);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(rotation), rotation, null);
                }
        }

        public void MarkOccupant(int plane, int x, int y, int width, int length, bool impenetrable)
        {
            for (var offsetX = 0; offsetX < width; offsetX++)
            for (var offsetY = 0; offsetY < length; offsetY++)
            {
                Set(plane, x + offsetX, y + offsetY, Occupant);
                if (!impenetrable)
                    continue;
                Set(plane, x + offsetX, y + offsetY, ImpenetrableOccupant);
            }
        }

        public bool IsTraversableNorthWest(int plane, int x, int y, int size)
        {
            for (var offsetX = 0; offsetX < size; offsetX++)
            for (var offsetY = 0; offsetY < size; offsetY++)
                if (!IsTraversableNorthWest(plane, x + offsetX, y + offsetY))
                    return false;
            return true;
        }

        private bool IsTraversableNorthWest(int plane, int x, int y, bool impenetrable = false)
        {
            if (impenetrable)
                return IsInactive(plane, x - 1, y + 1,
                           ImpenetrableWallEast | ImpenetrableWallSouth | ImpenetrableWallSouthEast | Occupant)
                       && IsInactive(plane, x - 1, y, ImpenetrableWallEast | ImpenetrableOccupant)
                       && IsInactive(plane, x, y + 1, ImpenetrableWallSouth | ImpenetrableOccupant);
            return IsInactive(plane, x - 1, y + 1, WallEast | WallSouth | WallSouthEast | Occupant | Blocked)
                   && IsInactive(plane, x - 1, y, WallEast | Occupant | Blocked) &&
                   IsInactive(plane, x, y + 1, WallSouth | Occupant | Blocked);
        }

        private bool IsInactive(int plane, int x, int y, int flag)
        {
            int regionX = x >> 6, regionY = y >> 6;
            int localX = x & 0x3f, localY = y & 0x3f;

            var region = _regions[regionX + regionY * Size];
            if (region == null)
                return false;

            var modifiedPlane = plane;
            if (region.GetTile(1, localX, localY).IsActive(Bridge))
                modifiedPlane = plane + 1;

            return region.GetTile(modifiedPlane, localX, localY).IsInactive(flag);
        }

        public bool IsTraversableNorth(int plane, int x, int y, int size)
        {
            for (var offsetX = 0; offsetX < size; offsetX++)
            for (var offsetY = 0; offsetY < size; offsetY++)
                if (!IsTraversableNorth(plane, x + offsetX, y + offsetY))
                    return false;
            return true;
        }

        private bool IsTraversableNorth(int plane, int x, int y, bool impenetrable = false)
        {
            return impenetrable
                ? IsInactive(plane, x, y + 1, ImpenetrableOccupant | ImpenetrableWallSouth)
                : IsInactive(plane, x, y + 1, WallSouth | Occupant | Blocked);
        }

        public bool IsTraversableNorthEast(int plane, int x, int y, int size)
        {
            for (var offsetX = 0; offsetX < size; offsetX++)
            for (var offsetY = 0; offsetY < size; offsetY++)
                if (!IsTraversableNorthEast(plane, x + offsetX, y + offsetY))
                    return false;
            return true;
        }

        private bool IsTraversableNorthEast(int plane, int x, int y, bool impenetrable = false)
        {
            if (impenetrable)
                return IsInactive(plane, x + 1, y + 1,
                           ImpenetrableWallWest | ImpenetrableWallSouth | ImpenetrableWallSouthWest | Occupant)
                       && IsInactive(plane, x + 1, y, ImpenetrableWallWest | ImpenetrableOccupant)
                       && IsInactive(plane, x, y + 1, ImpenetrableWallSouth | ImpenetrableOccupant);
            return IsInactive(plane, x + 1, y + 1, WallWest | WallSouth | WallSouthWest | Occupant | Blocked)
                   && IsInactive(plane, x + 1, y, WallWest | Occupant | Blocked) &&
                   IsInactive(plane, x, y + 1, WallSouth | Occupant | Blocked);
        }

        public bool IsTraversableWest(int plane, int x, int y, int size)
        {
            for (var offsetX = 0; offsetX < size; offsetX++)
            for (var offsetY = 0; offsetY < size; offsetY++)
                if (!IsTraversableWest(plane, x + offsetX, y + offsetY))
                    return false;
            return true;
        }

        private bool IsTraversableWest(int plane, int x, int y, bool impenetrable = false)
        {
            return impenetrable
                ? IsInactive(plane, x - 1, y, ImpenetrableOccupant | ImpenetrableWallEast)
                : IsInactive(plane, x - 1, y, WallEast | Occupant | Blocked);
        }

        public bool IsTraversableEast(int plane, int x, int y, int size)
        {
            for (var offsetX = 0; offsetX < size; offsetX++)
            for (var offsetY = 0; offsetY < size; offsetY++)
                if (!IsTraversableEast(plane, x + offsetX, y + offsetY))
                    return false;
            return true;
        }

        private bool IsTraversableEast(int plane, int x, int y, bool impenetrable = false)
        {
            return impenetrable
                ? IsInactive(plane, x + 1, y, ImpenetrableOccupant | ImpenetrableWallWest)
                : IsInactive(plane, x + 1, y, WallWest | Occupant | Blocked);
        }

        public bool IsTraversableSouthWest(int plane, int x, int y, int size)
        {
            for (var offsetX = 0; offsetX < size; offsetX++)
            for (var offsetY = 0; offsetY < size; offsetY++)
                if (!IsTraversableSouthWest(plane, x + offsetX, y + offsetY))
                    return false;
            return true;
        }

        private bool IsTraversableSouthWest(int plane, int x, int y, bool impenetrable = false)
        {
            if (impenetrable)
                return IsInactive(plane, x - 1, y - 1,
                           ImpenetrableWallEast | ImpenetrableWallNorth | ImpenetrableWallNorthEast | Occupant)
                       && IsInactive(plane, x - 1, y, ImpenetrableWallEast | ImpenetrableOccupant)
                       && IsInactive(plane, x, y - 1, ImpenetrableWallNorth | ImpenetrableOccupant);
            return IsInactive(plane, x - 1, y - 1, WallEast | WallNorth | WallNorthEast | Occupant | Blocked)
                   && IsInactive(plane, x - 1, y, WallEast | Occupant | Blocked) &&
                   IsInactive(plane, x, y - 1, WallNorth | Occupant | Blocked);
        }

        public bool IsTraversableSouth(int plane, int x, int y, int size)
        {
            for (var offsetX = 0; offsetX < size; offsetX++)
            for (var offsetY = 0; offsetY < size; offsetY++)
                if (!IsTraversableSouth(plane, x + offsetX, y + offsetY))
                    return false;
            return true;
        }

        private bool IsTraversableSouth(int plane, int x, int y, bool impenetrable = false)
        {
            return impenetrable
                ? IsInactive(plane, x, y - 1, ImpenetrableOccupant | ImpenetrableWallNorth)
                : IsInactive(plane, x, y - 1, WallNorth | Occupant | Blocked);
        }

        public bool IsTraversableSouthEast(int plane, int x, int y, int size)
        {
            for (var offsetX = 0; offsetX < size; offsetX++)
            for (var offsetY = 0; offsetY < size; offsetY++)
                if (!IsTraversableSouthEast(plane, x + offsetX, y + offsetY))
                    return false;
            return true;
        }

        private bool IsTraversableSouthEast(int plane, int x, int y, bool impenetrable = false)
        {
            if (impenetrable)
                return IsInactive(plane, x + 1, y - 1,
                           ImpenetrableWallWest | ImpenetrableWallNorth | ImpenetrableWallNorthWest | Occupant)
                       && IsInactive(plane, x + 1, y, ImpenetrableWallWest | ImpenetrableOccupant)
                       && IsInactive(plane, x, y - 1, ImpenetrableWallNorth | ImpenetrableOccupant);
            return IsInactive(plane, x + 1, y - 1, WallWest | WallNorth | WallNorthWest | Occupant | Blocked)
                   && IsInactive(plane, x + 1, y, WallWest | Occupant | Blocked) &&
                   IsInactive(plane, x, y - 1, WallNorth | Occupant | Blocked);
        }

        internal class Tile
        {
            public const int
                WallNorth = 0x1,
                WallSouth = 0x2,
                WallEast = 0x4,
                WallWest = 0x8,
                WallNorthEast = 0x10,
                WallNorthWest = 0x20,
                WallSouthEast = 0x40,
                WallSouthWest = 0x80,
                Occupant = 0x8000,
                ImpenetrableOccupant = 0x10000,
                ImpenetrableWallNorth = 0x100,
                ImpenetrableWallSouth = 0x200,
                ImpenetrableWallEast = 0x400,
                ImpenetrableWallWest = 0x800,
                ImpenetrableWallNorthEast = 0x800,
                ImpenetrableWallNorthWest = 0x1000,
                ImpenetrableWallSouthEast = 0x2000,
                ImpenetrableWallSouthWest = 0x4000,
                Blocked = 0x20000,
                Bridge = 0x40000,
                None = 0x0;

            public Tile(int flags = None) => Flags = flags;

            public int Flags { get; set; }

            public void Set(int flag) => Flags |= flag;
            public void Unset(int flag) => Flags &= 0xffff - flag;
            public bool IsActive(int flag) => (Flags & flag) != 0;
            public bool IsInactive(int flag) => (Flags & flag) == 0;
        }

        private class Region
        {
            private readonly Tile[,] _tiles;

            public Region()
            {
                _tiles = new Tile[MaximumPlane, RegionSize * RegionSize];
                for (var plane = 0; plane < MaximumPlane; plane++)
                for (var id = 0; id < RegionSize * RegionSize; id++)
                    _tiles[plane, id] = new Tile();
            }

            public Tile GetTile(int plane, int x, int y) => _tiles[plane, x + y * RegionSize];
        }
    }
}