using Server;

namespace Midgard.Engines.Races
{
    internal class HalfDrow : MidgardRace
    {
        private static int[] m_HairHues = new int[] { 1113 };
        private static int[] m_SkinHues = new int[] { 1405 };

        public HalfDrow( int raceID, int raceIndex )
            : base( raceID, raceIndex, "HalfDrow", "HalfDrowes", 400, 401, 402, 403 )
        {
        }

        public override RaceLanguageFlag LanguageFlag { get { return RaceLanguageFlag.HalfDrow; } }

        public override bool IsEvilAlignedRace
        {
            get { return true; }
        }

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
            return female ? 8253 : 8252;
        }

        public override bool ValidateFacialHair( bool female, int itemID )
        {
            return itemID == 786;
        }

        public override int RandomFacialHair( bool female )
        {
            return 786;
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

        public override double GetGainFactorBonuses( Skill skill )
        {
            if( skill.SkillName == SkillName.Fencing )
                return 0.1;

            return base.GetGainFactorBonuses( skill );
        }

        public override int InfravisionLevel { get { return 6; } }
        public override int InfravisionDuration { get { return 30; } }

        private static readonly RaceSkillMod[] m_SkillBonusList = new RaceSkillMod[]
        {
            new RaceSkillMod( SkillName.Magery, true, 5 ),
            new RaceSkillMod( SkillName.MagicResist, true, -5 ),
        };

        public override RaceSkillMod[] GetSkillBonuses()
        {
            return m_SkillBonusList;
        }

        public override bool SameHairSameSkinHues { get { return true; } }
    }
}