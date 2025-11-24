using Server;

namespace Midgard.Engines.Races
{
    internal class NorthernElf : MidgardRace
    {
        private static int[] m_HairHues = new int[] { 1931 };
        private static int[] m_SkinHues = new int[] { 1154 };

        public NorthernElf( int raceID, int raceIndex )
            : base( raceID, raceIndex, "Northern Elf", "Northern Elves", 400, 401, 402, 403 )
        {
        }

        public override RaceLanguageFlag LanguageFlag { get { return RaceLanguageFlag.NorthernElf; } }

        public override bool ValidateHair( bool female, int itemID )
        {
            if( itemID == 0 )
                return true;

            if( ( female && itemID == 0x2048 ) || ( !female && itemID == 0x2046 ) )
                return false; //Buns & Receeding Hair

            if( itemID >= 0x203B && itemID <= 0x203D )
                return true;

            if( itemID >= 0x2044 && itemID <= 0x204A )
                return true;

            return false;
        }

        public override int RandomHair( bool female )
        {
            return 8252;
        }

        public override bool ValidateFacialHair( bool female, int itemID )
        {
            return itemID == 785;
        }

        public override int RandomFacialHair( bool female )
        {
            return 785;
        }

        public override int ClipSkinHue( int hue )
        {
            return m_SkinHues[ 0 ];
        }

        public override int RandomSkinHue()
        {
            return m_SkinHues[ Utility.Random( m_SkinHues.Length ) ];
        }

        public override int ClipHairHue( int hue )
        {
            return m_HairHues[ 0 ];
        }

        public override int RandomHairHue()
        {
            return m_HairHues[ Utility.Random( m_HairHues.Length ) ];
        }

        private static readonly MorphEntry[] m_MorphList = new MorphEntry[]
        {
        };

        public override MorphEntry[] GetMorphList()
        {
            return m_MorphList;
        }

        public override double GetGainFactorBonuses( Skill skill )
        {
            if( skill.SkillName == SkillName.AnimalTaming )
                return 0.2;
            else if( skill.SkillName == SkillName.Archery )
                return 0.2;

            return base.GetGainFactorBonuses( skill );
        }

        private int[] m_ElementalBonuses = new int[] { 0, -10, 10, 0, 0 };

        public override int GetResistanceBonus( ResistanceType type )
        {
            return m_ElementalBonuses[ (int)type ];
        }

        public override int InfravisionLevel { get { return 10; } }
        public override int InfravisionDuration { get { return 30; } }

        private static readonly RaceSkillMod[] m_SkillBonusList = new RaceSkillMod[]
        {
            new RaceSkillMod( SkillName.Fletching, true, 10 ),
            new RaceSkillMod( SkillName.Archery, true, 10 ),
            new RaceSkillMod( SkillName.Macing, true, -10 ),
            new RaceSkillMod( SkillName.Mining, true, -10 ),
        };

        public override RaceSkillMod[] GetSkillBonuses()
        {
            return m_SkillBonusList;
        }

        public override bool SameHairSameSkinHues { get { return true; } }
    }
}