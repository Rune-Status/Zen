namespace Zen.Game.Model
{
    public class Animation
    {
        public Animation(int id, int delay = 0)
        {
            Id = id;
            Delay = delay;
        }

        public int Delay { get; }
        public int Id { get; }
    }
}