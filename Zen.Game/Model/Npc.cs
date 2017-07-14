namespace Zen.Game.Model
{
    public class Npc : Mob
    {
        public Npc(int type, Position position) : base(position)
        {
            Type = type;
        }

        public int Type { get; }
    }
}