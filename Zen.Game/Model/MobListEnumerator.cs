using System.Collections;
using System.Collections.Generic;

namespace Zen.Game.Model
{
    internal class MobListEnumerator<T> : IEnumerator<T> where T : Mob
    {
        private readonly T[] _entities;
        private readonly int[] _indicies;
        private MobList<T> _mobList;
        protected T current;

        protected int CurrentIndex;

        public MobListEnumerator(T[] entities, HashSet<int> indicies, MobList<T> mobList)
        {
            _entities = entities;
            _indicies = new int[indicies.Count];
            indicies.CopyTo(_indicies);
            _mobList = mobList;
            CurrentIndex = -1;
        }

        public T Current => current;

        object IEnumerator.Current => current;

        public bool MoveNext()
        {
            if (++CurrentIndex >= _indicies.Length)
                return false;
            current = _entities[_indicies[CurrentIndex]];
            return true;
        }

        public void Reset()
        {
            current = default(T);
            CurrentIndex = -1;
        }

        public void Dispose()
        {
            _mobList = null;
            current = default(T);
            CurrentIndex = -1;
        }

        public void Remove()
        {
            if (CurrentIndex >= 1)
                _mobList.Remove(_indicies[CurrentIndex - 1]);
        }
    }
}