using System.IO;

namespace Server.Mobiles
{
    public class RegionalFragment : SpeechFragment
    {
        private static string RegionalPath = "Regional";

        public RegionalFragment( string filename ) : base( Path.Combine( RegionalPath, filename ), SFBritainnia ) { }

        public static SpeechFragment Britain = new RegionalFragment( "britain.frg" );
        public static SpeechFragment BucsDen = new RegionalFragment( "bucden.frg" );
        public static SpeechFragment Cove = new RegionalFragment( "cove.frg" );
        public static SpeechFragment Jhelom = new RegionalFragment( "jhelom.frg" );
        public static SpeechFragment Magincia = new RegionalFragment( "magincia.frg" );
        public static SpeechFragment Minoc = new RegionalFragment( "minoc.frg" );
        public static SpeechFragment Moonglow = new RegionalFragment( "moonglow.frg" );
        public static SpeechFragment Nujelm = new RegionalFragment( "nujelm.frg" );
        public static SpeechFragment SerpsHold = new RegionalFragment( "serphold.frg" );
        public static SpeechFragment SkaraBrae = new RegionalFragment( "skara.frg" );
        public static SpeechFragment Trinsic = new RegionalFragment( "trinsic.frg" );
        public static SpeechFragment Vesper = new RegionalFragment( "vesper.frg" );
        public static SpeechFragment Wind = new RegionalFragment( "wind.frg" );
        public static SpeechFragment Yew = new RegionalFragment( "yew.frg" );
    }
}