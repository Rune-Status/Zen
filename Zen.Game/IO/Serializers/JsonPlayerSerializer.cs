using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Zen.Game.IO.Json;
using Zen.Game.Model;
using Zen.Shared;

namespace Zen.Game.IO.Serializers
{
    public class JsonPlayerSerializer : PlayerSerializer
    {
        private readonly Column[] _columns =
        {
            new AppearanceColumn(), new SkillColumn(), new InventoryColumn(), new EquipmentColumn()
        };

        public override SerializeResult Load(string username, string password)
        {
            var path = GameConstants.CharacterFolder + username + ".json";
            var player = new Player(username, password) {Position = new Position(3093, 3493)};
            if (!File.Exists(path))
                return new SerializeResult(LoginConstants.StatusOk, player);

            dynamic playerObject;
            using (var stream = File.OpenText(path))
            using (JsonReader reader = new JsonTextReader(stream))
                playerObject = JToken.ReadFrom(reader) as JObject;

            if (playerObject == null)
                return new SerializeResult(LoginConstants.StatusErrorLoadingProfile);

            var playerPassword = (string) playerObject.Password;
            if (!password.Equals(playerPassword))
                return new SerializeResult(LoginConstants.StatusInvalidPassword);

            player.Rights = playerObject.Rights;

            var positionObject = playerObject.Position;
            int x = positionObject.X;
            int y = positionObject.Y;
            int height = positionObject.Height;
            player.Position = new Position(x, y, height);

            foreach (var column in _columns)
                column.Load(playerObject, player);

            return new SerializeResult(LoginConstants.StatusOk, player);
        }

        public override void Save(Player player)
        {
            dynamic playerObject = new JObject();

            playerObject.Password = player.Password;
            playerObject.Rights = player.Rights;

            dynamic positionObject = new JObject();
            positionObject.X = player.Position.X;
            positionObject.Y = player.Position.Y;
            positionObject.Height = player.Position.Height;
            playerObject.Position = positionObject;

            foreach (var column in _columns)
                column.Save(playerObject, player);

            var path = GameConstants.CharacterFolder + player.Username + ".json";
            using (var stream = new StreamWriter(path))
            {
                using (JsonWriter writer = new JsonTextWriter(stream))
                {
                    writer.Formatting = Formatting.Indented;
                    playerObject.WriteTo(writer);
                }
            }
        }
    }
}