namespace Zen.Game.Model
{
    public class Position
    {
        public Position(int x, int y, int height = 0)
        {
            X = x;
            Y = y;
            Height = height;
        }

        public int CentralRegionX => X / 8;
        public int CentralRegionY => Y / 8;

        public int TopLeftRegionX => CentralRegionX - 6;
        public int TopLeftRegionY => CentralRegionY - 6;

        public int RegionX => X / 64;
        public int RegionY => Y / 64;

        public int X { get; }
        public int Y { get; }
        public int Height { get; }

        public int LocalX => GetLocalX(CentralRegionX);
        public int LocalY => GetLocalY(CentralRegionY);
        public int RegionId => RegionX * 256 + RegionY;

        public int GetLocalX(int centralRegionX) => X - (centralRegionX - 6) * 8;
        public int GetLocalY(int centralRegionY) => Y - (centralRegionY - 6) * 8;

        public bool IsWithinDistance(Position other)
        {
            var deltaX = other.X - X;
            var deltaY = other.Y - Y;
            return deltaX >= -16 && deltaX <= 15 && deltaY >= -16 && deltaY <= 15;
        }

        public override bool Equals(object obj)
        {
            var other = obj as Position;
            if (other == null) return false;
            return Height == other.Height && X == other.X && Y == other.Y;
        }

        public override int GetHashCode()
        {
            const int prime = 31;
            var result = 1;
            result = prime * result + Height;
            result = prime * result + X;
            result = prime * result + Y;
            return result;
        }

        public override string ToString() => $"Position [X={X}, Y={Y}, Height={Height}]";

        public Position Translate(int deltaX, int deltaY, int deltaHeight) =>
            new Position(X + deltaX, Y + deltaY, Height + deltaHeight);
    }
}