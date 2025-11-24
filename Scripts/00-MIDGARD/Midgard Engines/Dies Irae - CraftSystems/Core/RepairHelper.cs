using System;
using Server;

namespace Midgard.Engines.OldCraftSystem
{
    public static class RepairHelper
    {
        public static int GetWeakenChance( Mobile mob, SkillName skill, int curHits, int maxHits )
        {
            // 40% + (1% per hp lost) - (1% per 10 craft skill)
            return ( 40 + ( maxHits - curHits ) ) - (int)( mob.Skills[ skill ].Value / 10 );
        }

        public static bool CheckWeaken( Mobile mob, SkillName skill, int curHits, int maxHits )
        {
            return ( GetWeakenChance( mob, skill, curHits, maxHits ) > Utility.Random( 100 ) );
        }

        public static int GetRepairDifficulty( int curHits, int maxHits )
        {
            return ( ( ( maxHits - curHits ) * 1250 ) / Math.Max( maxHits, 1 ) ) - 250;
        }

        public static bool CheckRepairDifficulty( Mobile mob, SkillName skill, int curHits, int maxHits )
        {
            double difficulty = GetRepairDifficulty( curHits, maxHits ) * 0.1;

            return mob.CheckSkill( skill, difficulty - 25.0, difficulty + 25.0 );
        }
    }
}