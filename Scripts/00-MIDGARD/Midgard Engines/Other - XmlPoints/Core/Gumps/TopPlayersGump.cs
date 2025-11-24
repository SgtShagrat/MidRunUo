using System;
using Server.Gumps;
using Server.Network;

namespace Server.Engines.XmlPoints
{
    public class TopPlayersGump : Gump
    {
        private readonly XmlPointsAttach m_Attachment;

        public TopPlayersGump( XmlPointsAttach attachment )
            : base( 0, 0 )
        {
            if( XmlPointsAttach.RankList == null || attachment == null )
                return;

            m_Attachment = attachment;

            int numberToDisplay = 20;
            int height = numberToDisplay * 20 + 65;

            // prepare the page
            AddPage( 0 );

            int width = 740;
#if(FACTIONS)
				width = 790;
#endif

            AddBackground( 0, 0, width, height, 5054 );
            AddAlphaRegion( 0, 0, width, height );
            AddImageTiled( 20, 20, width - 40, height - 45, 0xBBC );
            AddLabel( 20, 2, 55, attachment.Text( 200239 ) ); // "Top Player Rankings"

            // guild filter
            AddLabel( 40, height - 20, 55, attachment.Text( 200240 ) ); // "Filter by Guild"
            string filter = null;
            filter = m_Attachment.m_GuildFilter;

            AddImageTiled( 140, height - 20, 160, 19, 0xBBC );
            AddTextEntry( 140, height - 20, 160, 19, 0, 200, filter );

            AddButton( 20, height - 20, 0x15E1, 0x15E5, 200, GumpButtonType.Reply, 0 );

            // name filter
            AddLabel( 340, height - 20, 55, attachment.Text( 200241 ) ); // "Filter by Name"
            string nfilter = null;
            nfilter = m_Attachment.m_NameFilter;

            AddImageTiled( 440, height - 20, 160, 19, 0xBBC );
            AddTextEntry( 440, height - 20, 160, 19, 0, 100, nfilter );

            AddButton( 320, height - 20, 0x15E1, 0x15E5, 100, GumpButtonType.Reply, 0 );

            XmlPointsAttach.RefreshRankList();

            int xloc = 23;
            AddLabel( xloc, 20, 0, attachment.Text( 200242 ) ); // "Name"
            xloc += 177;
            AddLabel( xloc, 20, 0, attachment.Text( 200243 ) ); // "Guild"
#if(FACTIONS)
				xloc += 35;
				AddLabel( xloc, 20, 0, attachment.Text(200640) );  // "Faction"
				xloc += 15;
#endif
            xloc += 50;
            AddLabel( xloc, 20, 0, attachment.Text( 200244 ) ); // "Points"
            xloc += 50;
            AddLabel( xloc, 20, 0, attachment.Text( 200245 ) ); // "Kills"
            xloc += 50;
            AddLabel( xloc, 20, 0, attachment.Text( 200246 ) ); // "Deaths"
            xloc += 70;
            AddLabel( xloc, 20, 0, attachment.Text( 200247 ) ); // "Rank"
            xloc += 45;
            AddLabel( xloc, 20, 0, attachment.Text( 200248 ) ); // "Change"
            xloc += 45;
            AddLabel( xloc, 20, 0, attachment.Text( 200249 ) ); // "Time at Rank"

            // go through the sorted list and display the top ranked players

            int y = 40;
            int count = 0;
            for( int i = 0; i < XmlPointsAttach.RankList.Count; i++ )
            {
                if( count >= numberToDisplay )
                    break;

                RankEntry r = XmlPointsAttach.RankList[ i ];

                if( r == null )
                    continue;

                XmlPointsAttach a = r.PointsAttachment;

                if( a == null )
                    continue;

                if( r.Killer != null && !r.Killer.Deleted && r.Rank > 0 && !a.Deleted )
                {
                    string guildname = null;

                    if( r.Killer.Guild != null )
                        guildname = r.Killer.Guild.Abbreviation;

#if(FACTIONS)
						string factionname = null;
    
						if(r.Killer is PlayerMobile && ((PlayerMobile)r.Killer).FactionPlayerState != null) 
							factionname = ((PlayerMobile)r.Killer).FactionPlayerState.Faction.ToString();
#endif
                    // check for any ranking change and update rank date
                    if( r.Rank != a.Rank )
                    {
                        a.WhenRanked = DateTime.Now;
                        if( a.Rank > 0 )
                            a.DeltaRank = a.Rank - r.Rank;
                        a.Rank = r.Rank;
                    }

                    // check for guild filter
                    if( !String.IsNullOrEmpty( m_Attachment.m_GuildFilter ) )
                    {
                        // parse the comma separated list
                        string[] args = m_Attachment.m_GuildFilter.Split( ',' );
                        if( args != null )
                        {
                            bool found = false;
                            foreach( string arg in args )
                            {
                                if( arg != null && guildname == arg.Trim() )
                                {
                                    found = true;
                                    break;
                                }
                            }
                            if( !found )
                                continue;
                        }
                    }

                    // check for name filter
                    if( !String.IsNullOrEmpty( m_Attachment.m_NameFilter ) )
                    {
                        // parse the comma separated list
                        string[] args = m_Attachment.m_NameFilter.Split( ',' );

                        if( args != null )
                        {
                            bool found = false;
                            foreach( string arg in args )
                            {
                                if( arg != null && r.Killer.Name != null &&
                                    ( r.Killer.Name.ToLower().IndexOf( arg.Trim().ToLower() ) >= 0 ) )
                                {
                                    found = true;
                                    break;
                                }
                            }
                            if( !found )
                                continue;
                        }
                    }

                    count++;

                    TimeSpan timeranked = DateTime.Now - a.WhenRanked;

                    var days = (int)timeranked.TotalDays;
                    var hours = (int)( timeranked.TotalHours - days * 24 );
                    var mins = (int)( timeranked.TotalMinutes - ( (int)timeranked.TotalHours ) * 60 );

                    string kills = "???";
                    try
                    {
                        kills = a.Kills.ToString();
                    }
                    catch( Exception ex )
                    {
                        Console.WriteLine( ex.ToString() );
                    }

                    string deaths = "???";
                    try
                    {
                        deaths = a.Deaths.ToString();
                    }
                    catch( Exception ex )
                    {
                        Console.WriteLine( ex.ToString() );
                    }

                    xloc = 23;
                    AddLabel( xloc, y, 0, r.Killer.Name );
                    xloc += 177;
                    AddLabel( xloc, y, 0, guildname );
#if(FACTIONS)
						xloc += 35;
						AddLabelCropped( xloc, y, 60, 21, 0, factionname );
						xloc += 15;
#endif
                    xloc += 50;
                    AddLabel( xloc, y, 0, a.Points.ToString() );
                    xloc += 50;
                    AddLabel( xloc, y, 0, kills );
                    xloc += 50;
                    AddLabel( xloc, y, 0, deaths );
                    xloc += 70;
                    AddLabel( xloc, y, 0, a.Rank.ToString() );

                    string label = null;

                    if( days > 0 )
                        label += String.Format( attachment.Text( 200250 ), days ); // "{0} days "
                    if( hours > 0 )
                        label += String.Format( attachment.Text( 200251 ), hours ); // "{0} hours "
                    if( mins > 0 )
                        label += String.Format( attachment.Text( 200252 ), mins ); // "{0} mins"

                    if( label == null )
                    {
                        label = attachment.Text( 200253 ); // "just changed"
                    }

                    string deltalabel = a.DeltaRank.ToString();
                    int deltahue = 0;
                    if( a.DeltaRank > 0 )
                    {
                        deltalabel = String.Format( "+{0}", a.DeltaRank );
                        deltahue = 68;
                    }
                    else if( a.DeltaRank < 0 )
                    {
                        deltahue = 33;
                    }
                    xloc += 50;
                    AddLabel( xloc, y, deltahue, deltalabel );
                    xloc += 40;
                    AddLabel( xloc, y, 0, label );

                    y += 20;
                }
            }
        }

        public override void OnResponse( NetState state, RelayInfo info )
        {
            if( state == null || state.Mobile == null || info == null )
                return;
            // Get the current name
            if( m_Attachment != null )
            {
                TextRelay entry = info.GetTextEntry( 200 );
                if( entry != null )
                    m_Attachment.m_GuildFilter = entry.Text;

                entry = info.GetTextEntry( 100 );
                if( entry != null )
                    m_Attachment.m_NameFilter = entry.Text;
            }

            switch( info.ButtonID )
            {
                case 100:
                case 200:
                    {
                        // redisplay the gump
                        state.Mobile.SendGump( new TopPlayersGump( m_Attachment ) );
                        break;
                    }
            }
        }
    }
}