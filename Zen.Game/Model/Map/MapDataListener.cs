using Zen.Game.Definition;
using Zen.Game.Model.Object;

namespace Zen.Game.Model.Map
{
    public class MapDataListener : MapListenerAdapter
    {
        private readonly TraversalMap _traversalMap;

        public MapDataListener(TraversalMap traversalMap)
        {
            _traversalMap = traversalMap;
        }

        public override void OnTileDecode(int flags, Position position)
        {
            if ((flags & MapLoader.BridgeFlag) != 0)
                _traversalMap.MarkBridge(position.Height, position.X, position.Y);

            if ((flags & MapLoader.FlagClip) != 0)
                _traversalMap.MarkBlocked(position.Height, position.X, position.Y);
        }

        public override void OnObjectDecode(int id, int rotation, ObjectType type, Position position)
        {
            var def = ObjectDefinition.ForId(id);
            if (!def.Solid) return;

            if (!_traversalMap.RegionInitialized(position.X, position.Y))
                _traversalMap.InitializeRegion(position.X, position.Y);

            if (type.IsWall())
                _traversalMap.MarkWall(rotation, position.Height, position.X, position.Y, type, def.Impenetrable);

            if ((int) type < 9 || (int) type > 12) return;

            var width = def.Width;
            var length = def.Length;

            if (1 == rotation || rotation == 3)
            {
                width = def.Length;
                length = def.Width;
            }

            _traversalMap.MarkOccupant(position.Height, position.X, position.Y, width, length, def.Impenetrable);
        }
    }
}