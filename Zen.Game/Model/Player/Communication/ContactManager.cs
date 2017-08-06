using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zen.Game.Msg.Impl;

namespace Zen.Game.Model.Player.Communication
{
    public class ContactManager
    {

        public const int MaxContacts = 200;
        public const int MaxIgnores = 100;

        private Player Player;
        public Dictionary<string, Contact> Contacts { get; set; }
        public List<string> Ignored { get; set; }

        public ContactManager(Player player)
        {
            Player = player;
            Contacts = new Dictionary<string, Contact>(MaxContacts);
            Ignored = new List<string>(MaxIgnores);
        }

        public void AddContact(Player player, string contact)
        {
            if (Contacts.Count() >= MaxContacts)
            {
                player.SendGameMessage("Your friends list is full.");
                return;
            }
            if (Contacts.ContainsKey(contact))
            {
                player.SendGameMessage(contact + " is already on your friends list.");
                return;
            }

            Contacts.Add(contact, new Contact(contact));
        }

        public void RemoveContact(Player player, string contact, bool block)
        {
            if (block)
            {
                Ignored.Remove(contact);
            }
            else
            {
                Contacts.Remove(contact);
            }
        }

        public void AddIgnore(Player player, string contact)
        {
            if (Ignored.Count() >= MaxIgnores)
            {
                player.SendGameMessage("Your ignore list is full.");
                return;
            }
            if (Ignored.Contains(contact))
            {
                player.SendGameMessage(contact + " is already on your ignore list.");
                return;
            }
            Ignored.Add(contact);
        }

        public bool HasContact(string contact) => Contacts.ContainsKey(contact);
    }
}
