using Zen.Game.Model;
using Zen.Game.Model.Player;
using Zen.Game.Plugin.Type;

namespace Zen.Game.Plugin.Commands
{
    public class PositionCommands : ICommandPlugin
    {
        public void Initiate(PluginRepository repository)
        {
            repository.Register(this, "tele", "pos");
        }

        public void OnCommand(Player player, string keyword, string[] arguments)
        {
            if (player.Rights < 2)
                return;

            if (keyword == "tele")
            {
                int x, y, height;

                switch (arguments.Length)
                {
                    case 1:
                        arguments = arguments[0].Split(',');
                        height = int.Parse(arguments[0]);
                        x = int.Parse(arguments[1]) << 6 | int.Parse(arguments[3]);
                        y = int.Parse(arguments[2]) << 6 | int.Parse(arguments[4]);
                        break;
                    case 2:
                    case 3:
                        x = int.Parse(arguments[0]);
                        y = int.Parse(arguments[1]);
                        height = player.Position.Height;

                        if (arguments.Length == 3)
                            height = int.Parse(arguments[2]);
                        break;
                    default:
                        player.SendGameMessage("Syntax: ::tele [x] [y] [height=0]");
                        return;
                }
                
                player.Teleport(new Position(x, y, height));
            }
            else if (keyword == "pos")
            {
                if (arguments.Length != 0)
                {
                    player.SendGameMessage("Syntax: ::pos");
                    return;
                }

                player.SendGameMessage(player.Position.ToString());
            }
        }
    }
}