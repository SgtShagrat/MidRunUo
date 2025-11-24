namespace Server.Mobiles
{
    public class PersonalVendorFragment : SpeechFragment
    {
        protected static SpeechFragment Shopkeeper = new SpeechFragment( "shopkeep.frg", null );

        // Job should come before shopkeep, some Jobs, like provisioner handle job words.
        public PersonalVendorFragment( SpeechFragment jobFrag ) : base( jobFrag, Shopkeeper ) { }

        public static SpeechFragment MageVendor = new PersonalVendorFragment( PersonalFragment.Mage );
    }
}