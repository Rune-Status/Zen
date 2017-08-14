using System.Collections.Generic;
using System.Linq;
using Zen.Game.Model.Object;

namespace Zen.Game.Model.Map
{
    public class MapTile
    {
        private readonly Dictionary<ObjectGroup, GameObject> _objects = new Dictionary<ObjectGroup, GameObject>();

        public void Add(ObjectGroup group, GameObject obj, bool replace)
        {
            if (!replace && _objects.ContainsKey(group))
                return;

            _objects[group] = obj;
        }

        public GameObject Get(int id) => (from kvp in _objects where kvp.Value.Id == id select kvp.Value).FirstOrDefault();
        public List<GameObject> Objects => new List<GameObject>(_objects.Values);
    }
}