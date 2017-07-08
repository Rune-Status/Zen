using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zen.Game.Model
{
    internal class EntityListEnumerator<T> : IEnumerator<T> where T : Entity
    {
        private readonly int[] _indicies;
        private readonly T[] _entities;
        private EntityList<T> _entityList;

        protected int CurrentIndex;
        protected T current;

        public EntityListEnumerator(T[] entities, HashSet<int> indicies, EntityList<T> entityList)
        {
            _entities = entities;
            _indicies = new int[indicies.Count]; 
            indicies.CopyTo(_indicies);
            _entityList = entityList;
            CurrentIndex = -1;
        }

        public T Current
        {
            get
            {
                return current;
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return current;
            }
        }

        public bool MoveNext()
        {
            if (CurrentIndex++ >= _indicies.Length)
            {
                return false;
            }
            current = _entities[_indicies[CurrentIndex]];
            return true;
        }

        public void Remove()
        {
            if (CurrentIndex >= 1)
            {
                _entityList.Remove(_indicies[CurrentIndex - 1]);
            }
        }

        public void Reset()
        {
            current = default(T);
            CurrentIndex = -1;
        }

        public void Dispose()
        {
            _entityList = null;
            current = default(T);
            CurrentIndex = -1;
        }
    }
}
