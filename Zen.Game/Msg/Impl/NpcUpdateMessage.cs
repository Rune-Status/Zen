using System.Collections.Generic;
using Zen.Game.Model;
using Zen.Game.Update.Descriptor;

namespace Zen.Game.Msg.Impl
{
    public class NpcUpdateMessage : IMessage
    {
        public NpcUpdateMessage(Position lastKnownRegion, Position position, int localNpcCount,
            List<NpcDescriptor> descriptors)
        {
            LastKnownRegion = lastKnownRegion;
            Position = position;
            LocalNpcCount = localNpcCount;
            Descriptors = descriptors;
        }

        public Position LastKnownRegion { get; }
        public Position Position { get; }
        public int LocalNpcCount { get; }
        public List<NpcDescriptor> Descriptors { get; }
    }
}