using Server;

namespace Midgard.Engines.Races
{
    internal class HighElf : MidgardRace
    {
        public static MidgardRace Instance { get; private set; }

        private static int[] m_HairHues = new int[] { 51 };
        private static int[] m_SkinHues = new int[] { 451 };

        public HighElf( int raceID, int raceIndex )
            : base( raceID, raceIndex, "High Elf", "High Elves", 400, 401, 402, 403 )
        {
            Instance = this;
        }

        public override RaceLanguageFlag LanguageFlag { get { return RaceLanguageFlag.HighElf; } }

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

            #region Mondain Legacy Elf hair definitions

            if( ( female && ( itemID == 0x2FCD || itemID == 0x2FBF ) ) || ( !female && ( itemID == 0x2FCC || itemID == 0x2FD0 ) ) )
                return false;

            if( itemID >= 0x2FBF && itemID <= 0x2FC2 )
                return true;

            if( itemID >= 0x2FCC && itemID <= 0x2FD1 )
                return true;

            #endregion

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

        public override int InfravisionLevel { get { return 15; } }
        public override int InfravisionDuration { get { return 60; } }

        private static readonly RaceSkillMod[] m_SkillBonusList = new RaceSkillMod[]
        {
            new RaceSkillMod( SkillName.Fletching, true, 10 ),
            new RaceSkillMod( SkillName.Archery, true, 10 ),
            new RaceSkillMod( SkillName.Macing, true, -10 ),
        };

        public override RaceSkillMod[] GetSkillBonuses()
        {
            return m_SkillBonusList;
        }

        public override bool SameHairSameSkinHues { get { return true; } }
    }
}