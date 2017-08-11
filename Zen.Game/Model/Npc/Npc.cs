namespace Zen.Game.Model.Npc
{
    public class Npc : Mob.Mob
    {
        public Npc(World world, int type, Position position) : base(world, position)
        {
            Type = type;
        }

        public int Type { get; }
    }
}