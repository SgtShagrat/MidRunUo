using System;

using Server;

namespace Midgard.Engines.Races
{
    internal class MountainDwarf : MidgardRace
    {
        private static int[] m_HairHues = new int[] { 1845, 1753 };
        private static int[] m_ValidHairs = new int[] { 0x203c, 0x203B, 0x2044, 0x2045, 0x2049 };

        public MountainDwarf( int raceID, int raceIndex )
            : base( raceID, raceIndex, "Mountain Dwarf", "Mountain Dwarves", 605, 606, 607, 608 )
        {
        }

        public override RaceLanguageFlag LanguageFlag { get { return RaceLanguageFlag.MountainDwarf; } }

        public override int GhostShroudId( Mobile mobile )
        {
            return 0x38C6;
        }

        public override bool ValidateHair( bool female, int itemID )
        {
            if( itemID == 0 )
                return true;

            if( female && itemID == 0x203c )
                return true;

            if( Array.LastIndexOf( m_ValidHairs, itemID ) > -1 )
                return true;

            return false;
        }

        public override int RandomHair( bool female )
        {
            return female ? 0x203c : Utility.RandomList( new int[] { 0x203c, 0x203B, 0x2044, 0x2045, 0x2049 } );
        }

        public override bool ValidateFacialHair( bool female, int itemID )
        {
            return female ? itemID == 0 : itemID == 0x63F || itemID == 0x640;
        }

        public override int RandomFacialHair( bool female )
        {
            return female ? 0 : Utility.RandomList( new int[] { 0x63F, 0x640 } );
        }

        public override int ClipSkinHue( int hue )
        {
            if( hue < 1002 )
                return 1002;
            else if( hue > 1058 )
                return 1058;
            else
                return hue;
        }

        public override int RandomSkinHue()
        {
            return Utility.Random( 1002, 57 ) | 0x8000;
        }

        public override int ClipHairHue( int hue )
        {
            return m_HairHues[ 0 ];
        }

        public override int RandomHairHue()
        {
            return m_HairHues[ Utility.Random( m_HairHues.Length ) ];
        }

        public override int InfravisionLevel { get { return 15; } }
        public override int InfravisionDuration { get { return 60; } }

        private static readonly RaceSkillMod[] m_SkillBonusList = new RaceSkillMod[]
        {
            new RaceSkillMod( SkillName.Blacksmith, true, 10 ),
            new RaceSkillMod( SkillName.Mining, true, 10 ),
            new RaceSkillMod( SkillName.Macing, true, 10 ),
            new RaceSkillMod( SkillName.Swords, true, -10 ),
            new RaceSkillMod( SkillName.Archery, true, -10 ),
        };

        public override RaceSkillMod[] GetSkillBonuses()
        {
            return m_SkillBonusList;
        }

        public override bool SupportsMagerySpells { get { return false; } }

        public override bool SameHairSameFacialHues { get { return true; } }
    }
}