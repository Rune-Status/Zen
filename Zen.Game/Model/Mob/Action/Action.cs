using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zen.Game.Scheduling;

namespace Zen.Game.Model.Mob.Action
{
    public abstract class Action<T> : ScheduledTask where T : Mob
    {

        protected readonly T Mob;

        private bool Stopping = false;

        protected Action(int delay, bool immediate, T mob) : base(delay, immediate)
        {
            this.Mob = mob;
        }

        public T GetMob()
        {
            return Mob;
        }

        public override void Stop()
        {
            base.Stop();
            if (!Stopping)
            {
                Stopping = true;
                Mob.StopAction();
            }
        }

    }
}
