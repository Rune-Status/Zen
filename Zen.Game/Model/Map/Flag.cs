namespace Zen.Game.Model.Map
{
    public enum Flag
    {
        /**
         * Wall Flags.
         */
        WallNorth = 0x1,
        WallSouth = 0x2,
        WallEast = 0x4,
        WallWest = 0x8,
        WallNorthEast = 0x10,
        WallNorthWest = 0x20,
        WallSouthEast = 0x40,
        WallSouthWest = 0x80,

        /**
         * Occupant Flags.
         */
        Occupant = 0x8000,
        ImpenetrableOccupant = 0x10000,

        /**
         * Impenetrable Wall Flags.
         */
        ImpenetrableWallNorth = 0x100,
        ImpenetrableWallSouth = 0x200,
        ImpenetrableWallEast = 0x400,
        ImpenetrableWallWest = 0x800,
        ImpenetrableWallNorthEast = 0x800,
        ImpenetrableWallNorthWest = 0x1000,
        ImpenetrableWallSouthEast = 0x2000,
        ImpenetrableWallSouthWest = 0x4000,

        /**
         * Miscellaneous Flags.
         */
        Blocked = 0x20000,
        Bridge = 0x40000,
        None = 0x0
    }
}