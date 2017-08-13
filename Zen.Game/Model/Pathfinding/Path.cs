using System.Collections.Generic;
using Zen.Util;

namespace Zen.Game.Model.Pathfinding
{
    public class Path
    {
        public LinkedList<Position> Tiles { get; } = new LinkedList<Position>();
        public bool Empty => Tiles.Count == 0;

        public void AddFirst(Position p) => Tiles.AddFirst(p);
        public void AddLast(Position p) => Tiles.AddLast(p);

        public Position Peek() => Tiles.First?.Value;
        public Position Poll() => Tiles.Poll();
    }
}