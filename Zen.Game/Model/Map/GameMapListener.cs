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
        }

        public void Decode(int id, int rotation, ObjectType type, Position position)
        {
        }
    }
}