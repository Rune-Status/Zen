using System.Collections.Generic;
using Zen.Game.Model;
using Zen.Game.Update;

namespace Zen.Game.Msg.Impl
{
    public class PlayerUpdateMessage : Message
    {
        public PlayerUpdateMessage(Position lastKnownRegion, Position position, int localPlayerCount,
            PlayerDescriptor selfDescriptor, List<PlayerDescriptor> descriptors)
        {
            LastKnownRegion = lastKnownRegion;
            Position = position;
            LocalPlayerCount = localPlayerCount;
            SelfDescriptor = selfDescriptor;
            Descriptors = descriptors;
        }

        public Position LastKnownRegion { get; }
        public Position Position { get; }
        public int LocalPlayerCount { get; }
        public PlayerDescriptor SelfDescriptor { get; }
        public List<PlayerDescriptor> Descriptors { get; }
    }
}