/***************************************************************************
 *                               Dies Irae - OnlineGuidesSystem.cs
 *
 *   begin                : 13 marzo 2010
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;

using Server;
using Server.Commands;
using Server.Gumps;
using Server.Network;

namespace Midgard.Engines.OnlineGuides
{
    public class Config
    {
        public static bool Enabled
        {
            get
            {
                return Packager.Core.Singleton[ typeof( Config ) ].Enabled;
            }
            set
            {
                Packager.Core.Singleton[ typeof( Config ) ].Enabled = value;
            }
        }

        public static bool Debug = false;

        public static object[] Package_Info = {
                                                  "Script Title",             "Online Guides System",
                                                  "Enabled by Default",       true,
                                                  "Script Version",           new Version(1,0,0,0),
                                                  "Author name",              "Dies Irae", 
                                                  "Creation Date",            new DateTime(2007, 1, 1), 
                                                  "Author mail-contact",      "tocasia@alice.it", 
                                                  "Author home site",         "http://www.midgardshard.it",
                                                  //"Author notes",           null,
                                                  "Script Copyrights",        "(C) Midgard Shard - Dies Irae",
                                                  "Provided packages",        new string[]{"Midgard.Engines.OnlineGuides"},
                                                  //"Required packages",      new string[0],
                                                  //"Conflicts with packages",new string[0],
                                                  "Research tags",            new string[]{"OnlineGuides"}
                                              };

        public static void Package_Initialize()
        {
            if( Enabled )
            {
                Core.RegisterCommands();
            }
        }
    }

    public class Core
    {
        public static void RegisterCommands()
        {
            CommandSystem.Register( "Sito", AccessLevel.Player, new CommandEventHandler( OnLineGuide_OnCommand ) );
            CommandSystem.Register( "Forum", AccessLevel.Player, new CommandEventHandler( LaunchForum_OnCommand ) );
        }

        [Usage( "Sito" )]
        [Description( "Lancia il proprio browser web alla pagina scelta" )]
        private static void OnLineGuide_OnCommand( CommandEventArgs e )
        {
            e.Mobile.SendGump( new SiteSelectionGump( e.Mobile, BuildList( m_GuideTable, e.Mobile ) ) );
            e.Mobile.SendMessage( "Select the guide you wish to read." );
        }

        [Usage( "Forum" )]
        [Description( "Lancia il proprio browser web alla pagina scelta del forum di Midgard" )]
        private static void LaunchForum_OnCommand( CommandEventArgs e )
        {
            e.Mobile.SendGump( new SiteSelectionGump( e.Mobile, BuildList( m_ForumTable, e.Mobile ) ) );
            e.Mobile.SendMessage( "Select the Midgard forum page you wish to read." );
        }

        private static SiteDefinition[] m_GuideTable = new SiteDefinition[]
                                                       {
                                                            new SiteDefinition ( "Status", "http://www.midgardshard.it/index.php?q=ingamestatus"),
                                                            new SiteDefinition ( "Razze", "http://www.midgardshard.it/index.php?q=node/25"),
                                                            new SiteDefinition ( "Classi", "http://www.midgardshard.it/index.php?q=node/38"),
                                                            new SiteDefinition ( "Background", "http://www.midgardshard.it/index.php?q=node/12"),
                                                            new SiteDefinition ( "Citta'", "http://www.midgardshard.it/index.php?q=node/82"),
                                                            new SiteDefinition ( "Dungeon", "http://www.midgardshard.it/index.php?q=node/73"),
                                                            new SiteDefinition ( "Fazioni", "http://www.midgardshard.it/index.php?q=node/21"),
                                                            new SiteDefinition ( "Gilde", "http://www.midgardshard.it/index.php?q=node/24"),
                                                            new SiteDefinition ( "News", "http://www.midgardshard.it/index.php?q=home_news")
                                                       };

        private static SiteDefinition[] m_ForumTable = new SiteDefinition[]
                                                       {
                                                           new SiteDefinition( "La Taverna", "http://www.mad4games.it/forum/forumdisplay.php?f=32"),
                                                           new SiteDefinition( "Il nuovo Mondo", "http://www.mad4games.it/forum/forumdisplay.php?f=114"),
                                                           new SiteDefinition( "Fiera di Midgard", "http://www.mad4games.it/forum/forumdisplay.php?f=115"), 
                                                           new SiteDefinition( "Il Tempio dell'Oracolo", "http://www.mad4games.it/forum/forumdisplay.php?f=116"), 
                                                           new SiteDefinition( "Incontro con i Creatori", "http://www.mad4games.it/forum/forumdisplay.php?f=117"),
                                                       };

        private static SiteDefinition TanaDeiConsoliDefinition = new SiteDefinition( "La sala dei Consoli", "http://www.mad4games.it/forum/forumdisplay.php?f=364" );
        private static SiteDefinition GenesiDefinition = new SiteDefinition( "Genesi", "http://www.mad4games.it/forum/forumdisplay.php?f=118" );
        private static SiteDefinition SignoriDelfatoDefinition = new SiteDefinition( "I Signori del Fato", "http://www.mad4games.it/forum/forumdisplay.php?f=121" );
        private static SiteDefinition AntroDefinition = new SiteDefinition( "Antro di Midgard", "http://www.mad4games.it/forum/forumdisplay.php?f=122" );
        private static SiteDefinition FucinaDefinition = new SiteDefinition( "La Fucina del Demiurgo", "http://www.mad4games.it/forum/forumdisplay.php?f=123" );
        private static SiteDefinition CreatoriDefinition = new SiteDefinition( "I Creatori", "http://www.mad4games.it/forum/forumdisplay.php?f=124" );
        private static SiteDefinition CollaboratoriDefinition = new SiteDefinition( "Forum Collaboratori", "http://www.mad4games.it/forum/forumdisplay.php?f=489" );
        private static SiteDefinition AreaWebDefinition = new SiteDefinition( "Area Web", "http://www.mad4games.it/forum/forumdisplay.php?f=488" );

        private static SiteDefinition[] BuildList( IEnumerable<SiteDefinition> playerlist, Mobile from )
        {
            List<SiteDefinition> list = new List<SiteDefinition>( playerlist );

            if( playerlist == m_ForumTable )
            {
                if( from.AccessLevel >= AccessLevel.Counselor )
                {
                    list.Add( TanaDeiConsoliDefinition );
                    list.Add( AntroDefinition );
                    list.Add( CollaboratoriDefinition );
                    list.Add( AreaWebDefinition );
                }

                if( from.AccessLevel >= AccessLevel.Seer )
                {
                    list.Add( GenesiDefinition );
                    list.Add( SignoriDelfatoDefinition );
                }

                if( from.AccessLevel >= AccessLevel.Administrator )
                {
                    list.Add( FucinaDefinition );
                    list.Add( CreatoriDefinition );
                }
            }

            return list.ToArray();
        }

        internal class SiteDefinition
        {
            public string Description { get; set; }
            public string Url { get; set; }

            public SiteDefinition( string description, string url )
            {
                Description = description;
                Url = url;
            }

            public void Launch( Mobile m )
            {
                m.LaunchBrowser( Url );
            }
        }

        internal class SiteSelectionGump : Gump
        {
            private readonly SiteDefinition[] m_Table;

            private const int Fields = 9;
            private const int HueTit = 662;
            private const int DeltaBut = 2;
            private const int FieldsDist = 25;
            private const int HuePrim = 92;

            private int m_Page;
            private readonly Mobile m_From;

            public SiteSelectionGump( Mobile from, SiteDefinition[] table )
                : this( from, table, 1 )
            {
            }

            private SiteSelectionGump( Mobile from, SiteDefinition[] table, int page )
                : base( 50, 50 )
            {
                Closable = false;
                Disposable = true;
                Dragable = true;
                Resizable = false;

                m_From = from;
                m_Page = page;
                m_Table = table;

                Design();
            }

            private enum Buttons
            {
                Close = 0,

                PrevPage = 200,
                NextPage = 300
            }

            private void Design()
            {
                AddPage( 0 );

                AddBackground( 0, 0, 275, 325, 9200 );

                AddImageTiled( 10, 10, 255, 25, 2624 );
                AddImageTiled( 10, 45, 255, 240, 2624 );
                AddImageTiled( 40, 295, 225, 20, 2624 );

                AddButton( 10, 295, 4017, 4018, 0, GumpButtonType.Reply, 0 );
                AddHtmlLocalized( 45, 295, 75, 20, 1011012, HueTit, false, false ); // CANCEL

                AddAlphaRegion( 10, 10, 255, 285 );
                AddAlphaRegion( 40, 295, 225, 20 );

                AddLabelCropped( 14, 12, 255, 25, HueTit, "Make your choice:" );

                if( m_Page > 1 )
                    AddButton( 225, 297, 5603, 5607, (int)Buttons.PrevPage, GumpButtonType.Reply, 0 );

                if( m_Page < Math.Ceiling( m_Table.Length / (double)Fields ) )
                    AddButton( 245, 297, 5601, 5605, (int)Buttons.NextPage, GumpButtonType.Reply, 0 );

                int indMax = ( m_Page * Fields ) - 1;
                int indMin = ( m_Page * Fields ) - Fields;
                int indTemp = 0;

                for( int i = 0; i < m_Table.Length; i++ )
                {
                    if( i >= indMin && i <= indMax )
                    {
                        AddLabelCropped( 35, 52 + ( indTemp * FieldsDist ), 225, 20, HuePrim, m_Table[ i ].Description );
                        AddButton( 15, 52 + DeltaBut + ( indTemp * FieldsDist ), 1209, 1210, i + 1, GumpButtonType.Reply, 0 );
                        indTemp++;
                    }
                }
            }

            public override void OnResponse( NetState sender, RelayInfo info )
            {
                Mobile from = sender.Mobile;

                if( info.ButtonID == 0 )
                    return;
                else if( info.ButtonID == (int)Buttons.PrevPage )
                {
                    m_Page--;
                    from.SendGump( new SiteSelectionGump( m_From, m_Table, m_Page ) );
                }
                else if( info.ButtonID == (int)Buttons.NextPage )
                {
                    m_Page++;
                    from.SendGump( new SiteSelectionGump( m_From, m_Table, m_Page ) );
                }
                else
                {
                    int index = info.ButtonID - 1;
                    if( index > -1 && index < m_Table.Length )
                        m_Table[ index ].Launch( from );
                }
            }
        }
    }
}