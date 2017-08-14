using System;
using Zen.Game.Model.Map;
using Zen.Game.Model.Mob;

namespace Zen.Game.Model.Pathfinding
{
    public class DumbPathFinder
    {
        public static Path Find(TraversalMap traversalMap, Position position, Position dest, int size, int max,
            bool inside)
        {
            var path = new Path();
            var p = position;

            for (var id = 0; id < max; id++)
            {
                p = BestDummyPosition(traversalMap, p, dest, size);
                if (p != null)
                {
                    if (!inside && p.Equals(dest))
                    {
                        var cur = path.Empty
                            ? position
                            : path.Tiles.Last.Value;

                        switch (WalkingQueue.GetDirectionBetween(cur, dest))
                        {
                            case Direction.None:
                            case Direction.East:
                            case Direction.North:
                            case Direction.South:
                            case Direction.West:
                                break;
                            case Direction.NorthEast:
                                if (WalkingQueue.IsTraversable(traversalMap, cur, Direction.North, size))
                                    path.AddLast(new Position(cur.X, cur.Y + 1, cur.Height));
                                else if (WalkingQueue.IsTraversable(traversalMap, cur, Direction.East, size))
                                    path.AddLast(new Position(cur.X + 1, cur.Y, cur.Height));
                                break;
                            case Direction.NorthWest:
                                if (WalkingQueue.IsTraversable(traversalMap, cur, Direction.North, size))
                                    path.AddLast(new Position(cur.X, cur.Y + 1, cur.Height));
                                else if (WalkingQueue.IsTraversable(traversalMap, cur, Direction.West, size))
                                    path.AddLast(new Position(cur.X - 1, cur.Y, cur.Height));
                                break;
                            case Direction.SouthEast:
                                if (WalkingQueue.IsTraversable(traversalMap, cur, Direction.South, size))
                                    path.AddLast(new Position(cur.X, cur.Y - 1, cur.Height));
                                else if (WalkingQueue.IsTraversable(traversalMap, cur, Direction.East, size))
                                    path.AddLast(new Position(cur.X + 1, cur.Y, cur.Height));
                                break;
                            case Direction.SouthWest:
                                if (WalkingQueue.IsTraversable(traversalMap, cur, Direction.South, size))
                                    path.AddLast(new Position(cur.X, cur.Y - 1, cur.Height));
                                else if (WalkingQueue.IsTraversable(traversalMap, cur, Direction.West, size))
                                    path.AddLast(new Position(cur.X - 1, cur.Y, cur.Height));
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        return path;
                    }

                    path.AddLast(p);
                }
                else
                {
                    return path;
                }
            }
            return path;
        }

        private static Position BestDummyPosition(TraversalMap traversalMap, Position cur, Position next, int size)
        {
            if (cur.Equals(next))
                return null;

            var deltaX = next.X - cur.X;
            var deltaY = next.Y - cur.Y;

            switch (WalkingQueue.GetDirectionBetween(cur, next))
            {
                case Direction.None:
                    return null;
                case Direction.North:
                    return new Position(cur.X, cur.Y + 1, cur.Height);
                case Direction.South:
                    return new Position(cur.X, cur.Y - 1, cur.Height);
                case Direction.East:
                    return new Position(cur.X + 1, cur.Y, cur.Height);
                case Direction.West:
                    return new Position(cur.X - 1, cur.Y, cur.Height);
                case Direction.NorthEast:
                    if (WalkingQueue.IsTraversable(traversalMap, cur, Direction.NorthEast, size))
                        return new Position(cur.X + 1, cur.Y + 1, cur.Height);

                    if (Math.Abs(deltaX) > Math.Abs(deltaY))
                        return WalkingQueue.IsTraversable(traversalMap, cur, Direction.East, size)
                            ? new Position(cur.X + 1, cur.Y, cur.Height)
                            : new Position(cur.X, cur.Y + 1, cur.Height);
                    else
                        return WalkingQueue.IsTraversable(traversalMap, cur, Direction.North, size)
                            ? new Position(cur.X, cur.Y + 1, cur.Height)
                            : new Position(cur.X + 1, cur.Y, cur.Height);
                case Direction.NorthWest:
                    if (WalkingQueue.IsTraversable(traversalMap, cur, Direction.NorthWest, size))
                        return new Position(cur.X - 1, cur.Y + 1, cur.Height);

                    if (Math.Abs(deltaX) > Math.Abs(deltaY))
                        return WalkingQueue.IsTraversable(traversalMap, cur, Direction.West, size)
                            ? new Position(cur.X - 1, cur.Y, cur.Height)
                            : new Position(cur.X, cur.Y + 1, cur.Height);
                    else
                        return WalkingQueue.IsTraversable(traversalMap, cur, Direction.North, size)
                            ? new Position(cur.X, cur.Y + 1, cur.Height)
                            : new Position(cur.X - 1, cur.Y, cur.Height);
                case Direction.SouthEast:
                    if (WalkingQueue.IsTraversable(traversalMap, cur, Direction.SouthEast, size))
                        return new Position(cur.X + 1, cur.Y - 1, cur.Height);

                    if (Math.Abs(deltaX) > Math.Abs(deltaY))
                        return WalkingQueue.IsTraversable(traversalMap, cur, Direction.East, size)
                            ? new Position(cur.X + 1, cur.Y, cur.Height)
                            : new Position(cur.X, cur.Y - 1, cur.Height);
                    else
                        return WalkingQueue.IsTraversable(traversalMap, cur, Direction.South, size)
                            ? new Position(cur.X, cur.Y - 1, cur.Height)
                            : new Position(cur.X + 1, cur.Y, cur.Height);
                case Direction.SouthWest:
                    if (WalkingQueue.IsTraversable(traversalMap, cur, Direction.SouthWest, size))
                        return new Position(cur.X - 1, cur.Y - 1, cur.Height);

                    if (Math.Abs(deltaX) > Math.Abs(deltaY))
                        return WalkingQueue.IsTraversable(traversalMap, cur, Direction.West, size)
                            ? new Position(cur.X - 1, cur.Y, cur.Height)
                            : new Position(cur.X, cur.Y - 1, cur.Height);
                    else
                        return WalkingQueue.IsTraversable(traversalMap, cur, Direction.South, size)
                            ? new Position(cur.X, cur.Y - 1, cur.Height)
                            : new Position(cur.X - 1, cur.Y, cur.Height);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}