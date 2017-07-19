using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zen.Game.Model.Mob.Action.Impl
{
    public class AnimateAction : Action<Mob>
    {
        public AnimateAction(Mob mob) : base(mob)
        {

        }

        protected override bool IsCancellable()
        {
            throw new NotImplementedException();
        }

        protected override void Start()
        {
            throw new NotImplementedException();
        }

        protected override void Stop()
        {
            throw new NotImplementedException();
        }
    }
}
