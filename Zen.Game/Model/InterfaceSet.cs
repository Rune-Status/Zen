using System;
using System.Collections.Generic;
using System.Linq;
using Zen.Game.Msg.Impl;

namespace Zen.Game.Model
{
    public class InterfaceSet
    {
        private readonly List<Interface> _interfaces = new List<Interface>();
        private readonly Player _player;

        public InterfaceSet(Player player)
        {
            _player = player;
        }

        public Interface Root { get; private set; }
        public bool Resizable { get; private set; }

        public void OnLogin(bool resizable)
        {
            /* Open Root Interface. */
            Resizable = resizable;
            OpenRootInterface(new Interface(resizable ? Interfaces.Resizable : Interfaces.Fixed));

            /* Setup Chatbox. */
            OpenInterface(Interfaces.Chatbox, resizable ? 70 : 75);
            OpenInterface(Interfaces.ChatboxOptions, resizable ? 23 : 14);
            OpenInterface(Interfaces.PrivateChat, resizable ? 71 : 10);

            /* Open Game Orbs. */
            OpenInterface(Orbs.Hitpoints, resizable ? 13 : 70);
            OpenInterface(Orbs.Prayer, resizable ? 14 : 71);
            OpenInterface(Orbs.Energy, resizable ? 15 : 72);
            OpenInterface(Orbs.Summoning, resizable ? 16 : 73);
            OpenChatboxInterface(Interfaces.ChatboxBar, 8);

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

        public bool OpenChatboxInterface(int id, int slot, bool transparent = true)
        {
            var chatbox = GetOpenInterface(Interfaces.Chatbox);
            if (chatbox == null) return false;

            OpenInterface(new Interface(id, chatbox, slot, transparent));
            return true;
        }

        public bool OpenInterface(int id, bool transparent = true)
        {
            if (Root == null || Root.Id != Interfaces.Fixed && Root.Id != Interfaces.Resizable)
                return false;

            OpenInterface(new Interface(id, Root,
                Resizable ? Slots.ResizableInterface : Slots.FixedInterface, transparent));
            return true;
        }

        public bool OpenInterface(int id, int slot, bool transparent = true)
        {
            if (Root == null || Root.Id != Interfaces.Fixed && Root.Id != Interfaces.Resizable)
                return false;

            OpenInterface(new Interface(id, Root, slot, transparent));
            return true;
        }

        public void OpenTab(int tab, int id)
        {
            if (Root == null || Root.Id != Interfaces.Fixed && Root.Id != Interfaces.Resizable)
                return;

            var slot = (Resizable ? Slots.ResizableTab : Slots.FixedTab) + tab;
            OpenInterface(new Interface(id, Root, slot, true));
        }

        private void OpenRootInterface(Interface root)
        {
            if (!root.Root) throw new ArgumentException();
            if (Root != null) CloseInterface(Root);
            if (!AddInterface(root)) throw new Exception("Could not add interface.");

            Root = root;
            _player.Send(new InterfaceRootMessage(root));
        }

        private bool AddInterface(Interface @interface)
        {
            if (Opened(@interface)) return false;
            _interfaces.Add(@interface);
            return true;
        }

        private bool RemoveInterface(Interface @interface)
        {
            if (!Opened(@interface)) return false;
            _interfaces.Remove(@interface);
            return true;
        }

        private bool Opened(Interface @interface) => _interfaces.Contains(@interface);

        private void OpenInterface(Interface @interface)
        {
            if (@interface.Root) return;
            if (Opened(@interface)) CloseInterface(@interface);
            if (@interface.Parent.SlotUsed(@interface.Slot))
                CloseInterface(@interface.Parent.GetChild(@interface.Slot));
            if (!AddInterface(@interface))
                throw new Exception("Could not add interface.");

            @interface.Parent.AddChild(@interface, @interface.Slot);
            _player.Send(new InterfaceOpenMessage(@interface));
        }

        private void CloseInterface(Interface @interface)
        {
            if (!Opened(@interface)) return;
            if (!RemoveInterface(@interface)) return;

            var children = @interface.GetChildren();
            foreach (var child in children)
                CloseInterface(child);

            @interface.Parent?.RemoveChild(@interface.Slot);

            if (@interface.Root) Root = null;
            else _player.Send(new InterfaceCloseMessage(@interface));
        }

        private Interface GetOpenInterface(int id)
        {
            return (from @interface in _interfaces
                where @interface.Id == id
                select Opened(@interface) ? @interface : null).FirstOrDefault();
        }

        public class Interface
        {
            private readonly Dictionary<int, Interface> _children = new Dictionary<int, Interface>();

            public Interface(int id)
            {
                Id = id;
                Root = true;
            }

            public Interface(int id, Interface parent, int slot, bool transparent)
            {
                Id = id;
                Parent = parent;
                Slot = slot;
                Transparent = transparent;
                Root = false;
            }

            public int Slot { get; }
            public Interface Parent { get; }
            public bool Root { get; }
            public bool Transparent { get; }
            public int Id { get; }

            public void AddChild(Interface child, int slot)
            {
                lock (_children)
                {
                    if (!SlotUsed(slot))
                    {
                        _children.Add(slot, child);
                    }
                    else
                    {
                        _children.Remove(slot);
                        _children.Add(slot, child);
                    }
                }
            }

            public void RemoveChild(int slot)
            {
                lock (_children)
                {
                    if (!SlotUsed(slot)) return;
                    _children.Remove(slot);
                }
            }

            public List<Interface> GetChildren()
            {
                lock (_children)
                {
                    return new List<Interface>(_children.Values);
                }
            }

            public Interface GetChild(int slot)
            {
                lock (_children)
                {
                    return !SlotUsed(slot) ? null : _children[slot];
                }
            }

            public bool SlotUsed(int slot)
            {
                lock (_children)
                {
                    return _children.ContainsKey(slot);
                }
            }

            protected bool Equals(Interface other)
            {
                return Slot == other.Slot && Id == other.Id;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != GetType()) return false;
                return Equals((Interface) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (Slot * 397) ^ Id;
                }
            }
        }

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
        }

        public class Slots
        {
            public const int FixedInterface = 11;
            public const int ResizableInterface = 6;

            public const int FixedTab = 83;
            public const int ResizableTab = 93;
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