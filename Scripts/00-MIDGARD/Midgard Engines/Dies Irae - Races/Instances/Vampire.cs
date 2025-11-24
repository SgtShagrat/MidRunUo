using System;

using Server;

namespace Midgard.Engines.Races
{
    internal class Vampire : MidgardRace
    {
        private static readonly int[] m_HairHues = new int[] { 0 };
        private static readonly int[] m_SkinHues = new int[] { 0 };

        public Vampire( int raceID, int raceIndex )
            : base( raceID, raceIndex, "Vampire", "Vampires", 400, 401, 402, 403 )
        {
        }

        public override RaceLanguageFlag LanguageFlag { get { return RaceLanguageFlag.Vampire; } }

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
            return female ? 8252 : 8253;
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
            new MorphEntry( 23, 2547, "a dire wolf", 10, 1, 0.0, true, true, false, false, TimeSpan.FromDays( 365.0 ), 0, 8, 10, false, 43, false, true, false, false ),
            new MorphEntry( 317, 2547, "a vampire bat", 10, 1 ),
            new MorphEntry( 260, 2547, "a vampire mist", 10, 1,  0.0, true, true, false, false, TimeSpan.FromDays( 365.0 * 2 ), 0, 1, 1, false, 100, false, false, false, false )
        };

        public override MorphEntry[] GetMorphList()
        {
            return m_MorphList;
        }

        public override double GetGainFactorBonuses( Skill skill )
        {
            if( skill.SkillName == SkillName.Poisoning )
                return 0.2;

            return base.GetGainFactorBonuses( skill );
        }

        public override int InfravisionLevel { get { return 15; } }
        public override int InfravisionDuration { get { return 60; } }

        public override bool SupportsBite { get { return true; } }
    }
}