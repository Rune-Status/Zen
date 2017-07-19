namespace Zen.Game.Model.Map
{
    public class Tile
    {
        public Tile(int flags = (int) Flag.None)
        {
            Flags = flags;
        }

        public void Set(int flag) => Flags |= flag;
        public void Unset(int flag) => Flags &= 0xffff - flag;

        public bool IsActive(int flag) => (Flags & flag) != 0;
        public bool IsInactive(int flag) => (Flags & flag) == 0;

        public int Flags { get; private set; }
    }
}