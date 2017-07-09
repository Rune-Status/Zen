using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Zen.Game.Model;
using Zen.Shared;

namespace Zen.Game.IO.Serializers
{
    public class JsonPlayerSerializer : PlayerSerializer
    {
        public override SerializeResult Load(string username, string password)
        {
            var filePath = GameConstants.CharacterFolder + username + ".json";
            if (!File.Exists(filePath))
            {
                return new SerializeResult(LoginConstants.StatusOk, new Player(username, password)
                {
                    Position = new Position(3093, 3493)
                });
            }

            using (var file = File.OpenText(filePath))
            {
                var serializer = new JsonSerializer
                {
                    Formatting = Formatting.Indented,
                    NullValueHandling = NullValueHandling.Ignore
                };

                var player = serializer.Deserialize(file, typeof(Player)) as Player;

                if (player == null) return new SerializeResult(LoginConstants.StatusErrorLoadingProfile);
                if (!player.Password.Equals(password)) return new SerializeResult(LoginConstants.StatusInvalidPassword);

                return new SerializeResult(LoginConstants.StatusOk, player);
            }
        }

        public override void Save(Player player)
        {
            var filePath = GameConstants.CharacterFolder + player.Username + ".json";

            var serializer = new JsonSerializer
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            };

            using (StreamWriter sw = new StreamWriter(filePath))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, player);
            }
        }
    }
}