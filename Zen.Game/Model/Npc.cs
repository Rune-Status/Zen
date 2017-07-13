namespace Zen.Game.Model
{
    public class Npc : Mob
    {
        public Npc(int type)
        {
            Type = type;
        }

        public int Type { get; }
    }
}