namespace Zen.Game.Model.Npc
{
    public class Npc : Mob.Mob
    {
        public Npc(int type, Position position) : base(GameWorld.Instance, position)
        {
            Type = type;
        }

        public int Type { get; }
    }
}