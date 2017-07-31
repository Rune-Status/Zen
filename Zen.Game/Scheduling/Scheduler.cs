using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zen.Util;

namespace Zen.Game.Scheduling
{
    public class Scheduler
    {

        private ConcurrentQueue<ScheduledTask> pending = new ConcurrentQueue<ScheduledTask>();

        private LinkedList<ScheduledTask> active = new LinkedList<ScheduledTask>();

        public void Pulse()
        {
            PollAll(pending, active);
            ScheduledTask task;
            while (pending.TryDequeue(out task))
            {
                task.Pulse();

                if (!task.IsRunning())
                {
                    active.Remove(task);
                }
            }    
        }

        public void Schedule(ScheduledTask task) => pending.Enqueue(task);


        public void PollAll<T>(ConcurrentQueue<T> queue, LinkedList<T> consumer)
        {
            if (queue == null || consumer == null)
                return;

            T element;
            while (queue.TryDequeue(out element))
            {
                consumer.AddFirst(element);
            }
        }

    }
}
