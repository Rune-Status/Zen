using System.Collections.Generic;
using System.Linq;

namespace Zen.Util
{
    public static class CollectionUtil
    {
        public static T Poll<T>(this LinkedList<T> list)
        {
            if (list.Count == 0) return default(T);

            var top = list.First();
            list.RemoveFirst();

            return top;
        }

        public static T RemoveFirst<T>(this Queue<T> queue)
        {
            return queue.Count == 0 ? default(T) : queue.Dequeue();
        }

        public static T GetLast<T>(this Queue<T> queue)
        {
            return queue.Count == 0 ? default(T) : queue.Last();
        }

        public static TU Get<T, TU>(this Dictionary<T, TU> dict, T key)
            where TU : class
        {
            dict.TryGetValue(key, out TU val);
            return val;
        }
    }
}