using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Zen.Game.Definition;
using Zen.Util;

namespace Zen.Game.Model.Object
{
    public class GroundObjectList
    {
        private static int _counter = 1;

        private readonly Dictionary<Position, Tile> _tiles = new Dictionary<Position, Tile>();
        private readonly List<GroundObjectListener> _listeners = new List<GroundObjectListener>();
        private readonly HashSet<GroundObject> _updatedObjects = new HashSet<GroundObject>();

        public bool RecordUpdates { get; set; }
        private static int IncrementCounter() => Interlocked.Increment(ref _counter);

        private class Tile
        {
            private readonly GroundObjectList _objectList;

            private readonly Dictionary<ObjectGroup, GroundObject> _objects =
                new Dictionary<ObjectGroup, GroundObject>();

            public Tile(GroundObjectList objectList)
            {
                _objectList = objectList;
            }

            public bool Put(ObjectGroup group, GroundObject groundObject, bool replace)
            {
                if (!replace && _objects.ContainsKey(group))
                    return false;

                groundObject.Uid = IncrementCounter();
                _objects[group] = groundObject;

                foreach (var listener in _objectList._listeners)
                    listener.OnAdded(groundObject);

                if (_objectList.RecordUpdates)
                    _objectList._updatedObjects.Add(groundObject);
                return true;
            }

            public GroundObject Get(int id) =>
                (from kvp in _objects where kvp.Value.Id == id select kvp.Value).FirstOrDefault();
        }

        public void AddListener(GroundObjectListener listener)
        {
            if (!_listeners.Contains(listener))
                _listeners.Add(listener);
        }

        public GroundObject Get(int id, Position position) => _tiles.Get(position)?.Get(id);

        public void RemoveListener(GroundObjectListener listener) => _listeners.Remove(listener);

        public GroundObject Put(Position position, int id, int rotation, ObjectType type, bool replace = false) =>
            Put(position, id, ObjectDefinition.ForId(id).Animation, rotation, type, false);

        public GroundObject Put(Position position, int id, int animation, int rotation, ObjectType type) =>
            Put(position, id, animation, rotation, type, false);

        public GroundObject Put(Position position, int id, int animation, int rotation, ObjectType type, bool replace)
        {
            var groundObject = new GroundObject(position, id, animation, rotation, type);
            var tile = _tiles.Get(position);

            if (tile == null)
            {
                tile = new Tile(this);
                _tiles[position] = tile;
            }

            return tile.Put(groundObject.Type.GetObjectGroup(), groundObject, replace) ? groundObject : null;
        }
    }
}