using Server;

namespace Midgard.Engines.Races
{
    public class SkillBonuses
    {
        internal static void RegisterHandler()
        {
            Skill.RacialHandler = new Skill.RacialBonusHandler( GetSkillBonus );
        }

        internal static RaceSkillMod[] EmptyList = new RaceSkillMod[] { };

        public static double GetSkillBonus( Race race, SkillName skill, double nonRacialValue )
        {
            if( !Config.StatBonusesEnabled )
                return nonRacialValue;

            if( race is MidgardRace )
            {
                if( !( (MidgardRace)race ).IsAllowedSkill( skill ) )
                    return 0;

                RaceSkillMod[] list = ( (MidgardRace)race ).GetSkillBonuses();
                if( list == null )
                    return nonRacialValue;

                foreach( RaceSkillMod rsm in list )
                {
                    if( rsm.Match( skill ) )
                        return nonRacialValue + rsm.Value;
                }
            }

            return nonRacialValue;
        }
    }

    public class RaceSkillMod
    {
        public RaceSkillMod( SkillName skill, bool relative, double value )
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