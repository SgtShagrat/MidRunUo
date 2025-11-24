namespace Server.Mobiles
{
    public class PersonalGuildMasterFragment : SpeechFragment
    {
        protected static SpeechFragment GuildMasterBase = new SpeechFragment( "master.frg", null );
        public PersonalGuildMasterFragment( SpeechFragment jobFragment ) : base( GuildMasterBase, jobFragment ) { }

        public static SpeechFragment BardGuildmaster = new PersonalGuildMasterFragment( PersonalFragment.Bard );
        public static SpeechFragment BlacksmithGuildmaster = new PersonalGuildMasterFragment( PersonalFragment.Blacksmith );
        public static SpeechFragment FisherGuildmaster = new PersonalGuildMasterFragment( PersonalFragment.Bard );
        public static SpeechFragment HealerGuildmaster = new PersonalGuildMasterFragment( PersonalFragment.Bard );
        public static SpeechFragment MageGuildmaster = new PersonalGuildMasterFragment( PersonalFragment.Mage );
        public static SpeechFragment MerchantGuildmaster = GuildMasterBase; // There is no fragment for Merchant.
        public static SpeechFragment MinerGuildmaster = new PersonalGuildMasterFragment( PersonalFragment.Miner );
        public static SpeechFragment RangerGuildmaster = new PersonalGuildMasterFragment( PersonalFragment.Ranger );
        public static SpeechFragment TailorGuildmaster = new PersonalGuildMasterFragment( PersonalFragment.Tailor );
        public static SpeechFragment ThiefGuildmaster = new PersonalGuildMasterFragment( PersonalFragment.Thief );
        public static SpeechFragment TinkerGuildmaster = new PersonalGuildMasterFragment( PersonalFragment.Tinker );
        public static SpeechFragment WarriorGuildmaster = new PersonalGuildMasterFragment( PersonalFragment.Fighter );
    }
}