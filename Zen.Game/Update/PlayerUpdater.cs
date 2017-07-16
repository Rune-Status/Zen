using System.Collections.Generic;
using Zen.Game.Model.Npc;
using Zen.Game.Model.Player;
using Zen.Game.Msg.Impl;
using Zen.Game.Update.Descriptor;

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
            foreach (var player in _world.Players)
                Preprocess(player);

            foreach (var npc in _world.Npcs)
                Preprocess(npc);

            foreach (var player in _world.Players)
            {
                UpdatePlayers(player);
                UpdateNpcs(player);
            }

            foreach (var player in _world.Players)
                Postprocess(player);

            foreach (var npc in _world.Npcs)
                Postprocess(npc);
        }

        private void Preprocess(Npc npc) => npc.WalkingQueue.Tick();

        private void Preprocess(Player player)
        {
            if (player.WalkingQueue.MinimapFlagReset)
                player.Send(new ResetMinimapFlagMessage());

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
            var playersToRemove = new List<Player>();

            var localPlayers = player.LocalPlayers;
            var localPlayerCount = localPlayers.Count;

            foreach (var p in localPlayers)
                if (!p.Active || p.Teleporting || !position.IsWithinDistance(p.Position))
                    playersToRemove.Add(p);
                else
                    descriptors.Add(PlayerDescriptor.Create(p, tickets));

            foreach (var p in playersToRemove)
            {
                localPlayers.Remove(p);
                descriptors.Add(new RemovePlayerDescriptor(p, tickets));
            }

            foreach (var p in _world.Players)
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

        private void UpdateNpcs(Player player)
        {
            var lastKnownRegion = player.LastKnownRegion;
            var position = player.Position;

            var descriptors = new List<NpcDescriptor>();
            var npcsToRemove = new List<Npc>();

            var localNpcs = player.LocalNpcs;
            var localNpcCount = localNpcs.Count;

            foreach (var n in localNpcs)
                if (!n.Active || n.Teleporting || !position.IsWithinDistance(n.Position))
                    npcsToRemove.Add(n);
                else
                    descriptors.Add(NpcDescriptor.Create(n));

            foreach (var n in npcsToRemove)
            {
                localNpcs.Remove(n);
                descriptors.Add(new RemoveNpcDescriptor(n));
            }

            foreach (var n in _world.Npcs)
            {
                if (localNpcs.Count >= 255)
                    break;
                if (!position.IsWithinDistance(n.Position) || localNpcs.Contains(n))
                    continue;
                localNpcs.Add(n);
                descriptors.Add(new AddNpcDescriptor(n));
            }

            player.Send(new NpcUpdateMessage(lastKnownRegion, position, localNpcCount, descriptors));
        }

        private void Postprocess(Player player) => player.Reset();
        private void Postprocess(Npc npc) => npc.Reset();

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