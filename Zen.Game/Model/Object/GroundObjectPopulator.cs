using Zen.Game.Definition;
using Zen.Game.Model.Map;

namespace Zen.Game.Model.Object
{
    public class GroundObjectPopulator : MapListenerAdapter
    {
        private readonly GroundObjectList _objectList;

        public GroundObjectPopulator(GroundObjectList objectList)
        {
            _objectList = objectList;
        }

        public override void OnObjectDecode(int id, int rotation, ObjectType type, Position position)
        {
            _objectList.RecordUpdates = false;
            var def = ObjectDefinition.ForId(id);

            if (!"null".Equals(def.Name) || Force(position) || def.TransformObjects != null)
                _objectList.Put(position, id, def.Animation, rotation, type);

            _objectList.RecordUpdates = true;
        }

        private static bool Force(Position position)
        {
            var x = position.CentralRegionX;
            var y = position.CentralRegionY;
            return x >= 232 && x < 247 && y >= 632 && y <= 639;
        }
    }
}