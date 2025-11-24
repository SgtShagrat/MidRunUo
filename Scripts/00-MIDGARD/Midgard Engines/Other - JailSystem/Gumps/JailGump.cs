using System;

using Server;
using Server.Gumps;
using Server.Network;

namespace Midgard.Engines.JailSystem
{
    public class JailGump : Gump
    {
        public static string[] Reasons = {
                                             "Unattended Macroing", "Disruptive behavior", "Arguing with Staff",
                                             "Harassing other players",
                                             "Exploiting Bugs", "Scamming", "Breaking out of Character",
                                             "Exposing a Staff Members Player Account"
                                         };

        private Mobile m_BadBoy;
        private Mobile m_Jailor;
        private JailSystem m_Js;
        private int m_Page;
        private string m_Reason = "Breaking Shard Rules";
        private bool m_Return;

        public JailGump( JailSystem tjs, Mobile owner, Mobile prisoner, int page, string error, string reason )
            : base( 100, 40 )
        {
            BuildIt( tjs, owner, prisoner, page, error, reason, "0", "0", "1", "0", "0", true );
        }

        public JailGump( JailSystem tjs, Mobile owner, Mobile prisoner, int page, string error, string reason,
                         string month, string week, string day,
                         string hour, string minute, bool fullreturn )
            : base( 100, 40 )
        {
            BuildIt( tjs, owner, prisoner, page, error, reason, month, week, day, hour, minute, fullreturn );
        }

        public void BuildIt( JailSystem tjs, Mobile owner, Mobile prisoner, int page, string error, string reason,
                             string month, string week,
                             string day, string hour, string minute, bool fullreturn )
        {
            m_Js = tjs;
            m_Return = fullreturn;
            m_Page = page;
            m_Reason = !string.IsNullOrEmpty( reason ) ? reason : Reasons[ 1 ];
            m_Jailor = owner;
            m_BadBoy = prisoner;
            m_Reason = reason;
            m_Jailor.CloseGump( typeof( JailGump ) );
            Closable = false;
            Dragable = false;
            AddPage( 0 );
            AddBackground( 0, 0, 326, 295, 5054 );
            AddImageTiled( 9, 6, 308, 140, 2624 );
            AddAlphaRegion( 9, 6, 308, 140 );
            AddLabel( 16, 98, 200, "Reason" );
            AddBackground( 14, 114, 290, 24, 0x2486 );
            AddTextEntry( 18, 116, 282, 20, 200, 0, m_Reason );
            AddButton( 14, 11, 1209, 1210, 3, GumpButtonType.Reply, 0 );
            AddLabel( 30, 7, 200, Reasons[ 0 ] );
            AddButton( 14, 29, 1209, 1210, 4, GumpButtonType.Reply, 0 );
            AddLabel( 30, 25, 200, Reasons[ 1 ] );
            AddButton( 14, 47, 1209, 1210, 5, GumpButtonType.Reply, 0 );
            AddLabel( 30, 43, 200, Reasons[ 2 ] );
            AddButton( 150, 11, 1209, 1210, 6, GumpButtonType.Reply, 0 );
            AddLabel( 170, 7, 200, Reasons[ 3 ] );
            AddButton( 150, 29, 1209, 1210, 7, GumpButtonType.Reply, 0 );
            AddLabel( 170, 24, 200, Reasons[ 4 ] );
            AddButton( 150, 47, 1209, 1210, 8, GumpButtonType.Reply, 0 );
            AddLabel( 170, 43, 200, Reasons[ 5 ] );
            AddButton( 14, 66, 1209, 1210, 9, GumpButtonType.Reply, 0 );
            AddLabel( 30, 62, 200, Reasons[ 6 ] );
            AddButton( 14, 84, 1209, 1210, 10, GumpButtonType.Reply, 0 );
            AddLabel( 30, 80, 200, Reasons[ 7 ] );
            //ok button
            AddButton( 258, 268, 2128, 2130, 1, GumpButtonType.Reply, 0 );
            AddImageTiled( 8, 153, 308, 113, 2624 );
            AddAlphaRegion( 8, 153, 308, 113 );
            if( m_Return )
                AddButton( 15, 210, 2153, 2151, 2, GumpButtonType.Reply, 0 );
            else
                AddButton( 15, 210, 2151, 2153, 2, GumpButtonType.Reply, 0 );
            AddLabel( 50, 212, 200, "Return to where jailed from on release" );
            if( !string.IsNullOrEmpty( error ) )
            {
                AddLabel( 10, 235, 200, error );
            }
            if( m_Page == 0 )
            {
                //auto
                //auto/manual
                AddButton( 11, 268, 2111, 2114, 25, GumpButtonType.Reply, 0 );
                AddLabel( 16, 160, 200, "Months" );
                AddBackground( 19, 178, 34, 24, 0x2486 );
                AddTextEntry( 21, 180, 30, 20, 0, 7, month );
                AddLabel( 62, 160, 200, "Weeks" );
                AddBackground( 63, 178, 34, 24, 0x2486 );
                AddTextEntry( 65, 180, 30, 20, 0, 6, week );
                AddLabel( 106, 160, 200, "Days" );
                AddBackground( 104, 178, 34, 24, 0x2486 );
                AddTextEntry( 107, 180, 30, 20, 0, 5, day );
                AddLabel( 145, 160, 200, "Hours" );
                AddBackground( 145, 178, 34, 24, 0x2486 );
                AddTextEntry( 147, 180, 30, 20, 0, 9, hour );
                AddLabel( 185, 160, 200, "Minutes" );
                AddBackground( 191, 178, 34, 24, 0x2486 );
                AddTextEntry( 194, 180, 30, 20, 0, 8, minute );
            }
            else
            {
                AddButton( 11, 268, 2114, 2111, 27, GumpButtonType.Reply, 0 );
                AddLabel( 14, 160, 200, "Account will be Jailed for one year" );
                AddLabel( 14, 178, 200, "or until released, which comes first" );
            }
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            Mobile from = sender.Mobile;
            switch( info.ButtonID )
            {
                //reason buttons
                case 25:
                    from.SendGump(
                        ( new JailGump( m_Js, from, m_BadBoy, 1, "", info.TextEntries[ 0 ].Text, "0", "0", "1", "0", "0",
                                        m_Return ) ) );
                    //, info.GetTextEntry(7), info.GetTextEntry(6), info.GetTextEntry(5), info.GetTextEntry(9), info.GetTextEntry(8),m_Return
                    break;
                case 27:
                    from.SendGump(
                        ( new JailGump( m_Js, from, m_BadBoy, 0, "", info.TextEntries[ 0 ].Text, "0", "0", "1", "0", "0",
                                        m_Return ) ) );
                    break;
                case 2:
                    m_Return = !m_Return;
                    if( m_Page == 1 )
                        from.SendGump(
                            ( new JailGump( m_Js, from, m_BadBoy, m_Page, "", info.TextEntries[ 0 ].Text, "0", "0", "1", "0",
                                            "0", m_Return ) ) );
                    else
                        from.SendGump(
                            ( new JailGump( m_Js, from, m_BadBoy, m_Page, "", info.TextEntries[ 0 ].Text,
                                            info.GetTextEntry( 7 ).Text, info.GetTextEntry( 6 ).Text,
                                            info.GetTextEntry( 5 ).Text, info.GetTextEntry( 9 ).Text,
                                            info.GetTextEntry( 8 ).Text, m_Return ) ) );
                    break;
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                    if( m_Page == 1 )
                        from.SendGump(
                            ( new JailGump( m_Js, from, m_BadBoy, m_Page, "", Reasons[ info.ButtonID - 3 ], "0", "0", "1",
                                            "0", "0", m_Return ) ) );
                    else
                        from.SendGump(
                            ( new JailGump( m_Js, from, m_BadBoy, m_Page, "", Reasons[ info.ButtonID - 3 ],
                                            info.GetTextEntry( 7 ).Text,
                                            info.GetTextEntry( 6 ).Text, info.GetTextEntry( 5 ).Text,
                                            info.GetTextEntry( 9 ).Text, info.GetTextEntry( 8 ).Text,
                                            m_Return ) ) );
                    break;
                case 1:
                    {
                        DateTime dtUnJail = DateTime.Now;
                        string mError = "";
                        int iDays = 0;
                        int iWeeks = 0;
                        int iMonths = 0;
                        int iMinutes = 0;
                        int iHours = 0;
                        if( m_Page == 0 )
                        {
                            try
                            {
                                iDays = Convert.ToInt32( ( info.GetTextEntry( 5 ) ).Text.Trim() );
                            }
                            catch
                            {
                                mError = "Bad day(s) entry! No negative values or chars.";
                            }
                            try
                            {
                                iWeeks = Convert.ToInt32( ( info.GetTextEntry( 6 ) ).Text.Trim() );
                            }
                            catch
                            {
                                if( mError == "" )
                                    mError = "Bad week(s) entry! No negative values or chars.";
                            }
                            try
                            {
                                iMonths = Convert.ToInt32( ( info.GetTextEntry( 7 ) ).Text.Trim() );
                            }
                            catch
                            {
                                if( mError == "" )
                                    mError = "Bad month(s) entry! No negative values or chars.";
                            }
                            try
                            {
                                iMinutes = Convert.ToInt32( ( info.GetTextEntry( 8 ) ).Text.Trim() );
                            }
                            catch
                            {
                                if( mError == "" )
                                    mError = "Bad minute(s) entry! No negative values or chars.";
                            }
                            try
                            {
                                iHours = Convert.ToInt32( ( info.GetTextEntry( 9 ) ).Text.Trim() );
                            }
                            catch
                            {
                                if( mError == "" )
                                    mError = "Bad hour(s) entry! No negative values or chars.";
                            }
                            if( ( ( iDays > 7 ) || ( iDays < 0 ) ) && ( mError == "" ) )
                            {
                                if( mError == "" )
                                    mError = "Bad day(s) entry! No negative values. 7 days max.";
                            }
                            if( ( ( iWeeks > 4 ) || ( iWeeks < 0 ) ) && ( mError == "" ) )
                            {
                                if( mError == "" )
                                    mError = "Bad week(s) entry! No negative values. 4 weeks max.";
                            }
                            if( ( ( iMonths > 12 ) || ( iMonths < 0 ) ) && ( mError == "" ) )
                            {
                                if( mError == "" )
                                    mError = "Bad month(s) entry! No negative values. 1 year max.";
                            }
                            if( ( ( iMinutes > 60 ) || ( iMinutes < 0 ) ) && ( mError == "" ) )
                            {
                                if( mError == "" )
                                    mError = "Bad minute(s) entry! No negative values. 1 hour max.";
                            }
                            if( ( ( iHours > 24 ) || ( iHours < 0 ) ) && ( mError == "" ) )
                            {
                                if( mError == "" )
                                    mError = "Bad hour(s) entry! No negative values. 1 day max.";
                            }
                            if( mError != "" )
                            {
                                from.SendGump( new JailGump( m_Js, from, m_BadBoy, m_Page, mError,
                                                             info.TextEntries[ 0 ].Text, iMonths.ToString(),
                                                             iWeeks.ToString(), iDays.ToString(), iHours.ToString(),
                                                             iMinutes.ToString(), m_Return ) );
                                break;
                            }
                            if( iDays > 0 )
                                dtUnJail = dtUnJail.AddDays( iDays );
                            if( iWeeks > 0 )
                                dtUnJail = dtUnJail.AddDays( ( iWeeks * 7 ) );
                            if( iMonths > 0 )
                                dtUnJail = dtUnJail.AddMonths( iMonths );
                            if( iMinutes > 0 )
                                dtUnJail = dtUnJail.AddMinutes( iMinutes );
                            if( iHours > 0 )
                                dtUnJail = dtUnJail.AddHours( iHours );
                            if( dtUnJail.Ticks <= DateTime.Now.Ticks )
                            {
                                mError = "Calculated date is in the past. Adjust your entries.";
                                from.SendGump( new JailGump( m_Js, from, m_BadBoy, m_Page, mError,
                                                             info.TextEntries[ 0 ].Text, iMonths.ToString(),
                                                             iWeeks.ToString(), iDays.ToString(), iHours.ToString(),
                                                             iMinutes.ToString(), m_Return ) );
                                break;
                            }
                        }
                        else
                        {
                            //page isn’t the time span
                            dtUnJail = dtUnJail.AddYears( 1 );
                            if( dtUnJail.Ticks <= DateTime.Now.Ticks )
                            {
                                mError = "Calculated date is in the past. Adjust your entries.";
                                from.SendGump( new JailGump( m_Js, from, m_BadBoy, m_Page, mError,
                                                             info.TextEntries[ 0 ].Text, "12", "0", "0", "0", "0",
                                                             m_Return ) );
                                break;
                            }
                        }
                        m_Js.FillJailReport( m_BadBoy, dtUnJail, info.TextEntries[ 0 ].Text, m_Return, from.Name );
                    }
                    from.CloseGump( typeof( JailGump ) );
                    from.SendGump( new JailReviewGump( from, m_BadBoy, 0, null ) );
                    break;
                default:
                    //they hit an unknown button
                    if( m_Page == 1 )
                        from.SendGump(
                            ( new JailGump( m_Js, from, m_BadBoy, m_Page, "", info.TextEntries[ 0 ].Text, "0", "0", "1", "0",
                                            "0", m_Return ) ) );
                    else
                        from.SendGump(
                            ( new JailGump( m_Js, from, m_BadBoy, m_Page, "", info.TextEntries[ 0 ].Text,
                                            info.GetTextEntry( 7 ).Text, info.GetTextEntry( 6 ).Text,
                                            info.GetTextEntry( 5 ).Text, info.GetTextEntry( 9 ).Text,
                                            info.GetTextEntry( 8 ).Text, m_Return ) ) );
                    //close the Gump, we're done
                    break;
            }
        }
    }
}