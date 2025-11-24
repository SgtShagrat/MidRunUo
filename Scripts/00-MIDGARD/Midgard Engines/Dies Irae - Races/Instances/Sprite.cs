using Server;

namespace Midgard.Engines.Races
{
    internal class Sprite : MidgardRace
    {
        private static int[] m_HairHues = new int[] { 2122 };
        private static int[] m_SkinHues = new int[] { 1401 };

        public Sprite( int raceID, int raceIndex )
            : base( raceID, raceIndex, "Sprite", "Sprites", 400, 401, 402, 403 )
        {
        }

        public override RaceLanguageFlag LanguageFlag { get { return RaceLanguageFlag.Sprite; } }

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
            return 8253;
        }

        public override bool ValidateFacialHair( bool female, int itemID )
        {
            return false;
        }

        public override int RandomFacialHair( bool female )
        {
            return 0;
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
            new MorphEntry( 74, 1401, "an imp", 10, 1 ),
        };

        public override MorphEntry[] GetMorphList()
        {
            return m_MorphList;
        }

        public override double GetGainFactorBonuses( Skill skill )
        {
            if( skill.SkillName == SkillName.Magery )
                return 0.2;

            return base.GetGainFactorBonuses( skill );
        }

        private int[] m_ElementalBonuses = new int[] { -10, 0, 0, 0, 0 };

        public override int GetResistanceBonus( ResistanceType type )
        {
            return m_ElementalBonuses[ (int)type ];
        }

        public override bool SupportsFireworks { get { return true; } }
    }
}