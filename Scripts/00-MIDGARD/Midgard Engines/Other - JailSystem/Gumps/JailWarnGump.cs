using System.Collections.Generic;

using Server;
using Server.Accounting;
using Server.Gumps;
using Server.Network;

namespace Midgard.Engines.JailSystem
{
    public class JailWarnGump : Gump
    {
        private readonly Mobile m_BadBoy;
        private readonly string m_Reason = "Breaking Shard Rules";
        private readonly List<AccountComment> m_Warn;
        private int m_ID;

        public JailWarnGump( Mobile from, Mobile m, string why, int id, List<AccountComment> warnings )
            : base( 100, 40 )
        {
            from.CloseGump( typeof( JailWarnGump ) );
            if( string.IsNullOrEmpty( why ) )
                why = JailGump.Reasons[ 0 ];
            m_BadBoy = m;
            m_Reason = why;
            m_ID = id;
            Closable = true;
            Dragable = true;
            AddPage( 0 );
            AddBackground( 0, 0, 326, 320, 5054 );
            AddImageTiled( 9, 6, 308, 140, 2624 );
            AddAlphaRegion( 9, 6, 308, 140 );
            AddLabel( 16, 98, 200, "Reason" );
            AddBackground( 14, 114, 290, 24, 0x2486 );
            AddTextEntry( 18, 116, 282, 20, 200, 0, m_Reason );
            AddButton( 14, 11, 1209, 1210, 3, GumpButtonType.Reply, 0 );
            AddLabel( 30, 7, 200, JailGump.Reasons[ 0 ] );
            AddButton( 14, 29, 1209, 1210, 4, GumpButtonType.Reply, 0 );
            AddLabel( 30, 25, 200, JailGump.Reasons[ 1 ] );
            AddButton( 14, 47, 1209, 1210, 5, GumpButtonType.Reply, 0 );
            AddLabel( 30, 43, 200, JailGump.Reasons[ 2 ] );
            AddButton( 150, 11, 1209, 1210, 6, GumpButtonType.Reply, 0 );
            AddLabel( 170, 7, 200, JailGump.Reasons[ 3 ] );
            AddButton( 150, 29, 1209, 1210, 7, GumpButtonType.Reply, 0 );
            AddLabel( 170, 24, 200, JailGump.Reasons[ 4 ] );
            AddButton( 150, 47, 1209, 1210, 8, GumpButtonType.Reply, 0 );
            AddLabel( 170, 43, 200, JailGump.Reasons[ 5 ] );
            AddButton( 14, 66, 1209, 1210, 9, GumpButtonType.Reply, 0 );
            AddLabel( 30, 62, 200, JailGump.Reasons[ 6 ] );
            AddButton( 14, 84, 1209, 1210, 10, GumpButtonType.Reply, 0 );
            AddLabel( 30, 80, 200, JailGump.Reasons[ 7 ] );
            //warn button
            AddButton( 218, 152, 2472, 2473, 1, GumpButtonType.Reply, 0 );
            AddLabel( 248, 155, 200, "Warn them" );
            //Jail button
            AddButton( 20, 152, 2472, 2473, 2, GumpButtonType.Reply, 0 );
            AddLabel( 50, 155, 200, "Jail them" );
            //previous button
            AddButton( 10, 300, 2466, 2467, 20, GumpButtonType.Reply, 0 );
            //next Button
            AddButton( 90, 300, 2469, 2470, 21, GumpButtonType.Reply, 0 );
            if( warnings == null )
            {
                m_Warn = new List<AccountComment>();
                foreach( AccountComment note in ( (Account)m.Account ).Comments )
                {
                    if( ( note.AddedBy == JailSystem.JSName + "-warning" ) ||
                        ( note.AddedBy == JailSystem.JSName + "-jailed" ) )
                    {
                        m_Warn.Add( note );
                    }
                }
                m_ID = m_Warn.Count - 1;
            }
            else
            {
                m_Warn = warnings;
            }
            AddImageTiled( 9, 186, 308, 110, 2624 );
            AddAlphaRegion( 9, 186, 308, 110 );
            string temp = "No prior warnings.";
            if( m_Warn.Count > 0 )
            {
                if( m_ID < 0 )
                    m_ID = m_Warn.Count - 1;
                if( m_ID >= m_Warn.Count )
                    m_ID = 0;
                temp = ( m_Warn[ m_ID ] ).Content;
                AddLabel( 12, 190, 200, "Issued: " + ( m_Warn[ m_ID ] ).LastModified );
            }
            else
            {
                //no prior warning	
                m_ID = -1;
            }
            AddLabel( 12, 210, 200, "Event " + ( m_ID + 1 ) + " of " + m_Warn.Count + " warnings/Jailings" );
            //AddLabel( 12, 230, 200, temp );
            AddHtml( 12, 230, 300, 62, temp, true, true );
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            Mobile from = sender.Mobile;
            switch( info.ButtonID )
            {
                case 20:
                    //previous button
                    m_ID--;
                    if( m_ID < 0 )
                        m_ID = m_Warn.Count - 1;
                    from.SendGump( new JailWarnGump( from, m_BadBoy, info.GetTextEntry( 0 ).Text, m_ID, m_Warn ) );
                    break;
                case 21:
                    //next button
                    m_ID++;
                    if( m_ID >= m_Warn.Count )
                        m_ID = 0;
                    from.SendGump( new JailWarnGump( from, m_BadBoy, info.GetTextEntry( 0 ).Text, m_ID, m_Warn ) );
                    break;
                //reason buttons
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                    from.SendGump( new JailWarnGump( from, m_BadBoy, JailGump.Reasons[ info.ButtonID - 3 ], m_ID, m_Warn ) );
                    break;
                case 1:
                    //warn them
                    from.CloseGump( typeof( JailWarnGump ) );
                    if( m_Reason == JailGump.Reasons[ 0 ] )
                    {
                        //they are macroing
                        JailSystem.MacroTest( from, m_BadBoy );
                    }
                    else
                    {
                        //not Unattended macroing
                        m_BadBoy.SendGump( new JailWarningGump( from, m_BadBoy, m_Reason ) );
                    }
                    break;
                case 2:
                    //jail them
                    from.CloseGump( typeof( JailWarnGump ) );
                    from.SendGump( new JailGump( JailSystem.Lockup( m_BadBoy ), from, m_BadBoy, 0, "", m_Reason, "0", "0",
                                                 "1", "0", "0", true ) );
                    break;
                default:
                    break;
            }
        }
    }
}