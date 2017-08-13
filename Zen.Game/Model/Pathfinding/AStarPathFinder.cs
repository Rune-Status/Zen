using System;
using System.Collections.Generic;
using Zen.Game.Model.Map;

namespace Zen.Game.Model.Pathfinding
{
    public class AStarPathFinder : PathFinder
    {
        public const int CostStraight = 10;

        private readonly HashSet<Node> _closed = new HashSet<Node>();
        private readonly HashSet<Node> _open = new HashSet<Node>();

        private Node _current;
        private Node[,] _nodes;

        public AStarPathFinder(TraversalMap traversalMap) : base(traversalMap)
        {
            /* Empty. */
        }

        public override Path Find(Position position, int width, int length, int srcX, int srcY, int destX, int destY,
            int size)
        {
            if (destX < 0 || destY < 0 || destX >= width || destY >= length)
                return null;

            _nodes = new Node[width, length];
            for (var x = 0; x < width; x++)
            for (var y = 0; y < length; y++)
                _nodes[x, y] = new Node(x, y);

            _open.Add(_nodes[srcX, srcY]);

            while (_open.Count > 0)
            {
                _current = GetLowestCost();
                if (_current.Equals(_nodes[destX, destY]))
                    break;

                _open.Remove(_current);
                _closed.Add(_current);

                int x = _current.X, y = _current.Y;

                if (y > 0 && TraversalMap.IsTraversableSouth(position.Height, position.X + x, position.Y + y, size))
                {
                    var n = _nodes[x, y - 1];
                    ExamineNode(n);
                }
                if (x > 0 && TraversalMap.IsTraversableWest(position.Height, position.X + x, position.Y + y, size))
                {
                    var n = _nodes[x - 1, y];
                    ExamineNode(n);
                }
                if (y < length - 1 &&
                    TraversalMap.IsTraversableNorth(position.Height, position.X + x, position.Y + y, size))
                {
                    var n = _nodes[x, y + 1];
                    ExamineNode(n);
                }
                if (x < width - 1 &&
                    TraversalMap.IsTraversableEast(position.Height, position.X + x, position.Y + y, size))
                {
                    var n = _nodes[x + 1, y];
                    ExamineNode(n);
                }
                if (x > 0 && y > 0 &&
                    TraversalMap.IsTraversableSouthWest(position.Height, position.X + x, position.Y + y, size))
                {
                    var n = _nodes[x - 1, y - 1];
                    ExamineNode(n);
                }
                if (x > 0 && y < length - 1 &&
                    TraversalMap.IsTraversableNorthWest(position.Height, position.X + x, position.Y + y, size))
                {
                    var n = _nodes[x - 1, y + 1];
                    ExamineNode(n);
                }

                if (x < width - 1 && y > 0 &&
                    TraversalMap.IsTraversableSouthEast(position.Height, position.X + x, position.Y + y, size))
                {
                    var n = _nodes[x + 1, y - 1];
                    ExamineNode(n);
                }
                if (x < width - 1 && y < length - 1 &&
                    TraversalMap.IsTraversableNorthEast(position.Height, position.X + x, position.Y + y, size))
                {
                    var n = _nodes[x + 1, y + 1];
                    ExamineNode(n);
                }
            }

            if (_nodes[destX, destY].Parent == null)
                return null;

            var path = new Path();
            var destNode = _nodes[destX, destY];

            while (destNode != null && !destNode.Equals(_nodes[srcX, srcY]))
            {
                path.AddFirst(new Position(destNode.X + position.X, destNode.Y + position.Y));
                destNode = destNode.Parent;
            }

            return path;
        }

        private void ExamineNode(Node n)
        {
            var heuristic = _current.EstimateDistance(n);
            var nextStepCost = _current.Cost + heuristic;

            if (nextStepCost < n.Cost)
            {
                _open.Remove(n);
                _closed.Remove(n);
            }


            if (_open.Contains(n) || _closed.Contains(n))
                return;

            n.Parent = _current;
            n.Cost = nextStepCost;
            _open.Add(n);
        }

        private Node GetLowestCost()
        {
            Node curLowest = null;
            foreach (var n in _open)
                if (curLowest == null)
                    curLowest = n;
                else if (n.Cost < curLowest.Cost)
                    curLowest = n;
            return curLowest;
        }

        private class Node : IComparable<Node>
        {
            public Node(int x, int y)
            {
                X = x;
                Y = y;
            }

            public Node Parent { get; set; }
            public int Cost { get; set; }
            public int X { get; }
            public int Y { get; }

            public int CompareTo(Node other)
            {
                return Cost - other.Cost;
            }

            private bool Equals(Node other)
            {
                return Equals(Parent, other.Parent) && Cost == other.Cost && X == other.X &&
                       Y == other.Y;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != GetType()) return false;
                return Equals((Node) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = Parent != null ? Parent.GetHashCode() : 0;
                    hashCode = (hashCode * 397) ^ Cost;
                    hashCode = (hashCode * 397) ^ X;
                    hashCode = (hashCode * 397) ^ Y;
                    return hashCode;
                }
            }

            public int EstimateDistance(Node dest)
            {
                var deltaX = X - dest.X;
                var deltaY = Y - dest.Y;
                return (Math.Abs(deltaX) + Math.Abs(deltaY)) * CostStraight;
            }
        }
    }
}