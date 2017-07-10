using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Zen.Game.Model
{
    public class MobList<T> : ICollection<T> where T : Mob
    {
        private readonly int _capacity;

        private readonly T[] _entities;
        private readonly HashSet<int> _indicies = new HashSet<int>();
        private int _currentIndex = 1;

        public MobList(int capacity)
        {
            _entities = new T[capacity];
            _capacity = capacity;
        }

        public MobList(IEnumerable<T> collection)
        {
            foreach (var entity in collection)
                Add(entity);
        }

        public T this[int index]
        {
            get { return _entities[index]; }
            set { _entities[index] = value; }
        }

        public void Add(T entity) => Add(entity, _currentIndex);

        public bool Remove(T entity)
        {
            _entities[entity.Id] = null;
            _indicies.Remove(entity.Id);
            DecreaseIndex();
            return true;
        }

        public void Clear()
        {
            foreach (var entity in _entities)
                Remove(entity);
        }

        public bool Contains(T entity) => _entities.Contains(entity);

        public void CopyTo(T[] entities, int index)
        {
            foreach (var entity in entities)
                Add(entity, index);
        }

        public int Count => _indicies.Count();

        public bool IsReadOnly => throw new NotImplementedException();

        public IEnumerator<T> GetEnumerator()
        {
            return new MobListEnumerator<T>(_entities, _indicies, this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _entities.GetEnumerator();
        }

        public void Add(T entity, int index)
        {
            if (_entities[_currentIndex] != null)
            {
                IncreaseIndex();
                Add(entity, _currentIndex);
            }
            else
            {
                _entities[_currentIndex] = entity;
                entity.Id = index;
                _indicies.Add(_currentIndex);
                IncreaseIndex();
            }
        }

        public T Remove(int index)
        {
            var temp = _entities[index];
            _entities[index] = null;
            _indicies.Remove(index);
            DecreaseIndex();
            return temp;
        }

        public int IndexOf(T entity)
        {
            foreach (var index in _indicies)
                if (_entities[index].Equals(entity))
                    return index;
            return -1;
        }

        private void IncreaseIndex()
        {
            _currentIndex++;
            if (_currentIndex >= _capacity)
                _currentIndex = 1;
        }

        private void DecreaseIndex()
        {
            _currentIndex--;
            if (_currentIndex <= _capacity)
                _currentIndex = 1;
        }
    }
}