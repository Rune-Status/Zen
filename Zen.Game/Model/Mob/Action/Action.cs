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

        protected abstract void Start();

        protected abstract void Stop();

        protected abstract bool IsCancellable();
    }
}
