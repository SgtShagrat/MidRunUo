using Server;

namespace Midgard.Engines.Classes
{
    public class SkillBonuses
    {
        internal static void RegisterHandler()
        {
            Skill.ClassHandler = new Skill.ClassBonusHandler( GetSkillBonus );
        }

        internal static ClassSkillMod[] EmptyList = new ClassSkillMod[] { };

        public static double GetSkillBonus( Mobile m, SkillName skill, double nonRacialValue )
        {
            if( !m.Player )
                return nonRacialValue;

            ClassSystem system = ClassSystem.Find( m );
            if( system == null )
                return nonRacialValue;

            if( !system.IsAllowedSkill( skill ) )
                return 0;

            ClassSkillMod[] list = system.GetSkillBonuses();
            if( list == null )
                return nonRacialValue;

            foreach( ClassSkillMod rsm in list )
            {
                if( rsm.Match( skill ) )
                    return nonRacialValue + rsm.Value;
            }

            return nonRacialValue;
        }
    }
}