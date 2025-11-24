using Server;

namespace Midgard.Engines.Races
{
    internal class HalfDaemon : MidgardRace
    {
        private static int[] m_SkinHues = new int[] { 2205 };

        public HalfDaemon( int raceID, int raceIndex )
            : base( raceID, raceIndex, "HalfDaemon", "HalfDaemons", 400, 401, 402, 403 )
        {
        }

        public override int InfravisionLevel { get { return 15; } }
        public override int InfravisionDuration { get { return 60; } }

        public override RaceLanguageFlag LanguageFlag { get { return RaceLanguageFlag.HalfDaemon; } }

        public override bool IsEvilAlignedRace
        {
            get { return true; }
        }

        public override bool ValidateHair( bool female, int itemID )
        {
            return itemID == 0x38c8;
        }

        public override int RandomHair( bool female )
        {
            return 0x38c8;
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
            return 0;
        }

        public override int RandomHairHue()
        {
            return 0;
        }

        public override bool SupportCustomBody { get { return true; } }

        public override void DressCustomBody( Mobile from )
        {
            HalfDaemonWings body = new HalfDaemonWings();
            body.Layer = Layer.Cloak;
            if( from.CheckEquip( body ) )
                from.EquipItem( body );
            else
                Config.Pkg.LogError( "Failed equipping half-daemon body." );
            body.Movable = false;
        }

        public override void UnDressCustomBody( Mobile from )
        {
            HalfDaemonWings oldBody = from.FindItemOnLayer( Layer.Cloak ) as HalfDaemonWings;
            if( oldBody != null )
                oldBody.Delete();
        }
    }
}