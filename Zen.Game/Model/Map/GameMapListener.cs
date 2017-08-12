using System;
using Zen.Game.Definition;
using Zen.Game.Model.Object;

namespace Zen.Game.Model.Map
{
    public class GameMapListener
    {
        private readonly TraversalMap _traversalMap;

        public GameMapListener(TraversalMap traversalMap)
        {
            _traversalMap = traversalMap;
        }

        public void Decode(int flags, Position position)
        {
            if ((flags & GameMap.FlagBridge) != 0)
                _traversalMap.MarkBridge(position.Height, position.X, position.Y);

            if ((flags & GameMap.FlagClip) != 0)
                _traversalMap.MarkBlocked(position.Height, position.X, position.Y);
        }

        public void Decode(int id, int rotation, ObjectType type, Position position)
        {
            var def = ObjectDefinition.ForId(id);
            if (!def.Solid)
                return;

            if (!_traversalMap.RegionInitialized(position.X, position.Y))
                _traversalMap.InitializeRegion(position.X, position.Y);

            if (type.IsWall())
                _traversalMap.MarkWall((Rotation) rotation, position.Height, position.X, position.Y, type, def.Impenetrable);

            if (type.Value < 9 || type.Value > 12)
                return;

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