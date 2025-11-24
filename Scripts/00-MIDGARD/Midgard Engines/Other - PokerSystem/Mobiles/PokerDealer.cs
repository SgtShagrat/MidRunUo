using System;
using System.Collections.Generic;

using Server.Commands;
using Server.Items;
using Server.Mobiles;

namespace Server.Poker
{
    public class PokerDealer : Mobile
    {
        public static void Initialize()
        {
            CommandSystem.Register( "AddPokerSeat", AccessLevel.Administrator, new CommandEventHandler( AddPokerSeat_OnCommand ) );
            CommandSystem.Register( "PokerKick", AccessLevel.Seer, new CommandEventHandler( PokerKick_OnCommand ) );

            EventSink.Disconnected += new DisconnectedEventHandler( EventSink_Disconnected );
        }

        private double m_Rake;
        private int m_MaxPlayers;
        private bool m_Active;

        public static int Jackpot { get; set; }

        [CommandProperty( AccessLevel.Seer )]
        public bool TournamentMode { get; set; }

        [CommandProperty( AccessLevel.Administrator )]
        public bool ClearSeats { get { return false; } set { Seats.Clear(); } }

        [CommandProperty( AccessLevel.Administrator )]
        public int RakeMax { get; set; }

        [CommandProperty( AccessLevel.Seer )]
        public int MinBuyIn { get; set; }

        [CommandProperty( AccessLevel.Seer )]
        public int MaxBuyIn { get; set; }

        [CommandProperty( AccessLevel.Seer )]
        public int SmallBlind { get; set; }

        [CommandProperty( AccessLevel.Seer )]
        public int BigBlind { get; set; }

        [CommandProperty( AccessLevel.Administrator )]
        public Point3D ExitLocation { get; set; }

        [CommandProperty( AccessLevel.Administrator )]
        public Map ExitMap { get; set; }

        [CommandProperty( AccessLevel.Administrator )]
        public double Rake
        {
            get { return m_Rake; }
            set
            {
                if( value > 1 )
                    m_Rake = 1;
                else if( value < 0 )
                    m_Rake = 0;
                else
                    m_Rake = value;
            }
        }

        [CommandProperty( AccessLevel.Seer )]
        public int MaxPlayers
        {
            get { return m_MaxPlayers; }
            set
            {
                if( value > 22 ) m_MaxPlayers = 22;
                else if( value < 0 ) m_MaxPlayers = 0;
                else m_MaxPlayers = value;
            }
        }

        [CommandProperty( AccessLevel.Seer )]
        public bool Active
        {
            get { return m_Active; }
            set
            {
                List<PokerPlayer> toRemove = new List<PokerPlayer>();

                if( !value )
                    foreach( PokerPlayer player in Game.Players.Players )
                        if( player.Owner != null )
                            toRemove.Add( player );

                foreach( PokerPlayer t in toRemove )
                {
                    t.Owner.SendMessage( 0x22, "The poker dealer has been set to inactive by a game master, and you are now being removed from the poker game and being refunded the money that you currently have." );
                    Game.RemovePlayer( t );
                }

                m_Active = value;
            }
        }

        public PokerGame Game { get; set; }
        public List<Point3D> Seats { get; set; }

        [Constructable]
        public PokerDealer()
            : this( 5 )
        {
        }

        [Constructable]
        public PokerDealer( int maxPlayers )
        {
            Blessed = true;
            Frozen = true;
            InitStats( 100, 100, 100 );

            Title = "the poker dealer";
            Hue = Utility.RandomSkinHue();
            NameHue = 0x35;
            Female = Utility.RandomBool();

            if( Female )
            {
                Body = 0x191;
                Name = NameList.RandomName( "female" );
            }
            else
            {
                Body = 0x190;
                Name = NameList.RandomName( "male" );
            }

            Dress();

            MaxPlayers = maxPlayers;
            Seats = new List<Point3D>();

            BigBlind = 100;
            SmallBlind = 50;

            ExitMap = Map;
            ExitLocation = CoveLocation;

            MaxBuyIn = 10000;
            MinBuyIn = 1000;

            m_Rake = 0.10;	    //10% rake default
            RakeMax = 5000;	    //5k maximum rake default

            Game = new PokerGame( this );
        }

        public static readonly Point3D CoveLocation = new Point3D( 2230, 1224, 0 );

        private void Dress()
        {
            AddItem( new FancyShirt( 0 ) );

            Item pants = new LongPants();
            pants.Hue = 1;
            AddItem( pants );

            Item shoes = new Shoes();
            shoes.Hue = 1;
            AddItem( shoes );

            Item sash = new BodySash();
            sash.Hue = 1;
            AddItem( sash );

            Utility.AssignRandomHair( this );
        }

        public static JackpotInfo JackpotWinners { get; set; }

        public static void AwardJackpot()
        {
            if( JackpotWinners != null && JackpotWinners.Winners != null && JackpotWinners.Winners.Count > 0 )
            {
                int award = Jackpot / JackpotWinners.Winners.Count;

                if( award <= 0 )
                    return;

                foreach( PokerPlayer m in JackpotWinners.Winners )
                {
                    if( m != null && m.Owner != null && m.Owner.BankBox != null )
                    {
                        m.Owner.BankBox.DropItem( new BankCheck( award ) );
                        World.Broadcast( 1161, true, "{0} has won the poker jackpot of {1} gold with {2}", m.Owner.Name, award.ToString( "#,###" ), HandRanker.RankString( JackpotWinners.Hand ) );
                    }
                }

                Jackpot = 0;
                JackpotWinners = null;
            }
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( !m_Active )
                from.SendMessage( 0x9A, "This table is inactive" );
            else if( !InRange( from.Location, 8 ) )
                from.PrivateOverheadMessage( Network.MessageType.Regular, 0x22, true, "I am too far away to do that", from.NetState );
            else if( MinBuyIn == 0 || MaxBuyIn == 0 )
                from.SendMessage( 0x9A, "This table is inactive" );
            else if( MinBuyIn > MaxBuyIn )
                from.SendMessage( 0x9A, "This table is inactive" );
            else if( Seats.Count < m_MaxPlayers )
                from.SendMessage( 0x9A, "This table is inactive" );
            else if( Game.GetIndexFor( from ) != -1 )
                return; //TODO: Grab more chips from the player's bankbox
            else if( Game.Players.Count >= m_MaxPlayers )
            {
                from.SendMessage( 0x22, "This table is full" );
                base.OnDoubleClick( from );
            }
            else if( Game.Players.Count < m_MaxPlayers )
            {
                from.CloseGump( typeof( PokerJoinGump ) );
                from.SendGump( new PokerJoinGump( from, Game ) );
            }
        }

        public override void OnDelete()
        {
            List<PokerPlayer> toRemove = new List<PokerPlayer>();

            foreach( PokerPlayer player in Game.Players.Players )
                if( player.Owner != null )
                    toRemove.Add( player );

            foreach( PokerPlayer t in toRemove )
            {
                t.Owner.SendMessage( 0x22, "The poker dealer has been deleted, and you are now being removed from the poker game and being refunded the money that you currently have." );
                Game.RemovePlayer( t );
            }

            base.OnDelete();
        }

        public static void PokerKick_OnCommand( CommandEventArgs e )
        {
            Mobile from = e.Mobile;

            if( from == null )
                return;

            foreach( Mobile m in from.GetMobilesInRange( 0 ) )
            {
                if( m is Midgard2PlayerMobile )
                {
                    Midgard2PlayerMobile pm = (Midgard2PlayerMobile)m;

                    PokerGame game = pm.PokerGame;

                    if( game != null )
                    {
                        PokerPlayer player = game.GetPlayer( m );

                        if( player != null )
                        {
                            game.RemovePlayer( player );
                            from.SendMessage( "They have been removed from the poker table" );
                            return;
                        }
                    }
                }
            }

            from.SendMessage( "No one found to kick from a poker table. Make sure you are standing on top of them." );
        }

        static void EventSink_Disconnected( DisconnectedEventArgs e )
        {
            Mobile from = e.Mobile;

            if( from == null )
                return;

            if( from is Midgard2PlayerMobile )
            {
                Midgard2PlayerMobile pm = (Midgard2PlayerMobile)from;

                PokerGame game = pm.PokerGame;

                if( game != null )
                {
                    PokerPlayer player = game.GetPlayer( from );

                    if( player != null )
                        game.RemovePlayer( player );
                }
            }
        }

        public static void AddPokerSeat_OnCommand( CommandEventArgs e )
        {
            Mobile from = e.Mobile;
            if( from == null )
                return;

            bool success = false;
            foreach( Mobile m in from.GetMobilesInRange( 3 ) )
            {
                if( !( m is PokerDealer ) )
                    continue;

                if( ( (PokerDealer)m ).AddPokerSeat( from, from.Location ) != -1 )
                {
                    from.SendMessage( 0x22, "A new seat was successfully created." );
                    success = true;
                    break;
                }
                else
                {
                    from.SendMessage( 0x22, "There is no more room at that table for another seat. Try increasing the value of MaxPlayers first." );
                    break;
                }
            }

            if( !success )
                from.SendMessage( 0x22, "No poker dealers were found in range." );
        }

        public int AddPokerSeat( Mobile from, Point3D seat )
        {
            if( Seats.Count >= m_MaxPlayers )
                return -1;

            Seats.Add( seat );
            return 0;
        }

        public bool SeatTaken( Point3D seat )
        {
            for( int i = 0; i < Game.Players.Count; ++i )
                if( Game.Players[ i ].Seat == seat )
                    return true;

            return false;
        }

        public int RakeGold( int gold )
        {
            double amount = gold * m_Rake;
            return (int)( amount > RakeMax ? RakeMax : amount );
        }

        public PokerDealer( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); //version

            writer.Write( m_Active );
            writer.Write( SmallBlind );
            writer.Write( BigBlind );
            writer.Write( MinBuyIn );
            writer.Write( MaxBuyIn );
            writer.Write( ExitLocation );
            writer.Write( ExitMap );
            writer.Write( m_Rake );
            writer.Write( RakeMax );
            writer.Write( m_MaxPlayers );

            writer.Write( Seats.Count );

            foreach( Point3D t in Seats )
                writer.Write( t );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            switch( version )
            {
                case 0:
                    m_Active = reader.ReadBool();
                    SmallBlind = reader.ReadInt();
                    BigBlind = reader.ReadInt();
                    MinBuyIn = reader.ReadInt();
                    MaxBuyIn = reader.ReadInt();
                    ExitLocation = reader.ReadPoint3D();
                    ExitMap = reader.ReadMap();
                    m_Rake = reader.ReadDouble();
                    RakeMax = reader.ReadInt();
                    m_MaxPlayers = reader.ReadInt();

                    int count = reader.ReadInt();
                    Seats = new List<Point3D>();

                    for( int i = 0; i < count; ++i )
                        Seats.Add( reader.ReadPoint3D() );

                    break;
            }

            Game = new PokerGame( this );
        }

        public class JackpotInfo
        {
            public List<PokerPlayer> Winners { get; private set; }
            public ResultEntry Hand { get; private set; }
            public DateTime Date { get; private set; }

            public JackpotInfo( List<PokerPlayer> winners, ResultEntry hand, DateTime date )
            {
                Winners = winners;
                Hand = hand;
                Date = date;
            }
        }
    }
}