using System.IO;

namespace Server.Mobiles
{
    public class SpecialFragment : SpeechFragment
    {
        public SpecialFragment( string filename )
            : base( Path.Combine( SpecialPath, filename ), null )
        {
        }

        private static string SpecialPath = "Special";

        public static SpeechFragment AcceptItem = new SpecialFragment( "acceptitem.frg" ); // Handled in britanni.frg
        public static SpeechFragment InternalSpace = new SpecialFragment( "internalspace.frg" );
        public static SpeechFragment Greetings = new SpecialFragment( "greetings.frg" );
        public static SpeechFragment Needresponse = new SpecialFragment( "needresponse.frg" );
        public static SpeechFragment Needs = new SpecialFragment( "needs.frg" );
        public static SpeechFragment Refuseitem = new SpecialFragment( "refuseitem.frg" );
        public static SpeechFragment Rehello = new SpecialFragment( "rehello.frg" );
        public static SpeechFragment Scavenger = new SpecialFragment( "scavenger.frg" );
        public static SpeechFragment ConvInit = new SpecialFragment( "convinit.frg" );
        public static SpeechFragment ConvBye = new SpecialFragment( "convbye.frg" );
    }
}