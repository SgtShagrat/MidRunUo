using System;

using Server;
using Server.Accounting;
using Server.Gumps;
using Server.Network;

namespace Midgard.Engines.JailSystem
{
    public class JailAdminGump : Gump
    {
        public enum AdminJailGumpPage
        {
            General,
            OOC,
            Language,
            Review
        }

        private const int BodyX = 5;
        private const int BodyY = 111;
        private const int GutterOffset = 3;
        private const int LabelColor32 = 0xFFFFFF;
        private const int LineStep = 25;
        private const int MessageX = 5;
        private const int MessageY = 387;
        private const int SelectedColor32 = 0x8080FF;
        private const int TitleX = 210;
        private const int TitleY = 7;

        private JailSystem m_Js;
        private int m_ID;
        private AdminJailGumpPage m_Page = AdminJailGumpPage.General;
        private int m_Subpage;

        public JailAdminGump()
            : base( 10, 30 )
        {
            Buildit( AdminJailGumpPage.Review, 0, 0 );
        }

        public JailAdminGump( AdminJailGumpPage page )
            : base( 10, 30 )
        {
            Buildit( page, 0, 0 );
        }

        public JailAdminGump( AdminJailGumpPage page, int subpage, int id )
            : base( 10, 30 )
        {
            Buildit( page, subpage, id );
        }

        public void AddTextField( int x, int y, int width, int height, int index )
        {
            AddTextField( x, y, width, height, index, "" );
        }

        public void AddTextField( int x, int y, int width, int height, int index, string content )
        {
            AddBackground( x - 2, y - 2, width + 4, height + 4, 0x2486 );
            AddTextEntry( x + 2, y + 2, width - 4, height - 4, 0, index, content );
        }

        public string Color( string text, int color )
        {
            return String.Format( "<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", color, text );
        }

        public void AddButtonLabeled( int x, int y, int buttonID, string text )
        {
            AddButton( x, y - 1, 4005, 4007, buttonID, GumpButtonType.Reply, 0 );
            AddHtml( x + 35, y, 240, 20, Color( text, LabelColor32 ), false, false );
        }

        public void AddToggleLabeled( int x, int y, int buttonID, string text, bool selected )
        {
            AddButton( x, y - 1, selected ? 2154 : 2152, selected ? 2152 : 2154, buttonID, GumpButtonType.Reply, 0 );
            AddHtml( x + 35, y, 240, 20, Color( text, LabelColor32 ), false, false );
        }

        public void AddPageLabeled( int x, int y, int buttonID, string text, AdminJailGumpPage page )
        {
            AddButton( x, y - 1, ( m_Page == page ) ? 4006 : 4005, 4007, buttonID, GumpButtonType.Reply, 0 );
            AddHtml( x + 35, y, 240, 20,
                     ( m_Page == page ) ? Color( text, LabelColor32 ) : Color( text, SelectedColor32 ), false, false );
        }

        public void AddBlackAlpha( int x, int y, int width, int height )
        {
            AddImageTiled( x, y, width, height, 2624 );
            AddAlphaRegion( x, y, width, height );
        }

        public void Message( string text )
        {
            AddColorLabel( MessageX + GutterOffset, MessageY + GutterOffset, 390, 40, text );
        }

        public void Message()
        {
            Message( "Settings changes will be saved during the world save." );
        }

        private static int Basex( int x )
        {
            return x + GutterOffset;
        }

        private static int Basey( int y, int lines )
        {
            return ( y + GutterOffset ) + ( lines * LineStep );
        }

        public void AddColorLabel( int x, int y, string text )
        {
            AddColorLabel( x, y, 240, 20, text );
        }

        public void AddColorLabel( int x, int y, int width, int height, string text )
        {
            AddHtml( x, y, width, height, Color( text, LabelColor32 ), false, false );
        }

        public void AddColorLabelScroll( int x, int y, int width, int height, string text )
        {
            AddHtml( x, y, width, height, Color( text, LabelColor32 ), true, true );
        }

        private void Buildit( AdminJailGumpPage page, int subpage, int id )
        {
            Closable = true;
            Dragable = true;
            m_ID = id;
            m_Page = page;
            m_Subpage = subpage;
            AddPage( 0 );

            AddBackground( 0, 0, 412, 439, 5054 );
            AddBlackAlpha( 5, 8, 200, 98 );
            AddBlackAlpha( TitleX, TitleY, 190, 98 );
            AddBlackAlpha( BodyX, BodyY, 396, 271 );
            AddBlackAlpha( MessageX, MessageY, 396, 46 );
            AddPageLabeled( 7, 12, 4, "Review Current Jailings", AdminJailGumpPage.Review );
            AddPageLabeled( 7, 36, 3, "Language Settings", AdminJailGumpPage.Language );
            AddPageLabeled( 7, 59, 2, "OOC Settings", AdminJailGumpPage.OOC );
            AddPageLabeled( 7, 82, 1, "General Settings", AdminJailGumpPage.General );

            AddButton( TitleX + 120, TitleY + 75, 241, 243, 5, GumpButtonType.Reply, 0 );
            switch( m_Page )
            {
                case AdminJailGumpPage.Review:
                    BuildReviews();
                    break;
                case AdminJailGumpPage.General:
                    BuildSettings();
                    break;
                case AdminJailGumpPage.Language:
                    BuildLanguage();
                    break;
                case AdminJailGumpPage.OOC:
                    BuildOOC();
                    break;
                default:
                    break;
            }
        }

        private void BuildLanguage()
        {
            AddToggleLabeled( Basex( TitleX ), Basey( TitleY, 0 ), 13, "Use Language", JailSystem.UseLanguageFilter );
            Message();
            if( !JailSystem.UseLanguageFilter )
                return;
            AddButton( TitleX + 50, TitleY + 75, 239, 240, 15, GumpButtonType.Reply, 0 );
            AddLabel( Basex( BodyX ), Basey( BodyY, 0 ), 200, "Misc." );
            AddLabel( Basex( BodyX ) + 15, Basey( BodyY, 1 ), 200, "Foul Jailor" );
            AddTextField( Basex( BodyX ) + 80, Basey( BodyY, 1 ), 150, 20, 12, JailSystem.FoulJailorName );
            AddToggleLabeled( Basex( BodyX ) + 15, Basey( BodyY, 2 ), 14, "Allow Staff to use bad words",
                              JailSystem.AllowStaffBadWords );

            AddLabel( Basex( BodyX ) + 240, Basey( BodyY, 0 ), 200, "Bad words" );
            string temp = "";
            foreach( string p in JailSystem.BadWords )
                temp += string.Format( "{0}\n", p );
            AddColorLabelScroll( Basex( BodyX ) + 240, Basey( BodyY, 1 ), 150, 60, temp.Trim() );

            AddTextField( Basex( BodyX ) + 240, Basey( BodyY, 1 ) + 65, 150, 20, 13 );
            AddButton( BodyX + 240, Basey( BodyY, 1 ) + 90, 2461, 2462, 26, GumpButtonType.Reply, 0 );
            AddButton( BodyX + 295, Basey( BodyY, 1 ) + 90, 2464, 2465, 27, GumpButtonType.Reply, 0 );

            AddLabel( Basex( BodyX ) + 240, Basey( BodyY, 5 ) + 10, 200, "Jail Terms" );
            temp = "";
            foreach( TimeSpan t in JailSystem.FoulMouthJailTimes )
                temp += string.Format( "d={0} h={1} m={2}\n", t.Days, t.Hours, t.Minutes );
            AddColorLabelScroll( Basex( BodyX ) + 240, Basey( BodyY, 6 ) + 10, 150, 60, temp.Trim() );

            AddLabel( Basex( BodyX ) + 240, Basey( BodyY, 6 ) + 75, 200, "D" );
            AddLabel( Basex( BodyX ) + 290, Basey( BodyY, 6 ) + 75, 200, "H" );
            AddLabel( Basex( BodyX ) + 340, Basey( BodyY, 6 ) + 75, 200, "M" );
            AddTextField( Basex( BodyX ) + 255, Basey( BodyY, 6 ) + 75, 30, 20, 8 );
            AddTextField( Basex( BodyX ) + 305, Basey( BodyY, 6 ) + 75, 30, 20, 9 );
            AddTextField( Basex( BodyX ) + 355, Basey( BodyY, 6 ) + 75, 30, 20, 10 );
            AddButton( BodyX + 240, Basey( BodyY, 6 ) + 100, 2461, 2462, 28, GumpButtonType.Reply, 0 );
            AddButton( BodyX + 295, Basey( BodyY, 6 ) + 100, 2464, 2465, 29, GumpButtonType.Reply, 0 );
        }

        private void BuildOOC()
        {
            AddToggleLabeled( Basex( TitleX ), Basey( TitleY, 0 ), 9, "Use OOC Filter", JailSystem.UseOOCFilter );
            Message();
            if( !JailSystem.UseOOCFilter )
                return;
            AddButton( TitleX + 50, TitleY + 75, 239, 240, 10, GumpButtonType.Reply, 0 );
            //AddLabel(Basex( BodyX),Basey(BodyY,0),200,"Commands");
            AddLabel( Basex( BodyX ) + 15, Basey( BodyY, 0 ), 200, "OOCList" );
            AddTextField( Basex( BodyX ) + 90, Basey( BodyY, 0 ), 130, 20, 11, JailSystem.OoclistCommand );
            //AddLabel(Basex( BodyX),Basey(BodyY,2),200,"Misc.");
            AddLabel( Basex( BodyX ) + 15, Basey( BodyY, 1 ), 200, "OOC Jailor" );
            AddTextField( Basex( BodyX ) + 90, Basey( BodyY, 1 ), 130, 20, 12, JailSystem.OocJailorName );
            AddToggleLabeled( Basex( BodyX ) + 15, Basey( BodyY, 2 ), 11, "Block OOC speech", JailSystem.BlockOOCSpeech );
            AddToggleLabeled( Basex( BodyX ) + 15, Basey( BodyY, 3 ), 12, "Allow Staff to go OOC",
                              JailSystem.AllowStaffOOC );
            AddLabel( Basex( BodyX ) + 15, Basey( BodyY, 4 ), 200, "OOC Warnings" );
            AddTextField( Basex( BodyX ) + 120, Basey( BodyY, 4 ), 50, 20, 13, JailSystem.Oocwarns.ToString() );

            AddLabel( Basex( BodyX ), Basey( BodyY, 5 ), 200, "OOC Parts" );
            string temp = "";
            foreach( string p in JailSystem.OOCParts )
                temp += string.Format( "{0}\n", p );
            AddColorLabelScroll( Basex( BodyX ), Basey( BodyY, 6 ), 150, 60, temp.Trim() );

            AddTextField( Basex( BodyX ), Basey( BodyY, 6 ) + 65, 150, 20, 15 );
            AddButton( Basex( BodyX ), Basey( BodyY, 6 ) + 90, 2461, 2462, 34, GumpButtonType.Reply, 0 );
            AddButton( Basex( BodyX ) + 55, Basey( BodyY, 6 ) + 90, 2464, 2465, 35, GumpButtonType.Reply, 0 );

            AddLabel( Basex( BodyX ) + 240, Basey( BodyY, 0 ), 200, "OOC Words" );
            temp = "";
            foreach( string p in JailSystem.OOCWords )
                temp += string.Format( "{0}\n", p );
            AddColorLabelScroll( Basex( BodyX ) + 240, Basey( BodyY, 1 ), 150, 60, temp.Trim() );

            AddTextField( Basex( BodyX ) + 240, Basey( BodyY, 1 ) + 65, 150, 20, 14 );
            AddButton( BodyX + 240, Basey( BodyY, 1 ) + 90, 2461, 2462, 32, GumpButtonType.Reply, 0 );
            AddButton( BodyX + 295, Basey( BodyY, 1 ) + 90, 2464, 2465, 33, GumpButtonType.Reply, 0 );

            AddLabel( Basex( BodyX ) + 240, Basey( BodyY, 5 ) + 10, 200, "Jail Terms" );
            temp = "";
            foreach( TimeSpan t in JailSystem.FoulMouthJailTimes )
                temp += string.Format( "d={0} h={1} m={2}\n", t.Days, t.Hours, t.Minutes );
            AddColorLabelScroll( Basex( BodyX ) + 240, Basey( BodyY, 6 ) + 10, 150, 60, temp.Trim() );

            AddLabel( Basex( BodyX ) + 240, Basey( BodyY, 6 ) + 75, 200, "D" );
            AddLabel( Basex( BodyX ) + 290, Basey( BodyY, 6 ) + 75, 200, "H" );
            AddLabel( Basex( BodyX ) + 340, Basey( BodyY, 6 ) + 75, 200, "M" );
            AddTextField( Basex( BodyX ) + 255, Basey( BodyY, 6 ) + 75, 30, 20, 8 );
            AddTextField( Basex( BodyX ) + 305, Basey( BodyY, 6 ) + 75, 30, 20, 9 );
            AddTextField( Basex( BodyX ) + 355, Basey( BodyY, 6 ) + 75, 30, 20, 10 );
            AddButton( BodyX + 240, Basey( BodyY, 6 ) + 100, 2461, 2462, 30, GumpButtonType.Reply, 0 );
            AddButton( BodyX + 295, Basey( BodyY, 6 ) + 100, 2464, 2465, 31, GumpButtonType.Reply, 0 );
        }

        private void BuildSettings()
        {
            Message();
            AddButton( TitleX + 50, TitleY + 75, 239, 240, 6, GumpButtonType.Reply, 0 );

            AddLabel( Basex( BodyX ), Basey( BodyY, 0 ), 200, "Commands" );
            AddLabel( Basex( BodyX ) + 15, Basey( BodyY, 1 ), 200, "Status" );
            AddLabel( Basex( BodyX ) + 15, Basey( BodyY, 2 ), 200, "Time" );
            AddTextField( Basex( BodyX ) + 60, Basey( BodyY, 1 ), 150, 20, 1, JailSystem.StatusCommand );
            AddTextField( Basex( BodyX ) + 60, Basey( BodyY, 2 ), 150, 20, 2, JailSystem.TimeCommand );

            AddLabel( Basex( BodyX ), Basey( BodyY, 3 ), 200, "Misc." );
            AddLabel( Basex( BodyX ) + 15, Basey( BodyY, 4 ), 200, "Name" );
            AddTextField( Basex( BodyX ) + 60, Basey( BodyY, 4 ), 150, 20, 3, JailSystem.JSName );
            AddLabel( Basex( BodyX ) + 65, Basey( BodyY, 5 ), 200,
                      string.Format( "Jail Facet:{0}", JailSystem.JailMap.Name ) );
            AddButton( Basex( BodyX ) + 15, Basey( BodyY, 5 ), 2471, 2470, 20, GumpButtonType.Reply, 0 );
            AddToggleLabeled( Basex( BodyX ) + 15, Basey( BodyY, 6 ), 7, "Use Smoking Shoes",
                              JailSystem.UseSmokingFootGear );

            AddLabel( Basex( BodyX ), Basey( BodyY, 7 ), 200, "Non-Default Release Setting" );
            AddToggleLabeled( Basex( BodyX ) + 15, Basey( BodyY, 8 ), 8, "Single Facet release",
                              JailSystem.SingleFacetOnly );
            AddLabel( Basex( BodyX ) + 65, Basey( BodyY, 9 ) + 10, 200,
                      string.Format( "Release Facet:{0}", JailSystem.DefaultReleaseFacet.Name ) );
            AddButton( BodyX + 15, Basey( BodyY, 9 ) + 10, 2471, 2470, 21, GumpButtonType.Reply, 0 );

            AddLabel( Basex( BodyX ) + 240, Basey( BodyY, 0 ), 200, "Cells" );
            string temp = "";
            foreach( Point3D p in JailSystem.Cells )
                temp += string.Format( "{0}\n", p );
            AddColorLabelScroll( Basex( BodyX ) + 240, Basey( BodyY, 1 ), 150, 60, temp.Trim() );

            AddTextField( Basex( BodyX ) + 240, Basey( BodyY, 1 ) + 65, 45, 20, 5 );
            AddTextField( Basex( BodyX ) + 290, Basey( BodyY, 1 ) + 65, 45, 20, 6 );
            AddTextField( Basex( BodyX ) + 340, Basey( BodyY, 1 ) + 65, 45, 20, 7 );
            AddButton( BodyX + 240, Basey( BodyY, 1 ) + 90, 2461, 2462, 22, GumpButtonType.Reply, 0 );
            AddButton( BodyX + 295, Basey( BodyY, 1 ) + 90, 2464, 2465, 23, GumpButtonType.Reply, 0 );

            AddLabel( Basex( BodyX ) + 240, Basey( BodyY, 5 ) + 10, 200, "Default Release Loctions" );
            temp = "";
            foreach( Point3D p in JailSystem.DefaultRelease )
                temp += p + "\n";
            AddColorLabelScroll( Basex( BodyX ) + 240, Basey( BodyY, 6 ) + 10, 150, 60, temp.Trim() );

            AddTextField( Basex( BodyX ) + 240, Basey( BodyY, 6 ) + 75, 45, 20, 8 );
            AddTextField( Basex( BodyX ) + 290, Basey( BodyY, 6 ) + 75, 45, 20, 9 );
            AddTextField( Basex( BodyX ) + 340, Basey( BodyY, 6 ) + 75, 45, 20, 10 );
            AddButton( BodyX + 240, Basey( BodyY, 6 ) + 100, 2461, 2462, 24, GumpButtonType.Reply, 0 );
            AddButton( BodyX + 295, Basey( BodyY, 6 ) + 100, 2464, 2465, 25, GumpButtonType.Reply, 0 );
        }

        private void BuildReviews()
        {
            if( JailSystem.JailSystemList.Count < m_ID )
                m_ID = 0;
            if( m_ID < 0 )
                m_ID = JailSystem.JailSystemList.Count - 1;
            if( JailSystem.JailSystemList.Count == 0 )
                m_ID = -1;
            int i = 0;
            if( m_ID >= 0 )
                foreach( JailSystem tj in JailSystem.JailSystemList.Values )
                {
                    if( ( i == 0 ) || ( i == m_ID ) )
                        m_Js = tj;
                    i++;
                }
            if( m_ID == -1 )
            {
                AddLabel( BodyX + GutterOffset, BodyY + GutterOffset, 200, "No accounts are currently jailed." );
                return;
            }
            AddLabel( TitleX + GutterOffset, TitleY + GutterOffset, 200, "Reviewing: " + m_Js.Name );
            AddLabel( TitleX + GutterOffset, TitleY + GutterOffset + LineStep, 200,
                      string.Format( "Jailed Account {0} of {1}", m_ID + 1, JailSystem.JailSystemList.Count ) );
            //previous button
            AddButton( TitleX + GutterOffset, TitleY + GutterOffset + LineStep + LineStep + 5, 2466, 2467, 44,
                       GumpButtonType.Reply, 0 );
            //next Button
            AddButton( TitleX + GutterOffset + 80, TitleY + GutterOffset + LineStep + LineStep + 5, 2469, 2470, 45,
                       GumpButtonType.Reply, 0 );
            string temp = "";
            if( m_Js.Prisoner == null )
                m_Js.KillJail();
            else
            {
                foreach( AccountComment note in m_Js.Prisoner.Comments )
                {
                    if( ( note.AddedBy == JailSystem.JSName + "-warning" ) ||
                        ( note.AddedBy == JailSystem.JSName + "-jailed" ) ||
                        ( note.AddedBy == JailSystem.JSName + "-note" ) )
                    {
                        temp = temp + note.AddedBy + "\n\r" + note.Content + "\n\r***********\n\r";
                    }
                }
                AddLabel( BodyX + 17, BodyY + 8, 200, "History" );
                AddHtml( BodyX + 13, 141, 300, 82, temp, true, true );
                //release
                AddButton( BodyX + 13, BodyY + 120, 2472, 2473, 41, GumpButtonType.Reply, 0 );
                AddLabel( BodyX + 43, BodyY + 123, 200, "Release" );
                AddButton( BodyX + 13, BodyY + 150, 2472, 2473, 50, GumpButtonType.Reply, 0 );
                AddLabel( BodyX + 43, BodyY + 153, 200, "Ban" );
                //add day
                AddButton( BodyX + 101, BodyY + 120, 250, 251, 43, GumpButtonType.Reply, 0 );
                AddButton( BodyX + 116, BodyY + 120, 252, 253, 47, GumpButtonType.Reply, 0 );
                AddLabel( 135 + BodyX, BodyY + 123, 200, "Week" );
                //add week
                AddButton( BodyX + 176, BodyY + 120, 250, 251, 42, GumpButtonType.Reply, 0 );
                AddButton( BodyX + 191, BodyY + 120, 252, 253, 46, GumpButtonType.Reply, 0 );
                AddLabel( BodyX + 210, BodyY + 123, 200, "Day" );
                //hours
                AddButton( BodyX + 251, BodyY + 120, 250, 251, 48, GumpButtonType.Reply, 0 );
                AddButton( BodyX + 266, BodyY + 120, 252, 253, 49, GumpButtonType.Reply, 0 );
                AddLabel( BodyX + 284, BodyY + 123, 200, "Hour" );

                AddLabel( BodyX + 13, BodyY + 170, 200, "Release at: " + m_Js.ReleaseDate );
                if( !m_Js.Jailed )
                {
                    Message( "This account has been released but currently has characters in jail." );
                }
                else
                {
                    Message( "This account is currently jailed." );
                }
                AddHtml( BodyX + 13, BodyY + 189, 300, 74, m_Js.Reason, true, true );
            }
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            Mobile from = sender.Mobile;
            string temp;
            switch( info.ButtonID )
            {
                case 50:
                    //ban an account;
                    from.SendGump( new JailBanGump( m_Js ) );
                    break;
                case 10:
                    temp = info.GetTextEntry( 11 ).Text.Trim().ToLower();
                    if( !string.IsNullOrEmpty( temp ) )
                        JailSystem.OoclistCommand = temp;
                    temp = info.GetTextEntry( 12 ).Text.Trim();
                    if( !string.IsNullOrEmpty( temp ) )
                        JailSystem.OocJailorName = temp;

                    temp = info.GetTextEntry( 13 ).Text.Trim().ToLower();
                    if( !string.IsNullOrEmpty( temp ) )
                        try
                        {
                            JailSystem.Oocwarns = Convert.ToInt32( temp );
                        }
                        catch
                        {
                            from.SendMessage( "Bad number of OOC Warnings." );
                        }
                    goto case 2;
                case 11:
                    JailSystem.BlockOOCSpeech = !JailSystem.BlockOOCSpeech;
                    goto case 10;
                case 12:
                    JailSystem.AllowStaffOOC = !JailSystem.AllowStaffOOC;
                    goto case 10;
                case 15:
                    //language section
                    temp = info.GetTextEntry( 12 ).Text.Trim();
                    if( !string.IsNullOrEmpty( temp ) )
                        JailSystem.FoulJailorName = temp;
                    JailSystem.FoulMouthJailTimes.Sort();
                    goto case 3;
                case 13:
                    JailSystem.UseLanguageFilter = !JailSystem.UseLanguageFilter;
                    goto case 3;
                case 14:
                    JailSystem.AllowStaffBadWords = !JailSystem.AllowStaffBadWords;
                    goto case 15;
                case 9:
                    JailSystem.UseOOCFilter = !JailSystem.UseOOCFilter;
                    goto case 2;
                //generenal section
                case 1:
                    from.SendGump( new JailAdminGump( AdminJailGumpPage.General ) );
                    break;
                case 2:
                    from.SendGump( new JailAdminGump( AdminJailGumpPage.OOC ) );
                    break;
                case 3:
                    from.SendGump( new JailAdminGump( AdminJailGumpPage.Language ) );
                    break;
                case 4:
                    from.SendGump( new JailAdminGump( AdminJailGumpPage.Review ) );
                    break;
                case 5:
                    from.CloseGump( typeof( JailAdminGump ) );
                    break;
                case 6:
                    temp = info.GetTextEntry( 1 ).Text.Trim().ToLower();
                    if( !string.IsNullOrEmpty( temp ) )
                        JailSystem.StatusCommand = temp;

                    temp = info.GetTextEntry( 2 ).Text.Trim().ToLower();
                    if( !string.IsNullOrEmpty( temp ) )
                        JailSystem.TimeCommand = temp;

                    temp = info.GetTextEntry( 3 ).Text.Trim();
                    if( !string.IsNullOrEmpty( temp ) )
                        JailSystem.JSName = temp;
                    goto case 1;
                case 7:
                    JailSystem.UseSmokingFootGear = !JailSystem.UseSmokingFootGear;
                    goto case 6;
                case 8:
                    JailSystem.SingleFacetOnly = !JailSystem.SingleFacetOnly;
                    goto case 6;
                case 20:
                    if( JailSystem.JailMap == Map.Felucca )
                        JailSystem.JailMap = Map.Trammel;
                    else if( JailSystem.JailMap == Map.Trammel )
                        JailSystem.JailMap = Map.Ilshenar;
                    else if( JailSystem.JailMap == Map.Ilshenar )
                        JailSystem.JailMap = Map.Malas;
                    else if( JailSystem.JailMap == Map.Malas )
                        JailSystem.JailMap = Map.Felucca;
                    goto case 6;
                case 21:
                    if( JailSystem.DefaultReleaseFacet == Map.Felucca )
                        JailSystem.DefaultReleaseFacet = Map.Trammel;
                    else if( JailSystem.DefaultReleaseFacet == Map.Trammel )
                        JailSystem.DefaultReleaseFacet = Map.Ilshenar;
                    else if( JailSystem.DefaultReleaseFacet == Map.Ilshenar )
                        JailSystem.DefaultReleaseFacet = Map.Malas;
                    else if( JailSystem.DefaultReleaseFacet == Map.Malas )
                        JailSystem.DefaultReleaseFacet = Map.Felucca;
                    //change facet
                    goto case 6;
                case 22:
                    //add cell
                    try
                    {
                        var p = new Point3D( Convert.ToInt32( info.GetTextEntry( 5 ).Text.Trim() ),
                                             Convert.ToInt32( info.GetTextEntry( 6 ).Text.Trim() ),
                                             Convert.ToInt32( info.GetTextEntry( 7 ).Text.Trim() ) );
                        if( JailSystem.Cells.Contains( p ) )
                            from.SendMessage( "Unable to add jail cell. It is already listed." );
                        else
                            JailSystem.Cells.Add( p );
                    }
                    catch
                    {
                        from.SendMessage( "Unable to add jail cell. Bad x,y,z." );
                    }
                    goto case 6;
                case 23:
                    //remove cell
                    try
                    {
                        var p = new Point3D( Convert.ToInt32( info.GetTextEntry( 5 ).Text.Trim() ),
                                             Convert.ToInt32( info.GetTextEntry( 6 ).Text.Trim() ),
                                             Convert.ToInt32( info.GetTextEntry( 7 ).Text.Trim() ) );
                        if( JailSystem.Cells.Contains( p ) )
                            JailSystem.Cells.Remove( p );
                        else
                            from.SendMessage( "Unable to remove jail cell. Cell not listed." );
                    }
                    catch
                    {
                        from.SendMessage( "Unable to remove jail cell. Bad x,y,z." );
                    }
                    goto case 6;
                case 24:
                    //add release
                    try
                    {
                        var p = new Point3D( Convert.ToInt32( info.GetTextEntry( 8 ).Text.Trim() ),
                                             Convert.ToInt32( info.GetTextEntry( 9 ).Text.Trim() ),
                                             Convert.ToInt32( info.GetTextEntry( 10 ).Text.Trim() ) );
                        if( JailSystem.DefaultRelease.Contains( p ) )
                            from.SendMessage( "Unable to add default release location. It is already listed." );
                        else
                            JailSystem.DefaultRelease.Add( p );
                    }
                    catch
                    {
                        from.SendMessage( "Unable to add release location. Bad x,y,z." );
                    }
                    goto case 6;
                case 25:
                    //remove release
                    try
                    {
                        var p = new Point3D( Convert.ToInt32( info.GetTextEntry( 8 ).Text.Trim() ),
                                             Convert.ToInt32( info.GetTextEntry( 9 ).Text.Trim() ),
                                             Convert.ToInt32( info.GetTextEntry( 10 ).Text.Trim() ) );
                        if( JailSystem.DefaultRelease.Contains( p ) )
                            JailSystem.DefaultRelease.Remove( p );
                        else
                            from.SendMessage( "Release location not listed." );
                    }
                    catch
                    {
                        from.SendMessage( "Unable to remove release location. Bad x,y,z." );
                    }
                    goto case 6;
                case 26:
                    //add foul word
                    try
                    {
                        temp = info.GetTextEntry( 13 ).Text.ToLower().Trim();
                        if( string.IsNullOrEmpty( temp ) )
                            from.SendMessage( "Unable to add word" );
                        else if( JailSystem.BadWords.Contains( temp ) )
                            from.SendMessage( "Word is already in the list." );
                        else
                            JailSystem.BadWords.Add( temp );
                    }
                    catch
                    {
                        from.SendMessage( "Unable to add word" );
                    }
                    goto case 15;
                case 27:
                    //remove foul word
                    try
                    {
                        temp = info.GetTextEntry( 13 ).Text.ToLower().Trim();
                        if( string.IsNullOrEmpty( temp ) )
                            from.SendMessage( "Unable to remove word" );
                        else if( JailSystem.BadWords.Contains( temp ) )
                            JailSystem.BadWords.Remove( temp );
                        else
                            from.SendMessage( "Word is not in the list." );
                    }
                    catch
                    {
                        from.SendMessage( "Unable to remove word" );
                    }
                    goto case 15;
                case 28:
                    //add jail term
                    try
                    {
                        var p = new TimeSpan( Convert.ToInt32( info.GetTextEntry( 8 ).Text.Trim() ),
                                              Convert.ToInt32( info.GetTextEntry( 9 ).Text.Trim() ),
                                              Convert.ToInt32( info.GetTextEntry( 10 ).Text.Trim() ), 0, 0 );
                        if( JailSystem.FoulMouthJailTimes.Contains( p ) )
                            from.SendMessage( "Unable to add jail term. It is already listed." );
                        else
                            JailSystem.FoulMouthJailTimes.Add( p );
                    }
                    catch
                    {
                        from.SendMessage( "Unable to add jail term. Bad D,H,M." );
                    }
                    goto case 15;
                case 29:
                    //remove jail term
                    try
                    {
                        var p = new TimeSpan( Convert.ToInt32( info.GetTextEntry( 8 ).Text.Trim() ),
                                              Convert.ToInt32( info.GetTextEntry( 9 ).Text.Trim() ),
                                              Convert.ToInt32( info.GetTextEntry( 10 ).Text.Trim() ), 0, 0 );
                        if( JailSystem.FoulMouthJailTimes.Contains( p ) )
                            JailSystem.FoulMouthJailTimes.Remove( p );
                        else
                            from.SendMessage( "Jail term not listed." );
                    }
                    catch
                    {
                        from.SendMessage( "Unable to remove Jail term. Bad D,H,M." );
                    }
                    goto case 15;

                case 30:
                    //add jail term
                    try
                    {
                        var p = new TimeSpan( Convert.ToInt32( info.GetTextEntry( 8 ).Text.Trim() ),
                                              Convert.ToInt32( info.GetTextEntry( 9 ).Text.Trim() ),
                                              Convert.ToInt32( info.GetTextEntry( 10 ).Text.Trim() ), 0, 0 );
                        if( JailSystem.OOCJailTimes.Contains( p ) )
                            from.SendMessage( "Unable to add jail term. It is already listed." );
                        else
                            JailSystem.OOCJailTimes.Add( p );
                    }
                    catch
                    {
                        from.SendMessage( "Unable to add jail term. Bad D,H,M." );
                    }
                    goto case 10;
                case 31:
                    //remove jail term
                    try
                    {
                        var p = new TimeSpan( Convert.ToInt32( info.GetTextEntry( 8 ).Text.Trim() ),
                                              Convert.ToInt32( info.GetTextEntry( 9 ).Text.Trim() ),
                                              Convert.ToInt32( info.GetTextEntry( 10 ).Text.Trim() ), 0, 0 );
                        if( JailSystem.OOCJailTimes.Contains( p ) )
                            JailSystem.OOCJailTimes.Remove( p );
                        else
                            from.SendMessage( "Jail term not listed." );
                    }
                    catch
                    {
                        from.SendMessage( "Unable to remove Jail term. Bad D,H,M." );
                    }
                    goto case 10;
                case 32:
                    //add ooc word
                    try
                    {
                        temp = info.GetTextEntry( 14 ).Text.ToLower().Trim();
                        if( string.IsNullOrEmpty( temp ) )
                            from.SendMessage( "Unable to add word" );
                        else if( JailSystem.OOCWords.Contains( temp ) )
                            from.SendMessage( "Word is already in the list." );
                        else
                            JailSystem.OOCWords.Add( temp );
                    }
                    catch
                    {
                        from.SendMessage( "Unable to add word" );
                    }
                    goto case 10;
                case 33:
                    //remove ooc word
                    try
                    {
                        temp = info.GetTextEntry( 14 ).Text.ToLower().Trim();
                        if( string.IsNullOrEmpty( temp ) )
                            from.SendMessage( "Unable to remove word" );
                        else if( JailSystem.OOCWords.Contains( temp ) )
                            JailSystem.OOCWords.Remove( temp );
                        else
                            from.SendMessage( "Word is not in the list." );
                    }
                    catch
                    {
                        from.SendMessage( "Unable to remove word" );
                    }
                    goto case 10;
                case 34:
                    //add ooc part
                    try
                    {
                        temp = info.GetTextEntry( 15 ).Text.ToLower().Trim();
                        if( string.IsNullOrEmpty( temp ) )
                            from.SendMessage( "Unable to add word" );
                        else if( JailSystem.OOCParts.Contains( temp ) )
                            from.SendMessage( "Word is already in the list." );
                        else
                            JailSystem.OOCParts.Add( temp );
                    }
                    catch
                    {
                        from.SendMessage( "Unable to add word" );
                    }
                    goto case 10;
                case 35:
                    //remove ooc part
                    try
                    {
                        temp = info.GetTextEntry( 15 ).Text.ToLower().Trim();
                        if( string.IsNullOrEmpty( temp ) )
                            from.SendMessage( "Unable to remove word" );
                        else if( JailSystem.OOCParts.Contains( temp ) )
                            JailSystem.OOCParts.Remove( temp );
                        else
                            from.SendMessage( "Word is not in the list." );
                    }
                    catch
                    {
                        from.SendMessage( "Unable to remove word" );
                    }
                    goto case 10;
                case 41:
                    m_Js.ForceRelease( from );
                    from.SendGump( new JailAdminGump( m_Page, m_Subpage, m_ID ) );
                    break;
                case 42:
                    m_Js.AddDays( 1 );
                    from.SendGump( new JailAdminGump( m_Page, m_Subpage, m_ID ) );
                    break;
                case 46:
                    m_Js.SubtractDays( 1 );
                    from.SendGump( new JailAdminGump( m_Page, m_Subpage, m_ID ) );
                    break;
                case 47:
                    m_Js.SubtractDays( 7 );
                    from.SendGump( new JailAdminGump( m_Page, m_Subpage, m_ID ) );
                    break;
                case 48:
                    m_Js.AddHours( 1 );
                    from.SendGump( new JailAdminGump( m_Page, m_Subpage, m_ID ) );
                    break;
                case 49:
                    m_Js.SubtractHours( 1 );
                    from.SendGump( new JailAdminGump( m_Page, m_Subpage, m_ID ) );
                    break;
                case 43:
                    m_Js.AddDays( 7 );
                    from.SendGump( new JailAdminGump( m_Page, m_Subpage, m_ID ) );
                    break;
                case 44:
                    //previous button
                    m_ID--;
                    if( m_ID < 0 )
                        m_ID = JailSystem.JailSystemList.Count - 1;
                    from.SendGump( new JailAdminGump( m_Page, m_Subpage, m_ID ) );
                    break;
                case 45:
                    //next button
                    m_ID++;
                    if( m_ID >= JailSystem.JailSystemList.Count )
                        m_ID = 0;
                    from.SendGump( new JailAdminGump( m_Page, m_Subpage, m_ID ) );
                    break;
                default:
                    break;
            }
            //from.CloseGump(typeof ( JailAdminGump ));
        }
    }
}