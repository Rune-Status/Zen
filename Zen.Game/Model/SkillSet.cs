using System;
using Zen.Game.Msg.Impl;

namespace Zen.Game.Model
{
    public class SkillSet
    {
        private readonly SkillMetadata[] _metadata;
        private readonly Player _player;

        public SkillSet(Player player)
        {
            _player = player;
            _metadata = new SkillMetadata[24];

            for (var id = 0; id < _metadata.Length; id++)
                _metadata[id] = new SkillMetadata(id, 1, 0);

            _metadata[(int) Skill.Hitpoints] = new SkillMetadata(3, 10, 1184);
        }

        public int GetLevel(Skill skill) => _metadata[(int) skill].Level;
        public double GetExperience(Skill skill) => _metadata[(int) skill].Experience;

        public void SetLevelAndExperience(Skill skill, int level, int experience = -1)
        {
            var metadata = _metadata[(int) skill];

            metadata.Level = level;
            metadata.Experience = experience == -1
                ? GetExperienceForLevel(level)
                : experience;

            Refresh(metadata);
        }

        public int GetExperienceForLevel(int level)
        {
            double points = 0;
            var output = 0;
            for (var lvl = 1; lvl <= level; lvl++)
            {
                points += Math.Floor(lvl + 300.0 * Math.Pow(2.0, lvl / 7.0));
                if (lvl >= level) return output;
                output = (int) Math.Floor(points / 4);
            }
            return 0;
        }

        public int GetLevelForExperience(Skill skill)
        {
            var experience = GetExperience(skill);
            double points = 0;
            for (var lvl = 1; lvl <= 99; lvl++)
            {
                points += Math.Floor(lvl + 300.0 * Math.Pow(2.0, lvl / 7.0));
                var output = (int) Math.Floor(points / 4);
                if (output - 1 >= experience) return lvl;
            }
            return 99;
        }

        public void Refresh(SkillMetadata metadata)
        {
            _player.Send(new SkillMessage(metadata));
        }

        public void RefreshAll()
        {
            foreach (var metadata in _metadata)
                Refresh(metadata);
        }

        public class SkillMetadata
        {
            public SkillMetadata(int id, int level, double experience)
            {
                Id = id;
                Level = level;
                Experience = experience;
            }

            public int Id { get; }
            public int Level { get; internal set; }
            public double Experience { get; internal set; }
        }
    }
}