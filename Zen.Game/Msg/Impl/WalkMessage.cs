namespace Zen.Game.Msg.Impl
{
    public class WalkMessage : IMessage
    {
        public WalkMessage(Step[] steps, bool running)
        {
            Steps = steps;
            Running = running;
        }

        public bool Running { get; }
        public Step[] Steps { get; }

        public class Step
        {
            public Step(int x, int y)
            {
                X = x;
                Y = y;
            }

            public int Y { get; }
            public int X { get; }
        }
    }
}