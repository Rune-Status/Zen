using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zen.Shared;

namespace Zen.Game.Model
{
    public class EntityList<T> : ICollection<T> where T : Entity
    {

        private readonly T[] _entities;
        private readonly HashSet<int> _indicies = new HashSet<int>();
        private readonly int _capacity;
        private int _currentIndex = 1;

        public EntityList(int capacity)
        {
            _entities = new T[capacity];
            _capacity = capacity;
        }

        public EntityList(IEnumerable<T> collection)
        {
            foreach (var entity in collection)
                Add(entity);
        }

        public T this[int index]
        {
            get { return _entities[index]; }
            set { _entities[index] = value; }
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
                entity.Index = index;
                _indicies.Add(_currentIndex);
                IncreaseIndex();
            }
        }

        public void Add(T entity) => Add(entity, _currentIndex);

        public bool Remove(T entity)
        {
            _entities[entity.Index] = null;
            _indicies.Remove(entity.Index);
            DecreaseIndex();
            return true;
        }

        public T Remove(int index)
        {
            var temp = _entities[index];
            _entities[index] = null;
            _indicies.Remove(index);
            DecreaseIndex();
            return temp;
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

        public int IndexOf(T entity)
        {
            foreach (var index in _indicies)
            {
                if (_entities[index].Equals(entity))
                {
                    return index;
                }
            }
            return -1;
        }

        public int Count => _indicies.Count;
        public bool IsReadOnly => throw new NotImplementedException();

        public IEnumerator<T> GetEnumerator()
        {
            return new EntityListEnumerator<T>(_entities, _indicies, this);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _entities.GetEnumerator();
        }

        private void IncreaseIndex()
        {
            _currentIndex++;
            if (_currentIndex >= _capacity)
            {
                _currentIndex = 1;
            }
        }

        private void DecreaseIndex()
        {
            _currentIndex--;
            if (_currentIndex <= _capacity)
                _currentIndex = 1;
        }
    }
}
