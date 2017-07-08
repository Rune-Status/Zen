namespace Zen.Game.Model
{
    public class SpotAnimation
    {
        public SpotAnimation(int id, int delay = 0, int height = 0)
        {
            Id = id;
            Delay = delay;
            Height = height;
        }

        public int Height { get; }
        public int Delay { get; }
        public int Id { get; }
    }
}