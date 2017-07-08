using System;
using System.Collections.Generic;
using Zen.Util;

namespace Zen.Game.Model
{
    public class WalkingQueue
    {
        private readonly Mob _mob;
        private readonly Queue<Position> _points = new Queue<Position>();

        public WalkingQueue(Mob mob)
        {
            _mob = mob;
        }

        public bool MinimapFlagReset { get; set; }
        public bool RunningQueue { get; set; }

        public void Reset()
        {
            _points.Clear();
            RunningQueue = false;
            MinimapFlagReset = true;
        }

        public void AddFirstStep(Position position)
        {
            _points.Clear();
            RunningQueue = false;
            AddStepImpl(position, _mob.Position);
        }

        private void AddStepImpl(Position position, Position last)
        {
            var deltaX = position.X - last.X;
            var deltaY = position.Y - last.Y;

            var max = Math.Max(Math.Abs(deltaX), Math.Abs(deltaY));

            for (var i = 0; i < max; i++)
            {
                if (deltaX < 0)
                    deltaX++;
                else if (deltaX > 0)
                    deltaX--;

                if (deltaY < 0)
                    deltaY++;
                else if (deltaY > 0)
                    deltaY--;

                _points.Enqueue(new Position(position.X - deltaX, position.Y - deltaY, position.Height));
            }
        }

        public void AddStep(Position position)
        {
            var lastPoint = _points.GetLast();
            AddStepImpl(position, lastPoint);
        }

        public void Tick()
        {
            var position = _mob.Position;

            var firstDirection = Direction.None;
            var secondDirection = Direction.None;

            var next = _points.RemoveFirst();
            if (next != null)
            {
                firstDirection = GetDirectionBetween(position, next);
                position = next;

                if (RunningQueue)
                {
                    next = _points.RemoveFirst();
                    if (next != null)
                    {
                        secondDirection = GetDirectionBetween(position, next);
                        position = next;
                    }
                }
            }

            _mob.SetDirections(firstDirection, secondDirection);
            _mob.Position = position;
        }

        private Direction GetDirectionBetween(Position cur, Position next)
        {
            var deltaX = next.X - cur.X;
            var deltaY = next.Y - cur.Y;

            switch (deltaY)
            {
                case 1:
                    switch (deltaX)
                    {
                        case 1:
                            return Direction.NorthEast;
                        case 0:
                            return Direction.North;
                        case -1:
                            return Direction.NorthWest;
                    }
                    break;
                case -1:
                    switch (deltaX)
                    {
                        case 1:
                            return Direction.SouthEast;
                        case 0:
                            return Direction.South;
                        case -1:
                            return Direction.SouthWest;
                    }
                    break;
                case 0:
                    switch (deltaX)
                    {
                        case 1:
                            return Direction.East;
                        case 0:
                            return Direction.None;
                        case -1:
                            return Direction.West;
                    }
                    break;
            }

            throw new ArgumentException();
        }
    }
}