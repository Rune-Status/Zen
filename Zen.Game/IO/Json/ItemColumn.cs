using System;
using Newtonsoft.Json.Linq;
using Zen.Game.Model;

namespace Zen.Game.IO.Json
{
    public abstract class ItemColumn : Column
    {
        public abstract ItemContainer GetContainer(Player player);

        public JArray CreateArray(Player player)
        {
            var containerObject = new JArray();

            var container = GetContainer(player);
            var items = container.ToArray();

            for (var slot = 0; slot < items.Length; slot++)
            {
                var item = items[slot];
                if (item == null) continue;

                dynamic itemObject = new JObject();

                itemObject.Slot = slot;
                itemObject.Id = item.Id;
                itemObject.Amount = item.Amount;

                containerObject.Add(itemObject);
            }

            return containerObject;
        }
    }
}