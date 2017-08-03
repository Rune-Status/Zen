using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zen.Game.Model.Player;

namespace Zen.Game.Model.Mob.Action.Impl
{
    public class PlayerAction : Action<Player.Player>
    {
        public PlayerAction(int delay, bool immediate, Player.Player mob) : base(delay, immediate, mob)
        {
        }

        public override void Execute()
        {
            this.Mob.SendGameMessage("Testing player action execution...");
        }
    }
}
