namespace Zen.Game.Model.Player.Communication
{
    public class Contact
    {
        public string Username { get; }
        public int WorldId { get; set; }

        public Contact(string username)
        {
            Username = username;
        }

    }
}
