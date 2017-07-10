using System.Collections.Generic;
using Zen.Game.Model;
using Zen.Game.Msg.Impl;

namespace Zen.Game.Update
{
    public class PlayerUpdater
    {
        private readonly GameWorld _world;

        public PlayerUpdater(GameWorld world)
        {
            _world = world;
        }

        public void Tick()
        {
            foreach (var player in _world.GetPlayers())
                Preprocess(player);

            foreach (var player in _world.GetPlayers())
                UpdatePlayers(player);

            foreach (var player in _world.GetPlayers())
                Postprocess(player);
        }

        private void Preprocess(Player player)
        {
            if (player.WalkingQueue.MinimapFlagReset)
            {
                // Send Reset Minimap Flag Reset.
            }

            if (IsRegionChangeRequired(player))
                player.Send(new RegionChangeMessage(player));

            player.WalkingQueue.Tick();
        }

        private void UpdatePlayers(Player player)
        {
            var lastKnownRegion = player.LastKnownRegion;
            var position = player.Position;
            var tickets = player.AppearanceTickets;

            var selfDescriptor = player.Teleporting
                ? new TeleportPlayerDescriptor(player, tickets)
                : PlayerDescriptor.Create(player, tickets);

            var descriptors = new List<PlayerDescriptor>();
            var localPlayers = player.LocalPlayers;
            var localPlayerCount = localPlayers.Count;

            foreach (var p in localPlayers)
                if (!p.Active || p.Teleporting || !position.IsWithinDistance(p.Position))
                {
                    localPlayers.Remove(p);
                    descriptors.Add(new RemovePlayerDescriptor(p, tickets));
                }
                else
                {
                    descriptors.Add(PlayerDescriptor.Create(p, tickets));
                }

            foreach (var p in _world.GetPlayers())
            {
                if (localPlayers.Count >= 255)
                    break;
                if (p == player || !position.IsWithinDistance(p.Position) || localPlayers.Contains(p))
                    continue;
                localPlayers.Add(p);
                descriptors.Add(new AddPlayerDescriptor(p, tickets));
            }

            player.Send(new PlayerUpdateMessage(lastKnownRegion, position, localPlayerCount, selfDescriptor,
                descriptors));
        }

        private void Postprocess(Player player)
        {
            player.Reset();
        }

        private bool IsRegionChangeRequired(Player player)
        {
            var lastKnownRegion = player.LastKnownRegion;
            var position = player.Position;

            var deltaX = position.GetLocalX(lastKnownRegion.CentralRegionX);
            var deltaY = position.GetLocalY(lastKnownRegion.CentralRegionY);

            return deltaX < 16 || deltaX >= 88 || deltaY < 16 || deltaY >= 88;
        }
    }
}