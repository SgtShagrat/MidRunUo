using System.Collections.Generic;

using Server;
using Server.Accounting;
using Server.Gumps;
using Server.Network;

namespace Midgard.Engines.JailSystem
{
    public class JailReviewGump : Gump
    {
        private Mobile m_BadBoy;
        //		private string m_reason="Breaking Shard Rules";
        private bool m_DisplayReleases;
        private int m_ID;
        private List<AccountComment> m_Warn;

        public JailReviewGump( Mobile from, Mobile m )
            : base( 1, 30 )
        {
            Buildit( from, m, 0, null, "" );
        }

        public JailReviewGump( Mobile from, Mobile m, int id, List<AccountComment> warnings )
            : base( 1, 30 )
        {
            Buildit( from, m, id, warnings, "" );
        }

        public JailReviewGump( Mobile from, Mobile m, int id, List<AccountComment> warnings, string note )
            : base( 1, 30 )
        {
            Buildit( from, m, id, warnings, note );
        }

        public JailReviewGump( Mobile from, Mobile m, int id, List<AccountComment> warnings, bool showRelease )
            : base( 1, 30 )
        {
            Buildit( from, m, id, warnings, "", true, showRelease );
        }

        public JailReviewGump( Mobile from, Mobile m, int id, List<AccountComment> warnings, string note,
                               bool showRelease )
            : base( 1, 30 )
        {
            Buildit( from, m, id, warnings, note, true, showRelease );
        }

        public void Buildit( Mobile from, Mobile m, int id, List<AccountComment> warnings, string aNote )
        {
            Buildit( from, m, id, warnings, aNote, true, false );
        }

        public void Buildit( Mobile from, Mobile m, int id, List<AccountComment> warnings, string aNote, bool tGo,
                             bool showRelease )
        {
            m_DisplayReleases = showRelease;
            from.CloseGump( typeof( JailReviewGump ) );
            m_BadBoy = m;
            m_ID = id;
            Closable = true;
            Dragable = true;
            AddPage( 0 );
            AddBackground( 0, 0, 326, 230, 5054 );
            AddLabel( 12, 4, 200, "Reviewing: " + m_BadBoy.Name + " (" + ( (Account)m_BadBoy.Account ).Username + ")" );
            if( tGo )
            {
                AddLabel( 300, 17, 200, "GO" );
                AddButton( 280, 20, 2223, 2224, 2, GumpButtonType.Reply, 0 );
            }
            AddLabel( 12, 200, 200, "Note" );
            AddBackground( 42, 198, 268, 24, 0x2486 );
            AddTextEntry( 46, 200, 250, 20, 200, 0, aNote );
            //add button
            AddButton( 70, 150, 2460, 2461, 1, GumpButtonType.Reply, 0 );
            //previous button
            AddButton( 120, 150, 2466, 2467, 20, GumpButtonType.Reply, 0 );
            //next Button
            AddButton( 200, 150, 2469, 2470, 21, GumpButtonType.Reply, 0 );
            //release toggle
            AddButton( 115, 167, m_DisplayReleases ? 2154 : 2151, m_DisplayReleases ? 2151 : 2154, 22, GumpButtonType.Reply,
                       0 );
            AddLabel( 147, 171, 200, "Show Releases" );
            if( warnings == null )
            {
                m_Warn = new List<AccountComment>();
                foreach( AccountComment note in ( (Account)m.Account ).Comments )
                {
                    if( ( note.AddedBy == JailSystem.JSName + "-warning" ) ||
                        ( note.AddedBy == JailSystem.JSName + "-jailed" ) ||
                        ( note.AddedBy == JailSystem.JSName + "-note" ) ||
                        ( ( m_DisplayReleases ) && ( ( note.AddedBy == JailSystem.JSName ) ) ) )
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
            AddImageTiled( 9, 36, 308, 110, 2624 );
            AddAlphaRegion( 9, 36, 308, 110 );
            string temp = "No prior warnings.";
            if( m_Warn.Count > 0 )
            {
                if( m_ID < 0 )
                    m_ID = m_Warn.Count - 1;
                if( m_ID >= m_Warn.Count )
                    m_ID = 0;
                temp = ( m_Warn[ m_ID ] ).Content;
                if( ( m_Warn[ m_ID ] ).AddedBy == JailSystem.JSName + "-warning" )
                    AddLabel( 12, 40, 53, "Warned" );
                else if( ( m_Warn[ m_ID ] ).AddedBy == JailSystem.JSName + "-jailed" )
                    AddLabel( 12, 40, 38, "Jailed" );
                else if( ( m_Warn[ m_ID ] ).AddedBy == JailSystem.JSName + "-note" )
                    AddLabel( 12, 40, 2, "Note" );
                else
                    AddLabel( 12, 40, 2, "Release" );
                AddLabel( 60, 40, 200, "Issued: " + ( m_Warn[ m_ID ] ).LastModified );
            }
            else
            {
                //no prior warning	
                m_ID = -1;
            }
            AddLabel( 12, 60, 200, "Event " + ( m_ID + 1 ) + " of " + m_Warn.Count );
            //AddLabel( 12, 230, 200, temp );
            AddHtml( 12, 80, 300, 62, temp, true, true );
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            Mobile from = sender.Mobile;
            switch( info.ButtonID )
            {
                case 22:
                    m_DisplayReleases = ( !m_DisplayReleases );
                    from.SendGump( new JailReviewGump( from, m_BadBoy, 0, null, info.GetTextEntry( 0 ).Text,
                                                       m_DisplayReleases ) );
                    break;
                case 20:
                    //previous button
                    m_ID--;
                    if( m_ID < 0 )
                        m_ID = m_Warn.Count - 1;
                    from.SendGump( new JailReviewGump( from, m_BadBoy, m_ID, m_Warn, info.GetTextEntry( 0 ).Text,
                                                       m_DisplayReleases ) );
                    break;
                case 21:
                    //next button
                    m_ID++;
                    if( m_ID >= m_Warn.Count )
                        m_ID = 0;
                    from.SendGump( new JailReviewGump( from, m_BadBoy, m_ID, m_Warn, info.GetTextEntry( 0 ).Text,
                                                       m_DisplayReleases ) );
                    break;
                //reason buttons
                case 2:
                    from.SendGump( new JailReviewGump( from, m_BadBoy, m_ID, m_Warn, info.GetTextEntry( 0 ).Text,
                                                       m_DisplayReleases ) );
                    from.Hidden = true;
                    from.Location = m_BadBoy.Location;
                    from.Map = m_BadBoy.Map == Map.Internal ? Map.Felucca : m_BadBoy.Map;
                    break;
                case 1:
                    if( info.GetTextEntry( 0 ).Text != "note added" )
                    {
                        ( (Account)m_BadBoy.Account ).Comments.Add( new AccountComment( JailSystem.JSName + "-note",
                                                                                      info.GetTextEntry( 0 ).Text +
                                                                                      " by: " + from.Name ) );
                        from.SendGump( new JailReviewGump( from, m_BadBoy, 0, null, "note added", m_DisplayReleases ) );
                    }
                    else
                        from.SendGump( new JailReviewGump( from, m_BadBoy, 0, null, "", m_DisplayReleases ) );
                    break;
                default:
                    break;
            }
        }
    }
}