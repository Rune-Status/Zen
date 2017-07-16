using Newtonsoft.Json.Linq;
using Zen.Game.Model.Player;

namespace Zen.Game.IO.Json
{
    public class SkillColumn : Column
    {
        public override void Load(dynamic playerObject, Player player)
        {
            var skillArray = playerObject.Skills;

            foreach (var skill in skillArray)
            {
                int id = skill.Id;
                int level = skill.Level;
                double experience = skill.Experience;

                var metadata = new SkillSet.SkillMetadata(id, level, experience);
                player.SkillSet.SetMetadata(id, metadata);
            }
        }

        public override void Save(dynamic playerObject, Player player)
        {
            var skillArray = new JArray();

            for (var id = 0; id < 24; id++)
            {
                dynamic skillObject = new JObject();

                skillObject.Id = id;
                skillObject.Level = player.SkillSet.GetLevel((Skill) id);
                skillObject.Experience = player.SkillSet.GetExperience((Skill) id);

                skillArray.Add(skillObject);
            }

            playerObject.Skills = skillArray;
        }
    }
}