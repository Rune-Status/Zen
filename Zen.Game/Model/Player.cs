using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Zen.Game.Msg;
using Zen.Game.Msg.Impl;

namespace Zen.Game.Model
{
    public class Player : Mob
    {
        public Player(string username, string password)
        {
            Username = username;
            Password = password;
            UpdateMembers();
        }

        public string Username { get; }
        public string Password { get; }
        public int Rights { get; set; } = 2;
        public DateTime CreationDateTime { get; set; } = DateTime.UtcNow;
        public PlayerSession Session { get; set; }
        public Appearance Appearance { get; } = Appearance.DefaultAppearance;
        public int[] AppearanceTickets { get; } = new int[2500];
        public List<Player> LocalPlayers { get; } = new List<Player>();
        public bool RegionChanging { get; private set; }
        public Position LastKnownRegion { get; private set; }
        public InterfaceSet InterfaceSet { get; private set; }
        public ChatMessage ChatMessage { get; private set; }
        public SkillSet SkillSet { get; private set; }
        public Inventory Inventory { get; private set; }
        public Equipment Equipment { get; private set; }

        private void UpdateMembers()
        {
            InterfaceSet = new InterfaceSet(this);
            SkillSet = new SkillSet(this);
            Inventory = new Inventory(this);
            Equipment = new Equipment(this);
        }

        public new void Reset()
        {
            base.Reset();
            RegionChanging = false;
            ChatMessage = null;
        }

        public int GetStance()
        {
            var weapon = Equipment.Get(Equipment.Weapon);
            return weapon == null ? 1426 : weapon.EquipmentDefinition.Stance;
        }

        public int Stance => Equipment.Get(Equipment.Weapon)?.EquipmentDefinition.Stance ?? 1426;
        public Task Send(Message message) => Session.Send(message);
        public bool IsChatUpdated() => ChatMessage != null;
        public void UpdateChatMessage(ChatMessage message) => ChatMessage = message;
        public void SendGameMessage(string text) => Send(new GameMessage(text));

        public void SetLastKnownRegion(Position lastKnownRegion)
        {
            LastKnownRegion = lastKnownRegion;
            RegionChanging = true;
        }
    }
}