using Zen.Game.Model.Map;

namespace Zen.Game.Model.Pathfinding
{
    public abstract class PathFinder
    {
        protected PathFinder(TraversalMap traversalMap)
        {
            TraversalMap = traversalMap;
        }

        protected TraversalMap TraversalMap { get; }

        public Path Find(Mob.Mob mob, Position dest) => Find(mob, dest.X, dest.Y);

        private Path Find(Mob.Mob mob, int destX, int destY)
        {
            var position = mob.Position;

            int baseLocalX = position.BaseLocalX, baseLocalY = position.BaseLocalY;
            int destLocalX = destX - baseLocalX, destLocalY = destY - baseLocalY;

            return Find(new Position(baseLocalX, baseLocalY, position.Height), 104, 104, position.LocalX,
                position.LocalY, destLocalX, destLocalY, mob.Size);
        }

        public abstract Path Find(Position position, int width, int length, int srcX, int srcY, int destX, int destY,
            int size);
    }
}