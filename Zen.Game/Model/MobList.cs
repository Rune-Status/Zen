using System;
using System.Collections;
using System.Collections.Generic;

namespace Zen.Game.Model
{
    public class MobList<T> : IList<T> where T : Mob
    {
        private readonly T[] _mobs;

        public MobList(int capacity)
        {
            _mobs = new T[capacity];
        }

        public IEnumerator<T> GetEnumerator() => new MobListEnumerator(_mobs);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(T mob)
        {
            for (var id = 0; id < _mobs.Length; id++)
            {
                if (_mobs[id] != null)
                    continue;

                _mobs[id] = mob;
                Count++;

                mob.Id = id + 1;
                break;
            }
        }

        public void Clear()
        {
            foreach (var t in _mobs)
                Remove(t);
        }

        public bool Contains(T mob) => IndexOf(mob) != -1;

        public bool Remove(T mob)
        {
            var id = mob.Id;
            if (id == 0) return false;

            id--;
            if (_mobs[id] != mob) return false;

            _mobs[id] = null;
            Count--;

            mob.ResetId();
            return true;
        }

        public int Count { get; private set; }
        public bool IsReadOnly => false;

        public int IndexOf(T mob)
        {
            for (var id = 0; id < _mobs.Length; id++)
                if (_mobs[id] != null && _mobs[id].Id == mob.Id)
                    return id;

            return -1;
        }

        public void Insert(int index, T item) => throw new NotImplementedException();
        public void RemoveAt(int index) => throw new NotImplementedException();
        public void CopyTo(T[] array, int arrayIndex) => throw new NotImplementedException();

        public T this[int index]
        {
            get => _mobs[index];
            set => _mobs[index] = value;
        }

        private class MobListEnumerator : IEnumerator<T>
        {
            private readonly T[] _mobs;
            private int _index;

            public MobListEnumerator(T[] mobs)
            {
                _mobs = mobs;
            }

            public bool MoveNext()
            {
                for (; _index < _mobs.Length; _index++)
                {
                    if (_mobs[_index] == null)
                        continue;

                    Current = _mobs[_index++];
                    return true;
                }
                return false;
            }

            public void Reset()
            {
                Current = default(T);
                _index = 0;
            }

            public void Dispose() => Reset();
            public T Current { get; private set; }
            object IEnumerator.Current => Current;
        }
    }
}