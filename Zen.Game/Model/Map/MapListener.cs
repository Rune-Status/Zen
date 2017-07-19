using Zen.Game.Model.Object;

namespace Zen.Game.Model.Map
{
    public abstract class MapListener
    {
        public abstract void OnTileDecode(int flags, Position position);

        public abstract void OnObjectDecode(int id, int rotation, ObjectType type, Position position);
    }
}