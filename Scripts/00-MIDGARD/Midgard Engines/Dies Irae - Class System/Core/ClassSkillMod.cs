using Server;

namespace Midgard.Engines.Classes
{
    public class ClassSkillMod
    {
        public ClassSkillMod( SkillName skill, bool relative, double value )
        {
            Skill = skill;
            Relative = relative;
            Value = value;
        }

        public bool Relative { get; private set; }

        public double Value { get; private set; }

        public SkillName Skill { get; private set; }

        public bool Match( SkillName skill )
        {
            return skill == Skill;
        }
    }
}