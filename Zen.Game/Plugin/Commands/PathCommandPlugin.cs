using System;
using Zen.Game.Model;
using Zen.Game.Model.Pathfinding;
using Zen.Game.Model.Player;
using Zen.Game.Plugin.Type;

namespace Zen.Game.Plugin.Commands
{
    public class PathCommandPlugin : ICommandPlugin
    {
        public void Initiate(PluginRepository repository)
        {
            repository.Register(this, "path");
        }

        public void OnCommand(Player player, string keyword, string[] arguments)
        {
            var pathFinder = new AStarPathFinder(player.World.TraversalMap);
            var x = int.Parse(arguments[0]);
            var y = int.Parse(arguments[1]);

            player.WalkingQueue.Reset();
            var path = pathFinder.Find(player, new Position(x, y, player.Position.Height));

            if (path == null)
                return;

            player.WalkingQueue.AddFirstStep(path.Poll());
            while (!path.Empty)
                player.WalkingQueue.AddStep(path.Poll());
        }
    }
}