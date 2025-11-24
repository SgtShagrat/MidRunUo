using System;
using System.Collections.Generic;
using System.Text;

using Server.Mobiles;

namespace Server.Poker
{
    public class PokerGame
    {
        public static void Initialize()
        {
            GameBackup.PokerGames = new List<PokerGame>();
            EventSink.Crashed += new CrashedEventHandler( EventSink_Crashed );
        }

        private static void EventSink_Crashed( CrashedEventArgs e )
        {
            foreach( PokerGame game in GameBackup.PokerGames )
            {
                List<PokerPlayer> toRemove = new List<PokerPlayer>();

                foreach( PokerPlayer player in game.Players.Players )
                    if( player.Owner != null )
                        toRemove.Add( player );

                foreach( PokerPlayer player in toRemove )
                {
                    player.SendMessage( 0x22, "The server has crashed, and you are now being removed from the poker game and being refunded the money that you currently have." );
                    game.RemovePlayer( player );
                }
            }
        }

        private List<Card> m_CommunityCards;
        private PokerGameTimer m_Timer;

        public bool NeedsGumpUpdate { get; set; }
        public int CommunityGold { get; set; }
        public int CurrentBet { get; set; }
        public Deck Deck { get; set; }
        public PokerGameState State { get; set; }
        public PokerDealer Dealer { get; set; }
        public PokerPlayer DealerButton { get; private set; }
        public PokerPlayer SmallBlind { get; private set; }
        public PokerPlayer BigBlind { get; private set; }
        public List<Card> CommunityCards { get { return m_CommunityCards; } set { m_CommunityCards = value; } }
        public PokerGameTimer Timer { get { return m_Timer; } set { m_Timer = value; } }
        public PlayerStructure Players { get; private set; }

        public bool IsBettingRound { get { return ( (int)State % 2 == 0 ); } }

        public PokerGame( PokerDealer dealer )
        {
            Dealer = dealer;
            NeedsGumpUpdate = false;
            m_CommunityCards = new List<Card>();
            State = PokerGameState.Inactive;
            Deck = new Deck();
            m_Timer = new PokerGameTimer( this );
            Players = new PlayerStructure( this );
        }

        public void PokerMessage( Mobile from, string message )
        {
            from.PublicOverheadMessage( Network.MessageType.Regular, 0x9A, true, message );

            for( int i = 0; i < Players.Count; ++i )
                if( Players[ i ].Owner != null )
                    Players[ i ].Owner.SendMessage( 0x9A, "[{0}]: {1}", from.Name, message );
        }

        public void PokerGame_PlayerMadeDecision( PokerPlayer player )
        {
            if( Players.Peek() == player )
            {
                if( player.Owner == null )
                    return;

                bool resetTurns = false;

                switch( player.Action )
                {
                    case PlayerAction.None: break;
                    case PlayerAction.Bet:
                        {
                            PokerMessage( player.Owner, String.Format( "I bet {0}.", player.Bet ) );
                            CurrentBet = player.Bet;
                            player.RoundBet = player.Bet;
                            player.Gold -= player.Bet;
                            player.RoundGold += player.Bet;
                            CommunityGold += player.Bet;
                            resetTurns = true;

                            break;
                        }
                    case PlayerAction.Raise:
                        {
                            PokerMessage( player.Owner, String.Format( "I raise by {0}.", player.Bet ) );
                            CurrentBet += player.Bet;
                            int diff = CurrentBet - player.RoundBet;
                            player.Gold -= diff;
                            player.RoundGold += diff;
                            player.RoundBet += diff;
                            CommunityGold += diff;
                            player.Bet = diff;
                            resetTurns = true;

                            break;
                        }
                    case PlayerAction.Call:
                        {
                            PokerMessage( player.Owner, "I call." );

                            int diff = CurrentBet - player.RoundBet; //how much they owe in the pot
                            player.Bet = diff;
                            player.Gold -= diff;
                            player.RoundGold += diff;
                            player.RoundBet += diff;
                            CommunityGold += diff;

                            break;
                        }
                    case PlayerAction.Check:
                        {
                            if( !player.LonePlayer )
                                PokerMessage( player.Owner, "Check." );

                            break;
                        }
                    case PlayerAction.Fold:
                        {
                            PokerMessage( player.Owner, "I fold." );

                            if( Players.Round.Contains( player ) )
                                Players.Round.Remove( player );
                            if( Players.Turn.Contains( player ) )
                                Players.Turn.Remove( player );

                            if( Players.Round.Count == 1 )
                            {
                                DoShowdown( true );
                                return;
                            }

                            break;
                        }
                    case PlayerAction.AllIn:
                        {
                            if( !player.IsAllIn )
                            {
                                if( player.Forced )
                                    PokerMessage( player.Owner, "I call: all-in." );
                                else
                                    PokerMessage( player.Owner, "All in." );

                                int diff = player.Gold - CurrentBet;

                                if( diff > 0 )
                                    CurrentBet += diff;

                                player.Bet = player.Gold;
                                player.RoundGold += player.Gold;
                                player.RoundBet += player.Gold;
                                CommunityGold += player.Gold;
                                player.Gold = 0;

                                //We need to check to see if this is a follow up action, or a first call
                                //before we reset the turns
                                if( Players.Prev() != null )
                                {
                                    resetTurns = ( Players.Prev().Action == PlayerAction.Check );

                                    PokerPlayer prev = Players.Prev();

                                    if( prev.Action == PlayerAction.Check ||
                                        ( prev.Action == PlayerAction.Bet && prev.Bet < player.Bet ) ||
                                        ( prev.Action == PlayerAction.AllIn && prev.Bet < player.Bet ) ||
                                        ( prev.Action == PlayerAction.Call && prev.Bet < player.Bet ) ||
                                        ( prev.Action == PlayerAction.Raise && prev.Bet < player.Bet ) )
                                        resetTurns = true;
                                }
                                else
                                    resetTurns = true;

                                player.IsAllIn = true;
                                player.Forced = false;
                            }

                            break;
                        }
                }

                if( resetTurns )
                {
                    Players.Turn.Clear();
                    Players.Push( player );
                }

                m_Timer.LastPlayer = null;
                m_Timer.HasWarned = false;

                if( Players.Turn.Count == Players.Round.Count )
                    State = (PokerGameState)( (int)State + 1 );
                else
                    AssignNextTurn();

                NeedsGumpUpdate = true;
            }
        }

        public void Begin()
        {
            Players.Clear();
            CurrentBet = 0;

            List<PokerPlayer> dispose = new List<PokerPlayer>();

            foreach( PokerPlayer player in Players.Players )
                if( player.RequestLeave || !player.IsOnline() )
                    dispose.Add( player );

            foreach( PokerPlayer player in dispose )
                if( Players.Contains( player ) )
                    RemovePlayer( player );

            foreach( PokerPlayer player in Players.Players )
            {
                player.ClearGame();
                player.Game = this;

                if( player.Gold >= Dealer.BigBlind && player.IsOnline() )
                    Players.Round.Add( player );
            }

            if( DealerButton == null ) //First round / more player
            {
                if( Players.Round.Count == 2 ) //Only use dealer button and small blind
                {
                    DealerButton = Players.Round[ 0 ];
                    SmallBlind = Players.Round[ 1 ];
                    BigBlind = null;
                }
                else if( Players.Round.Count > 2 )
                {
                    DealerButton = Players.Round[ 0 ];
                    SmallBlind = Players.Round[ 1 ];
                    BigBlind = Players.Round[ 2 ];
                }
                else
                    return;
            }
            else
            {
                if( Players.Round.Count == 2 ) //Only use dealer button and small blind
                {
                    if( DealerButton == Players.Round[ 0 ] )
                    {
                        DealerButton = Players.Round[ 1 ];
                        SmallBlind = Players.Round[ 0 ];
                    }
                    else
                    {
                        DealerButton = Players.Round[ 0 ];
                        SmallBlind = Players.Round[ 1 ];
                    }

                    BigBlind = null;
                }
                else if( Players.Round.Count > 2 )
                {
                    int index = Players.Round.IndexOf( DealerButton );

                    if( index == -1 ) //Old dealer button was lost :(
                    {
                        DealerButton = null;
                        Begin(); //Start over
                        return;
                    }

                    if( index == Players.Round.Count - 1 )
                    {
                        DealerButton = Players.Round[ 0 ];
                        SmallBlind = Players.Round[ 1 ];
                        BigBlind = Players.Round[ 2 ];
                    }
                    else if( index == Players.Round.Count - 2 )
                    {
                        DealerButton = Players.Round[ Players.Round.Count - 1 ];
                        SmallBlind = Players.Round[ 0 ];
                        BigBlind = Players.Round[ 1 ];
                    }
                    else if( index == Players.Round.Count - 3 )
                    {
                        DealerButton = Players.Round[ Players.Round.Count - 2 ];
                        SmallBlind = Players.Round[ Players.Round.Count - 1 ];
                        BigBlind = Players.Round[ 0 ];
                    }
                    else
                    {
                        DealerButton = Players.Round[ index + 1 ];
                        SmallBlind = Players.Round[ index + 2 ];
                        BigBlind = Players.Round[ index + 3 ];
                    }
                }
                else
                    return;
            }

            m_CommunityCards.Clear();
            Deck = new Deck();

            State = PokerGameState.DealHoleCards;

            if( BigBlind != null )
            {
                BigBlind.Gold -= Dealer.BigBlind;
                CommunityGold += Dealer.BigBlind;
                BigBlind.RoundGold = Dealer.BigBlind;
                BigBlind.RoundBet = Dealer.BigBlind;
                BigBlind.Bet = Dealer.BigBlind;
            }

            SmallBlind.Gold -= BigBlind == null ? Dealer.BigBlind : Dealer.SmallBlind;
            CommunityGold += BigBlind == null ? Dealer.BigBlind : Dealer.SmallBlind;
            SmallBlind.RoundGold = BigBlind == null ? Dealer.BigBlind : Dealer.SmallBlind;
            SmallBlind.RoundBet = BigBlind == null ? Dealer.BigBlind : Dealer.SmallBlind;
            SmallBlind.Bet = BigBlind == null ? Dealer.BigBlind : Dealer.SmallBlind;

            if( BigBlind != null )
            {
                //m_Players.Push( m_BigBlind );
                BigBlind.SetBBAction();
                CurrentBet = Dealer.BigBlind;
            }
            else
            {
                //m_Players.Push( m_SmallBlind );
                SmallBlind.SetBBAction();
                CurrentBet = Dealer.BigBlind;
            }

            if( Players.Next() == null )
                return;

            NeedsGumpUpdate = true;
            m_Timer = new PokerGameTimer( this );
            m_Timer.Start();
        }

        public void End()
        {
            State = PokerGameState.Inactive;

            foreach( PokerPlayer player in Players.Players )
            {
                player.CloseGump( typeof( PokerTableGump ) );
                player.SendGump( new PokerTableGump( this, player ) );
            }

            if( m_Timer.Running )
                m_Timer.Stop();
        }

        public void DealHoleCards()
        {
            for( int i = 0; i < 2; ++i ) //Simulate passing one card out at a time, going around the circle of players 2 times
                foreach( PokerPlayer player in Players.Round )
                    player.AddCard( Deck.Pop() );
        }

        public PokerPlayer AssignNextTurn()
        {
            PokerPlayer nextTurn = Players.Next();

            if( nextTurn == null )
                return null;

            if( nextTurn.RequestLeave )
            {
                Players.Push( nextTurn );
                nextTurn.BetStart = DateTime.Now;
                nextTurn.Action = PlayerAction.Fold;
                return nextTurn;
            }

            if( nextTurn.IsAllIn )
            {
                Players.Push( nextTurn );
                nextTurn.BetStart = DateTime.Now;
                nextTurn.Action = PlayerAction.AllIn;
                return nextTurn;
            }

            if( nextTurn.LonePlayer )
            {
                Players.Push( nextTurn );
                nextTurn.BetStart = DateTime.Now;
                nextTurn.Action = PlayerAction.Check;
                return nextTurn;
            }

            bool canCall = false;

            PokerPlayer currentTurn = Players.Peek();

            if( currentTurn != null && currentTurn.Action != PlayerAction.Check && currentTurn.Action != PlayerAction.Fold )
                canCall = true;
            if( currentTurn == null && State == PokerGameState.PreFlop )
                canCall = true;

            Players.Push( nextTurn );
            nextTurn.BetStart = DateTime.Now;

            ResultEntry entry = new ResultEntry( nextTurn );
            List<Card> bestCards;

            entry.Rank = nextTurn.GetBestHand( m_CommunityCards, out bestCards );
            entry.BestCards = bestCards;

            nextTurn.SendMessage( 0x22, String.Format( "You have {0}.", HandRanker.RankString( entry ) ) );
            nextTurn.CloseGump( typeof( PokerBetGump ) );
            nextTurn.SendGump( new PokerBetGump( this, nextTurn, canCall ) );

            NeedsGumpUpdate = true;

            return nextTurn;
        }

        public List<PokerPlayer> GetWinners( bool silent )
        {
            List<ResultEntry> results = new List<ResultEntry>();

            foreach( PokerPlayer t in Players.Round )
            {
                ResultEntry entry = new ResultEntry( t );
                List<Card> bestCards;

                entry.Rank = HandRanker.GetBestHand( entry.Player.GetAllCards( m_CommunityCards ), out bestCards );
                entry.BestCards = bestCards;

                results.Add( entry );

                /*if ( !silent )
                {
                    //Check if kickers needed
                    PokerMessage( entry.Player.Mobile, String.Format( "I have {0}.", HandRanker.RankString( entry ) ) );
                }*/
            }

            results.Sort();

            if( results.Count < 1 )
                return null;

            List<PokerPlayer> winners = new List<PokerPlayer>();

            foreach( ResultEntry t in results )
                if( HandRanker.IsBetterThan( t, results[ 0 ] ) == RankResult.Same )
                    winners.Add( t.Player );

            if( !silent )
            {
                //Only hands that have made it past the showdown may be considered for the jackpot
                foreach( ResultEntry t in results )
                {
                    if( winners.Contains( t.Player ) )
                    {
                        if( PokerDealer.JackpotWinners != null )
                        {
                            if( HandRanker.IsBetterThan( t, PokerDealer.JackpotWinners.Hand ) == RankResult.Better )
                            {
                                PokerDealer.JackpotWinners = null;
                                PokerDealer.JackpotWinners = new PokerDealer.JackpotInfo( winners, t, DateTime.Now );

                                break;
                            }
                        }
                        else
                        {
                            PokerDealer.JackpotWinners = new PokerDealer.JackpotInfo( winners, t, DateTime.Now );
                            break;
                        }
                    }
                }

                results.Reverse();

                foreach( ResultEntry entry in results )
                {
                    //if ( !winners.Contains( entry.Player ) )
                    PokerMessage( entry.Player.Owner, String.Format( "I have {0}.", HandRanker.RankString( entry ) ) );
                    /*else
                    {
                        if ( !HandRanker.UsesKicker( entry.Rank ) )
                            PokerMessage( entry.Player, String.Format( "I have {0}.", HandRanker.RankString( entry ) ) );
                        else //Hand rank uses a kicker
                        {
                            switch ( entry.Rank )
                            {
                            }
                        }
                    }*/
                }
            }

            return winners;
        }

        public void AwardPotToWinners( List<PokerPlayer> winners, bool silent )
        {
            //** Casino Rake - Will take a percentage of each pot awarded and place it towards
            //**				the casino jackpot for the highest ranked hand.

            if( !silent ) //Only rake pots that have made it past the showdown.
            {
                int rake = Math.Min( (int)( CommunityGold * Dealer.Rake ), Dealer.RakeMax );

                if( rake > 0 )
                {
                    CommunityGold -= rake;
                    PokerDealer.Jackpot += rake;
                }
            }

            int lowestBet = 0;

            foreach( PokerPlayer player in winners )
                if( player.RoundGold < lowestBet || lowestBet == 0 )
                    lowestBet = player.RoundGold;

            foreach( PokerPlayer player in Players.Round )
            {
                int diff = player.RoundGold - lowestBet;

                if( diff > 0 )
                {
                    player.Gold += diff;
                    CommunityGold -= diff;
                    PokerMessage( Dealer, String.Format( "{0}gp has been returned to {1}.", diff, player.Owner.Name ) );
                }
            }

            int splitPot = CommunityGold / winners.Count;

            foreach( PokerPlayer player in winners )
            {
                player.Gold += splitPot;
                PokerMessage( Dealer, String.Format( "{0} has won {1}gp.", player.Owner.Name, splitPot ) );
            }

            CommunityGold = 0;
        }

        public void DoShowdown( bool silent )
        {
            List<PokerPlayer> winners = GetWinners( silent );

            if( winners != null && winners.Count > 0 )
                AwardPotToWinners( winners, silent );

            End();

            Begin();
        }

        public void DoRoundAction() //Happens once State is changed (once per state)
        {
            if( State == PokerGameState.Showdown )
                DoShowdown( false );
            else if( State == PokerGameState.DealHoleCards )
            {
                DealHoleCards();
                State = PokerGameState.PreFlop;
                NeedsGumpUpdate = true;
            }
            else if( !IsBettingRound )
            {
                int numberOfCards = 0;
                string round = String.Empty;

                switch( State )
                {
                    case PokerGameState.Flop: numberOfCards += 3; round = "flop"; State = PokerGameState.PreTurn; break;
                    case PokerGameState.Turn: ++numberOfCards; round = "turn"; State = PokerGameState.PreRiver; break;
                    case PokerGameState.River: ++numberOfCards; round = "river"; State = PokerGameState.PreShowdown; break;
                }

                if( numberOfCards != 0 ) //Pop the appropriate number of cards from the top of the deck
                {
                    StringBuilder sb = new StringBuilder();

                    sb.Append( "The " + round + " shows: " );

                    for( int i = 0; i < numberOfCards; ++i )
                    {
                        Card popped = Deck.Pop();
                        if( i == 2 || numberOfCards == 1 )
                            sb.Append( popped.Name + "." );
                        else
                            sb.Append( popped.Name + ", " );

                        m_CommunityCards.Add( popped );
                    }

                    PokerMessage( Dealer, sb.ToString() );
                    Players.Turn.Clear();
                    //AssignNextTurn();
                    NeedsGumpUpdate = true;
                }
            }
            else
            {
                if( Players.Turn.Count == Players.Round.Count )
                {
                    switch( State )
                    {
                        case PokerGameState.PreFlop: State = PokerGameState.Flop; break;
                        case PokerGameState.PreTurn: State = PokerGameState.Turn; break;
                        case PokerGameState.PreRiver: State = PokerGameState.River; break;
                        case PokerGameState.PreShowdown: State = PokerGameState.Showdown; break;
                    }

                    //m_Players.Turn.Clear();
                }
                else if( Players.Turn.Count == 0 && State != PokerGameState.PreFlop ) //We need to initiate betting for this round
                {
                    ResetPlayerActions();
                    CheckLonePlayer();
                    AssignNextTurn();
                }
                else if( Players.Turn.Count == 0 && State == PokerGameState.PreFlop )
                {
                    CheckLonePlayer();
                    AssignNextTurn();
                }
            }
        }

        public void CheckLonePlayer()
        {
            int allInCount = 0;

            foreach( PokerPlayer t in Players.Round )
                if( t.IsAllIn )
                    ++allInCount;

            PokerPlayer loner = null;

            if( allInCount == Players.Round.Count - 1 )
                foreach( PokerPlayer t in Players.Round )
                    if( !t.IsAllIn )
                        loner = t;

            if( loner != null )
                loner.LonePlayer = true;
        }

        public void ResetPlayerActions()
        {
            for( int i = 0; i < Players.Count; ++i )
            {
                Players[ i ].Action = PlayerAction.None;
                Players[ i ].RoundBet = 0;
            }
        }

        public int GetIndexFor( Mobile from )
        {
            for( int i = 0; i < Players.Count; ++i )
                if( Players[ i ].Owner != null && from != null )
                    if( Players[ i ].Owner.Serial == from.Serial )
                        return i;

            return -1;
        }

        public PokerPlayer GetPlayer( Mobile from )
        {
            return GetIndexFor( from ) == -1 ? null : Players[ GetIndexFor( from ) ];
        }

        public int GetIndexForPlayerInRound( Mobile from )
        {
            for( int i = 0; i < Players.Round.Count; ++i )
                if( Players.Round[ i ].Owner != null && from != null )
                    if( Players.Round[ i ].Owner.Serial == from.Serial )
                        return i;

            return -1;
        }

        public void AddPlayer( PokerPlayer player )
        {
            Mobile from = player.Owner;

            if( from == null )
                return;

            if( !Dealer.InRange( from.Location, 8 ) )
                from.PrivateOverheadMessage( Network.MessageType.Regular, 0x22, true, "I am too far away to do that", from.NetState );
            else if( GetIndexFor( from ) != -1 )
                from.SendMessage( 0x22, "You are already seated at this table" );
            else if( Players.Count >= Dealer.MaxPlayers )
                from.SendMessage( 0x22, "Sorry, that table is full" );
            /*else if ( TournamentSystem.TournamentCore.SignedUpTeam( from ) != null || TournamentSystem.TournamentCore.FindTeam( from ) != null )
                from.SendMessage( 0x22, "You may not join a poker game while signed up for a tournament." );*/
            else if( Banker.Withdraw( from, player.Gold ) )
            {
                Point3D seat = Point3D.Zero;

                foreach( Point3D seats in Dealer.Seats )
                    if( !Dealer.SeatTaken( seats ) )
                    {
                        seat = seats;
                        break;
                    }

                if( seat == Point3D.Zero )
                {
                    from.SendMessage( 0x22, "Sorry, that table is full" );
                    return;
                }

                player.Game = this;
                player.Seat = seat;
                player.TeleportToSeat();
                Players.Players.Add( player );

                ( (Midgard2PlayerMobile)from ).PokerGame = this;
                from.SendMessage( 0x22, "You have been seated at the table" );

                if( Players.Count == 1 && !GameBackup.PokerGames.Contains( this ) )
                    GameBackup.PokerGames.Add( this );
                else if( State == PokerGameState.Inactive && Players.Count > 1 && !Dealer.TournamentMode )
                    Begin();
                else if( State == PokerGameState.Inactive && Players.Count >= Dealer.MaxPlayers && Dealer.TournamentMode )
                {
                    Dealer.TournamentMode = false;
                    Begin();
                }

                player.CloseGump( typeof( PokerTableGump ) );
                player.SendGump( new PokerTableGump( this, player ) );
                NeedsGumpUpdate = true;
            }
            else
                from.SendMessage( 0x22, "Your bank box lacks the funds to join this poker table" );
        }

        public void RemovePlayer( PokerPlayer player )
        {
            Mobile from = player.Owner;

            if( from != null && Players.Contains( player ) )
            {
                Players.Players.Remove( player );

                if( Players.Peek() == player ) //It is currently their turn, fold them.
                {
                    player.CloseGump( typeof( PokerBetGump ) );
                    m_Timer.LastPlayer = null;
                    player.Action = PlayerAction.Fold;
                }

                if( Players.Round.Contains( player ) )
                    Players.Round.Remove( player );
                if( Players.Turn.Contains( player ) )
                    Players.Turn.Remove( player );

                if( Players.Round.Count == 0 )
                {
                    player.Gold += CommunityGold;
                    CommunityGold = 0;

                    if( GameBackup.PokerGames.Contains( this ) )
                        GameBackup.PokerGames.Remove( this );
                }

                if( player.Gold > 0 )
                {
                    if( from.BankBox == null ) //Should NEVER happen, but JUST IN CASE!
                    {
                        Utility.PushColor( ConsoleColor.Red );
                        Console.WriteLine( "WARNING: Player \"{0}\" with account \"{1}\" had null bankbox while trying to deposit {2} gold. Player will NOT recieve their gold.", from.Name, ( from.Account == null ? "(-null-)" : from.Account.Username ), player.Gold );
                        Utility.PopColor();

                        try
                        {
                            using( System.IO.StreamWriter op = new System.IO.StreamWriter( "Logs/poker_error.log", true ) )
                                op.WriteLine( "WARNING: Player \"{0}\" with account \"{1}\" had null bankbox while poker script was trying to deposit {2} gold. Player will NOT recieve their gold.", from.Name, ( from.Account == null ? "(-null-)" : from.Account.Username ), player.Gold );
                        }
                        catch { }

                        from.SendMessage( 0x22, "WARNING: Could not find your bankbox. All of your poker money has been lost in this error. Please contact a Game Master to resolve this issue." );
                    }
                    else
                    {
                        Banker.Deposit( from.BankBox, player.Gold );
                        from.SendMessage( 0x22, "{0}gp has been deposited into your bankbox.", player.Gold );
                    }
                }

                player.CloseAllGumps();
                ( (Midgard2PlayerMobile)from ).PokerGame = null;
                from.Location = Dealer.ExitLocation;
                from.Map = Dealer.ExitMap;
                from.SendMessage( 0x22, "You have left the table" );

                NeedsGumpUpdate = true;
            }
        }
    }
}