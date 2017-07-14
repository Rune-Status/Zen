using System.Collections.Generic;
using System.Linq;
using Zen.Game.Msg.Impl;

namespace Zen.Game.Model
{
    public class InterfaceSet
    {
        private readonly Dictionary<int, int> _interfaces = new Dictionary<int, int>();
        private readonly Player _player;

        public InterfaceSet(Player player)
        {
            _player = player;
        }

        public int Root { get; private set; }
        public bool Resizable { get; private set; }

        public void OnLogin(bool resizable)
        {
            /* Open Root Interface. */
            Resizable = resizable;
            OpenRootInterface(resizable ? Interfaces.Resizable : Interfaces.Fixed);

            /* Setup Chatbox. */
            OpenWindowInterface(Interfaces.Chatbox, resizable ? 70 : 75);
            OpenWindowInterface(Interfaces.ChatboxOptions, resizable ? 23 : 14);
            OpenWindowInterface(Interfaces.PrivateChat, resizable ? 71 : 10);
            Open(Interfaces.ChatboxBar, Interfaces.Chatbox, 8);

            /* Open Game Orbs. */
            OpenWindowInterface(Orbs.Hitpoints, resizable ? 13 : 70);
            OpenWindowInterface(Orbs.Prayer, resizable ? 14 : 71);
            OpenWindowInterface(Orbs.Energy, resizable ? 15 : 72);
            OpenWindowInterface(Orbs.Summoning, resizable ? 16 : 73);

            /* Open Game Tabs. */
            Equipment.OpenAttackTab(_player);
            OpenTab(Tabs.Skills, Interfaces.Skills);
            OpenTab(Tabs.Quest, Interfaces.Quests);
            OpenTab(Tabs.Inventory, Interfaces.Inventory);
            OpenTab(Tabs.Equipment, Interfaces.Equipment);
            OpenTab(Tabs.Prayer, Interfaces.Prayer);
            OpenTab(Tabs.Magic, Interfaces.Magic);
            OpenTab(Tabs.Friends, Interfaces.Friends);
            OpenTab(Tabs.Ignores, Interfaces.Ignores);
            OpenTab(Tabs.Clan, Interfaces.Clan);
            OpenTab(Tabs.Settings, Interfaces.Settings);
            OpenTab(Tabs.Emotes, Interfaces.Emotes);
            OpenTab(Tabs.Music, Interfaces.Music);
            OpenTab(Tabs.Logout, Interfaces.Logout);
        }

        public void Open(int id, int parent, int parentChild, bool transparent = true)
        {
            var bitpackedId = ToBitpackedId(parent, parentChild);
            if (_interfaces.ContainsKey(bitpackedId))
                ClearChildren(_interfaces[bitpackedId]);

            _interfaces[bitpackedId] = id;
            _player.Send(new InterfaceOpenMessage(id, bitpackedId, transparent));
        }

        public void OpenRootInterface(int root)
        {
            Root = root;
            _player.Send(new InterfaceRootMessage(root));
        }

        private void ClearChildren(int parent)
        {
            foreach (var key in _interfaces.Keys.Where(x => x >> 16 == parent))
                _interfaces.Remove(key);
        }

        public void OpenWindowInterface(int id, int child) => Open(id, Root, child);
        public void OpenTab(int tab, int id) => Open(id, Root, (Resizable ? Slots.ResizableTab : Slots.FixedTab) + tab);

        public void OpenChatboxInterface(int id, int child) => Open(id,
            Resizable ? Slots.ResizableChatbox : Slots.FixedChatbox, child);

        public void OpenInterface(int id) => Open(id, Root, Resizable ? Slots.ResizableInterface : Slots.FixedInterface,
            false);

        private static int ToBitpackedId(int parent, int child) => (parent << 16) | child;

        public class Interfaces
        {
            public const int Fixed = 548;
            public const int Resizable = 746;

            public const int Skills = 320;
            public const int Quests = 274;
            public const int Inventory = 149;
            public const int Equipment = 387;
            public const int Prayer = 271;
            public const int Magic = 192;
            public const int Summoning = 662;
            public const int Friends = 550;
            public const int Ignores = 551;
            public const int Clan = 589;
            public const int Settings = 261;
            public const int Emotes = 464;
            public const int Music = 187;
            public const int Logout = 182;

            public const int Chatbox = 752;
            public const int ChatboxOptions = 751;
            public const int PrivateChat = 754;
            public const int ChatboxBar = 137;

            public const int Axe = 75;
            public const int Maul = 76;
            public const int Bow = 77;
            public const int Claws = 78;
            public const int Longbow = 79;
            public const int FixedDevice = 80;
            public const int Godsword = 81;
            public const int Sword = 82;
            public const int Pickaxe = 83;
            public const int Halberd = 84;
            public const int Staff = 85;
            public const int Scythe = 86;
            public const int Spear = 87;
            public const int Mace = 88;
            public const int Dagger = 89;
            public const int MagicStaff = 90;
            public const int Thrown = 91;
            public const int Unarmed = 92;
            public const int Whip = 93;

            public const int DisplaySettings = 742;
            public const int AudioSettings = 743;
        }

        public class Slots
        {
            public const int FixedInterface = 11;
            public const int ResizableInterface = 6;

            public const int FixedTab = 83;
            public const int ResizableTab = 93;

            public const int FixedChatbox = 75;
            public const int ResizableChatbox = 70;
        }

        public class Orbs
        {
            public const int Hitpoints = 748;
            public const int Prayer = 749;
            public const int Energy = 750;
            public const int Summoning = 747;
        }

        public class Tabs
        {
            public const int Attack = 0;
            public const int Skills = 1;
            public const int Quest = 2;
            public const int Inventory = 3;
            public const int Equipment = 4;
            public const int Prayer = 5;
            public const int Magic = 6;
            public const int Summoning = 7;
            public const int Friends = 8;
            public const int Ignores = 9;
            public const int Clan = 10;
            public const int Settings = 11;
            public const int Emotes = 12;
            public const int Music = 13;
            public const int Logout = 14;
        }
    }
}