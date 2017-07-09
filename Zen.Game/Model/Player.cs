using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Zen.Game.Msg;
using Zen.Game.Msg.Impl;

namespace Zen.Game.Model
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Player : Mob
    {
        public Player(string username, string password)
        {
            Username = username;
            Password = password;
            InitializeMembers();
        }

        [JsonProperty]
        public string Username { get; }
        [JsonProperty]
        public string Password { get; }
        [JsonProperty]
        public int Rights { get; } = 2;
        [JsonProperty]
        [JsonConverter(typeof(JavaScriptDateTimeConverter))]
        public DateTime CreationDateTime { get; set; }
        public PlayerSession Session { get; set; }
        [JsonProperty]
        public Appearance Appearance { get; } = Appearance.DefaultAppearance;
        public int[] AppearanceTickets { get; } = new int[2500];
        public List<Player> LocalPlayers { get; } = new List<Player>();
        public bool RegionChanging { get; private set; }
        public Position LastKnownRegion { get; private set; }
        public InterfaceSet InterfaceSet { get; private set; }
        public ChatMessage ChatMessage { get; private set; }
        [JsonProperty]
        public SkillSet SkillSet { get; private set; }
        [JsonProperty]
        public Inventory Inventory { get; private set; }
        [JsonProperty]
        public Equipment Equipment { get; private set; }

        private void InitializeMembers()
        {
            InterfaceSet = new InterfaceSet(this);
            SkillSet = new SkillSet(this);
            Inventory = new Inventory(this);
            Equipment = new Equipment(this);
            CreationDateTime = DateTime.UtcNow;
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