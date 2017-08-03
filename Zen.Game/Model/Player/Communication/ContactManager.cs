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
        public List<Contact> Contacts { get; set; }
        public List<string> Ignored { get; set; }

        public ContactManager(Player player)
        {
            Player = player;
            Contacts = new List<Contact>(MaxContacts);
            Ignored = new List<string>(MaxIgnores);

        }
    }
}
