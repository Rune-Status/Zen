using Zen.Game.Model.Player;

namespace Zen.Game.IO
{
    public abstract class PlayerSerializer
    {
        public abstract SerializeResult Load(World world, string username, string password);
        public abstract void Save(Player player);

        public class SerializeResult
        {
            public SerializeResult(int status, Player player = null)
            {
                Status = status;
                Player = player;
            }

            public int Status { get; }
            public Player Player { get; }
        }
    }
}