using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Zen.Game.Msg;
using Zen.Game.Msg.Impl;
using Zen.Shared;

namespace Zen.Game.Model
{
    public class Player : Mob
    {
        public Player(string username, string password)
        {
            Username = username;
            Password = password;
            Init();
        }

        public string Username { get; }
        public string Password { get; }
        public int Rights { get; set; } = 2;
        public DateTime CreationDateTime { get; set; } = DateTime.UtcNow;
        public PlayerSession Session { get; set; }
        public Appearance Appearance { get; } = Appearance.DefaultAppearance;
        public int[] AppearanceTickets { get; } = new int[2500];
        public List<Player> LocalPlayers { get; } = new List<Player>();
        public List<Npc> LocalNpcs { get; } = new List<Npc>();
        public bool RegionChanging { get; private set; }
        public Position LastKnownRegion { get; private set; }
        public InterfaceSet InterfaceSet { get; private set; }
        public ChatMessage ChatMessage { get; private set; }
        public SkillSet SkillSet { get; private set; }
        public Container Inventory { get; } = new Container(28);
        public Container Equipment { get; } = new Container(14);

        public int Stance => Equipment.Get(EquipmentConstants.Weapon)?.EquipmentDefinition.Stance ?? 1426;

        private void Init()
        {
            /* Initialize members with instances of this player. */
            InterfaceSet = new InterfaceSet(this);
            SkillSet = new SkillSet(this);

            /* Register container listeners. */
            Inventory.AddListener(new ContainerMessageListener(this, 149, 0, 93));
            Inventory.AddListener(new ContainerFullListener(this, "inventory"));

            Equipment.AddListener(new ContainerMessageListener(this, 387, 28, 94));
            Equipment.AddListener(new ContainerFullListener(this, "equipment"));
            Equipment.AddListener(new ContainerAppearanceListener(this));
        }

        public new void Reset()
        {
            base.Reset();
            RegionChanging = false;
            ChatMessage = null;
        }

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