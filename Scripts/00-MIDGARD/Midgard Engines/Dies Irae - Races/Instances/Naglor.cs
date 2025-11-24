using Server;

namespace Midgard.Engines.Races
{
    internal class Naglor : MidgardRace
    {
        private static int[] m_HairHues = new int[] { 2198, 2222 };
        private static int[] m_SkinHues = new int[] { 1080 };

        public Naglor( int raceID, int raceIndex )
            : base( raceID, raceIndex, "Naglor", "Naglors", 400, 401, 402, 403 )
        {
        }

        public override int InfravisionLevel { get { return 15; } }
        public override int InfravisionDuration { get { return 60; } }

        public override RaceLanguageFlag LanguageFlag { get { return RaceLanguageFlag.Naglor; } }

        public override bool ValidateHair( bool female, int itemID )
        {
            if( itemID == 0 )
                return true;

            if( ( female && itemID == 0x2048 ) || ( !female && itemID == 0x2046 ) )
                return false;	//Buns & Receeding Hair

            if( itemID >= 0x203B && itemID <= 0x203D )
                return true;

            if( itemID >= 0x2044 && itemID <= 0x204A )
                return true;

            return false;
        }

        public override int RandomHair( bool female )
        {
            switch( Utility.Random( 9 ) )
            {
                case 0: return 0x203B;	//Short
                case 1: return 0x203C;	//Long
                case 2: return 0x203D;	//Pony Tail
                case 3: return 0x2044;	//Mohawk
                case 4: return 0x2045;	//Pageboy
                case 5: return 0x2047;	//Afro
                case 6: return 0x2049;	//Pig tails
                case 7: return 0x204A;	//Krisna
                default: return ( female ? 0x2046 : 0x2048 );	//Buns or Receeding Hair
            }
        }

        public override bool ValidateFacialHair( bool female, int itemID )
        {
            return itemID == 0;
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
    }
}