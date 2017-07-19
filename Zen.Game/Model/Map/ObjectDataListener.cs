using Zen.Game.Definition;
using Zen.Game.Model.Object;

namespace Zen.Game.Model.Map
{
    public class ObjectDataListener : GroundObjectListenerAdapter
    {
        private readonly TraversalMap _traversalMap;

        public ObjectDataListener(TraversalMap traversalMap)
        {
            _traversalMap = traversalMap;
        }

        public override void OnAdded(GroundObject groundObject)
        {
            var def = ObjectDefinition.ForId(groundObject.Id);
            if (!def.Solid)
                return;

            var position = groundObject.Position;
            if (!_traversalMap.RegionInitialized(position.X, position.Y))
                _traversalMap.InitializeRegion(position.X, position.Y);

            if (groundObject.Type.IsWall())
                _traversalMap.MarkWall(groundObject.Rotation, position.Height, position.X, position.Y,
                    groundObject.Type, def.Impenetrable);

            if ((int) groundObject.Type < 9 || (int) groundObject.Type > 12) return;

            var width = def.Width;
            var length = def.Length;
            if (groundObject.Rotation == 1 || groundObject.Rotation == 3)
            {
                width = def.Length;
                length = def.Width;
            }

            _traversalMap.MarkOccupant(position.Height, position.X, position.Y, width, length, def.Impenetrable);
        }

        public override void OnRemoved(GroundObject groundObject)
        {
            var def = ObjectDefinition.ForId(groundObject.Id);
            if (!def.Solid)
                return;

            var position = groundObject.Position;
            if (!_traversalMap.RegionInitialized(position.X, position.Y))
                _traversalMap.InitializeRegion(position.X, position.Y);

            if (groundObject.Type.IsWall())
                _traversalMap.UnmarkWall(groundObject.Rotation, position.Height, position.X, position.Y,
                    groundObject.Type, def.Impenetrable);

            if ((int) groundObject.Type < 9 || (int) groundObject.Type > 12) return;

            var width = def.Width;
            var length = def.Length;

            if (1 == groundObject.Rotation || groundObject.Rotation == 3)
            {
                width = def.Length;
                length = def.Width;
            }

            _traversalMap.UnmarkOccupant(position.Height, position.X, position.Y, width, length, def.Impenetrable);
        }
    }
}