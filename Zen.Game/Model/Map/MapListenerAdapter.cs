using Zen.Game.Model.Object;

namespace Zen.Game.Model.Map
{
    public abstract class MapListenerAdapter : MapListener
    {
        public override void OnTileDecode(int flags, Position position)
        {
            /* Empty. */
        }

        public override void OnObjectDecode(int id, int rotation, ObjectType type, Position position)
        {
            /* Empty. */
        }
    }
}