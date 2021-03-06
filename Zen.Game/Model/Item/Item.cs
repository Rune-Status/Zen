﻿using System;
using Zen.Game.Definition;
using Zen.Game.Model.Player;

namespace Zen.Game.Model.Item
{
    public class Item
    {
        public Item(int id, int amount = 1)
        {
            if (amount < 0)
                throw new ArgumentException();

            Id = id;
            Amount = amount;
        }

        public ItemDefinition Definition => ItemDefinition.ForId(Id);
        public EquipmentDefinition EquipmentDefinition => EquipmentDefinition.ForId(Id);

        public int Amount { get; }
        public int Id { get; }

        protected bool Equals(Item other)
        {
            return Amount == other.Amount && Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Item) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Amount * 397) ^ Id;
            }
        }
    }
}