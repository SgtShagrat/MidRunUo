/***************************************************************************
 *                                  ArcaioloIncontentabileQuest.cs
 *                            		------------------------------
 *  begin                	: Maggio, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 *              Autore della quest: 
 *                                      Seer Amariel
 * 
 ***************************************************************************/

using System;
using System.Collections.Generic;
using Midgard.Engines.Races;
using Server.Items;
using Server.Accounting;
using Server.Mobiles;

namespace Server.Engines.Quests
{
    public class ArcolaioIncontentabileParteUnoQuest : BaseQuest
    {
        public override QuestChain ChainID { get { return QuestChain.ArcaioloIncontentabile; } }

        public override Type NextQuest { get { return typeof( ArcolaioIncontentabileParteDueQuest ); } }

        public override TimeSpan RestartDelay { get { return TimeSpan.FromHours( 96.0 ); } }

        public override bool CanOffer()
        {
            List<QuestRestartInfo> doneQuests = new List<QuestRestartInfo>();
            Account a = Owner.Account as Account;

            if( a != null )
            {
                for( int i = 0; i < a.Length; i++ )
                {
                    if( a[ i ] != null )
                    {
                        PlayerMobile pm = a[ i ] as PlayerMobile;

                        if( pm != null )
                        {
                            // un player mobile non puo' chiedere la quest due volte 
                            // anche con pg diversi dello stesso account
                            if( QuestHelper.GetQuest( pm, typeof( ArcolaioIncontentabileParteUnoQuest ) ) != null ||
                                QuestHelper.GetQuest( pm, typeof( ArcolaioIncontentabileParteDueQuest ) ) != null ||
                                QuestHelper.GetQuest( pm, typeof( ArcolaioIncontentabileParteTreQuest ) ) != null )
                            {
                                Console.WriteLine( "Player {0} has already quest {1}.", pm.Name, Title );
                                return false;
                            }
                        }

                        doneQuests.AddRange( ( (PlayerMobile)a[ i ] ).DoneQuests );
                    }
                }
            }

            for( int i = doneQuests.Count - 1; i >= 0; i-- )
            {
                QuestRestartInfo restartInfo = doneQuests[ i ];

                if( restartInfo.QuestType == GetType() && DateTime.Now < restartInfo.RestartTime )
                    return false;
            }

            return base.CanOffer();
        }

        public override object Title { get { return "L'arcaiolo incontentabile (parte 1)"; } }

        public override object Description
        {
            get
            {
                return "Hey tu, ho un lavoro per te.<br>" +
                        "Mi chiamo Karin, e produco i migliori archi di Sosaria<br>" +
                        "Devi sapere però che ormai non ce la faccio più a recuperare da sola tutte le risorse per gli archi<br>" +
                        "Tu mi sembri capace con l'ascia e la lavorazione della legna.<br>" +
                        "Hai voglia di darmi una mano?<br>" +
                        "Per iniziare mi serve parecchia legna normale, quando l'avrai presa portamela, e ti dirò cos'altro mi serve<br>" +
                        "Buon lavoro.<br>";
            }
        }

        public override object Refuse { get { return "Non preoccuparti... pagherò qualcun'altro per questo lavoro."; } }

        public override object Uncomplete { get { return "Il tuo impegno non è ancora finito."; } }

        public override object Complete { get { return "Ben fatto. Grazie per le risorse che mi hai procurato."; } }

        public ArcolaioIncontentabileParteUnoQuest()
        {
            AddObjective( new ObtainObjective( typeof( Board ), "tavole di legno normale", 500 + ( Utility.Random( 10 ) * 10 ) ) );

            switch( Utility.Random( 3 ) )
            {
                case 0: AddObjective( new ObtainObjective( typeof( OakBoard ), "tavole di legno Oak", 250 ) ); break;
                case 1: AddObjective( new ObtainObjective( typeof( WalnutBoard ), "tavole di legno Walnut", 250 ) ); break;
                case 2: AddObjective( new ObtainObjective( typeof( OhiiBoard ), "tavole di legno Ohii", 250 ) ); break;
            }

            switch( Utility.Random( 2 ) )
            {
                case 0: AddObjective( new ObtainObjective( typeof( CedarBoard ), "tavole di legno Cedar", 250 ) ); break;
                case 1: AddObjective( new ObtainObjective( typeof( WillowBoard ), "tavole di legno Willow", 250 ) ); break;
                // case 2: AddObjective( new ObtainObjective( typeof( CypressBoard ), "tavole di legno Cypress", 250 ) ); break;
            }

            AddReward( new BaseReward( typeof( Gold ), 5000, "oro" ) );
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }

    public class ArcolaioIncontentabileParteDueQuest : BaseQuest
    {
        public override QuestChain ChainID { get { return QuestChain.ArcaioloIncontentabile; } }

        public override Type NextQuest { get { return typeof( ArcolaioIncontentabileParteTreQuest ); } }

        public override TimeSpan RestartDelay { get { return TimeSpan.FromHours( 96.0 ); } }

        public override bool CanOffer()
        {
            List<QuestRestartInfo> doneQuests = new List<QuestRestartInfo>();
            Account a = Owner.Account as Account;

            if( a != null )
            {
                for( int i = 0; i < a.Length; i++ )
                {
                    if( a[ i ] != null )
                    {
                        PlayerMobile pm = a[ i ] as PlayerMobile;

                        if( pm != null )
                        {
                            // un player mobile non puo' chiedere la quest due volte 
                            // anche con pg diversi dello stesso account
                            if( QuestHelper.GetQuest( pm, typeof( ArcolaioIncontentabileParteUnoQuest ) ) != null ||
                                QuestHelper.GetQuest( pm, typeof( ArcolaioIncontentabileParteDueQuest ) ) != null ||
                                QuestHelper.GetQuest( pm, typeof( ArcolaioIncontentabileParteTreQuest ) ) != null )
                            {
                                Console.WriteLine( "Player {0} has already quest {1}.", pm.Name, Title );
                                return false;
                            }
                        }

                        doneQuests.AddRange( ( (PlayerMobile)a[ i ] ).DoneQuests );
                    }
                }
            }

            for( int i = doneQuests.Count - 1; i >= 0; i-- )
            {
                QuestRestartInfo restartInfo = doneQuests[ i ];

                if( restartInfo.QuestType == GetType() && DateTime.Now < restartInfo.RestartTime )
                    return false;
            }

            return base.CanOffer();
        }

        public override object Title { get { return "L'arcaiolo incontentabile (parte2)"; } }

        public override object Description
        {
            get
            {
                return "ben fatto! ora mi serve della legna speciale, con cui preparerò gli archi più belli di Herima.<br>" +
                        "Torna da me con queste quantità, e finiremo gli archi in poco tempo." +
                        "Buon lavoro.<br>";
            }
        }

        public override object Refuse { get { return "Non preoccuparti... pagherò qualcun'altro per questo lavoro."; } }

        public override object Uncomplete { get { return "Il tuo impegno non è ancora finito."; } }

        public override object Complete { get { return "Ben fatto. Grazie per le risorse che mi hai procurato."; } }

        public ArcolaioIncontentabileParteDueQuest()
        {
            #region obiettivo 2: legni mediamente rari
            switch( Utility.Random( 2 ) )
            {
                case 0: AddObjective( new ObtainObjective( typeof( YewBoard ), "tavole di legno Yew", 100 ) ); break;
                case 1: AddObjective( new ObtainObjective( typeof( PearBoard ), "tavole di legno Pear", 100 ) ); break;
                // case 2: AddObjective( new ObtainObjective( typeof( BananaBoard ), "tavole di legno Banana", 100 ) ); break;
            }

            switch( Utility.Random( 2 ) )
            {
                case 0: AddObjective( new ObtainObjective( typeof( AppleBoard ), "tavole di legno Apple", 100 ) ); break;
                case 1: AddObjective( new ObtainObjective( typeof( PeachBoard ), "tavole di legno Peach", 100 ) ); break;
            }
            #endregion

            #region obiettivo 3: legni rari
            switch( Utility.Random( 3 ) )
            {
                case 0: AddObjective( new ObtainObjective( typeof( StonewoodBoard ), "tavole di legno Stonewood", 10 ) ); break;
                case 1: AddObjective( new ObtainObjective( typeof( SilverBoard ), "tavole di legno Silver", 10 ) ); break;
                case 2: AddObjective( new ObtainObjective( typeof( CrystalBoard ), "tavole di legno Crystal", 10 ) ); break;
            }

            switch( Utility.Random( 3 ) )
            {
                case 0: AddObjective( new ObtainObjective( typeof( ElvenBoard ), "tavole di legno Elven", 10 ) ); break;
                case 1: AddObjective( new ObtainObjective( typeof( ElderBoard ), "tavole di legno Elder", 10 ) ); break;
                case 2: AddObjective( new ObtainObjective( typeof( EnchantedBoard ), "tavole di legno Enchanted", 10 ) ); break;
            }
            #endregion

            AddReward( new BaseReward( typeof( Gold ), 7500, "oro" ) );
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }

    public class ArcolaioIncontentabileParteTreQuest : BaseQuest
    {
        public override QuestChain ChainID { get { return QuestChain.ArcaioloIncontentabile; } }

        public override TimeSpan RestartDelay { get { return TimeSpan.FromHours( 96.0 ); } }

        public override bool CanOffer()
        {
            List<QuestRestartInfo> doneQuests = new List<QuestRestartInfo>();
            Account a = Owner.Account as Account;

            if( a != null )
            {
                for( int i = 0; i < a.Length; i++ )
                {
                    if( a[ i ] != null )
                    {
                        PlayerMobile pm = a[ i ] as PlayerMobile;

                        if( pm != null )
                        {
                            // un player mobile non puo' chiedere la quest due volte 
                            // anche con pg diversi dello stesso account
                            if( QuestHelper.GetQuest( pm, typeof( ArcolaioIncontentabileParteUnoQuest ) ) != null ||
                                QuestHelper.GetQuest( pm, typeof( ArcolaioIncontentabileParteDueQuest ) ) != null ||
                                QuestHelper.GetQuest( pm, typeof( ArcolaioIncontentabileParteTreQuest ) ) != null )
                            {
                                Console.WriteLine( "Player {0} has already quest {1}.", pm.Name, Title );
                                return false;
                            }
                        }

                        doneQuests.AddRange( ( (PlayerMobile)a[ i ] ).DoneQuests );
                    }
                }
            }

            for( int i = doneQuests.Count - 1; i >= 0; i-- )
            {
                QuestRestartInfo restartInfo = doneQuests[ i ];

                if( restartInfo.QuestType == GetType() && DateTime.Now < restartInfo.RestartTime )
                    return false;
            }

            return base.CanOffer();
        }

        public override object Title { get { return "L'arcaiolo incontentabile (parte3)"; } }

        public override object Description
        {
            get
            {
                return "Ottimo, la legna è proprio come la volevo.<br>" +
                        "Ma per tendere le frecce mi serve del filato, ma non filo qualsiasi, ma di lino, che è resistente " +
                        "ed è perfetto per le mie creazioni. Portamente parecchie rocchette, " +
                        "e gli archi saranno terminati<br>" +
                        "Buon lavoro.<br>";
            }
        }

        public override object Refuse { get { return "Non preoccuparti... pagherò qualcun'altro per questo lavoro."; } }

        public override object Uncomplete { get { return "Il tuo impegno non è ancora finito."; } }

        public override object Complete { get { return "Ben fatto. Grazie per le risorse che mi hai procurato."; } }

        public ArcolaioIncontentabileParteTreQuest()
        {
            AddObjective( new ObtainObjective( typeof( SpoolOfThread ), "rocchetti di filo", 100 ) );

            AddReward( new BaseReward( typeof( Gold ), 15000, "oro" ) );
            AddReward( new BaseReward( typeof( RunicFletcherKit ), "attrezzo runico da Arcaiolo" ) );
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }

    public class Karin : MondainQuester
    {
        public override Type[] Quests { get { return new Type[] { typeof( ArcolaioIncontentabileParteUnoQuest ) }; } }

        [Constructable]
        public Karin()
            : base( "Karin", ", l'arcaiolo" )
        {
            SetSkill( SkillName.Fletching, 60.0, 83.0 );
            SetSkill( SkillName.Focus, 60.0, 83.0 );
        }

        public Karin( Serial serial )
            : base( serial )
        {
        }

        public override void InitBody()
        {
            InitStats( 100, 100, 25 );

            Female = true;
            HairItemID = 12241;
            Race = Midgard.Engines.Races.Core.HighElf;
        }

        public override void InitOutfit()
        {
            AddItem( new ElvenPants() );
            AddItem( new ElvenDarkShirt() );
            AddItem( new ElvenBoots() );
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }
}