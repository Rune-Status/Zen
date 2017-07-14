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
        public DisplayMode Mode { get; private set; }
        public bool Resizable => Mode == DisplayMode.Resizable;

        public bool InterfaceOpen => OpenedInterfaceAtParent(
            Resizable ? Slots.ResizableInterface : Slots.FixedInterface);

        public void OnLogin(int displayMode)
        {
            /* Determine display mode and open root interface. */
            if (displayMode == 0 || displayMode == 1)
                Mode = DisplayMode.Fixed;
            else
                Mode = DisplayMode.Resizable;

            OpenRootInterface(Resizable ? Interfaces.Resizable : Interfaces.Fixed);

            /* Setup Chatbox. */
            OpenWindowInterface(Interfaces.Chatbox, Resizable ? 70 : 75);
            OpenWindowInterface(Interfaces.ChatboxOptions, Resizable ? 23 : 14);
            OpenWindowInterface(Interfaces.PrivateChat, Resizable ? 71 : 10);
            Open(Interfaces.ChatboxBar, Interfaces.Chatbox, 8);

            /* Open Game Orbs. */
            OpenWindowInterface(Orbs.Hitpoints, Resizable ? 13 : 70);
            OpenWindowInterface(Orbs.Prayer, Resizable ? 14 : 71);
            OpenWindowInterface(Orbs.Energy, Resizable ? 15 : 72);
            OpenWindowInterface(Orbs.Summoning, Resizable ? 16 : 73);

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
            var toRemove = _interfaces.Keys.Where(x => x >> 16 == parent).ToList();
            foreach (var key in toRemove)
                _interfaces.Remove(key);
        }

        public void RemoveInterfaceByParent(int bitpackedId)
        {
            if (!_interfaces.ContainsKey(bitpackedId))
                return;

            var @interface = _interfaces[bitpackedId];
            _interfaces.Remove(bitpackedId);

            ClearChildren(@interface);
            _player.Send(new InterfaceCloseMessage(bitpackedId));
        }


        public bool OpenedInterfaceAtParent(int child) => OpenedAtParent(Root, child);
        public bool OpenedAtParent(int parent, int child) => _interfaces.ContainsKey(ToBitpackedId(parent, child));
        public void RemoveInterface(int child) => RemoveInterfaceByParent(Root, child);


        public void OpenWindowInterface(int id, int child) => Open(id, Root, child);
        public void OpenTab(int tab, int id) => Open(id, Root, (Resizable ? Slots.ResizableTab : Slots.FixedTab) + tab);

        public void OpenChatboxInterface(int id, int child) => Open(id,
            Resizable ? Slots.ResizableChatbox : Slots.FixedChatbox, child);

        public void OpenInterface(int id) => Open(id, Root, Resizable ? Slots.ResizableInterface : Slots.FixedInterface,
            false);

        public void RemoveOpenInterface() => RemoveInterface(
            Resizable ? Slots.ResizableInterface : Slots.FixedInterface);

        public void RemoveInterfaceByParent(int parent, int child) => RemoveInterfaceByParent(ToBitpackedId(parent,
            child));

        private static int ToBitpackedId(int parent, int child) => (parent << 16) | child;
    }
}