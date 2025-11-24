using Server;

namespace Midgard.Engines.Races
{
    internal class HighOrc : MidgardRace
    {
        private static int[] m_HairHues = new int[] { 0 };
        private static int[] m_SkinHues = new int[] { 0x5A2, 0x709, 0x70E };

        public HighOrc( int raceID, int raceIndex )
            : base( raceID, raceIndex, "High Orc", "HighOrcs", 400, 401, 402, 403 )
        {
        }

        public override RaceLanguageFlag LanguageFlag { get { return RaceLanguageFlag.HighOrc; } }

        public override bool IsEvilAlignedRace
        {
            get { return true; }
        }

        public override bool ValidateHair( bool female, int itemID )
        {
            if( itemID == 0 )
                return true;

            return false;
        }

        public override int RandomHair( bool female )
        {
            return 0;
        }

        public override bool ValidateFacialHair( bool female, int itemID )
        {
            return itemID == 0x141B;
        }

        public override int RandomFacialHair( bool female )
        {
            return 0x141B;
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
            if( skill.SkillName == SkillName.Macing )
                return 0.2;

            return base.GetGainFactorBonuses( skill );
        }

        public override bool SupportsBless { get { return true; } }

        private static readonly RaceSkillMod[] m_SkillBonusList = new RaceSkillMod[]
        {
            new RaceSkillMod( SkillName.Macing, true, 10 ),
            new RaceSkillMod( SkillName.MagicResist, true, 10 ),
            new RaceSkillMod( SkillName.Swords, true, -10 ),
            new RaceSkillMod( SkillName.Archery, true, -10 ),
        };

        public override RaceSkillMod[] GetSkillBonuses()
        {
            return m_SkillBonusList;
        }

        public override bool SameHairSameSkinHues { get { return true; } }
    }
}