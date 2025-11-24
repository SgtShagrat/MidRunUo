using System;
using System.Collections.Generic;
using System.Reflection;
using Server.Mobiles;
using Server.Network;

/*
** Modified from RunUO 1.0.0 AddGump.cs
** by ArteGordon
** 3/13/05
*/

namespace Server.Gumps
{
    public class XmlPartialCategorizedAddGump : Gump
    {
        private string m_SearchString;
        private List<SearchEntry> m_SearchResults;
        private int m_Page;
        private Gump m_Gump;
        private int m_EntryIndex = -1;
        private XmlSpawner m_Spawner;

        public XmlPartialCategorizedAddGump( Mobile from, string searchString, int page, List<SearchEntry> searchResults,
                                            bool explicitSearch, int entryindex, Gump gump )
            : base( 50, 50 )
        {
            if( gump is XmlSpawnerGump )
            {
                // keep track of the spawner for xmlspawnergumps
                m_Spawner = ( (XmlSpawnerGump)gump ).Spawner;
            }

            // keep track of the gump
            m_Gump = gump;


            m_SearchString = searchString;
            m_SearchResults = searchResults;
            m_Page = page;

            m_EntryIndex = entryindex;

            from.CloseGump( typeof( XmlPartialCategorizedAddGump ) );

            AddPage( 0 );

            AddBackground( 0, 0, 420, 280, 5054 );

            AddImageTiled( 10, 10, 400, 20, 2624 );
            AddAlphaRegion( 10, 10, 400, 20 );
            AddImageTiled( 41, 11, 184, 18, 0xBBC );
            AddImageTiled( 42, 12, 182, 16, 2624 );
            AddAlphaRegion( 42, 12, 182, 16 );

            AddButton( 10, 9, 4011, 4013, 1, GumpButtonType.Reply, 0 );
            AddTextEntry( 44, 10, 180, 20, 0x480, 0, searchString );

            AddHtmlLocalized( 230, 10, 100, 20, 3010005, 0x7FFF, false, false );

            AddImageTiled( 10, 40, 400, 200, 2624 );
            AddAlphaRegion( 10, 40, 400, 200 );

            if( searchResults.Count > 0 )
            {
                for( int i = ( page * 10 ); i < ( ( page + 1 ) * 10 ) && i < searchResults.Count; ++i )
                {
                    int index = i % 10;

                    SearchEntry se = searchResults[ i ];

                    string labelstr = se.EntryType.Name;

                    if( se.Parameters.Length > 0 )
                    {
                        for( int j = 0; j < se.Parameters.Length; j++ )
                        {
                            labelstr += ", " + se.Parameters[ j ].Name;
                        }
                    }

                    AddLabel( 44, 39 + ( index * 20 ), 0x480, labelstr );
                    AddButton( 10, 39 + ( index * 20 ), 4023, 4025, 4 + i, GumpButtonType.Reply, 0 );
                }
            }
            else
            {
                AddLabel( 15, 44, 0x480, explicitSearch ? "Nothing matched your search terms." : "No results to display." );
            }

            AddImageTiled( 10, 250, 400, 20, 2624 );
            AddAlphaRegion( 10, 250, 400, 20 );

            if( m_Page > 0 )
                AddButton( 10, 249, 4014, 4016, 2, GumpButtonType.Reply, 0 );
            else
                AddImage( 10, 249, 4014 );

            AddHtmlLocalized( 44, 250, 170, 20, 1061028, m_Page > 0 ? 0x7FFF : 0x5EF7, false, false ); // Previous page

            if( ( ( m_Page + 1 ) * 10 ) < searchResults.Count )
                AddButton( 210, 249, 4005, 4007, 3, GumpButtonType.Reply, 0 );
            else
                AddImage( 210, 249, 4005 );

            AddHtmlLocalized( 244, 250, 170, 20, 1061027, ( ( m_Page + 1 ) * 10 ) < searchResults.Count ? 0x7FFF : 0x5EF7,
                             false, false ); // Next page
        }

        private static Type m_TypeofItem = typeof( Item ), m_TypeofMobile = typeof( Mobile );

        private static void Match( string match, Type[] types, ICollection<SearchEntry> results )
        {
            if( match.Length == 0 )
                return;

            match = match.ToLower();

            for( int i = 0; i < types.Length; ++i )
            {
                Type t = types[ i ];

                foreach( SearchEntry entry in results )
                {
                    if( entry.EntryType == t )
                        continue;
                }

                if( ( m_TypeofMobile.IsAssignableFrom( t ) || m_TypeofItem.IsAssignableFrom( t ) ) &&
                    t.Name.ToLower().IndexOf( match ) >= 0 )
                {
                    ConstructorInfo[] ctors = t.GetConstructors();

                    for( int j = 0; j < ctors.Length; ++j )
                    {
                        if( /*ctors[j].GetParameters().Length == 0 && */
                            ctors[ j ].IsDefined( typeof( ConstructableAttribute ), false ) )
                        {
                            var s = new SearchEntry();
                            s.EntryType = t;
                            s.Parameters = ctors[ j ].GetParameters();
                            results.Add( s );
                        }
                    }
                }
            }
        }

        public static List<SearchEntry> Match( string match )
        {
            var results = new List<SearchEntry>();
            Type[] types;

            Assembly[] asms = ScriptCompiler.Assemblies;

            for( int i = 0; i < asms.Length; ++i )
            {
                types = ScriptCompiler.GetTypeCache( asms[ i ] ).Types;
                Match( match, types, results );
            }

            types = ScriptCompiler.GetTypeCache( Core.Assembly ).Types;
            Match( match, types, results );

            results.Sort( new TypeNameComparer() );

            return results;
        }

        private class TypeNameComparer : IComparer<SearchEntry>
        {
            public int Compare( SearchEntry x, SearchEntry y )
            {
                SearchEntry a = x;
                SearchEntry b = y;

                return a.EntryType.Name.CompareTo( b.EntryType.Name );
            }
        }


        public override void OnResponse( NetState sender, RelayInfo info )
        {
            Mobile from = sender.Mobile;

            switch( info.ButtonID )
            {
                case 1: // Search
                    {
                        TextRelay te = info.GetTextEntry( 0 );
                        string match = ( te == null ? "" : te.Text.Trim() );

                        if( match.Length < 3 )
                        {
                            from.SendMessage( "Invalid search string." );
                            from.SendGump( new XmlPartialCategorizedAddGump( from, match, m_Page, m_SearchResults, false,
                                                                           m_EntryIndex, m_Gump ) );
                        }
                        else
                        {
                            from.SendGump( new XmlPartialCategorizedAddGump( from, match, 0, Match( match ), true,
                                                                           m_EntryIndex, m_Gump ) );
                        }

                        break;
                    }
                case 2: // Previous page
                    {
                        if( m_Page > 0 )
                            from.SendGump( new XmlPartialCategorizedAddGump( from, m_SearchString, m_Page - 1,
                                                                           m_SearchResults, true, m_EntryIndex, m_Gump ) );

                        break;
                    }
                case 3: // Next page
                    {
                        if( ( m_Page + 1 ) * 10 < m_SearchResults.Count )
                            from.SendGump( new XmlPartialCategorizedAddGump( from, m_SearchString, m_Page + 1,
                                                                           m_SearchResults, true, m_EntryIndex, m_Gump ) );

                        break;
                    }
                default:
                    {
                        int index = info.ButtonID - 4;

                        if( index >= 0 && index < m_SearchResults.Count )
                        {
                            Type type = m_SearchResults[ index ].EntryType;

                            if( m_Gump is XmlAddGump && type != null )
                            {
                                var mXmlAddGump = (XmlAddGump)m_Gump;
                                if( mXmlAddGump.defs != null && mXmlAddGump.defs.NameList != null &&
                                    m_EntryIndex >= 0 && m_EntryIndex < mXmlAddGump.defs.NameList.Length )
                                {
                                    mXmlAddGump.defs.NameList[ m_EntryIndex ] = type.Name;
                                    XmlAddGump.Refresh( from, true );
                                }
                            }
                            else if( m_Spawner != null && type != null )
                            {
                                XmlSpawnerGump xg = m_Spawner.SpawnerGump;

                                if( xg != null )
                                {
                                    xg.Rentry = new XmlSpawnerGump.ReplacementEntry();
                                    xg.Rentry.Typename = type.Name;
                                    xg.Rentry.Index = m_EntryIndex;
                                    xg.Rentry.Color = 0x1436;

                                    Timer.DelayCall( TimeSpan.Zero,
                                                    new TimerStateCallback( XmlSpawnerGump.Refresh_Callback ),
                                                    new object[] { from } );
                                    //from.CloseGump(typeof(XmlSpawnerGump));
                                    //from.SendGump( new XmlSpawnerGump(xg.m_Spawner, xg.X, xg.Y, xg.m_ShowGump, xg.xoffset, xg.page, xg.Rentry) );
                                }
                            }
                        }

                        break;
                    }
            }
        }
    }

    public class SearchEntry
    {
        public Type EntryType;
        public ParameterInfo[] Parameters;
    }
}