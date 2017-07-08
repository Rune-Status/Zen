using Zen.Game.Model;
using Zen.Game.Msg.Impl;

namespace Zen.Game.Msg.Handler
{
    public class WalkMessageHandler : MessageHandler<WalkMessage>
    {
        public override void Handle(Player player, WalkMessage message)
        {
            var height = player.Position.Height;
            var queue = player.WalkingQueue;

            var steps = message.Steps;
            var position = new Position(steps[0].X, steps[0].Y, height);
            queue.AddFirstStep(position);
            queue.RunningQueue = message.Running;

            for (var i = 1; i < steps.Length; i++)
            {
                position = new Position(steps[i].X, steps[i].Y, height);
                queue.AddStep(position);
            }
        }
    }
}