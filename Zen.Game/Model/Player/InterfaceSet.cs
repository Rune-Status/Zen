using System.Collections.Generic;
using System.Linq;
using Zen.Game.Msg.Impl;

namespace Zen.Game.Model.Player
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
            Resizable ? Slot.ResizableInterface : Slot.FixedInterface);

        public void OnLogin(int displayMode)
        {
            /* Determine display mode. */
            if (displayMode == 0 || displayMode == 1)
                Mode = DisplayMode.Fixed;
            else
                Mode = DisplayMode.Resizable;

            /* Send the Welcome Screen. */
            OpenRootInterface(Interface.Empty);
            Open(Interface.WelcomeScreen, Interface.Empty, 2);
            Open(Interface.MessageOfTheWeek, Interface.Empty, 3);
        }

        public void OpenGameFrame()
        {
            OpenRootInterface(Resizable ? Interface.Resizable : Interface.Fixed);

            /* Setup Chatbox. */
            OpenWindowInterface(Interface.Chatbox, Resizable ? 70 : 75);
            OpenWindowInterface(Interface.ChatboxOptions, Resizable ? 23 : 14);
            OpenWindowInterface(Interface.PrivateChat, Resizable ? 71 : 10);
            Open(Interface.ChatboxBar, Interface.Chatbox, 8);

            /* Open Game Orbs. */
            OpenWindowInterface(Orb.Hitpoints, Resizable ? 13 : 70);
            OpenWindowInterface(Orb.Prayer, Resizable ? 14 : 71);
            OpenWindowInterface(Orb.Energy, Resizable ? 15 : 72);
            OpenWindowInterface(Orb.Summoning, Resizable ? 16 : 73);

            /* Open Game Tabs. */
            Equipment.OpenAttackTab(_player);
            OpenTab(Tab.Skills, Interface.Skills);
            OpenTab(Tab.Quest, Interface.Quests);
            OpenTab(Tab.Inventory, Interface.Inventory);
            OpenTab(Tab.Equipment, Interface.Equipment);
            OpenTab(Tab.Prayer, Interface.Prayer);
            OpenTab(Tab.Magic, Interface.Magic);
            OpenTab(Tab.Friends, Interface.Friends);
            OpenTab(Tab.Ignores, Interface.Ignores);
            OpenTab(Tab.Clan, Interface.Clan);
            OpenTab(Tab.Settings, Interface.Settings);
            OpenTab(Tab.Emotes, Interface.Emotes);
            OpenTab(Tab.Music, Interface.Music);
            OpenTab(Tab.Logout, Interface.Logout);
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
        public void OpenTab(int tab, int id) => Open(id, Root, (Resizable ? Slot.ResizableTab : Slot.FixedTab) + tab);

        public void OpenChatboxInterface(int id, int child) => Open(id,
            Resizable ? Slot.ResizableChatbox : Slot.FixedChatbox, child);

        public void OpenInterface(int id) => Open(id, Root, Resizable ? Slot.ResizableInterface : Slot.FixedInterface,
            false);

        public void RemoveOpenInterface() => RemoveInterface(
            Resizable ? Slot.ResizableInterface : Slot.FixedInterface);

        public void RemoveInterfaceByParent(int parent, int child) => RemoveInterfaceByParent(ToBitpackedId(parent,
            child));

        private static int ToBitpackedId(int parent, int child) => (parent << 16) | child;
    }
}