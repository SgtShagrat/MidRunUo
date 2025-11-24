using System;
using Server;
using Server.Gumps;
using Server.Network;

namespace Midgard.Engines.ChessSystem
{
    /// <summary>
    /// Summary description for ChessSetGump.
    /// </summary>
    public class ChessSetGump : Gump
    {
        private const int LabelHue = 0x480;
        private const int GreenHue = 0x40;
        private static string[] m_Sets;

        /// <summary>
        /// Gets the list of chess sets available
        /// </summary>
        private static string[] Sets
        {
            get
            {
                if( m_Sets == null )
                    m_Sets = Enum.GetNames( typeof( ChessSet ) );

                return m_Sets;
            }
        }

        private ChessGame m_Game;
        private Mobile m_User;
        private bool m_IsOwner;
        private bool m_AllowSpectators;
        private int m_Page;

        public ChessSetGump( Mobile m, ChessGame game, bool isOwner, bool allowSpectators, int page )
            : base( 200, 200 )
        {
            m_Game = game;
            m_User = m;
            m_IsOwner = isOwner;
            m_AllowSpectators = allowSpectators;
            m_Page = page;

            m_User.CloseGump( typeof( ChessSetGump ) );

            MakeGump();
        }

        public ChessSetGump( Mobile m, ChessGame game, bool isOwner, bool allowSpectators )
            : this( m, game, isOwner, allowSpectators, 0 )
        {
        }

        private void MakeGump()
        {
            Closable = false;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddPage( 0 );
            AddBackground( 0, 0, 320, 170, 9250 );
            AddAlphaRegion( 0, 0, 320, 170 );

            for( int i = 0; i < 4 && 4 * m_Page + i < Sets.Length; i++ )
            {
                AddButton( 35, 45 + i * 20, 5601, 5605, 10 + 4 * m_Page + i, GumpButtonType.Reply, 0 );
                AddLabel( 60, 43 + i * 20, LabelHue, Sets[ 4 * m_Page + i ] );
            }

            if( m_Page > 0 )
            {
                AddButton( 15, 15, 5603, 5607, 1, GumpButtonType.Reply, 0 );
            }

            int totalPages = ( Sets.Length - 1 ) / 4;

            // Prev page : 1
            // Next page : 2

            if( totalPages > m_Page )
            {
                AddButton( 35, 15, 5601, 5605, 2, GumpButtonType.Reply, 0 );
            }

            AddLabel( 60, 13, GreenHue, "Chess set selection" );

            // Cancel 3
            AddButton( 15, 130, 4020, 4021, 3, GumpButtonType.Reply, 0 );
            AddLabel( 55, 130, GreenHue, "Cancel Game" );
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            switch( info.ButtonID )
            {
                case 0:
                    return;

                case 1:
                    sender.Mobile.SendGump( new ChessSetGump( m_User, m_Game, m_IsOwner, m_AllowSpectators, --m_Page ) );
                    break;

                case 2:
                    sender.Mobile.SendGump( new ChessSetGump( m_User, m_Game, m_IsOwner, m_AllowSpectators, ++m_Page ) );
                    break;

                case 3:
                    m_Game.CancelGameStart( sender.Mobile );
                    break;

                default:
                    int index = info.ButtonID - 10;

                    ChessSet s = (ChessSet)Enum.Parse( typeof( ChessSet ), Sets[ index ], false );
                    m_Game.SetChessSet( s );
                    sender.Mobile.SendGump( new StartGameGump( sender.Mobile, m_Game, m_IsOwner, m_AllowSpectators ) );
                    sender.Mobile.Target = new ChessTarget( m_Game, sender.Mobile, "Please select your parnter...",
                                                           new ChessTargetCallback( m_Game.ChooseOpponent ) );
                    break;
            }
        }
    }
}