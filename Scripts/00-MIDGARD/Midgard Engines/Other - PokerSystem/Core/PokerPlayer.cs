using System;
using System.Collections.Generic;

using Server.Gumps;

namespace Server.Poker
{
    public class PokerPlayer
    {
        private PlayerAction m_Action;

        public int Gold { get; set; }
        public int Bet { get; set; }
        public int RoundGold { get; set; }
        public int RoundBet { get; set; }
        public bool RequestLeave { get; set; }
        public bool IsAllIn { get; set; }
        public bool Forced { get; set; }
        public bool LonePlayer { get; set; }
        public Mobile Owner { get; set; }
        public PokerGame Game { get; set; }
        public Point3D Seat { get; set; }
        public DateTime BetStart { get; set; }
        public List<Card> HoleCards { get; private set; }

        public PlayerAction Action
        {
            get { return m_Action; }
            set
            {
                m_Action = value;

                switch( m_Action )
                {
                    case PlayerAction.None:
                        break;
                    default:
                        if( Game != null )
                            Game.PokerGame_PlayerMadeDecision( this );

                        break;
                }
            }
        }

        public bool HasDealerButton
        {
            get { return ( Game.DealerButton == this ); }
        }

        public bool HasSmallBlind
        {
            get { return ( Game.SmallBlind == this ); }
        }

        public bool HasBigBlind
        {
            get { return ( Game.BigBlind == this ); }
        }

        public bool HasBlindBet
        {
            get { return ( Game.SmallBlind == this || Game.BigBlind == this ); }
        }

        public PokerPlayer( Mobile from )
        {
            Owner = from;
            HoleCards = new List<Card>();
        }

        public void ClearGame()
        {
            Bet = 0;
            RoundGold = 0;
            RoundBet = 0;
            HoleCards.Clear();
            Game = null;
            CloseAllGumps();
            m_Action = PlayerAction.None;
            IsAllIn = false;
            Forced = false;
            LonePlayer = false;
        }

        public void AddCard( Card card )
        {
            HoleCards.Add( card );
        }

        public void SetBBAction()
        {
            m_Action = PlayerAction.Bet;
        }

        public HandRank GetBestHand( List<Card> communityCards, out List<Card> bestCards )
        {
            return HandRanker.GetBestHand( GetAllCards( communityCards ), out bestCards );
        }

        public List<Card> GetAllCards( List<Card> communityCards )
        {
            List<Card> hand = new List<Card>( communityCards );
            hand.AddRange( HoleCards );
            hand.Sort();
            return hand;
        }

        public void CloseAllGumps()
        {
            CloseGump( typeof( PokerTableGump ) );
            CloseGump( typeof( PokerLeaveGump ) );
            CloseGump( typeof( PokerJoinGump ) );
            CloseGump( typeof( PokerBetGump ) );
        }

        public void CloseGump( Type type )
        {
            if( Owner != null )
                Owner.CloseGump( type );
        }

        public void SendGump( Gump toSend )
        {
            if( Owner != null )
                Owner.SendGump( toSend );
        }

        public void SendMessage( string message )
        {
            if( Owner != null )
                Owner.SendMessage( message );
        }

        public void SendMessage( int hue, string message )
        {
            if( Owner != null )
                Owner.SendMessage( hue, message );
        }

        public void TeleportToSeat()
        {
            if( Owner != null && Seat != Point3D.Zero )
                Owner.Location = Seat;
        }

        public bool IsOnline()
        {
            if( Owner != null )
            {
                if( Owner.NetState != null )
                {
                    if( Owner.NetState.Socket != null )
                    {
                        if( Owner.NetState.Socket.Connected )
                            return true;
                    }
                }
            }

            return false;
        }
    }
}