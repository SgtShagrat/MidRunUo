using Server;

namespace Midgard.Engines.Races
{
    internal class Undead : MidgardRace
    {
        private static int[] m_HairHues = new int[] { 0 };
        private static int[] m_SkinHues = new int[] { 1072 };

        public Undead( int raceID, int raceIndex )
            : base( raceID, raceIndex, "Undead", "Undeads", 400, 401, 402, 403 )
        {
        }

        public override RaceLanguageFlag LanguageFlag { get { return RaceLanguageFlag.Undead; } }

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
            return 0;
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
            new MorphEntry( 3, 0, "a zombie", 10, 1 ),
        };

        public override MorphEntry[] GetMorphList()
        {
            return m_MorphList;
        }

        public override double GetGainFactorBonuses( Skill skill )
        {
            if( skill.SkillName == SkillName.Necromancy )
                return 0.2;

            return base.GetGainFactorBonuses( skill );
        }

        public override bool SupportCustomBody { get { return true; } }

        public override void DressCustomBody( Mobile from )
        {
            UndeadBody body = new UndeadBody();
            body.Layer = Layer.Cloak;
            if( from.CheckEquip( body ) )
                from.EquipItem( body );
            else
                Config.Pkg.LogError( "Failed equipping undead body." );
            body.Movable = false;
        }

        public override void UnDressCustomBody( Mobile from )
        {
            UndeadBody oldBody = from.FindItemOnLayer( Layer.Cloak ) as UndeadBody;
            if( oldBody != null )
                oldBody.Delete();
        }
    }
}