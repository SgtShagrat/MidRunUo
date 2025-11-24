using System;
using System.Collections.Generic;
using Server.Engines.XmlSpawner2;
using Server.Gumps;
using Server.Network;
using Server.Targeting;

/*
** DeathBallGump
** ArteGordon
** updated 12/06/04
**
*/

namespace Server.Engines.XmlPoints
{
    public class DeathBallGump : Gump
    {
        private const int Textentrybackground = 0x24A8;
        private readonly DeathBallGauntlet m_Gauntlet;
        private readonly int m_Viewpage;
        private readonly List<BaseChallengeEntry> m_WorkingList;
        private int MaxTeamSize = 0; // zero means unlimited number of players
        private int PlayersPerPage = 10;
        private int y_inc = 25;

        public DeathBallGump( DeathBallGauntlet gauntlet, Mobile from )
            : this( gauntlet, from, 0 )
        {
        }

        public DeathBallGump( DeathBallGauntlet gauntlet, Mobile from, int page )
            : base( 20, 30 )
        {
            if( from == null || gauntlet == null || gauntlet.Deleted || gauntlet.Challenger == null )
                return;

            from.CloseGump( typeof( DeathBallGump ) );

            m_Gauntlet = gauntlet;

            m_Viewpage = page;

            int height = 520;

            AddBackground( 0, 0, 350, height, 0xDAC );
            //AddAlphaRegion( 0, 0, 340, height );

            AddLabel( 100, 10, 0, XmlPointsAttach.GetText( from, 200570 ) ); // "Deathball Challenge"
            AddLabel( 20, 30, 0, String.Format( XmlPointsAttach.GetText( from, 200501 ), gauntlet.Challenger.Name ) );
            // "Organized by: {0}"
            AddLabel( 20, 50, 0, String.Format( XmlPointsAttach.GetText( from, 200502 ), m_Gauntlet.EntryFee ) );
            // "Entry Fee: {0}"
            AddLabel( 20, 70, 0, String.Format( XmlPointsAttach.GetText( from, 200503 ), m_Gauntlet.ArenaSize ) );
            // "Arena Size: {0}"

            AddImageTiled( 15, 130, 320, 20, 0xdb3 );


            // display all of the current team members
            if( gauntlet.Participants != null )
            {
                // copy the master list to a temporary working list
                m_WorkingList = new List<BaseChallengeEntry>( gauntlet.Participants );

                AddLabel( 150, 50, 0,
                         String.Format( XmlPointsAttach.GetText( from, 200504 ), m_WorkingList.Count * m_Gauntlet.EntryFee ) );
                // "Total Purse: {0}"

                AddLabel( 150, 70, 0,
                         String.Format( XmlPointsAttach.GetText( from, 200505 ), m_Gauntlet.Location, m_Gauntlet.Map ) );
                // "Loc: {0} {1}"

                AddLabel( 20, 90, 0, String.Format( XmlPointsAttach.GetText( from, 200506 ), gauntlet.Participants.Count ) );
                // "Players: {0}"

                AddLabel( 150, 90, 0, String.Format( XmlPointsAttach.GetText( from, 200507 ), gauntlet.ActivePlayers() ) );
                // "Active: {0}"

                if( gauntlet.TargetScore > 0 )
                    AddLabel( 20, 110, 0, String.Format( XmlPointsAttach.GetText( from, 200561 ), gauntlet.TargetScore ) );
                // "Target Score: {0}"
                else
                    AddLabel( 20, 110, 0, XmlPointsAttach.GetText( from, 200562 ) ); // "Target Score: None"

                int yoffset = 155;

                // page up and down buttons
                AddButton( 300, 130, 0x15E0, 0x15E4, 13, GumpButtonType.Reply, 0 );
                AddButton( 320, 130, 0x15E2, 0x15E6, 12, GumpButtonType.Reply, 0 );


                // find the players entry to determine the viewing page
                for( int i = 0; i < m_WorkingList.Count; i++ )
                {
                    var entry = (DeathBallGauntlet.ChallengeEntry)m_WorkingList[ i ];

                    if( entry == null )
                        continue;

                    if( entry.Participant == from )
                    {
                        m_Viewpage = entry.PageBeingViewed;
                        break;
                    }
                }

                AddLabel( 220, 130, 0,
                         String.Format( XmlPointsAttach.GetText( from, 200508 ), m_Viewpage + 1,
                                       ( m_WorkingList.Count / PlayersPerPage ) + 1 ) ); // "Page: {0}/{1}"


                for( int i = 0; i < m_WorkingList.Count; i++ )
                {
                    // determine which page is being viewed

                    if( ( i / PlayersPerPage ) != m_Viewpage )
                        continue;

                    var entry = (DeathBallGauntlet.ChallengeEntry)m_WorkingList[ i ];

                    if( entry == null )
                        continue;

                    // display the entry with a color indicating whether they have accepted or not
                    Mobile m = entry.Participant;

                    string statusmsg = XmlPointsAttach.GetText( from, 200509 ); // "Waiting"
                    int texthue = 0;

                    // this section is available during game setup
                    if( !m_Gauntlet.GameLocked )
                    {
                        statusmsg = XmlPointsAttach.GetText( from, 200509 ); // "Waiting"
                        if( entry.Accepted )
                        {
                            texthue = 68;
                            statusmsg = XmlPointsAttach.GetText( from, 200510 ); // "Accepted"
                        }

                        // check to see if they have the Entry fee
                        if( !m_Gauntlet.HasEntryFee( m ) )
                        {
                            texthue = 33;
                            statusmsg = XmlPointsAttach.GetText( from, 200511 ); // "Insufficient funds"
                        }


                        // if the game is still open then enable these buttons

                        // if this is the participant then add the accept button to the entry
                        if( m == from )
                        {
                            AddButton( 15, yoffset, entry.Accepted ? 0xd1 : 0xd0, entry.Accepted ? 0xd0 : 0xd1, 1000 + i,
                                      GumpButtonType.Reply, 0 );
                        }
                        // if this is the organizer then add the kick button to each entry
                        if( from == m_Gauntlet.Challenger || from == entry.Participant )
                        {
                            AddButton( 190, yoffset, 0xFB1, 0xFB3, 2000 + i, GumpButtonType.Reply, 0 );
                        }
                    }
                    else
                    {
                        // this section is active after the game has started

                        // enable the forfeit button
                        if( m == from && entry.Status == ChallengeStatus.Active && !m_Gauntlet.GameCompleted )
                        {
                            AddButton( 190, yoffset, 0xFB1, 0xFB3, 4000 + i, GumpButtonType.Reply, 0 );
                        }

                        if( entry.Status == ChallengeStatus.Forfeit )
                        {
                            texthue = 33;
                            statusmsg = XmlPointsAttach.GetText( from, 200520 ); // "Forfeit"
                        }
                        else if( entry.Caution == ChallengeStatus.Hidden && entry.Status == ChallengeStatus.Active )
                        {
                            texthue = 53;
                            statusmsg = XmlPointsAttach.GetText( from, 200521 ); // "Hidden"
                        }
                        else if( entry.Caution == ChallengeStatus.OutOfBounds && entry.Status == ChallengeStatus.Active )
                        {
                            texthue = 53;
                            statusmsg = XmlPointsAttach.GetText( from, 200522 ); // "Out of Bounds"
                        }
                        else if( entry.Caution == ChallengeStatus.Offline && entry.Status == ChallengeStatus.Active )
                        {
                            texthue = 53;
                            statusmsg = XmlPointsAttach.GetText( from, 200523 ); // "Offline"
                        }
                        else if( entry.Status == ChallengeStatus.Active )
                        {
                            texthue = 68;
                            if( gauntlet.Winner == entry.Participant )
                                statusmsg = XmlPointsAttach.GetText( from, 200524 ); // "Winner"
                            else
                                statusmsg = XmlPointsAttach.GetText( from, 200525 ); // "Active"
                        }
                        else if( entry.Status == ChallengeStatus.Dead )
                        {
                            texthue = 33;
                            statusmsg = XmlPointsAttach.GetText( from, 200526 ); // "Dead"
                        }
                        else if( entry.Status == ChallengeStatus.Disqualified )
                        {
                            texthue = 33;
                            statusmsg = XmlPointsAttach.GetText( from, 200527 ); // "Disqualified"
                        }
                    }

                    if( m != null )
                    {
                        AddLabel( 40, yoffset, 0, m.Name );

                        AddLabel( 225, yoffset, texthue, statusmsg );

                        if( m_Gauntlet.GameInProgress )
                        {
                            AddLabel( 13, yoffset, 0, entry.Score.ToString() );
                        }
                    }

                    yoffset += y_inc;
                }
            }


            // the challenger gets additional options
            if( from == gauntlet.Challenger && !m_Gauntlet.GameLocked )
            {
                AddImageTiled( 15, height - 110, 320, 20, 0xdb3 );

                AddButton( 130, height - 35, 0xFA8, 0xFAA, 100, GumpButtonType.Reply, 0 );
                AddLabel( 165, height - 35, 0, XmlPointsAttach.GetText( from, 200528 ) ); // "Add"

                AddButton( 240, height - 35, 0xFB7, 0xFB9, 300, GumpButtonType.Reply, 0 );
                AddLabel( 275, height - 35, 0, XmlPointsAttach.GetText( from, 200529 ) ); // "Start"

                // set entry fee
                AddButton( 20, height - 85, 0xFAE, 0xFAF, 10, GumpButtonType.Reply, 0 );
                AddImageTiled( 130, height - 85, 60, 19, Textentrybackground );
                AddTextEntry( 130, height - 85, 60, 25, 0, 10, m_Gauntlet.EntryFee.ToString() );
                AddLabel( 55, height - 85, 0, XmlPointsAttach.GetText( from, 200572 ) ); // "Entry Fee: "

                // set arena size
                AddButton( 20, height - 60, 0xFAE, 0xFAF, 20, GumpButtonType.Reply, 0 );
                AddImageTiled( 130, height - 60, 30, 19, Textentrybackground );
                AddTextEntry( 130, height - 60, 30, 25, 0, 20, m_Gauntlet.ArenaSize.ToString() );
                AddLabel( 55, height - 60, 0, XmlPointsAttach.GetText( from, 200573 ) ); //  "Arena Size: "

                // set target score
                AddButton( 200, height - 85, 0xFAE, 0xFAF, 30, GumpButtonType.Reply, 0 );
                AddImageTiled( 280, height - 85, 40, 19, Textentrybackground );
                AddTextEntry( 280, height - 85, 40, 25, 0, 30, m_Gauntlet.TargetScore.ToString() );
                AddLabel( 235, height - 85, 0, XmlPointsAttach.GetText( from, 200566 ) ); //  "Score: "
            }
            else
            {
                AddImageTiled( 15, height - 60, 320, 20, 0xdb3 );
            }

            AddButton( 20, height - 35, 0xFAB, 0xFAD, 1, GumpButtonType.Reply, 0 );
            AddLabel( 55, height - 35, 0, XmlPointsAttach.GetText( from, 200532 ) ); // "Refresh"

            if( gauntlet.GameInProgress )
            {
                AddLabel( 150, height - 35, 68, XmlPointsAttach.GetText( from, 200533 ) ); // "Game is in progress!"
            }

            //AddButton( 30, height - 35, 0xFB7, 0xFB9, 0, GumpButtonType.Reply, 0 );
            //AddLabel( 70, height - 35, 0, "Close" );
        }

        public override void OnResponse( NetState state, RelayInfo info )
        {
            if( state == null || info == null || state.Mobile == null || m_Gauntlet == null ||
                m_Gauntlet.Challenger == null )
                return;

            var afrom = (XmlPointsAttach)XmlAttach.FindAttachment( state.Mobile, typeof( XmlPointsAttach ) );

            switch( info.ButtonID )
            {
                case 1:
                    // refresh

                    m_Gauntlet.CheckForDisqualification();

                    state.Mobile.SendGump( new DeathBallGump( m_Gauntlet, state.Mobile, m_Viewpage ) );
                    break;

                case 10:
                    // Entry fee
                    int val = 0;
                    TextRelay tr = info.GetTextEntry( 10 );
                    if( tr != null )
                    {
                        try
                        {
                            val = int.Parse( tr.Text );
                        }
                        catch
                        {
                        }
                    }
                    m_Gauntlet.EntryFee = val;

                    m_Gauntlet.ResetAcceptance();

                    // update all the gumps
                    RefreshAllGumps( m_Gauntlet, true );

                    state.Mobile.SendGump( new DeathBallGump( m_Gauntlet, state.Mobile, m_Viewpage ) );
                    break;
                case 12:
                    // page up
                    // try doing the default for non-participants
                    int nplayers = 0;
                    if( m_Gauntlet.Participants != null )
                        nplayers = m_Gauntlet.Participants.Count;

                    int page = m_Viewpage + 1;
                    if( page > ( nplayers / PlayersPerPage ) )
                    {
                        page = ( nplayers / PlayersPerPage );
                    }

                    foreach( DeathBallGauntlet.ChallengeEntry entry in m_WorkingList )
                    {
                        if( entry != null )
                        {
                            if( entry.Participant == state.Mobile )
                            {
                                entry.PageBeingViewed++;

                                if( entry.PageBeingViewed > ( nplayers / PlayersPerPage ) )
                                    entry.PageBeingViewed = ( nplayers / PlayersPerPage );
                                page = entry.PageBeingViewed;
                                break;
                            }
                        }
                    }

                    state.Mobile.SendGump( new DeathBallGump( m_Gauntlet, state.Mobile, page ) );
                    break;
                case 13:
                    // page down
                    // try doing the default for non-participants

                    page = m_Viewpage - 1;
                    if( page < 0 )
                    {
                        page = 0;
                    }
                    foreach( DeathBallGauntlet.ChallengeEntry entry in m_WorkingList )
                    {
                        if( entry != null )
                        {
                            if( entry.Participant == state.Mobile )
                            {
                                entry.PageBeingViewed--;

                                if( entry.PageBeingViewed < 0 )
                                    entry.PageBeingViewed = 0;
                                page = entry.PageBeingViewed;
                                break;
                            }
                        }
                    }

                    state.Mobile.SendGump( new DeathBallGump( m_Gauntlet, state.Mobile, page ) );
                    break;

                case 20:
                    // arena size
                    val = 0;
                    tr = info.GetTextEntry( 20 );
                    if( tr != null )
                    {
                        try
                        {
                            val = int.Parse( tr.Text );
                        }
                        catch
                        {
                        }
                    }
                    m_Gauntlet.ArenaSize = val;

                    m_Gauntlet.ResetAcceptance();

                    // update all the gumps
                    RefreshAllGumps( m_Gauntlet, true );

                    state.Mobile.SendGump( new DeathBallGump( m_Gauntlet, state.Mobile, m_Viewpage ) );
                    break;

                case 30:
                    // target score
                    val = 0;
                    tr = info.GetTextEntry( 30 );
                    if( tr != null )
                    {
                        try
                        {
                            val = int.Parse( tr.Text );
                        }
                        catch
                        {
                        }
                    }
                    m_Gauntlet.TargetScore = val;

                    m_Gauntlet.ResetAcceptance();

                    // update all the gumps
                    RefreshAllGumps( m_Gauntlet, true );

                    state.Mobile.SendGump( new DeathBallGump( m_Gauntlet, state.Mobile, m_Viewpage ) );
                    break;

                case 100:

                    // add to Participants
                    if( m_Gauntlet.Participants == null )
                        m_Gauntlet.Participants = new List<BaseChallengeEntry>();

                    if( MaxTeamSize > 0 && m_Gauntlet.Participants.Count >= MaxTeamSize )
                    {
                        XmlPointsAttach.SendText( state.Mobile, 100535 ); // "Challenge is full!"
                    }
                    else
                    {
                        state.Mobile.Target = new MemberTarget( m_Gauntlet, m_Gauntlet.Participants );
                    }


                    state.Mobile.SendGump( new DeathBallGump( m_Gauntlet, state.Mobile, m_Viewpage ) );
                    break;

                case 300:
                    // Start game
                    if( m_WorkingList == null )
                        return;

                    bool complete = true;
                    foreach( DeathBallGauntlet.ChallengeEntry entry in m_WorkingList )
                    {
                        if( entry != null )
                        {
                            Mobile m = entry.Participant;

                            if( m == null )
                                continue;

                            if( !m_Gauntlet.CheckQualify( m ) )
                            {
                                complete = false;
                                break;
                            }

                            if( !entry.Accepted )
                            {
                                XmlPointsAttach.SendText( m_Gauntlet.Challenger, 100539, m.Name );
                                // "{0} has not accepted yet."
                                complete = false;
                                break;
                            }
                        }
                    }

                    if( m_WorkingList.Count < 2 )
                    {
                        XmlPointsAttach.SendText( m_Gauntlet.Challenger, 100540 ); // "Insufficient number of players."
                        complete = false;
                    }

                    if( m_Gauntlet.TargetScore <= 0 )
                    {
                        XmlPointsAttach.SendText( m_Gauntlet.Challenger, 100568 ); // "No valid end condition for match."
                        complete = false;
                    }
                    // copy all of the accepted entries to the final participants list


                    if( complete )
                    {
                        m_Gauntlet.Participants = new List<BaseChallengeEntry>();

                        foreach( DeathBallGauntlet.ChallengeEntry entry in m_WorkingList )
                        {
                            if( entry != null )
                            {
                                Mobile m = entry.Participant;

                                if( m == null )
                                    continue;

                                // try to collect any entry fee
                                if( !m_Gauntlet.CollectEntryFee( m, m_Gauntlet.EntryFee ) )
                                    continue;

                                // set up the challenge on each player
                                var a = (XmlPointsAttach)XmlAttach.FindAttachment( m, typeof( XmlPointsAttach ) );
                                if( a != null )
                                {
                                    a.ChallengeGame = m_Gauntlet;
                                }

                                entry.Status = ChallengeStatus.Active;

                                m_Gauntlet.Participants.Add( entry );
                            }
                        }

                        // and lock the game
                        m_Gauntlet.StartGame();

                        // refresh all gumps
                        RefreshAllGumps( m_Gauntlet, true );
                    }
                    else
                    {
                        state.Mobile.SendGump( new DeathBallGump( m_Gauntlet, state.Mobile, m_Viewpage ) );
                    }

                    break;

                default:
                    {
                        // forfeit buttons
                        if( info.ButtonID >= 4000 )
                        {
                            int selection = info.ButtonID - 4000;

                            if( selection < m_WorkingList.Count )
                            {
                                var entry = (DeathBallGauntlet.ChallengeEntry)m_WorkingList[ selection ];

                                // find the master participants list entry with the same participant
                                if( m_Gauntlet.Participants != null )
                                {
                                    DeathBallGauntlet.ChallengeEntry forfeitentry = null;

                                    foreach( DeathBallGauntlet.ChallengeEntry masterentry in m_Gauntlet.Participants )
                                    {
                                        if( entry == masterentry )
                                        {
                                            forfeitentry = masterentry;
                                            break;
                                        }
                                    }

                                    // and remove it
                                    if( forfeitentry != null )
                                    {
                                        forfeitentry.Status = ChallengeStatus.Forfeit;

                                        // inform him that he has been kicked
                                        m_Gauntlet.Forfeit( forfeitentry.Participant );
                                    }
                                }
                            }
                        }
                        // kick buttons
                        if( info.ButtonID >= 2000 )
                        {
                            int selection = info.ButtonID - 2000;

                            if( selection < m_WorkingList.Count )
                            {
                                var entry = (DeathBallGauntlet.ChallengeEntry)m_WorkingList[ selection ];
                                // find the master participants list entry with the same participant
                                if( m_Gauntlet.Participants != null )
                                {
                                    DeathBallGauntlet.ChallengeEntry kickentry = null;

                                    foreach( DeathBallGauntlet.ChallengeEntry masterentry in m_Gauntlet.Participants )
                                    {
                                        if( entry == masterentry )
                                        {
                                            kickentry = masterentry;
                                            break;
                                        }
                                    }

                                    // and remove it
                                    if( kickentry != null )
                                    {
                                        m_Gauntlet.Participants.Remove( kickentry );

                                        // refresh his gump and inform him that he has been kicked
                                        if( kickentry.Participant != null )
                                        {
                                            XmlPointsAttach.SendText( kickentry.Participant, 100545,
                                                                     m_Gauntlet.ChallengeName );
                                            // "You have been kicked from {0}"
                                            kickentry.Participant.SendGump( new DeathBallGump( m_Gauntlet,
                                                                                             kickentry.Participant,
                                                                                             m_Viewpage ) );
                                        }
                                    }
                                }

                                m_Gauntlet.ResetAcceptance();
                            }

                            // refresh all gumps
                            RefreshAllGumps( m_Gauntlet, true );
                            //state.Mobile.SendGump( new DeathBallGump( m_Gauntlet, state.Mobile));
                        }
                        else
                            // accept buttons
                            if( info.ButtonID >= 1000 )
                            {
                                int selection = info.ButtonID - 1000;
                                // set the acceptance flag of the participant
                                if( selection < m_WorkingList.Count )
                                {
                                    var entry = (DeathBallGauntlet.ChallengeEntry)m_WorkingList[ selection ];

                                    entry.Accepted = !entry.Accepted;
                                }

                                // refresh all gumps
                                RefreshAllGumps( m_Gauntlet, true );

                                //state.Mobile.SendGump( new DeathBallGump( m_Gauntlet, state.Mobile));
                            }


                        break;
                    }
            }
        }

        public static void RefreshAllGumps( DeathBallGauntlet gauntlet, bool forced )
        {
            if( gauntlet.Participants != null )
            {
                foreach( DeathBallGauntlet.ChallengeEntry entry in gauntlet.Participants )
                {
                    if( entry.Participant != null && entry.Status != ChallengeStatus.Forfeit )
                    {
                        if( forced || entry.Participant.HasGump( typeof( DeathBallGump ) ) )
                        {
                            entry.Participant.SendGump( new DeathBallGump( gauntlet, entry.Participant ) );
                        }
                    }
                }
            }

            // update for the organizer
            if( gauntlet.Challenger != null )
            {
                if( forced || gauntlet.Challenger.HasGump( typeof( DeathBallGump ) ) )
                {
                    gauntlet.Challenger.SendGump( new DeathBallGump( gauntlet, gauntlet.Challenger ) );
                }
            }
        }

        #region Nested type: MemberTarget

        private class MemberTarget : Target
        {
            private readonly DeathBallGauntlet m_Gauntlet;
            private readonly List<BaseChallengeEntry> m_List;


            public MemberTarget( DeathBallGauntlet gauntlet, List<BaseChallengeEntry> list )
                : base( 30, false, TargetFlags.None )
            {
                m_List = list;
                m_Gauntlet = gauntlet;
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                if( from == null || targeted == null || m_Gauntlet == null || m_List == null )
                    return;

                if( targeted is Mobile && ( (Mobile)targeted ).Player )
                {
                    var pm = targeted as Mobile;

                    // make sure they qualify
                    if( !m_Gauntlet.CheckQualify( pm ) )
                    {
                        return;
                    }

                    // see if they are already in the game
                    if( m_Gauntlet.Participants != null )
                    {
                        foreach( DeathBallGauntlet.ChallengeEntry entry in m_Gauntlet.Participants )
                        {
                            if( pm == entry.Participant )
                            {
                                XmlPointsAttach.SendText( from, 100548, pm.Name );
                                // "{0} has already been added to the game."
                                return;
                            }
                        }
                    }

                    m_List.Add( new DeathBallGauntlet.ChallengeEntry( pm ) );

                    XmlPointsAttach.SendText( from, 100549, pm.Name ); // "You have added {0} to the challenge."

                    XmlPointsAttach.SendText( pm, 100550, m_Gauntlet.Name );
                    // "You have been invited to participate in {0}."

                    // clear all acceptances
                    m_Gauntlet.ResetAcceptance();

                    // refresh all gumps
                    RefreshAllGumps( m_Gauntlet, true );

                    // refresh the gump with the new member
                    //from.SendGump( new DeathBallGump( m_Gauntlet, from));
                }
            }
        }

        #endregion
    }
}