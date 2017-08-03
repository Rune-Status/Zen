using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zen.Game.Scheduling
{
    public abstract class ScheduledTask
    {

        private int Delay { get; set; }

        private int Pulses { get; set; }

        private bool Running { get; set; }

        protected ScheduledTask(int delay, bool immediate)
        {
            this.Delay = delay;
            this.Pulses = immediate ? 0 : delay;
        }

        public bool IsRunning() => Running;

        public virtual void Stop()
        {
            Running = false;
        }

        public abstract void Execute();

        public void Pulse()
        {
            if (Running && --Pulses <= 0)
            {
                Execute();
                Pulses = Delay;
            }
        }

    }
}
