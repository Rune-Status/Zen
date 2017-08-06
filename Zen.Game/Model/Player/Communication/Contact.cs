using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zen.Game.Model.Player.Communication
{
    public class Contact
    {

        private string Username { get; set; }
        private int WorldId { get; set; }

        public Contact(string username)
        {
            Username = username;
        }

    }
}
