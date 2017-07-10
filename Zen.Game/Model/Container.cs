using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Zen.Game.Model
{
    public class Container
    {
        public enum StackMode
        {
            Always,
            StackableOnly
        }

        private readonly Item[] _items;
        private readonly List<IContainerListener> _listeners = new List<IContainerListener>();
        private readonly StackMode _stackMode;

        public Container(int slots, StackMode stackMode = StackMode.StackableOnly)
        {
            _stackMode = stackMode;
            _items = new Item[slots];
        }

        public int FreeSlots => _items.Count(t => t == null);
        public bool Empty => _items.All(t => t == null);

        public void Reset(int slot) => Set(slot, null);
        public bool IsStackable(Item item) => _stackMode == StackMode.Always || item.Definition.Stackable;
        public bool Contains(int id) => SlotOf(id) != -1;
        public void Refresh() => FireItemsChanged();

        public void AddListener(IContainerListener listener) => _listeners.Add(listener);
        public void RemoveListener(IContainerListener listener) => _listeners.Remove(listener);
        public void RemoveListeners() => _listeners.Clear();

        private void FireItemChanged(int slot)
        {
            foreach (var listener in _listeners)
                listener.ItemChanged(this, slot, _items[slot]);
        }

        private void FireItemsChanged()
        {
            foreach (var listener in _listeners)
                listener.ItemsChanged(this);
        }

        public void FireCapacityExceeded()
        {
            foreach (var listener in _listeners)
                listener.CapacityExceeded(this);
        }

        public Item Get(int slot)
        {
            CheckSlot(slot);
            return _items[slot];
        }

        public Item Add(Item item, int preferredSlot = -1)
        {
            var id = item.Id;
            var stackable = IsStackable(item);

            if (stackable)
            {
                var slot = SlotOf(id);
                if (slot != -1)
                {
                    var other = _items[slot];
                    var total = (long) other.Amount + item.Amount;
                    int amount;

                    Item remaining = null;
                    if (total > int.MaxValue)
                    {
                        amount = int.MaxValue;
                        remaining = new Item(id, (int) (total - amount));
                        FireCapacityExceeded();
                    }
                    else
                    {
                        amount = (int) total;
                    }

                    Set(slot, new Item(item.Id, amount));
                    return remaining;
                }

                if (preferredSlot != -1)
                {
                    CheckSlot(preferredSlot);
                    if (_items[preferredSlot] == null)
                    {
                        Set(preferredSlot, item);
                        return null;
                    }
                }

                for (slot = 0; slot < _items.Length; slot++)
                {
                    if (_items[slot] != null) continue;
                    Set(slot, item);
                    return null;
                }

                FireCapacityExceeded();
                return item;
            }
            {
                var single = new Item(id);
                var remaining = item.Amount;

                if (remaining == 0)
                    return null;

                if (preferredSlot != -1)
                {
                    CheckSlot(preferredSlot);
                    if (_items[preferredSlot] == null)
                    {
                        Set(preferredSlot, single);
                        remaining--;
                    }
                }

                if (remaining == 0)
                    return null;

                for (var slot = 0; slot < _items.Length; slot++)
                {
                    if (_items[slot] == null)
                    {
                        Set(slot, single);
                        remaining--;
                    }

                    if (remaining == 0)
                        return null;
                }

                FireCapacityExceeded();
                return new Item(id, remaining);
            }
        }

        public Item Remove(Item item, int preferredSlot = -1)
        {
            var id = item.Id;
            var stackable = IsStackable(item);
            var removed = 0;

            if (stackable)
            {
                var slot = SlotOf(id);
                if (slot == -1) return null;

                var other = _items[slot];
                if (other.Amount >= item.Amount)
                {
                    Set(slot, null);
                    return new Item(id, other.Amount);
                }

                other = new Item(id, other.Amount - item.Amount);
                Set(slot, other);
                return item;
            }

            if (preferredSlot != -1)
            {
                CheckSlot(preferredSlot);
                if (_items[preferredSlot].Id == id)
                {
                    Set(preferredSlot, null);
                    if (++removed >= item.Amount)
                        return new Item(id, removed);
                }
            }

            for (var slot = 0; slot < _items.Length; slot++)
            {
                var other = _items[slot];
                if (other == null || other.Id != id) continue;

                Set(slot, null);
                if (++removed >= item.Amount)
                    return new Item(id, removed);
            }

            return removed == 0 ? null : new Item(id, removed);
        }

        public void Shift()
        {
            var destSlot = 0;
            for (var slot = 0; slot < _items.Length; slot++)
            {
                var item = _items[slot];
                if (item != null)
                    _items[destSlot++] = item;
            }

            for (var slot = destSlot; slot < _items.Length; slot++)
                _items[slot] = null;

            FireItemsChanged();
        }

        public void Clear()
        {
            for (var slot = 0; slot < _items.Length; slot++)
                _items[slot] = null;

            FireItemsChanged();
        }

        public void Set(int slot, Item item, bool fire = true)
        {
            CheckSlot(slot);
            _items[slot] = item;
            if (fire) FireItemChanged(slot);
        }

        public void Swap(int originalSlot, int newSlot)
        {
            CheckSlot(originalSlot);
            CheckSlot(newSlot);

            var tmp = _items[originalSlot];
            _items[originalSlot] = _items[newSlot];
            _items[newSlot] = tmp;

            FireItemChanged(originalSlot);
            FireItemChanged(newSlot);
        }

        public int SlotOf(int id)
        {
            for (var slot = 0; slot < _items.Length; slot++)
            {
                var item = _items[slot];
                if (item != null && item.Id == id)
                    return slot;
            }

            return -1;
        }

        public Item[] ToArray()
        {
            var array = new Item[_items.Length];
            Array.Copy(_items, 0, array, 0, _items.Length);
            return array;
        }

        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        private void CheckSlot(int slot)
        {
            if (slot < 0 || slot >= _items.Length)
                throw new IndexOutOfRangeException("Slot out of range.");
        }
    }
}