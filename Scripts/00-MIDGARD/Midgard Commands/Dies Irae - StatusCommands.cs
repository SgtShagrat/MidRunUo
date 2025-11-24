/***************************************************************************
 *                                   StatusCommands.cs
 *                            		-------------------
 *  begin                	: Gennaio, 2007
 *  version					: 1.0
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 * 
 * 	Info:	
 * 	
 *  
 ***************************************************************************/

using System;
using System.Collections.Generic;

using Midgard.Engines.MidgardTownSystem;

using Server;
using Server.Accounting;
using Server.Commands;
using Server.Guilds;
using Server.Gumps;
using Server.Items;
using Server.Misc;
using Server.Mobiles;
using Server.Network;

using Notoriety = Server.Notoriety;

namespace Midgard.Gumps
{
    public class MidgardStatusEngine
    {
        public static void Initialize()
        {
            CommandSystem.Register( "StaffOnLine", AccessLevel.Player, new CommandEventHandler( SStatus_OnCommand ) );
            CommandSystem.Register( "ServerStatus", AccessLevel.Player, new CommandEventHandler( Status_OnCommand ) );
            CommandSystem.Register( "SStaff", AccessLevel.Player, new CommandEventHandler( SStatus_OnCommand ) );
            CommandSystem.Register( "SStat", AccessLevel.Player, new CommandEventHandler( Status_OnCommand ) );
        }

        [Aliases( "SStaff" )]
        [Usage( "StaffOnLine or SStaff" )]
        [Description( "Apre un gump con la lista dei player staff online" )]
        public static void SStatus_OnCommand( CommandEventArgs e )
        {
            Mobile from = e.Mobile;
            if( from != null )
            {
                if( e.Length == 0 )
                    from.SendGump( new SStatusGump( from ) );
                else
                    from.SendMessage( "Command use: [SStaff or [StaffOnLine" );
            }
        }

        [Aliases( "SStat" )]
        [Usage( "ServerStatus or Sstat" )]
        [Description( "Apre un gump con informazioni sul server e sui giocatori online" )]
        private static void Status_OnCommand( CommandEventArgs e )
        {
            Mobile from = e.Mobile;
            if( from != null )
            {
                if( e.Length == 0 )
                    from.SendGump( new StatusGump( from ) );
                else
                    from.SendMessage( "Command use: [ServerStatus or [Sstat" );
            }
        }

        public class SStatusGump : Gump
        {
            private const int Fields = 7;

            private readonly List<Mobile> m_Mobiles;
            private int m_Page;

            public SStatusGump( Mobile owner )
                : this( owner, BuildList(), 1 )
            {
            }

            public SStatusGump( Mobile owner, List<Mobile> list, int page )
                : base( 50, 50 )
            {
                Closable = true;
                Disposable = true;
                Dragable = true;
                Resizable = false;

                owner.CloseGump( typeof( SStatusGump ) );

                m_Mobiles = list;

                Design( page );
            }

            public void Design( int page )
            {
                m_Page = page;

                AddPage( 0 );
                AddBackground( 0, 0, 320, 240, 0x53 );

                AddImageTiled( 15, 15, 290, 17, 5154 );
                AddHtml( 15, 15, 290, 17, "<div align=\"center\" color=\"2100\">" + ServerList.ServerName + " Staff Online " + "</div>", false, false );

                AddBackground( 15, 39, 290, 186, 0x2454 );
                AddBlackAlpha( 18, 42, 283, 180 );

                if( m_Mobiles.Count == 0 )
                    AddLabel( 20, 60, 0x25, "Non ci sono Pg Staff online." );

                // Pagina precedente
                if( m_Page > 1 )
                    AddButton( 264, 42, 0x15E3, 0x15E7, 200, GumpButtonType.Reply, 0 );
                else
                    AddImage( 264, 42, 0x25EA );

                // Pagina successiva
                if( m_Page * Fields < m_Mobiles.Count )
                    AddButton( 281, 42, 0x15E1, 0x15E5, 300, GumpButtonType.Reply, 0 );
                else
                    AddImage( 281, 42, 0x25E6 );

                int indMax = ( m_Page * Fields ) - 1;
                int indMin = ( m_Page * Fields ) - Fields;
                int indTemp = 0;

                for( int i = 0; i < m_Mobiles.Count; ++i )
                {
                    if( i >= indMin && i <= indMax )
                    {
                        Mobile m = m_Mobiles[ i ];
                        AddLabelCropped( 25, 60 + ( indTemp * 20 ), 220, 20, GetHueFor( m ), m.AccessLevel + " " + m.Name );
                        indTemp++;
                    }
                }
            }

            public override void OnResponse( NetState sender, RelayInfo info )
            {
                Mobile from = sender.Mobile;

                if( info.ButtonID == 200 ) // Pagina precedente
                {
                    m_Page--;
                    from.SendGump( new SStatusGump( from, m_Mobiles, m_Page ) );
                    return;
                }
                else if( info.ButtonID == 300 ) // Pagina successiva
                {
                    m_Page++;
                    from.SendGump( new SStatusGump( from, m_Mobiles, m_Page ) );
                    return;
                }
            }

            public void AddBlackAlpha( int x, int y, int width, int height )
            {
                AddImageTiled( x, y, width, height, 2624 );
                AddAlphaRegion( x, y, width, height );
            }

            private static int GetHueFor( Mobile m )
            {
                switch( m.AccessLevel )
                {
                    case AccessLevel.Owner:
                    case AccessLevel.Developer:
                    case AccessLevel.Administrator: return 0x516;
                    case AccessLevel.Seer: return 0x144;
                    case AccessLevel.GameMaster: return 0x21;
                    case AccessLevel.Counselor: return 0x2;
                    default: return 0x58;
                }
            }

            public static List<Mobile> BuildList()
            {
                List<Mobile> list = new List<Mobile>();
                List<NetState> states = NetState.Instances;

                foreach( NetState netState in states )
                {
                    Mobile m = netState.Mobile;
                    if( m != null && m is Midgard2PlayerMobile && ( (Midgard2PlayerMobile)m ).OnlineVisible && m.AccessLevel > AccessLevel.Player )
                        list.Add( m );
                }

                return list;
            }
        }

        public class StatusGump : Gump
        {
            private const int Fields = 25;

            private readonly Mobile m_Owner;
            private readonly List<Mobile> m_Mobiles;

            private int m_Page;

            public StatusGump( Mobile owner )
                : this( owner, BuildList(), 1 )
            {
            }

            public StatusGump( Mobile owner, List<Mobile> list, int page )
                : base( 50, 50 )
            {
                Closable = true;
                Disposable = true;
                Dragable = true;
                Resizable = false;

                owner.CloseGump( typeof( StatusGump ) );

                m_Owner = owner;
                m_Mobiles = list;

                Design( page );
            }

            private const int NameOffset = 20;
            private const int TownOffset = 200;
            private const int GuildOffset = 350;
            private const int KarmaOffset = 550;
            private const int FameOffset = 600;
            private const int SumSkillsOffset = 650;
            private const int SumStatsOffset = 700;
            private const int KillsOffset = 750;

            public void Design( int page )
            {
                m_Page = page;

                AddPage( 0 );

                AddBackground( 0, 0, 800, 600, 0x53 );

                AddImageTiled( 15, 15, 770, 17, 5154 );
                AddHtml( 15, 15, 770, 17, "<div align=\"center\" color=\"2100\">" + ServerList.ServerName + "</div>", false, false );

                AddImageTiled( 15, 37, 190, 17, 5154 );
                AddLabel( 17, 36, 0x25, "Online :" );
                AddHtml( 160, 37, 30, 17, "<div align=\"right\" color=\"2100\">" + NetState.Instances.Count + "</div>", false, false );

                AddImageTiled( 210, 37, 190, 17, 5154 );
                AddLabel( 212, 36, 0x68, "Accounts :" );
                AddHtml( 357, 37, 30, 17, "<div align=\"right\" color=\"2100\">" + Accounts.Count + "</div>", false, false );

                AddImageTiled( 405, 37, 190, 17, 5154 );
                AddLabel( 407, 36, 2100, "Uptime :" );
                AddHtml( 485, 37, 109, 17, "<div align=\"right\" color=\"2100\">" + FormatTimeSpan( DateTime.Now - Clock.ServerStart ) + "</div>", false, false );

                AddImageTiled( 600, 37, 185, 17, 5154 );
                AddLabel( 602, 36, 2100, "RAM in use :" );
                AddHtml( 700, 37, 75, 17, "<div align=\"right\" color=\"2100\">" + FormatByteAmount( GC.GetTotalMemory( false ) ) + "</div>", false, false );

                AddBackground( 15, 59, 770, 526, 0x2454 );
                AddBlackAlpha( 18, 62, 763, 520 );
                AddLabelCropped( NameOffset, 60, 220, 20, 2100, "Name" );
                AddLabelCropped( TownOffset, 60, 209, 20, 2100, "Town" );
                AddLabelCropped( GuildOffset, 60, 209, 20, 2100, "Guild" );
                AddLabelCropped( KarmaOffset, 60, 60, 20, 2100, "Karma" );
                AddLabelCropped( FameOffset, 60, 60, 20, 2100, "Fame" );

                if( m_Owner.AccessLevel >= AccessLevel.GameMaster )
                {
                    AddLabelCropped( SumStatsOffset, 60, 60, 20, 2100, "Stats" );
                    AddLabelCropped( SumSkillsOffset, 60, 60, 20, 2100, "Skills" );
                    AddLabelCropped( KillsOffset, 60, 60, 20, 2100, "Kills" );
                }

                // Pagina precedente
                if( m_Page > 1 )
                    AddButton( 744, 62, 0x15E3, 0x15E7, 200, GumpButtonType.Reply, 0 );
                else
                    AddImage( 744, 62, 0x25EA );

                // Pagina successiva
                if( m_Page * Fields < m_Mobiles.Count )
                    AddButton( 761, 62, 0x15E1, 0x15E5, 300, GumpButtonType.Reply, 0 );
                else
                    AddImage( 761, 62, 0x25E6 );

                // Potrebbero non esserci clien connessi ma remoti
                if( m_Mobiles.Count == 0 )
                    AddLabel( 20, 80, 0x25, "There are no players online at the moment." );

                int indMax = ( m_Page * Fields ) - 1;
                int indMin = ( m_Page * Fields ) - Fields;
                int indTemp = 0;

                for( int i = 0; i < m_Mobiles.Count; ++i )
                {
                    if( i >= indMin && i <= indMax )
                    {
                        Midgard2PlayerMobile m = m_Mobiles[ i ] as Midgard2PlayerMobile;

                        m = FindAliasOrRealPlayer( m );

                        if( m == null )
                        {
                            AddLabelCropped( 20, 80 + ( indTemp * 20 ), 220, 20, 2100, "(logging in)" );
                        }
                        else if( m.AccessLevel > AccessLevel.Player && m.OnlineVisible )
                        {
                            AddLabelCropped( 20, 80 + ( indTemp * 20 ), 220, 20, GetHueFor( m_Owner, m ), string.Format( "{0} {1}", m.AccessLevel, m.Name ) );
                        }
                        else if( m.AccessLevel == AccessLevel.Player )
                        {
                            AddLabelCropped( NameOffset, 80 + ( indTemp * 20 ), 150, 20, GetHueFor( m_Owner, m ), m.Name );

                            TownSystem system = TownSystem.Find( m );
                            if( system != null )
                                AddLabelCropped( TownOffset, 80 + ( indTemp * 20 ), 150, 20, 2100, system.Definition.TownName );

                            Guild g = m.Guild as Guild; // La gilda e il titolo vengono mostrati a tutti
                            if( g != null )
                            {
                                string title = m.GuildTitle;
                                string formattedTitle = "[";

                                if( title != null )
                                    title = title.Trim();
                                else
                                    title = "";
                                if( title.Length > 0 )
                                    formattedTitle = formattedTitle + title + ",";

                                formattedTitle = formattedTitle + g.Abbreviation + "]";

                                AddLabelCropped( GuildOffset, 80 + ( indTemp * 20 ), 150, 20, 2100, formattedTitle );
                            }

                            AddLabelCropped( KarmaOffset, 80 + ( indTemp * 20 ), 50, 20, 2100, m.Karma.ToString() );
                            AddLabelCropped( FameOffset, 80 + ( indTemp * 20 ), 50, 20, 2100, m.Fame.ToString() );

                            if( m_Owner.AccessLevel >= AccessLevel.GameMaster )
                            {
                                AddLabelCropped( SumStatsOffset, 80 + ( indTemp * 20 ), 50, 20, 2100, m.RawStatTotal.ToString() );
                                AddLabelCropped( SumSkillsOffset, 80 + ( indTemp * 20 ), 50, 20, 2100, m.SkillsTotal.ToString() );
                                AddLabelCropped( KillsOffset, 80 + ( indTemp * 20 ), 50, 20, 2100, m.Kills.ToString() );
                            }
                        }

                        indTemp++;
                    }
                }
            }

            public Midgard2PlayerMobile FindAliasOrRealPlayer( Midgard2PlayerMobile m )
            {
                if( m == null )
                    return null;

                if( m.Alias != null && m.Alias is Midgard2PlayerMobile )
                    return (Midgard2PlayerMobile)m.Alias;

                return m;
            }

            public void AddBlackAlpha( int x, int y, int width, int height )
            {
                AddImageTiled( x, y, width, height, 2624 );
                AddAlphaRegion( x, y, width, height );
            }

            public static string FormatTimeSpan( TimeSpan ts )
            {
                return String.Format( "{0:D2}:{1:D2}:{2:D2}:{3:D2}", ts.Days, ts.Hours % 24, ts.Minutes % 60, ts.Seconds % 60 );
            }

            public static string FormatByteAmount( long totalBytes )
            {
                if( totalBytes > 1000000000 )
                    return String.Format( "{0:F1} GB", (double)totalBytes / 1073741824 );

                if( totalBytes > 1000000 )
                    return String.Format( "{0:F1} MB", (double)totalBytes / 1048576 );

                if( totalBytes > 1000 )
                    return String.Format( "{0:F1} KB", (double)totalBytes / 1024 );

                return String.Format( "{0} Bytes", totalBytes );
            }

            private static int GetHueFor( Mobile owner, Mobile m )
            {
                switch( m.AccessLevel )
                {
                    case AccessLevel.Owner:
                    case AccessLevel.Developer:
                    case AccessLevel.Administrator: return 0x516;
                    case AccessLevel.Seer: return 0x144;
                    case AccessLevel.GameMaster: return 0x21;
                    case AccessLevel.Counselor: return 0x2;
                    case AccessLevel.Player: return Notoriety.GetHue( Notoriety.Compute( owner, m ) );
                    default: return 0x58;
                }
            }

            private static List<Mobile> BuildList()
            {
                List<Mobile> list = new List<Mobile>();
                List<NetState> states = NetState.Instances;

                foreach( NetState t in states )
                {
                    Mobile m = t.Mobile;

                    if( m != null && m is Midgard2PlayerMobile && ( (Midgard2PlayerMobile)m ).OnlineVisible )
                        list.Add( m );
                }

                list.Sort( InternalComparer.Instance );

                return list;
            }

            public override void OnResponse( NetState sender, RelayInfo info )
            {
                Mobile from = sender.Mobile;

                if( info.ButtonID == 200 ) // Pagina precedente
                {
                    m_Page--;
                    from.SendGump( new StatusGump( from, m_Mobiles, m_Page ) );
                    return;
                }
                else if( info.ButtonID == 300 ) // Pagina successiva
                {
                    m_Page++;
                    from.SendGump( new StatusGump( from, m_Mobiles, m_Page ) );
                    return;
                }
            }

            private class InternalComparer : IComparer<Mobile>
            {
                public static readonly IComparer<Mobile> Instance = new InternalComparer();

                private InternalComparer()
                {
                }

                public int Compare( Mobile x, Mobile y )
                {
                    if( x == null || y == null )
                        throw new ArgumentException();

                    if( x.AccessLevel > y.AccessLevel )
                        return -1;
                    else if( x.AccessLevel < y.AccessLevel )
                        return 1;
                    else
                        return Insensitive.Compare( x.Name, y.Name );
                }
            }
        }
    }
}