/***************************************************************************
 *                                    Gambler.cs
 *                            		--------------
 *  begin                	: Luglio 2007
 *  version					: 2.1 **VERSIONE PER RUNUO 2.0**
 *  original concept		: Unknown
 * 	rebuild					: Dies Irae
 *
 ***************************************************************************/

/***************************************************************************
* 
* 	Info:
* 			NPC che gioca a Blackjack o al poker.
*	Versionamento:
* 	2.1		Ribuild del codice;
* 			Messo enum all'inizio coi moltiplicatori delle vincite
* 			Messe varie regioni per distinguere meglio il codice.
* 			Rinominate molte props.
* 
***************************************************************************/

using System.Collections;
using Server.Gumps;
using Server.Items;
using Server.Network;

namespace Server.Mobiles
{
    public class Gambler : BaseCreature
    {
        public override SpeechFragment PersonalFragmentObj { get { return PersonalFragment.Gambler; } } // mod by Dies Irae

        #region enums
        public enum PokerRewards
        {
            Coppia = 1,
            DoppiaCoppia = 2,
            Tris = 3,
            Scala = 5,
            Colore = 7,
            Full = 10,
            Poker = 40,
            ScalaColore = 100,
            ScalaReale = 500
        }
        #endregion

        #region campi
        private int m_CurrentCard = 53;
        private int[] m_Cards = new int[ 53 ];
        private int[] m_DealerCards = new int[ 5 ];
        private int[] m_PlayerCards = new int[ 5 ];
        private int[] m_GameStats = new int[ 6 ];
        private bool m_BlackJackPlayer = false;
        private bool m_BlackJackDealer = false;
        private int m_PlayerBet = 100;
        private bool m_IsRoundEnded;
        private bool m_IsDealerCardHidden;
        private bool m_IsBusy;
        private string m_PokerMessage = string.Empty;
        private int m_PokerGameStatus = 0;
        private int m_PokerWinPlayer = 0;
        private Mobile m_Player;
        #endregion

        #region poprietà
        public int currentCard
        {
            get { return m_CurrentCard; }
            set { m_CurrentCard = value; }
        }

        public int[] cards
        {
            get { return m_Cards; }
            set { m_Cards = value; }
        }

        public int[] dealerCards
        {
            get { return m_DealerCards; }
            set { m_DealerCards = value; }
        }

        public int[] playerCards
        {
            get { return m_PlayerCards; }
            set { m_PlayerCards = value; }
        }

        public int[] gameStats
        {
            get { return m_GameStats; }
            set { m_GameStats = value; }
        }

        public bool blackJackPlayer
        {
            get { return m_BlackJackPlayer; }
            set { m_BlackJackPlayer = value; }
        }

        public bool blackJackDealer
        {
            get { return m_BlackJackDealer; }
            set { m_BlackJackDealer = value; }
        }

        public int playerBet
        {
            get { return m_PlayerBet; }
            set { m_PlayerBet = value; }
        }

        public bool isRoundEnded
        {
            get { return m_IsRoundEnded; }
            set { m_IsRoundEnded = value; }
        }

        public bool isDealerCardHidden
        {
            get { return m_IsDealerCardHidden; }
            set { m_IsDealerCardHidden = value; }
        }

        public bool isBusy
        {
            get { return m_IsBusy; }
            set { m_IsBusy = value; }
        }

        public string pokerMessage
        {
            get { return m_PokerMessage; }
            set { m_PokerMessage = value; }
        }

        public int pokerGameStatus
        {
            get { return m_PokerGameStatus; }
            set { m_PokerGameStatus = value; }
        }

        public int pokerWinPlayer
        {
            get { return m_PokerWinPlayer; }
            set { m_PokerWinPlayer = value; }
        }

        public Mobile player
        {
            get { return m_Player; }
            set { m_Player = value; }
        }

        public override bool CanTeach
        {
            get { return true; }
        }

        public override bool DisallowAllMoves
        {
            get { return true; }
        }
        #endregion

        #region costruttori
        [Constructable]
        public Gambler()
            : base( AIType.AI_Melee, FightMode.Weakest, 10, 1, 0.8, 3.0 )
        {
            // Stats
            SetStr( 304, 400 );
            SetDex( 102, 150 );
            SetInt( 204, 300 );

            SetHits( 66, 125 );
            SetDamage( 30, 50 );

            CantWalk = true;

            // Resistances
            SetResistance( ResistanceType.Physical, 40, 50 );
            SetResistance( ResistanceType.Fire, 40, 50 );
            SetResistance( ResistanceType.Cold, 40, 50 );
            SetResistance( ResistanceType.Poison, 40, 50 );
            SetResistance( ResistanceType.Energy, 40, 50 );

            // Skills
            SetSkill( SkillName.EvalInt, 90.0, 100.0 );
            SetSkill( SkillName.Inscribe, 90.0, 100.0 );
            SetSkill( SkillName.Magery, 90.0, 100.0 );
            SetSkill( SkillName.Meditation, 90.0, 100.0 );
            SetSkill( SkillName.MagicResist, 90.0, 100.0 );
            SetSkill( SkillName.Wrestling, 90.0, 100.0 );

            // Fama e Karma
            Fame = Utility.RandomMinMax( 100, 1000 );
            Karma = -( Utility.RandomMinMax( 100, 1000 ) );

            // Hair
            HairItemID = 0x203B;
            HairHue = Utility.RandomHairHue();

            // Speech & Skin hues
            SpeechHue = Utility.RandomDyedHue();
            Hue = Utility.RandomSkinHue();

            // Title
            Title = "the gambler";

            // Genre and Name
            if( Female = Utility.RandomBool() )
            {
                Name = NameList.RandomName( "female" );
                Title = ", the pretty gambler";
                Body = 0x191;
            }
            else
            {
                Name = NameList.RandomName( "male" );
                Title = ", the lazy gambler";
                Body = 0x190;
            }

            // Equip:
            Item hat = null;
            if( Female )
            {
                switch( Utility.Random( 3 ) )
                {
                    case 0: hat = new FloppyHat(); break;
                    case 1: hat = new FeatheredHat(); break;
                    case 2: hat = new Bonnet(); break;
                    case 3: hat = new Cap(); break;
                }
            }
            else
            {
                switch( Utility.Random( 5 ) )
                {
                    case 0: hat = new SkullCap(); break;
                    case 1: hat = new Bandana(); break;
                    case 2: hat = new WideBrimHat(); break;
                    case 3: hat = new TallStrawHat(); break;
                    case 4: hat = new StrawHat(); break;
                    case 5: hat = new TricorneHat(); break;
                }
            }

            if( hat != null )
            {
                hat.Hue = Utility.RandomNeutralHue();
                AddItem( hat );
            }

            AddItem( new LongPants( Utility.RandomNeutralHue() ) );
            AddItem( new FancyShirt( Utility.RandomNeutralHue() ) );
            AddItem( new Boots( Utility.RandomNeutralHue() ) );
            AddItem( new Cloak( Utility.RandomNeutralHue() ) );
            AddItem( new BodySash( Utility.RandomNeutralHue() ) );

            AddToBackpack( new Gold( 500, 1000 ) );

            ResetGambler();
        }

        public Gambler( Serial serial )
            : base( serial )
        {
            isRoundEnded = true;
            isBusy = false;
        }
        #endregion

        #region metodi
        public void ResetGambler()
        {
            for( int i = 0; i <= 5; ++i )
            {
                gameStats[ i ] = 0;
            }
        }

        public override bool CheckTeach( SkillName skill, Mobile from )
        {
            if( !base.CheckTeach( skill, from ) )
                return false;

            return skill == SkillName.Stealing;
        }

        public override void GenerateLoot()
        {
            AddLoot( LootPack.Rich, 2 );
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( from.AccessLevel >= AccessLevel.Administrator )
                from.SendGump( new GamblerStatsGump( this ) );
            else
                base.OnDoubleClick( from );
        }

        public override bool HandlesOnSpeech( Mobile from )
        {
            return true;
        }

        #region SpeechLists
        private string[] m_GreetingsList = new string[]
		{
			"ciao", "salve", "hallo", "hail", "hi", "greetings", "gambler"
		};

        private string[] m_PlayList = new string[]
		{
			"play", "gioca", "giochiamo",
		};

        private string[] m_BlackJackList = new string[]
		{
			"blackjack", "play blackjack"
		};

        private string[] m_PokerList = new string[]
		{
			"poker", "play poker"
		};
        #endregion

        public override void OnSpeech( SpeechEventArgs e )
        {
            base.OnSpeech( e );

            Mobile from = e.Mobile;
            string text = e.Speech;

            ResetPlayer();

            if( !e.Handled && from.InRange( this, 3 ) && CanSee( from ) )
            {
                if( from.AccessLevel >= AccessLevel.GameMaster && Insensitive.Equals( "reset", text ) )
                {
                    isBusy = false;
                    Say( "I am no longer busy." );
                }
                foreach( string s in m_GreetingsList )
                {
                    if( Insensitive.Equals( s, text ) )
                        Say( "Let's play blackjack or poker." );
                }
                foreach( string s in m_PlayList )
                {
                    if( Insensitive.Equals( s, text ) )
                        Say( "Would you like to play blackjack or poker?" );
                }

                #region blackJack
                foreach( string s in m_BlackJackList )
                {
                    if( Insensitive.Equals( s, text ) )
                    {
                        if( !m_IsBusy )
                        {
                            m_IsBusy = true;
                            m_IsRoundEnded = true;
                            m_IsDealerCardHidden = false;

                            m_PlayerBet = 100;
                            m_CurrentCard = 53;
                            m_PokerGameStatus = 0;
                            m_PokerWinPlayer = 0;

                            m_DealerCards[ 0 ] = 12;
                            m_PlayerCards[ 0 ] = 13;
                            m_DealerCards[ 1 ] = 11;
                            m_PlayerCards[ 1 ] = 26;

                            for( int i = 2; i <= 4; ++i )
                            {
                                m_DealerCards[ i ] = 0;
                                m_PlayerCards[ i ] = 0;
                            }

                            m_PokerMessage = "Midgard Renaissance";

                            Say( "So, you want to try your luck." );

                            m_Player = from;

                            PlayBlackJack( from );
                        }
                        else if( m_Player.NetState == from.NetState )
                            Say( "We are already playing cards." );
                        else
                            Say( "I am busy playing cards." );
                    }
                }
                #endregion

                #region poker
                foreach( string s in m_PokerList )
                {
                    if( Insensitive.Equals( s, text ) )
                    {
                        if( !isBusy )
                        {
                            m_IsBusy = true;
                            m_IsRoundEnded = true;

                            m_PlayerBet = 100;
                            m_CurrentCard = 53;
                            m_PokerWinPlayer = 0;
                            m_PokerGameStatus = 0;

                            for( int i = 0; i <= 4; ++i )
                                m_PlayerCards[ i ] = 35 + i;

                            m_PokerMessage = "Choose to deal or high the bet.";

                            m_Player = from;

                            PlayPoker( from );

                            Say( "So, you want to try your luck." );
                        }
                        else if( m_Player.NetState == from.NetState )
                            Say( "We are already playing cards." );
                        else
                            Say( "I am busy playing cards." );
                    }
                }
                #endregion
            }
        }

        public override bool OnGoldGiven( Mobile from, Gold dropped )
        {
            Say( "Are you trying to bribe me to win?" );
            return false;
        }

        private void ResetPlayer()
        {
            if( m_Player != null && m_Player.NetState == null )
                m_IsBusy = false;
        }

        public void PayPlayer( Mobile from, int quantity )
        {
            from.AddToBackpack( new Gold( quantity ) );
        }

        public bool PayDealer( Mobile from, int quantity )
        {
            return from.Backpack.ConsumeTotal( typeof( Gold ), quantity );
        }

        public string CardSuit( int card )
        {
            if( card >= 1 && card <= 13 )
                return "C";
            else if( card >= 14 && card <= 26 )
                return "D";
            else if( card >= 27 && card <= 39 )
                return "H";
            else
                return "S";
        }

        public string CardName( int card )
        {
            while( card > 13 )
                card -= 13;

            if( card == 1 )
                return "A";
            else if( card == 11 )
                return "J";
            else if( card == 12 )
                return "Q";
            else if( card == 13 )
                return "K";
            else
                return card.ToString();
        }

        public int CardValue( int card )
        {
            while( card > 13 )
                card -= 13;

            if( card == 1 )
                return 11;

            if( card > 10 )
                return 10;

            return card;
        }

        public int CardColor( string card )
        {
            if( card == "D" || card == "H" )
                return 32;

            return 0;
        }

        public int PokerCardValue( int card )
        {
            while( card > 13 )
                card -= 13;

            if( card == 1 )
                return 14;

            return card;
        }

        public void ShuffleCards()
        {
            int i;

            for( i = 1; i < 53; ++i )
                cards[ i ] = i;

            for( i = 52; i >= 1; --i )
            {
                int tempcard = Utility.Random( i ) + 1;
                int tempcard2 = cards[ tempcard ];
                cards[ tempcard ] = cards[ i ];
                cards[ i ] = tempcard2;
            }
            m_CurrentCard = 1;
        }

        public int PickCard( Mobile from )
        {
            if( m_CurrentCard == 53 )
            {
                Effects.PlaySound( from.Location, from.Map, 0x3D );
                ShuffleCards();
            }

            return cards[ m_CurrentCard++ ];
        }

        public void PlayBlackJack( Mobile from )
        {
            from.CloseGump( typeof( BlackjackGump ) );
            from.CloseGump( typeof( PokerGump ) );
            from.SendGump( new BlackjackGump( this, this ) );
        }

        public void PlayPoker( Mobile from )
        {
            from.CloseGump( typeof( BlackjackGump ) );
            from.CloseGump( typeof( PokerGump ) );
            from.SendGump( new PokerGump( this, this ) );
        }

        public void SlicePokerGameStatus()
        {
            if( m_PokerGameStatus == 1 )
                m_PokerGameStatus++;

            if( m_PokerGameStatus == 3 )
            {
                m_PokerGameStatus = 0;
                m_IsRoundEnded = true;
            }
        }
        #endregion

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int)0 ); // version

            for( int i = 0; i <= 5; ++i )
                writer.Write( m_GameStats[ i ] );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();

            for( int i = 0; i <= 5; ++i )
                m_GameStats[ i ] = reader.ReadInt();
        }
        #endregion

        public class GamblerStatsGump : Gump
        {
            #region campi
            private Gambler m_Gambler;
            #endregion

            #region costruttori
            public GamblerStatsGump( Gambler gambler )
                : base( 10, 10 )
            {
                m_Gambler = gambler;

                AddPage( 0 );

                AddBackground( 30, 100, 90, 160, 5120 );

                AddLabel( 45, 100, 70, "Blackjack" );
                AddLabel( 45, 115, 600, "Wins: " + m_Gambler.gameStats[ 0 ] );
                AddLabel( 45, 130, 600, "Loss: " + m_Gambler.gameStats[ 1 ] );
                AddLabel( 45, 145, 600, "Tied: " + m_Gambler.gameStats[ 2 ] );

                AddLabel( 45, 165, 70, "Poker" );
                AddLabel( 45, 180, 600, "Wins: " + m_Gambler.gameStats[ 3 ] );
                AddLabel( 45, 195, 600, "Loss: " + m_Gambler.gameStats[ 4 ] );
                AddLabel( 45, 210, 600, "Tied: " + m_Gambler.gameStats[ 5 ] );

                AddLabel( 45, 230, 1500, "Reset" );

                AddButton( 85, 235, 2117, 2118, 101, GumpButtonType.Reply, 0 );
            }
            #endregion

            #region metodi
            public override void OnResponse( NetState sender, RelayInfo info )
            {
                if( info.ButtonID == 101 )
                    m_Gambler.ResetGambler();
            }
            #endregion
        }

        public class PokerGump : Gump
        {
            #region campi
            private Gambler m_Gambler;
            #endregion

            #region costruttori
            public PokerGump( Mobile player, Gambler gambler )
                : base( 10, 10 )
            {
                Closable = false;
                Disposable = false;
                Dragable = false;
                Resizable = false;

                m_Gambler = gambler;

                AddPage( 0 );
                AddImageTiled( 30, 100, 460, 160, 2624 );
                AddAlphaRegion( 90, 100, 460, 105 );

                // label indicante se il player sta giocando o deve cominciare
                string playerNameLabel = "Start";
                if( m_Gambler.pokerGameStatus == 2 || m_Gambler.pokerGameStatus == 1 )
                    playerNameLabel = "Player: 1";
                AddLabel( 35, 109, 600, playerNameLabel );

                // bottone per chiudere il gump
                AddButton( 33, 243, 3, 4, 0, GumpButtonType.Reply, 0 );

                // label con la vincita del player
                if( m_Gambler.pokerWinPlayer > 0 )
                    AddLabel( 45, 129, 70, m_Gambler.pokerWinPlayer.ToString() );

                // label e bottoni per la posta e per l'avvio
                AddLabel( 160, 205, 800, m_Gambler.playerBet.ToString() );
                AddButton( 140, 208, 2117, 2118, 105, GumpButtonType.Reply, 0 );
                AddLabel( 240, 205, 800, "Deal" );
                AddButton( 220, 208, 2117, 2118, 101, GumpButtonType.Reply, 0 );

                // label col messaggio attuale di gioco
                AddLabel( 130, 230, 64, m_Gambler.pokerMessage );

                for( int i = 0; i <= 4; ++i )
                {
                    if( m_Gambler.pokerGameStatus == 1 )
                        m_Gambler.playerCards[ i ] = m_Gambler.PickCard( player );

                    int temp = m_Gambler.playerCards[ i ];

                    if( temp > 0 )
                    {
                        if( m_Gambler.pokerGameStatus == 1 || m_Gambler.pokerGameStatus == 2 )
                            AddCheck( 25 + ( ( i + 1 ) * 75 ), 105, 4095 + temp, 4154, false, ( i + 1 ) );
                        else
                            AddImage( 25 + ( ( i + 1 ) * 75 ), 105, 4095 + temp );
                    }
                }

                m_Gambler.SlicePokerGameStatus();
            }
            #endregion

            #region metodi
            public override void OnResponse( NetState sender, RelayInfo info )
            {
                Mobile player = sender.Mobile;

                switch( info.ButtonID )
                {
                    #region deal
                    case 101:
                        {
                            m_Gambler.pokerMessage = "Midgard";

                            if( !player.InRange( m_Gambler.Location, 4 ) )
                            {
                                m_Gambler.isRoundEnded = true;
                                m_Gambler.isBusy = false;
                            }
                            else
                            {
                                if( m_Gambler.pokerGameStatus == 0 )
                                {
                                    if( m_Gambler.PayDealer( player, m_Gambler.playerBet ) )
                                    {
                                        if( ( m_Gambler.m_CurrentCard + 10 ) > 52 )
                                        {
                                            Effects.PlaySound( player.Location, player.Map, 0x3D );
                                            m_Gambler.ShuffleCards();
                                        }

                                        for( int i = 0; i <= 4; ++i )
                                            m_Gambler.playerCards[ i ] = 0;

                                        m_Gambler.pokerGameStatus = 1;
                                        m_Gambler.isRoundEnded = false;
                                        m_Gambler.pokerMessage = "Click on the cards you want re-dealt.";
                                    }
                                    else
                                    {
                                        m_Gambler.pokerMessage = "You need more money!";
                                    }
                                }
                                else if( m_Gambler.pokerGameStatus == 2 )
                                {
                                    m_Gambler.pokerGameStatus = 3;
                                    ArrayList Selections = new ArrayList( info.Switches );

                                    for( int i = 0; i <= 4; ++i )
                                    {
                                        if( Selections.Contains( i + 1 ) )
                                            m_Gambler.playerCards[ i ] = m_Gambler.PickCard( player );
                                    }
                                    FinishPokerGame( player );
                                }
                            }
                            player.SendGump( new PokerGump( player, m_Gambler ) );
                            break;
                        }
                    #endregion
                    #region bet
                    case 105:
                        {
                            if( m_Gambler.isRoundEnded )
                            {
                                m_Gambler.playerBet += 100;
                                if( m_Gambler.playerBet > 1000 )
                                    m_Gambler.playerBet = 100;
                            }
                            player.SendGump( new PokerGump( player, m_Gambler ) );
                            break;
                        }
                    #endregion
                    #region quit
                    case 0:
                        {
                            m_Gambler.isRoundEnded = true;
                            m_Gambler.isBusy = false;
                            Effects.PlaySound( player.Location, player.Map, 0x1E9 );
                            break;
                        }
                    #endregion
                }
            }

            public void FinishPokerGame( Mobile from )
            {
                int match1 = 0, match2 = 0, match3 = 0, match4 = 0, match5 = 0, temp;
                bool isStrt = true, isFlush = false;
                string Temp;

                // Copia le carte per manipolarle
                for( int i = 0; i <= 4; i++ )
                    m_Gambler.m_DealerCards[ i ] = m_Gambler.playerCards[ i ];

                // Le ordina per valore crescente
                for( int j = 4; j >= 0; j-- )
                {
                    for( int i = 0; i < 4; i++ )
                    {
                        if( m_Gambler.PokerCardValue( m_Gambler.m_DealerCards[ i ] ) >= m_Gambler.PokerCardValue( m_Gambler.m_DealerCards[ i + 1 ] ) )
                        {
                            temp = m_Gambler.m_DealerCards[ i ];
                            m_Gambler.m_DealerCards[ i ] = m_Gambler.m_DealerCards[ i + 1 ];
                            m_Gambler.m_DealerCards[ i + 1 ] = temp;
                        }
                    }
                }

                // Se almeno una non e' in scala allora finisce la partita
                for( int i = 4; i > 0; i-- )
                {
                    if( m_Gambler.PokerCardValue( m_Gambler.m_DealerCards[ i ] ) != ( m_Gambler.PokerCardValue( m_Gambler.m_DealerCards[ 0 ] ) + i ) )
                    {
                        isStrt = false;
                        m_Gambler.pokerMessage = "Game Over.";
                        m_Gambler.pokerWinPlayer = 0;
                    }
                }

                #region scala e colore
                // Controlla se sono in scala A 2 3 4 5
                /*
                if( ( m_Gambler.PokerCardValue( m_Gambler.m_DealerCards[ 0 ] ) == 2 ) &&
                   ( m_Gambler.PokerCardValue( m_Gambler.m_DealerCards[ 1 ] ) == 3 ) &&
                   ( m_Gambler.PokerCardValue( m_Gambler.m_DealerCards[ 2 ] ) == 4 ) &&
                   ( m_Gambler.PokerCardValue( m_Gambler.m_DealerCards[ 3 ] ) == 5 ) &&
                   ( m_Gambler.PokerCardValue( m_Gambler.m_DealerCards[ 4 ] ) == 14 ) )
                {
                    isStrt = true;
                    m_Gambler.pokerMessage = "Straight.";
                    m_Gambler.pokerWinPlayer = m_Gambler.playerBet * (int)PokerRewards.Scala;
                }
                */

                bool middleStr = true;

                // Se le prime 4 carte sono in 
                for( int i = 1; i < 4; i++ )
                {
                    if( m_Gambler.PokerCardValue( m_Gambler.m_DealerCards[ i ] ) != m_Gambler.PokerCardValue( m_Gambler.m_DealerCards[ i - 1 ] ) + 1 )
                        middleStr = false;
                }

                if( middleStr )
                {
                    int fifth = m_Gambler.m_DealerCards[ 4 ];
                    int forth = m_Gambler.m_DealerCards[ 3 ];

                    // Se la quinta carta non e' la quarta + 1 o un asso allora non sono sicuramente in scala
                    if( ( fifth != forth + 1 ) && fifth != 14 )
                        middleStr = false;

                    if( middleStr )
                    {
                        isStrt = true;
                        m_Gambler.pokerMessage = "Straight.";
                        m_Gambler.pokerWinPlayer = m_Gambler.playerBet * (int)PokerRewards.Scala;
                    }
                }

                // Se hanno tutte lo stesso seme e' colore
                Temp = m_Gambler.CardSuit( m_Gambler.m_DealerCards[ 0 ] );
                if( Temp == m_Gambler.CardSuit( m_Gambler.m_DealerCards[ 1 ] ) &&
                    Temp == m_Gambler.CardSuit( m_Gambler.m_DealerCards[ 2 ] ) &&
                    Temp == m_Gambler.CardSuit( m_Gambler.m_DealerCards[ 3 ] ) &&
                    Temp == m_Gambler.CardSuit( m_Gambler.m_DealerCards[ 4 ] ) )
                {
                    isFlush = true;
                    m_Gambler.pokerMessage = "Flush.";
                    m_Gambler.pokerWinPlayer = m_Gambler.playerBet * (int)PokerRewards.Colore;
                }

                if( isFlush && isStrt )
                {
                    if( m_Gambler.m_DealerCards[ 0 ] == 10 )
                    {
                        m_Gambler.pokerMessage = "Royal Straight Flush";
                        m_Gambler.pokerWinPlayer = m_Gambler.playerBet * (int)PokerRewards.ScalaColore;
                    }
                    else
                    {
                        m_Gambler.pokerMessage = "Straight Flush";
                        m_Gambler.pokerWinPlayer = m_Gambler.playerBet * (int)PokerRewards.ScalaReale;
                    }
                }
                #endregion

                #region coppia doppiacoppia tris full poker
                if( !isStrt && !isFlush )
                {
                    for( int i = 0; i <= 4; i++ )
                    {
                        temp = m_Gambler.PokerCardValue( m_Gambler.m_DealerCards[ i ] );

                        if( ( m_Gambler.PokerCardValue( m_Gambler.m_DealerCards[ 0 ] ) == temp ) && i != 0 )
                            match1++;
                        if( ( m_Gambler.PokerCardValue( m_Gambler.m_DealerCards[ 1 ] ) == temp ) && i != 1 )
                            match2++;
                        if( ( m_Gambler.PokerCardValue( m_Gambler.m_DealerCards[ 2 ] ) == temp ) && i != 2 )
                            match3++;
                        if( ( m_Gambler.PokerCardValue( m_Gambler.m_DealerCards[ 3 ] ) == temp ) && i != 3 )
                            match4++;
                        if( ( m_Gambler.PokerCardValue( m_Gambler.m_DealerCards[ 4 ] ) == temp ) && i != 4 )
                            match5++;
                    }

                    // Se ne trova 3 + 1 per tipo allora e' Poker
                    if( ( match1 == 3 ) || ( match2 == 3 ) || ( match3 == 3 ) || ( match4 == 3 ) || ( match5 == 3 ) )
                    {
                        m_Gambler.pokerMessage = "4 of a Kind: Poker!";
                        m_Gambler.pokerWinPlayer = m_Gambler.playerBet * (int)PokerRewards.Poker;
                    }

                    // Se ne trova 2 + 1 per tipo allora e' Tris
                    if( ( match1 == 2 ) || ( match2 == 2 ) || ( match3 == 2 ) || ( match4 == 2 ) || ( match5 == 2 ) )
                    {
                        m_Gambler.pokerMessage = "3 of a Kind: Tris";
                        m_Gambler.pokerWinPlayer = m_Gambler.playerBet * (int)PokerRewards.Tris;
                    }

                    // Se la somma dei match vale 8 e' Full
                    if( ( match1 + match2 + match3 + match4 + match5 ) == 8 )
                    {
                        m_Gambler.pokerMessage = "Full House.";
                        m_Gambler.pokerWinPlayer = m_Gambler.playerBet * (int)PokerRewards.Full;
                    }

                    // Se la somma e' 4 allora e' doppiacoppia
                    if( ( match1 + match2 + match3 + match4 + match5 ) == 4 )
                    {
                        m_Gambler.pokerMessage = "Two Pair.";
                        m_Gambler.pokerWinPlayer = m_Gambler.playerBet * (int)PokerRewards.DoppiaCoppia;
                    }

                    temp = 0;
                    if( ( match1 + match2 + match3 + match4 + match5 ) == 2 )
                    {
                        if( match1 == 1 )
                            temp = m_Gambler.PokerCardValue( m_Gambler.m_DealerCards[ 0 ] );
                        if( match2 == 1 )
                            temp = m_Gambler.PokerCardValue( m_Gambler.m_DealerCards[ 1 ] );
                        if( match3 == 1 )
                            temp = m_Gambler.PokerCardValue( m_Gambler.m_DealerCards[ 2 ] );
                        if( match4 == 1 )
                            temp = m_Gambler.PokerCardValue( m_Gambler.m_DealerCards[ 3 ] );
                        if( temp >= 10 )
                        {
                            m_Gambler.pokerMessage = "Pair 10's +";
                            m_Gambler.pokerWinPlayer = m_Gambler.playerBet;
                        }
                    }
                }
                #endregion

                if( m_Gambler.pokerWinPlayer > 0 )
                {
                    m_Gambler.PayPlayer( from, m_Gambler.pokerWinPlayer );
                    Effects.PlaySound( from.Location, from.Map, 0x36 );
                    m_Gambler.gameStats[ 4 ] += 1;
                }
                else if( m_Gambler.pokerWinPlayer == m_Gambler.playerBet )
                    m_Gambler.gameStats[ 5 ] += 1;
                else
                    m_Gambler.gameStats[ 3 ] += 1;

            }
            #endregion
        }

        public class BlackjackGump : Gump
        {
            #region campi
            private Gambler m_Gambler;
            #endregion

            #region proprietà
            #endregion

            #region costruttori
            public BlackjackGump( Mobile player, Gambler gambler )
                : base( 10, 10 )
            {
                Closable = false;
                Disposable = false;
                Dragable = false;
                Resizable = false;

                m_Gambler = gambler;

                int i, dealervalue = 0, temp;
                string scoredmsg, scorepmsg;

                AddPage( 0 );

                AddImageTiled( 30, 100, 460, 280, 2624 );
                AddAlphaRegion( 90, 100, 460, 230 );

                AddLabel( 35, 109, 1500, "Dealer:" );
                AddLabel( 35, 229, 600, "Player:" );

                if( m_Gambler.pokerGameStatus > 0 )
                    AddLabel( 45, 129, 70, m_Gambler.pokerGameStatus.ToString() );

                if( m_Gambler.pokerWinPlayer > 0 )
                    AddLabel( 45, 249, 70, m_Gambler.pokerWinPlayer.ToString() );

                AddButton( 40, 333, 2117, 2118, 101, GumpButtonType.Reply, 0 );
                AddLabel( 60, 330, 800, "Deal" );

                AddButton( 150, 333, 2117, 2118, 102, GumpButtonType.Reply, 0 );
                AddLabel( 170, 330, 800, "Hit" );

                AddButton( 200, 333, 2117, 2118, 103, GumpButtonType.Reply, 0 );
                AddLabel( 220, 330, 800, "Stand" );

                AddButton( 280, 333, 2117, 2118, 104, GumpButtonType.Reply, 0 );
                AddLabel( 300, 330, 800, "Double Down" );

                AddButton( 90, 333, 2117, 2118, 105, GumpButtonType.Reply, 0 );
                AddButton( 33, 363, 3, 4, 666, GumpButtonType.Reply, 0 );

                for( i = 0; i <= 4; ++i )
                {
                    temp = m_Gambler.dealerCards[ i ];
                    if( temp > 0 )
                    {
                        if( !m_Gambler.isDealerCardHidden || ( m_Gambler.isDealerCardHidden && i > 0 ) )
                        {
                            AddImage( 25 + ( ( i + 1 ) * 75 ), 110, 4095 + temp );
                            dealervalue += m_Gambler.CardValue( temp );
                        }
                        else
                        {
                            AddImage( 25 + ( ( i + 1 ) * 75 ), 110, 4154 );
                        }
                    }
                }

                for( i = 0; i <= 4; ++i )
                {
                    temp = m_Gambler.playerCards[ i ];
                    if( temp > 0 )
                        AddImage( 25 + ( ( i + 1 ) * 75 ), 230, 4095 + temp );
                }

                AddLabel( 110, 330, 800, "" + m_Gambler.playerBet );

                if( !m_Gambler.isDealerCardHidden )
                    dealervalue = DealerCardValue();

                if( m_Gambler.CardValue( m_Gambler.dealerCards[ 0 ] ) +
                    m_Gambler.CardValue( m_Gambler.dealerCards[ 1 ] ) == 21 &&
                    !m_Gambler.isDealerCardHidden )
                    scoredmsg = "BJ";
                else
                    scoredmsg = dealervalue.ToString();

                if( m_Gambler.CardValue( m_Gambler.playerCards[ 1 ] ) +
                    m_Gambler.CardValue( m_Gambler.playerCards[ 1 ] ) == 21 )
                    scorepmsg = "BJ";
                else
                    scorepmsg = PlayerCardValue().ToString();

                AddLabel( 63, 155, 1500, scoredmsg );
                AddLabel( 63, 274, 600, scorepmsg );
                AddLabel( 100, 350, 64, m_Gambler.pokerMessage );

            }
            #endregion

            #region metodi
            public override void OnResponse( NetState sender, RelayInfo info )
            {
                Mobile from = sender.Mobile;

                int i, temp;

                switch( info.ButtonID )
                {
                    #region deal
                    case 101:
                        {
                            m_Gambler.pokerMessage = "Let's playe on Midgard";

                            if( !from.InRange( m_Gambler.Location, 4 ) )
                            {
                                m_Gambler.isRoundEnded = true;
                                m_Gambler.isBusy = false;
                            }
                            else
                            {
                                if( m_Gambler.isRoundEnded )
                                {
                                    if( m_Gambler.playerBet > 1000 )
                                        m_Gambler.playerBet = 1000;

                                    if( m_Gambler.PayDealer( from, m_Gambler.playerBet ) )
                                    {
                                        m_Gambler.pokerGameStatus = 0;
                                        m_Gambler.pokerWinPlayer = 0;
                                        m_Gambler.isRoundEnded = false;
                                        m_Gambler.isDealerCardHidden = true;

                                        for( i = 2; i <= 4; ++i )
                                        {
                                            m_Gambler.dealerCards[ i ] = 0;
                                            m_Gambler.playerCards[ i ] = 0;
                                        }

                                        m_Gambler.dealerCards[ 0 ] = m_Gambler.PickCard( from );
                                        m_Gambler.playerCards[ 0 ] = m_Gambler.PickCard( from );
                                        m_Gambler.dealerCards[ 1 ] = m_Gambler.PickCard( from );
                                        m_Gambler.playerCards[ 1 ] = m_Gambler.PickCard( from );

                                        if( m_Gambler.CardValue( m_Gambler.dealerCards[ 0 ] ) +
                                            m_Gambler.CardValue( m_Gambler.dealerCards[ 1 ] ) == 21 )
                                            m_Gambler.blackJackDealer = true;
                                        else if( m_Gambler.CardValue( m_Gambler.playerCards[ 1 ] ) +
                                                 m_Gambler.CardValue( m_Gambler.playerCards[ 1 ] ) == 21 )
                                            m_Gambler.blackJackPlayer = true;
                                        if( m_Gambler.blackJackPlayer )
                                            FinishBlackjackGame( from );
                                    }
                                    else
                                        m_Gambler.pokerMessage = "You need more money!";
                                }
                                from.SendGump( new BlackjackGump( from, m_Gambler ) );
                            }
                            break;
                        }
                    #endregion
                    #region hit
                    case 102:
                        {
                            if( !m_Gambler.isRoundEnded )
                            {
                                temp = 0;
                                for( i = 2; i <= 4; ++i )
                                {
                                    if( m_Gambler.playerCards[ i ] == 0 && temp == 0 )
                                    {
                                        m_Gambler.playerCards[ i ] = m_Gambler.PickCard( from );
                                        temp = i;
                                        i = 6;
                                    }
                                }

                                if( ( temp > 0 && PlayerCardValue() <= 21 ) && i != 5 )
                                    from.SendGump( new BlackjackGump( from, m_Gambler ) );
                                else
                                    FinishBlackjackGame( from );
                            }
                            else
                                from.SendGump( new BlackjackGump( from, m_Gambler ) );
                            break;
                        }
                    #endregion
                    #region stand
                    case 103:
                        {
                            if( !m_Gambler.isRoundEnded )
                                FinishBlackjackGame( from );
                            else
                                from.SendGump( new BlackjackGump( from, m_Gambler ) );
                            break;
                        }
                    #endregion
                    #region double down
                    case 104:
                        {
                            if( !m_Gambler.isRoundEnded )
                            {
                                temp = 0;
                                for( i = 0; i <= 4; ++i )
                                {
                                    if( m_Gambler.playerCards[ i ] > 0 )
                                        temp++;
                                }

                                if( /* temp == 2 */ temp <= 4 && m_Gambler.PayDealer( from, m_Gambler.playerBet ) )
                                {
                                    m_Gambler.playerCards[ /* 2 */ temp ] = m_Gambler.PickCard( from );
                                    m_Gambler.playerBet *= 2;
                                    FinishBlackjackGame( from );
                                    return;
                                }

                                from.SendMessage( "I'm lost... sorry, i want to reset the game!" );
                                FinishBlackjackGame( from );
                            }
                            else
                                from.SendGump( new BlackjackGump( from, m_Gambler ) );
                            break;
                        }
                    #endregion
                    #region bet
                    case 105:
                        {
                            if( m_Gambler.isRoundEnded )
                            {
                                m_Gambler.playerBet += 100;
                                if( m_Gambler.playerBet > 1000 )
                                    m_Gambler.playerBet = 100;
                            }
                            from.SendGump( new BlackjackGump( from, m_Gambler ) );
                            break;
                        }
                    #endregion
                    #region quit
                    case 666:
                        {
                            m_Gambler.isRoundEnded = true;
                            m_Gambler.isBusy = false;
                            Effects.PlaySound( from.Location, from.Map, 0x1e9 );
                            break;
                        }
                    #endregion
                }
            }

            public void FinishBlackjackGame( Mobile from )
            {
                int temp, dealervalue = DealerCardValue(), playervalue = PlayerCardValue();
                temp = ( m_Gambler.playerBet / 2 );

                if( m_Gambler.blackJackDealer && m_Gambler.blackJackPlayer )
                {
                    m_Gambler.pokerGameStatus = temp;
                    m_Gambler.pokerWinPlayer = m_Gambler.playerBet + temp;

                    m_Gambler.PayPlayer( from, m_Gambler.pokerWinPlayer );
                    m_Gambler.gameStats[ 2 ] += 1;
                    m_Gambler.pokerMessage = "We have a push.";

                }
                else if( m_Gambler.blackJackDealer )
                {
                    m_Gambler.gameStats[ 0 ] += 1;
                    m_Gambler.pokerMessage = "Looks like I won.";
                    m_Gambler.pokerGameStatus = m_Gambler.playerBet;
                    m_Gambler.pokerWinPlayer = 0;
                }
                else if( m_Gambler.blackJackPlayer )
                {
                    m_Gambler.pokerGameStatus = 0;
                    m_Gambler.pokerWinPlayer = ( m_Gambler.playerBet * 2 ) + temp;

                    m_Gambler.PayPlayer( from, m_Gambler.pokerWinPlayer );
                    m_Gambler.gameStats[ 1 ] += 1;
                    m_Gambler.pokerMessage = "You won this one.";
                }
                else
                {
                    if( playervalue > 21 || ( dealervalue > playervalue && dealervalue <= 21 ) )
                    {
                        m_Gambler.gameStats[ 0 ] += 1;
                        m_Gambler.pokerMessage = "Looks like I won.";
                        m_Gambler.pokerGameStatus = m_Gambler.playerBet;
                        m_Gambler.pokerWinPlayer = 0;
                    }
                    else
                    {
                        if( dealervalue < 17 )
                        {
                            int i;
                            for( i = 2; i <= 4; ++i )
                            {
                                if( m_Gambler.m_DealerCards[ i ] == 0 )
                                {
                                    m_Gambler.m_DealerCards[ i ] = m_Gambler.PickCard( from );
                                    dealervalue = DealerCardValue();
                                }
                                if( dealervalue >= 17 )
                                    i = 6;
                            }
                        }

                        if( playervalue > 21 || ( dealervalue > playervalue && dealervalue <= 21 ) )
                        {
                            m_Gambler.gameStats[ 0 ] += 1;
                            m_Gambler.pokerMessage = "I won this round.";
                            m_Gambler.pokerGameStatus = m_Gambler.playerBet;
                            m_Gambler.pokerWinPlayer = 0;
                        }
                        else if( dealervalue == playervalue )
                        {
                            m_Gambler.pokerGameStatus = temp;
                            m_Gambler.pokerWinPlayer = m_Gambler.playerBet + temp;

                            m_Gambler.PayPlayer( from, m_Gambler.pokerWinPlayer );
                            m_Gambler.gameStats[ 2 ] += 1;
                            m_Gambler.pokerMessage = "We have a push.";
                        }
                        else
                        {
                            if( playervalue == 21 )
                            {
                                m_Gambler.pokerGameStatus = 0;
                                m_Gambler.pokerWinPlayer = ( m_Gambler.playerBet * 2 );

                                m_Gambler.PayPlayer( from, m_Gambler.pokerWinPlayer );
                                m_Gambler.gameStats[ 1 ] += 1;
                                m_Gambler.pokerMessage = "You have won another round.";
                            }
                            else
                            {
                                m_Gambler.pokerGameStatus = 0;
                                m_Gambler.pokerWinPlayer = ( m_Gambler.playerBet * 2 );

                                m_Gambler.PayPlayer( from, m_Gambler.pokerWinPlayer );
                                m_Gambler.gameStats[ 1 ] += 1;
                                m_Gambler.pokerMessage = "You won this one.";
                            }
                        }
                    }
                }
                m_Gambler.blackJackDealer = false;
                m_Gambler.blackJackPlayer = false;
                m_Gambler.pokerWinPlayer = ( m_Gambler.pokerWinPlayer - m_Gambler.playerBet );
                m_Gambler.isDealerCardHidden = false;
                m_Gambler.isRoundEnded = true;
                Effects.PlaySound( from.Location, from.Map, 0x36 );
                from.SendGump( new BlackjackGump( from, m_Gambler ) );
            }

            public int DealerCardValue()
            {
                int i;
                int gotace = 0, dealervalue = 0;

                for( i = 0; i <= 4; ++i )
                {
                    int tempcard = m_Gambler.CardValue( m_Gambler.dealerCards[ i ] );
                    if( tempcard == 11 )
                        gotace++;

                    dealervalue += tempcard;
                }

                while( dealervalue > 21 && gotace > 0 )
                {
                    dealervalue -= 10;
                    gotace--;
                }

                return dealervalue;
            }

            public int PlayerCardValue()
            {
                int i;
                int gotace = 0, playervalue = 0;

                for( i = 0; i <= 4; ++i )
                {
                    int tempcard = m_Gambler.CardValue( m_Gambler.playerCards[ i ] );
                    if( tempcard == 11 )
                        gotace++;

                    playervalue += tempcard;
                }

                while( playervalue > 21 && gotace > 0 )
                {
                    playervalue -= 10;
                    gotace--;
                }

                return playervalue;
            }
            #endregion
        }
    }
}
