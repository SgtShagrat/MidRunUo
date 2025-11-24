using System.IO;

namespace Server.Mobiles
{
    public class PersonalFragment : SpeechFragment
    {
        public PersonalFragment( string filename )
            : base( Path.Combine( PersonalPath, filename ), null )
        {
        }

        // From: http://forum.uosecondage.com/viewtopic.php?f=8&t=10044
        //Gambler (a hireling with a daily wage of 40. I believe they would also play games against you.. such as dice. If you would give them gold, they would roll and you could win/lose the game... not clear on this, though. According to the guide, they should "appraise" also.)
        //Glassblower (Sells/Buys: flasks (empty), jars (empty), glass pitchers, vials (empty)
        //Judge (Located around Yew jails & Trinsic Jails)
        //Jailor (RIght now I believe we only have Guards in the Yew/Trinsic jails)
        //Magincia Council Members (keyword: "parliament"?) [I am fairly sure these are coded in RunUO]
        //Magincia Servant (keyword "labor" "servant"?)
        //Mayor
        //Miller (keyword "wheat"?; Located in the flour mill in Yew)
        //Monk (list of keywords: "abbey" "empath" "knowledge" "monk" "relvinian" "scholar" "wisdom" "wine"; located in the winery near the Abbey)
        //Occlo Cashual* (basically Occlos version of Mage Shopkeepers. According to the strat guide, though, they sold different levels of spells according to their level of being. I.E. A fifth level cashual sold 1st - 5th circle)
        //Occlo Priest/Priestess*
        //Occlo Runner*

        //*All Occlo-based NPCs should speak about "huansuytin" due to its place in their beliefs.

        private static string PersonalPath = "Personal";

        public static SpeechFragment Actor = new PersonalFragment( "actor.frg" );
        public static SpeechFragment Alchemist = new PersonalFragment( "alchemist.frg" );
        public static SpeechFragment AnimalTrainer = new PersonalFragment( "animal.frg" );
        public static SpeechFragment Architect = new PersonalFragment( "architect.frg" );
        public static SpeechFragment Armourer = new PersonalFragment( "armourer.frg" );
        public static SpeechFragment Artist = new PersonalFragment( "artist.frg" );
        public static SpeechFragment Baker = new PersonalFragment( "baker.frg" );
        public static SpeechFragment Banker = new PersonalFragment( "banker.frg" );
        public static SpeechFragment Bard = new PersonalFragment( "bard.frg" );
        public static SpeechFragment Beekeeper = new PersonalFragment( "beekeeper.frg" );
        // public static SpeechFragment Beggar = new PersonalFragment( "beggar.frg" ); does exist?
        public static SpeechFragment Blacksmith = new PersonalFragment( "blacksmith.frg" );
        public static SpeechFragment Bowyer = new PersonalFragment( "bowyer.frg" );
        public static SpeechFragment Brigand = new PersonalFragment( "brigand.frg" ); //TODO: 
        public static SpeechFragment Butcher = new PersonalFragment( "butcher.frg" );
        public static SpeechFragment Carpenter = new PersonalFragment( "carpenter.frg" );
        // public static SpeechFragment Cashual = new PersonalFragment( "cashual.frg" );
        public static SpeechFragment Cobbler = new PersonalFragment( "cobbler.frg" );
        public static SpeechFragment Cook = new PersonalFragment( "cook.frg" );
        public static SpeechFragment Farmer = new PersonalFragment( "farmer.frg" );
        public static SpeechFragment Fighter = new PersonalFragment( "fighter.frg" );
        public static SpeechFragment Fisher = new PersonalFragment( "fisher.frg" );
        public static SpeechFragment Furtrader = new PersonalFragment( "furtrader.frg" );
        public static SpeechFragment Gambler = new PersonalFragment( "gambler.frg" );
        public static SpeechFragment Glassblower = new PersonalFragment( "glassblower.frg" );
        public static SpeechFragment Guard = new PersonalFragment( "guard.frg" ); //TODO: 
        public static SpeechFragment Gypsy = new PersonalFragment( "gypsy.frg" );
        public static SpeechFragment Healer = new PersonalFragment( "healer.frg" );
        public static SpeechFragment Herbalist = new PersonalFragment( "herbalist.frg" );
        public static SpeechFragment HorseTrader = new PersonalFragment( "horse.frg" ); //TODO: 
        //public static SpeechFragment Innkeep = new Personal("innkeep.frg");  throws error: Not a valid fragment file, no closing bracket   // this one has some interesting macros, such as [getBuilding(getLocation(Actor()))]
        public static SpeechFragment Innkeeper = new PersonalFragment( "innkeeper.frg" ); // this one omits the wierd marcos, but supports the Hints engine.
        public static SpeechFragment Jailor = new PersonalFragment( "jailor.frg" ); //TODO: 
        public static SpeechFragment Jeweler = new PersonalFragment( "jeweler.frg" );
        public static SpeechFragment Judge = new PersonalFragment( "judge.frg" ); //TODO: 
        public static SpeechFragment Laborer = new PersonalFragment( "laborer.frg" );
        public static SpeechFragment Mage = new PersonalFragment( "mage.frg" );
        public static SpeechFragment Mapmaker = new PersonalFragment( "mapmaker.frg" );
        public static SpeechFragment Mayor = new PersonalFragment( "mayor.frg" ); //TODO: 
        public static SpeechFragment Miller = new PersonalFragment( "miller.frg" ); //TODO: 
        public static SpeechFragment Miner = new PersonalFragment( "miner.frg" );
        public static SpeechFragment Minter = new PersonalFragment( "minter.frg" );
        public static SpeechFragment Monk = new PersonalFragment( "monk.frg" ); //TODO: 
        public static SpeechFragment Noble = new PersonalFragment( "noble.frg" );
        public static SpeechFragment PitOverseer = new PersonalFragment( "overseer.frg" );
        public static SpeechFragment Paladin = new PersonalFragment( "paladin.frg" ); //TODO: 
        public static SpeechFragment Parliamentarian = new PersonalFragment( "parliament.frg" ); //TODO: Parliamentarian
        public static SpeechFragment Pirate = new PersonalFragment( "pirate.frg" );
        public static SpeechFragment Priest = new PersonalFragment( "priest.frg" ); //TODO: Priest
        public static SpeechFragment Prisoner = new PersonalFragment( "prisoner.frg" );
        public static SpeechFragment Provisioner = new PersonalFragment( "provisioner.frg" );
        public static SpeechFragment Rancher = new PersonalFragment( "rancher.frg" );
        public static SpeechFragment Ranger = new PersonalFragment( "ranger.frg" );
        public static SpeechFragment Realtor = new PersonalFragment( "realtor.frg" );
        public static SpeechFragment Messenger = new PersonalFragment( "runner.frg" );
        public static SpeechFragment Sailor = new PersonalFragment( "sailor.frg" );
        public static SpeechFragment Scholar = new PersonalFragment( "scholar.frg" ); //TODO: Scholar
        public static SpeechFragment Scribe = new PersonalFragment( "scribe.frg" );
        public static SpeechFragment Sculptor = new PersonalFragment( "sculptor.frg" );
        public static SpeechFragment Servant = new PersonalFragment( "servant.frg" ); //TODO: Maginican Servant
        public static SpeechFragment Shepherd = new PersonalFragment( "shepherd.frg" );
        public static SpeechFragment Shipwright = new PersonalFragment( "shipwright.frg" );
        public static SpeechFragment Tailor = new PersonalFragment( "tailor.frg" );
        public static SpeechFragment Tanner = new PersonalFragment( "tanner.frg" );
        //public static SpeechFragment TavernKeeper = new Personal("tavernkeeper.frg");   // note %0 macro, passing back the key argument.
        public static SpeechFragment TavernKeeper = new PersonalFragment( "tavkeep.frg" ); // supports hints
        public static SpeechFragment Thief = new PersonalFragment( "thief.frg" );
        public static SpeechFragment Tinker = new PersonalFragment( "tinker.frg" );
        public static SpeechFragment Veterinarian = new PersonalFragment( "vet.frg" );
        public static SpeechFragment Waiter = new PersonalFragment( "waiter.frg" );
        public static SpeechFragment WanderingHealer = new PersonalFragment( "wanderhealer.frg" );
        public static SpeechFragment Weaponsmith = new PersonalFragment( "weaponsmith.frg" );
        public static SpeechFragment Weaponstrainer = new PersonalFragment( "weaponstrainer.frg" ); //TODO: WeaponsTrainer
        public static SpeechFragment Weaver = new PersonalFragment( "weaver.frg" );
        public static SpeechFragment GypsyAnimalTrainer = new SpeechFragment( AnimalTrainer, Gypsy );
        public static SpeechFragment GypsyBanker = new SpeechFragment( Banker, Gypsy );
    }
}