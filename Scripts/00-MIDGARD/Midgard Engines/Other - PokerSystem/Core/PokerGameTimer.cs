using System;

namespace Server.Poker
{
    public class PokerGameTimer : Timer
    {
        private readonly PokerGame m_Game;
        private PokerGameState m_LastState;

        public PokerPlayer LastPlayer;
        public bool HasWarned;

        public PokerGameTimer( PokerGame game )
            : base( TimeSpan.FromSeconds( 1.0 ), TimeSpan.FromSeconds( 1.0 ) )
        {
            m_Game = game;
            m_LastState = PokerGameState.Inactive;

            LastPlayer = null;
        }

        protected override void OnTick()
        {
            if( m_Game.State != PokerGameState.Inactive && m_Game.Players.Count < 2 )
                m_Game.End();

            for( int i = 0; i < m_Game.Players.Count; ++i )
                if( !m_Game.Players.Round.Contains( m_Game.Players[ i ] ) )
                    if( m_Game.Players[ i ].RequestLeave )
                        m_Game.RemovePlayer( m_Game.Players[ i ] );

            if( m_Game.NeedsGumpUpdate )
            {
                foreach( PokerPlayer player in m_Game.Players.Players )
                {
                    player.CloseGump( typeof( PokerTableGump ) );
                    player.SendGump( new PokerTableGump( m_Game, player ) );
                }

                m_Game.NeedsGumpUpdate = false;
            }

            if( m_Game.State != m_LastState && m_Game.Players.Round.Count > 1 )
            {
                m_LastState = m_Game.State;
                m_Game.DoRoundAction();
                LastPlayer = null;
            }

            if( m_Game.Players.Peek() == null )
                return;

            if( LastPlayer == null )
                LastPlayer = m_Game.Players.Peek(); //Changed timer from 25.0 and 30.0 to 45.0 and 60.0

            if( LastPlayer.BetStart.AddSeconds( 45.0 ) <= DateTime.Now /*&& m_LastPlayer.Mobile.HasGump( typeof( PokerBetGump ) )*/ && !HasWarned )
            {
                LastPlayer.SendMessage( 0x22, "You have 15 seconds left to make a choice. (You will automatically fold if no choice is made)" );
                HasWarned = true;
            }
            else if( LastPlayer.BetStart.AddSeconds( 60.0 ) <= DateTime.Now /*&& m_LastPlayer.Mobile.HasGump( typeof( PokerBetGump ) )*/ )
            {
                PokerPlayer temp = LastPlayer;
                LastPlayer = null;

                temp.CloseGump( typeof( PokerBetGump ) );
                temp.Action = PlayerAction.Fold;
                HasWarned = false;
            }
        }
    }
}