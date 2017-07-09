using Zen.Game.Model;
using Zen.Shared;

namespace Zen.Game.IO
{
    public class DummyPlayerSerializer : PlayerSerializer
    {
        public override SerializeResult Load(string username, string password)
        {
            var player = new Player(username, password)
            {
                Position = new Position(3093, 3493)
            };
            return new SerializeResult(LoginConstants.StatusOk, player);
        }

        public override void Save(Player player)
        {
            /* Discord player. */
        }
    }
}