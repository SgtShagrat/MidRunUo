using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Server.Network;

namespace Server
{
    public enum LanguageType
    {
        Eng,
        Ita
    }

    public enum MessageHues
    {
        Grey = 0x3B2,
        Red = 0x25,
        Yellow = 0x35,
        MiscOne = 0x482,
        MiscTwo = 0x22,
        MiscThree = 0x2B2,
        MiscFour = 0x24
    }

    public class TextHelper
    {
        private static Dictionary<LanguageType, Dictionary<int, string>> m_TextDictionary = new Dictionary<LanguageType, Dictionary<int, string>>();

        public static LanguageType SystemLanguage = LanguageType.Eng; // system default language setting
        public static LanguageType ItalianLanguage = LanguageType.Ita;

        public static void LoadLocalization( string fileName )
        {
            string filePath = Path.Combine( Core.BaseDirectory, Path.Combine( "Data", Path.Combine( "Localization", fileName ) ) );

            if( !File.Exists( filePath ) )
            {
                Console.WriteLine( "Warning: file {0} not found", filePath );
                return;
            }

            using( StreamReader ip = new StreamReader( filePath ) )
            {
                string line;
                int i = 0;

                while( ( line = ip.ReadLine() ) != null )
                {
                    if( line.StartsWith( "#" ) || line.StartsWith( "//" ) )
                        continue;

                    string[] split = line.Split( '\t' );

                    try
                    {
                        LanguageType lan = (LanguageType)Enum.Parse( typeof( LanguageType ), split[ 0 ], true );
                        int index = Convert.ToInt32( split[ 1 ] );
                        string text = split[ 2 ].Trim();

                        AddText( lan, index, text );
                        i++;
                    }
                    catch
                    {
                        Console.WriteLine( "Could not parse line '{0}'.", line );
                    }
                }

                Console.WriteLine( "Info: {0} lines found in '{1}' localization file.", i, fileName );
            }
        }

        public static void SendDict()
        {
            StringBuilder sb = new StringBuilder();

            foreach( KeyValuePair<LanguageType, Dictionary<int, string>> kvp in m_TextDictionary )
            {
                foreach( KeyValuePair<int, string> dict in kvp.Value )
                    sb.AppendLine( string.Format( "{0} - {1}", dict.Key, dict.Value ) );
            }

            Console.WriteLine( sb.ToString() );
        }

        public static void AddText( LanguageType language, int index, string text )
        {
            // init the main dictionary of languages-stringLists
            if( m_TextDictionary == null )
                m_TextDictionary = new Dictionary<LanguageType, Dictionary<int, string>>();

            // init a new stringList for this language
            if( !m_TextDictionary.ContainsKey( language ) )
                m_TextDictionary[ language ] = new Dictionary<int, string>();

            Dictionary<int, string> stringList = m_TextDictionary[ language ];
            if( !stringList.ContainsKey( index ) )
                stringList.Add( index, text );
            else
            {
                Console.WriteLine( "Warning: duplicate insertion in class system text dictionary." );
                Console.WriteLine( "\t{0} - {1}", index, text );
            }
        }

        public static string DefaultText( int index )
        {
            Dictionary<int, string> stringList = m_TextDictionary[ SystemLanguage ];

            if( stringList == null || !stringList.ContainsKey( index ) )
            {
                stringList = m_TextDictionary[ ItalianLanguage ];
                if( stringList == null || !stringList.ContainsKey( index ) )
                {
                    LogError( index );
                    return string.Empty;
                }
            }

            return stringList[ index ];
        }

        public static string Text( int index, LanguageType language )
        {
            if( language == SystemLanguage )
                return DefaultText( index );
            else
            {
                Dictionary<int, string> stringList = m_TextDictionary.ContainsKey( language ) ? m_TextDictionary[ language ] : null;

                if( stringList == null || !stringList.ContainsKey( index ) )
                    return DefaultText( index );
                else
                    return stringList[ index ];
            }
        }

        public static void LogError( int number )
        {
            string message = String.Format( "Warning: Error in ClassSystem text. Entry {0}.", number );

            Console.WriteLine( message );

            try
            {
                using( StreamWriter op = new StreamWriter( "Logs/classText-errors.log", true ) )
                {
                    op.WriteLine( "{0}\t{1}", DateTime.Now, message );
                    op.WriteLine( new StackTrace( 2 ).ToString() );
                    op.WriteLine();
                }
            }
            catch( Exception ex )
            {
                Console.WriteLine( ex.ToString() );
            }
        }

        public static void SendLangMessage( Mobile to, LanguageType language, int index, params object[] args )
        {
            string format = Text( index, language );

            if( !string.IsNullOrEmpty( format ) )
            {
                string message = string.Format( format, args );

                to.SendAsciiMessage( message );
            }
            else
                LogError( index );
        }

        public static void SendLangMessage( Mobile to, LanguageType language, int index )
        {
            string message = Text( index, language );

            if( message != null )
                to.SendAsciiMessage( message );
            else
                LogError( index );
        }

        public static void SendLangMessage( Mobile to, LanguageType language, MessageHues hue, int index, params object[] args )
        {
            string format = Text( index, language );

            if( !string.IsNullOrEmpty( format ) )
            {
                string message = string.Format( format, args );

                to.SendAsciiMessage( (int)hue, message );
            }
            else
                LogError( index );
        }

        public static void SendLangMessage( Mobile to, LanguageType language, MessageHues hue, int index )
        {
            string message = Text( index, language );

            if( message != null )
                to.SendAsciiMessage( (int)hue, message );
            else
                LogError( index );
        }

        public static void SendLocalizedMessageTo( Item from, Mobile to, LanguageType language, int number )
        {
            SendLocalizedMessageTo( from, to, language, number, 0 );
        }

        public static void SendLocalizedMessageTo( Item from, Mobile to, LanguageType language, int number, int hue )
        {
            SendLangLocalizedMessageTo( from, to, language, number, hue );
        }

        public static void SendLangLocalizedMessageTo( Item from, Mobile to, LanguageType language, int index, int hue, params object[] args )
        {
            string format = Text( index, language );

            if( !string.IsNullOrEmpty( format ) )
            {
                string message = string.Format( format, args );

                to.Send( new AsciiMessage( from.Serial, from.ItemID, MessageType.Regular, hue, 3, "", message ) );
            }
            else
                LogError( index );
        }

        public static void LocalLangOverheadMessageTo( Mobile to, LanguageType language, int index, params object[] args )
        {
            LocalLangOverheadMessageTo( to, language, index, 0, args );
        }

        public static void LocalLangOverheadMessageTo( Mobile to, LanguageType language, int index, int hue, params object[] args )
        {
            NetState ns = to.NetState;
            string format = Text( index, language );

            if( !string.IsNullOrEmpty( format ) )
            {
                string message = string.Format( format, args );

                if( ns != null )
                    ns.Send( new AsciiMessage( to.Serial, to.Body, MessageType.Regular, hue, 3, to.Name, message ) );
            }
            else
                LogError( index );
        }

        public static void LocalLangOverheadMessageTo( Mobile to, LanguageType language, int index )
        {
            LocalLangOverheadMessageTo( to, language, index, MessageType.Regular, 0 );
        }

        public static void LocalLangOverheadMessageTo( Mobile to, LanguageType language, int index, MessageType type, int hue )
        {
            NetState ns = to.NetState;
            string message = Text( index, language );

            if( message != null )
            {
                if( ns != null )
                    ns.Send( new AsciiMessage( to.Serial, to.Body, type, hue, 3, to.Name, message ) );
            }
            else
                LogError( index );
        }
    }
}