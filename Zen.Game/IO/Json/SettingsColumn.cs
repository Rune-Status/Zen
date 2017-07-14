using Newtonsoft.Json.Linq;
using Zen.Game.Model;

namespace Zen.Game.IO.Json
{
    public class SettingsColumn : Column
    {
        public override void Load(dynamic playerObject, Player player)
        {
            var settingsObject = playerObject.Settings;
            if (settingsObject == null) return;

            var settings = player.Settings;
            settings.AutoRetaliating = settingsObject.AutoRetaliating;
            settings.AttackStyle = settingsObject.AttackStyle;
            settings.ChatFancy = settingsObject.ChatFancy;
            settings.Running = settingsObject.Running;
            settings.TwoButtonMouse = settingsObject.TwoButtonMouse;
            settings.SplitPrivateChat = settingsObject.SplitPrivateChat;
            settings.AcceptingAid = settingsObject.AcceptingAid;
        }

        public override void Save(dynamic playerObject, Player player)
        {
            dynamic settingsObject = new JObject();

            var settings = player.Settings;
            settingsObject.AutoRetaliating = settings.AutoRetaliating;
            settingsObject.AttackStyle = settings.AttackStyle;
            settingsObject.ChatFancy = settings.ChatFancy;
            settingsObject.Running = settings.Running;
            settingsObject.TwoButtonMouse = settings.TwoButtonMouse;
            settingsObject.SplitPrivateChat = settings.SplitPrivateChat;
            settingsObject.AcceptingAid = settings.AcceptingAid;

            playerObject.Settings = settingsObject;
        }
    }
}