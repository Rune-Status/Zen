using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zen.Game.Model.Mob.Action
{
    public class ActionQueue
    {

        public Queue<Action<Mob>> Actions = new Queue<Action<Mob>>();

        public Action<Mob> PeekAction() => Actions.Peek();

        public void AddAction(Action<Mob> action) => Actions.Enqueue(action);

        public Action<Mob> RemoveAction() => Actions.Dequeue();

        public void ClearActions() => Actions.Clear();

        public bool ContainsAction(Action<Mob> action) => Actions.Contains(action);
    }
}
