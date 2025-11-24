using Server;

namespace Midgard.Engines.Races
{
    internal class Drow : MidgardRace
    {
        private static readonly int[] m_HairHues = new int[] { 0x86E };
        private static readonly int[] m_SkinHues = new int[] { 0x581 };

        public Drow( int raceID, int raceIndex )
            : base( raceID, raceIndex, "Drow", "Dwowes", 400, 401, 402, 403 )
        {
        }

        public override RaceLanguageFlag LanguageFlag { get { return RaceLanguageFlag.Drow; } }

        public override bool IsEvilAlignedRace
        {
            get { return true; }
        }

        public override bool ValidateHair( bool female, int itemID )
        {
            if( itemID == 0 )
                return false;

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
            return Utility.RandomList( 0x203B, 0x2049, 0x2048, 0x203C );
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
            if( skill.SkillName == SkillName.Fencing )
                return 0.2;

            return base.GetGainFactorBonuses( skill );
        }

        private readonly int[] m_ElementalBonuses = new int[] { 0, +5, 0, 0, -5 };

        public override int GetResistanceBonus( ResistanceType type )
        {
            return m_ElementalBonuses[ (int)type ];
        }

        public override int InfravisionLevel { get { return 15; } }
        public override int InfravisionDuration { get { return 60; } }

        private static readonly RaceSkillMod[] m_SkillBonusList = new RaceSkillMod[]
        {
            new RaceSkillMod( SkillName.Magery, true, 10 ),
            new RaceSkillMod( SkillName.Poisoning, true, 5 ),
            new RaceSkillMod( SkillName.Fencing, true, 5 ),
            new RaceSkillMod( SkillName.MagicResist, true, -10 ),
            new RaceSkillMod( SkillName.Macing, true, -10 ),
            new RaceSkillMod( SkillName.Swords, true, -10 ),
        };

        public override RaceSkillMod[] GetSkillBonuses()
        {
            return m_SkillBonusList;
        }

        public override bool SameHairSameSkinHues { get { return true; } }
    }
}