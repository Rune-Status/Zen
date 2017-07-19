using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zen.Game.Model.Mob.Action
{
    public abstract class Action<T> where T : Mob
    {

        protected T Mob { get; set; }

        protected Action(T mob)
        {
            this.Mob = mob;
        }

        public abstract void Start();

        public abstract void Process();

        public abstract void ProcessWithDelay(int delay);

        public abstract void Stop();

        public abstract bool IsRunning();

        public abstract bool IsCancellable();
    }
}
