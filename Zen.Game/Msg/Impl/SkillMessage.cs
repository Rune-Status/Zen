using Zen.Game.Model;

namespace Zen.Game.Msg.Impl
{
    public class SkillMessage : Message
    {
        public SkillMessage(SkillSet.SkillMetadata metadata)
        {
            Metadata = metadata;
        }

        public SkillSet.SkillMetadata Metadata { get; }
    }
}