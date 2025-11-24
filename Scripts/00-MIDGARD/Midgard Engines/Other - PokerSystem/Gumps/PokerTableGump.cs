using System;

using Server.Gumps;

namespace Server.Poker
{
    public class PokerTableGump : Gump
    {
        private const int Red = 38;
        private const int Black = 0;
        private const int CardX = 300;
        private const int CardY = 270;

        private const int ColorWhite = 0xFFFFFF;
        private const int ColorYellow = 0xFFFF00;
        private const int ColorGold = 0xFFD700;
        private const int ColorBlack = 0x111111;
        private const int ColorGreen = 0x00FF00;
        private const int ColorOffWhite = 0xFFFACD;
        private const int ColorPink = 0xFF0099;

        private readonly PokerGame m_Game;
        private readonly PokerPlayer m_Player;

        public PokerTableGump( PokerGame game, PokerPlayer player )
            : base( 0, 0 )
        {
            m_Game = game;
            m_Player = player;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;
            AddPage( 0 );

            if( m_Game.State > PokerGameState.PreFlop )
                DrawCards();

            DrawPlayers();

            if( m_Game.State > PokerGameState.Inactive )
                AddLabel( 350, 340, 148, "Pot: " + m_Game.CommunityGold.ToString( "#,###" ) );
        }

        private void DrawPlayers()
        {
            int RADIUS = 240;
            int centerX = CardX + ( m_Game.State < PokerGameState.Turn ? 15 : m_Game.State < PokerGameState.River ? 30 : 45 );
            int centerY = CardY + RADIUS;

            if( m_Game.State > PokerGameState.DealHoleCards )
            {
                int lastX = centerX;
                int lastY = centerY - 85;

                foreach( Card t in m_Player.HoleCards )
                {
                    AddBackground( lastX, lastY, 71, 95, 9350 );
                    AddLabelCropped( lastX + 10, lastY + 5, 80, 60, t.GetSuitColor(), t.GetRankLetter() );
                    AddLabelCropped( lastX + 6, lastY + 25, 75, 60, t.GetSuitColor(), t.GetSuitString() );

                    lastX += 30;
                }
            }

            int playerIndex = m_Game.GetIndexFor( m_Player.Owner );
            int counter = m_Game.Players.Count - 1;

            for( double i = playerIndex + 1; counter >= 0; ++i )
            {
                if( i == m_Game.Players.Count )
                    i = 0;

                PokerPlayer current = m_Game.Players[ (int)i ];
                double xdist = RADIUS * Math.Sin( counter * 2.0 * Math.PI / m_Game.Players.Count );
                double ydist = RADIUS * Math.Cos( counter * 2.0 * Math.PI / m_Game.Players.Count );

                int x = centerX + (int)xdist;
                int y = CardY + (int)ydist;

                AddBackground( x, y, 101, 65, 9270 ); //changed from 9200.  This is the gump that shows your name and gold left.

                if( current.HasBlindBet || current.HasDealerButton )
                    AddHtml( x, y - 15, 101, 45, Color( Center( current.HasBigBlind ? "(Big Blind)" : current.HasSmallBlind ? "(Small Blind)" : "(Dealer Button)" ), ColorGreen ), false, false ); // changed from COLOR_YELLOW

                AddHtml( x, y + 5, 101, 45, Color( Center( current.Owner.Name ), ( m_Game.Players.Peek() == current ? ColorGreen : !m_Game.Players.Round.Contains( current ) ? ColorOffWhite : ColorPink ) ), false, false );
                AddHtml( x + 2, y + 24, 101, 45, Color( Center( "(" + current.Gold.ToString( "#,###" ) + ")" ), ColorGold ), false, false );

                --counter;
            }
        }

        private void DrawCards()
        {
            int lastX = CardX;
            int lastY = CardY;

            foreach( Card t in m_Game.CommunityCards )
            {
                AddBackground( lastX, lastY, 71, 95, 9350 );
                AddLabelCropped( lastX + 10, lastY + 5, 80, 60, t.GetSuitColor(), t.GetRankLetter() );
                AddLabelCropped( lastX + 6, lastY + 25, 75, 60, t.GetSuitColor(), t.GetSuitString() );

                lastX += 30;
            }
        }

        private static string Center( string text )
        {
            return String.Format( "<CENTER>{0}</CENTER>", text );
        }

        private static string Color( string text, int color )
        {
            return String.Format( "<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", color, text );
        }
    }
}