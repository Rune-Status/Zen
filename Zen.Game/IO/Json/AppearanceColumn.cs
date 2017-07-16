using Newtonsoft.Json.Linq;
using Zen.Game.Model.Player;

namespace Zen.Game.IO.Json
{
    public class AppearanceColumn : Column
    {
        public override void Load(dynamic playerObject, Player player)
        {
            var appearanceObject = playerObject.Appearance;

            var gender = appearanceObject.Gender;
            var styleArray = appearanceObject.Style;
            var colorArray = appearanceObject.Colors;

            player.Appearance.Gender = (Gender) gender;

            for (var index = 0; index < styleArray.Count; index++)
                player.Appearance.Style[index] = (int) styleArray[index];

            for (var index = 0; index < colorArray.Count; index++)
                player.Appearance.Colors[index] = (int) colorArray[index];
        }

        public override void Save(dynamic playerObject, Player player)
        {
            dynamic appearanceObject = new JObject();

            var styles = player.Appearance.Style;
            var styleArray = new JArray();
            foreach (var style in styles)
                styleArray.Add(style);

            var colors = player.Appearance.Colors;
            var colorArray = new JArray();
            foreach (var color in colors)
                colorArray.Add(color);

            appearanceObject.Gender = player.Appearance.Gender;
            appearanceObject.Style = styleArray;
            appearanceObject.Colors = colorArray;

            playerObject.Appearance = appearanceObject;
        }
    }
}