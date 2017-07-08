using System.Collections.Generic;
using System.Linq;

namespace Zen.Util
{
    public static class CollectionUtil
    {
        public static T RemoveFirst<T>(this Queue<T> queue)
        {
            return queue.Count == 0 ? default(T) : queue.Dequeue();
        }

        public static T GetLast<T>(this Queue<T> queue)
        {
            return queue.Count == 0 ? default(T) : queue.Last();
        }
    }
}