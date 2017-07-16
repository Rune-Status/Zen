using Zen.Game.Model.Player;

namespace Zen.Game.Msg.Impl
{
    public class SkillMessage : IMessage
    {
        public SkillMessage(SkillSet.SkillMetadata metadata)
        {
            Metadata = metadata;
        }

        public SkillSet.SkillMetadata Metadata { get; }
    }
}