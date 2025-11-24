using System;
using System.Collections.Generic;
using System.IO;

namespace Midgard.Engines.AuctionSystem
{
    /// <summary>
    /// Provides access to localized text used by the system
    /// </summary>
    public class StringTable
    {
        public enum Language
        {
            Italian,
            English
        }

        private static readonly Language DefaultLanguage = Language.English;

        private static Dictionary<int, string> m_ItalianTable;
        private static Dictionary<int, string> m_EnglishTable;

        private static Dictionary<int, string> m_Table; // Default localization table

        public static void Configure()
        {
            m_Table = new Dictionary<int, string>();
            LoadStrings( @"Data/AuctionStrings.txt", ref m_Table );

            m_EnglishTable = new Dictionary<int, string>();
            LoadStrings( null, ref m_EnglishTable );

            m_ItalianTable = new Dictionary<int, string>();
            LoadStrings( @"Data/AuctionStrings.ita", ref m_ItalianTable );
        }

        private static void LoadStrings( string fileName, ref Dictionary<int, string> table )
        {
            if( fileName == null )
            {
                if( m_Table != null )
                    table = m_Table;
                else
                    Console.WriteLine( "Warning: auction localization default table called while null." );

                return;
            }

            if( File.Exists( fileName ) )
            {
                using( StreamReader ip = new StreamReader( fileName ) )
                {
                    string line;

                    while( ( line = ip.ReadLine() ) != null )
                    {
                        try
                        {
                            if( line.Length == 0 || line.StartsWith( "#" ) ) // ignore comments
                                continue;

                            int pos = line.LastIndexOf( "//" ); // trim lines with trailing '//'
                            if( pos > -1 )
                                line = line.Remove( pos );

                            string[] split = line.Split( '\t' );

                            int id = int.Parse( split[ 0 ] );

                            string tmp = line.Remove( 0, split[ 0 ].Length + 1 );
                            //Console.WriteLine( id + ' - ' + tmp );

                            tmp = tmp.Trim(); // trim leading and trailing spaces
                            tmp = tmp.Trim( '"' ); // trim leading and trailing " chars

                            if( split.Length > 2 )
                                Console.WriteLine( "Warning: auction localization tabbed line {0}", id );

                            table.Add( id, tmp );
                        }
                        catch
                        {
                            Console.WriteLine( "Warning: Invalid auction localization string:" );
                            Console.WriteLine( line );
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine( "Warning: Invalid auction localization path. File not found." );
            }
        }

        /// <summary>
        /// Gets the localized string for the Auction System
        /// </summary>
        public string this[ int index ]
        {
            get { return this[ index, DefaultLanguage ]; }
        }

        /// <summary>
        /// Gets the localized string for the Auction System with a given language
        /// </summary>
        public string this[ int index, Language language ]
        {
            get
            {
                string s;

                switch( language )
                {
                    case Language.Italian:
                        s = m_ItalianTable[ index ];
                        break;
                    case Language.English:
                        s = m_EnglishTable[ index ];
                        break;
                    default:
                        s = m_Table[ index ];
                        break;
                }

                if( string.IsNullOrEmpty( s ) )
                {
                    Console.WriteLine( "Warning: Auction System Localization Missing: {0} for language {1}", index, language );
                    return "";
                }

                return s;
            }
        }
    }
}