using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zen.Game.Model.Mob.Action.Impl
{
    public class PlayerAction : Action<Player.Player>
    {
        public PlayerAction(Player.Player mob) : base(mob)
        {
        }

        public override void Start()
        {
            throw new NotImplementedException();
        }

        public override void Stop()
        {
            throw new NotImplementedException();
        }

        public override void Process()
        {
            throw new NotImplementedException();
        }

        public override void ProcessWithDelay(int delay)
        {
            throw new NotImplementedException();
        }

        public override bool IsCancellable()
        {
            throw new NotImplementedException();
        }

        public override bool IsRunning()
        {
            throw new NotImplementedException();
        }
    }
}
