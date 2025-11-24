using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Server
{
    public class StringList
    {
        private readonly Dictionary<int, StringEntry> m_EntTable;

        public StringEntry[] Entries { get; private set; }

        public Dictionary<int, string> Table { get; private set; }

        public string Language { get; private set; }

        private static byte[] m_Buffer = new byte[ 1024 ];

        private static StringList m_Localization;
        private static StringList m_LocalizationIta;

        public static StringList Localization
        {
            get { return m_Localization ?? ( m_Localization = new StringList( "ENU" ) ); }
            set
            {
                m_Localization = value;
            }
        }

        public static StringList LocalizationIta
        {
            get { return m_LocalizationIta ?? ( m_LocalizationIta = new StringList( "ITA" ) ); }
            set
            {
                m_LocalizationIta = value;
            }
        }

        public string this[ int number ]
        {
            get
            {
                if( !Table.ContainsKey( number ) )
                {
                    LogError( number );
                    return "*cliloc error. contact Dies*";
                }
                else
                    return Table[ number ];
            }
        }

        #region constructors
        public StringList( string language )
            : this( language, true )
        {
        }

        public StringList( string language, bool format )
        {
            Console.Write( "Localization: Loading..." );

            Language = language;
            Table = new Dictionary<int, string>();
            m_EntTable = new Dictionary<int, StringEntry>();

            string filePath = Core.FindDataFile( String.Format( "cliloc.{0}", language ) );

            if( File.Exists( filePath ) )
            {
                List<StringEntry> list = new List<StringEntry>();

                using( BinaryReader bin = new BinaryReader( new FileStream( filePath, FileMode.Open, FileAccess.Read, FileShare.Read ), Encoding.UTF8 ) )
                {
                    bin.ReadInt32();
                    bin.ReadInt16();

                    try
                    {
                        while( bin.BaseStream.Position != bin.BaseStream.Length )
                        {
                            int number = bin.ReadInt32();
                            bin.ReadByte();
                            int length = bin.ReadInt16();

                            if( length > m_Buffer.Length )
                                m_Buffer = new byte[ ( length + 1023 ) & ~1023 ];

                            bin.Read( m_Buffer, 0, length );

                            try
                            {
                                string text = Encoding.UTF8.GetString( m_Buffer, 0, length );

                                if( format )
                                    text = FormatArguments( text );

                                StringEntry ent = new StringEntry( number, text );

                                list.Add( ent );
                                Table[ number ] = text;
                                m_EntTable[ number ] = ent;
                            }
                            catch( Exception e )
                            {
                                Console.WriteLine( e.ToString() );
                            }
                        }
                    }
                    catch( EndOfStreamException eof )
                    {
                        Console.WriteLine( eof.ToString() );
                    }

                    Entries = list.ToArray();

                    Console.WriteLine( "done." );
                }
            }
            else
            {
                Console.ReadKey();
                Console.WriteLine( "Cliloc (lang:" + language + ") file was not found at \"" + filePath + "\"" );
                Console.WriteLine( "Make sure your Scripts/Misc/DataPath.cs is properly configured" );
                Console.WriteLine( "After pressing return an exception will be thrown and the server will terminate" );

                throw new Exception( String.Format( "Cliloc: {0} not found", filePath ) );
            }
        }
        #endregion

        private static readonly Regex FormatExpression = new Regex( @"~(\d)+_.*?~", RegexOptions.IgnoreCase );

        private static string MatchComparison( Match m )
        {
            string comparison = "{" + ( Utility.ToInt32( m.Groups[ 1 ].Value ) - 1 ) + "}";
            return comparison;
        }

        /// <summary>
        /// Evaluate the first not-null between defaultName and Localization[ number ]
        /// </summary>
        /// <param name="defaultName">first string evaluated. If not null this value is returned</param>
        /// <param name="number">localization number to evaluate</param>
        /// <param name="language"> </param>
        /// <returns>string evaluated</returns>
        public static string GetClilocString( string defaultName, int number, string language )
        {
            if( !string.IsNullOrEmpty( defaultName ) )
                return defaultName;

            if( language == "ITA" && LocalizationIta.Table.ContainsKey( number ) )
                return StringUtility.ConvertItemName( LocalizationIta[ number ], 1, true, language );
            else if ( Localization.Table.ContainsKey( number ) )
		        return Localization[ number ];//edit per gli oggetti non stackable
		    else
            {
                LogError( number );
                return null;
            }
        }

        public static string GetClilocString( string defaultName, int number, bool capitalized, string language )
        {
            return StringUtility.Capitalize( GetClilocString( defaultName, number, language ) );
        }

        public static string GetClilocString( string defaultName, int number, bool capitalized )
        {
            return StringUtility.Capitalize( GetClilocString( defaultName, number, "ITA" ) );
        }

        public static string GetClilocString( string defaultName, int number )
        {
            return GetClilocString( defaultName, number, "ITA" );
        }

        /// <summary>
        /// Replace cliloc formats with standard string formats.
        /// For ex: 
        ///     ~1_name~ misses the target altogether.
        ///     {0} misses the target altogether.
        /// </summary>
        /// <param name="entry">cliloc string to format</param>
        /// <returns>fixed string</returns>
        private static string FormatArguments( string entry )
        {
            return FormatExpression.Replace( entry, new MatchEvaluator( MatchComparison ) );
        }

        /*
        /// <summary>
        /// Format a string with arguments passed through a '\t' argument string
        /// </summary>
        /// <param name="str">string to format</param>
        /// <param name="args">'\t' argument string list</param>
        /// <returns>fromatted string</returns>
        /// <see cref="Server.Network.MessageLocalized"/>
        public static string CombineArguments( string str, string args )
        {
            return String.Format( str, args.Split( '\t' ) );
        }
        */

        /// <summary>
        /// Extract a localized StringEntry and format the result through a '\t' argument string
        /// </summary>
        /// <param name="num">localized string number</param>
        /// <param name="argstr">'\t' argument string</param>
        /// <returns>resulting formatted string</returns>
        public string SplitFormat( int num, string argstr )
        {
            StringEntry ent = m_EntTable[ num ];

            if( ent != null )
                return ent.SplitFormat( argstr );

            LogError( num );

            return string.Empty;
        }

        /// <summary>
        /// Extract a localized StringEntry and format the result through an array of string arguments
        /// </summary>
        /// <param name="num">localized string number</param>
        /// <param name="args">array of string arguments</param>
        /// <returns>resulting formatted string</returns>
        public string Format( int num, params object[] args )
        {
            StringEntry ent = m_EntTable[ num ];

            if( ent != null )
                return ent.Format( args );

            LogError( num );

            return string.Empty;
        }

        /// <summary>
        /// Method to log null cliloc entries.
        /// </summary>
        /// <param name="number">localized number we have to report</param>
        public static void LogError( int number )
        {
            string message = String.Format( "Warning: Attempted to load null cliloc string for key {0}.", number );

            Console.WriteLine( message );

            try
            {
                using( StreamWriter op = new StreamWriter( "Logs/clloc-errors.log", true ) )
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
    }

    public class StringEntry
    {
        private static readonly Regex m_RegEx = new Regex( @"~(\d+)[_\w]+~", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.CultureInvariant );

        private string m_FmtTxt;

        public int Number { get; private set; }
        public string Text { get; private set; }

        public StringEntry( int number, string text )
        {
            Number = number;
            Text = text;
            m_FmtTxt = null;
        }

        private static readonly object[] m_Args = new object[] { "", "", "", "", "", "", "", "", "", "", "" };

        /// <summary>
        /// Format our string entry through an array of arguments
        /// </summary>
        /// <param name="args">array of arguments</param>
        /// <returns>formatted string</returns>
        public string Format( params object[] args )
        {
            if( m_FmtTxt == null )
                m_FmtTxt = m_RegEx.Replace( Text, @"{$1}" ); // replace ~1_name~ pars with {0} formats

            for( int i = 0; i < args.Length && i < 10; i++ )
            {
                if( args[ i ] is string && ( (string)args[ i ] ).StartsWith( "#" ) )
                {
                    int loc = -1;

                    try
                    {
                        loc = int.Parse( ( (string)args[ i ] ).Substring( 1 ) );
                    }
                    catch( Exception e )
                    {
                        Console.WriteLine( e.ToString() );
                    }

                    if( loc != -1 )
                        m_Args[ i ] = StringList.Localization[ loc ];
                }
                else
                    m_Args[ i ] = args[ i ];
            }

            try
            {
                return String.Format( m_FmtTxt, m_Args );
            }
            catch
            {
                try
                {
                    using( StreamWriter op = new StreamWriter( "Logs/cliloc-errors.log", true ) )
                    {
                        op.WriteLine( "Warning: Not same parameters number in StringEntry.Format ." );
                        op.WriteLine( "Number >>> {0}", Number );
                        op.WriteLine( new StackTrace( 2 ).ToString() );

                        op.WriteLine( "args.Length >>> {0}", args.Length );
                        for( int i = 0; i < args.Length; i++ )
                            op.WriteLine( "{0}: {1}", i, args[ i ] );

                        op.WriteLine();

                        op.WriteLine( "m_Args.Length >>> {0}", m_Args.Length );
                        for( int i = 0; i < m_Args.Length; i++ )
                            op.WriteLine( "{0}: {1}", i, m_Args[ i ] );

                        op.WriteLine();
                    }
                }
                catch( Exception e )
                {
                    Console.WriteLine( e.ToString() );
                }
            }

            return m_FmtTxt;
        }

        /// <summary>
        /// Format our string entry through a '\t' argument string
        /// </summary>
        /// <param name="argstr">'\t' argument string</param>
        /// <returns>formatted string</returns>
        public string SplitFormat( string argstr )
        {
            if( m_FmtTxt == null )
                m_FmtTxt = m_RegEx.Replace( Text, @"{$1}" ); // replace ~1_name~ pars with {0} formats

            string[] args;

            if( argstr != null )
                args = argstr.Split( '\t' ); // adds an extra on to the args array
            else
            {
                Console.WriteLine( "Warning: null argstr in StringEntry.SplitFormat" );
                StringList.LogError( Number );
                return m_FmtTxt;
            }

            for( int i = 0; i < args.Length && i < 10; i++ )
            {
                if( args[ i ].StartsWith( "#" ) )
                {
                    int loc = -1;

                    try
                    {
                        loc = int.Parse( args[ i ].Substring( 1 ) );
                    }
                    catch( Exception e )
                    {
                        Console.WriteLine( e.ToString() );
                    }

                    if( loc != -1 )
                        m_Args[ i ] = StringList.Localization[ loc ];
                }
                else
                    m_Args[ i ] = args[ i ];
            }
            return String.Format( m_FmtTxt, m_Args );
        }
    }

    public static class StringUtility
    {
        // private static readonly char[] vowels = new char[] { 'a', 'e', 'i', 'o', 'u', 'y' };
        private static readonly char[] m_Vowels = "AEIOUYaeiouy".ToCharArray();

        public static string ConvertItemName( int locid )
        {
            return ConvertItemName( locid, 1, true, "ITA" );
        }

        public static string ConvertItemName( int locid, bool toLower )
        {
            return ConvertItemName( locid, 1, toLower, "ITA" );
        }

        public static string ConvertItemName( int locid, bool toLower, string language )
        {
            return ConvertItemName( locid, 1, toLower, language );
        }

        public static string ConvertItemName( int locid, int amount )
        {
            return ConvertItemName( StringList.Localization[ locid ], amount, true, "ENG" );
        }

        public static string ConvertItemName( int locid, int amount, string language )
        {
            return ConvertItemName( language == "ITA" ? StringList.LocalizationIta[ locid ] : StringList.Localization[ locid ], amount, true, language );
        }

        public static string ConvertItemName( int locid, int amount, bool toLower )
        {
            return ConvertItemName( StringList.Localization[ locid ], amount, toLower, "ENG" );
        }

        public static string ConvertItemName( int locid, int amount, bool toLower, string language )
        {
            return ConvertItemName( language == "ITA" ? StringList.LocalizationIta[ locid ] : StringList.Localization[ locid ], amount, toLower, language );
        }

        public static string ConvertItemName( string itemname )
        {
            return ConvertItemName( itemname, 1, true, "ITA" );
        }

        public static string ConvertItemName( string itemname, string language )
        {
            return ConvertItemName( itemname, 1, true, language );
        }

        public static string ConvertItemName( string itemname, bool toLower )
        {
            return ConvertItemName( itemname, 1, toLower, "ITA" );
        }

        public static string ConvertItemName( string itemname, bool toLower, string language )
        {
            return ConvertItemName( itemname, 1, toLower, language );
        }

        public static string ConvertItemName( string itemname, int amount, string language )
        {
            return ConvertItemName( itemname, amount, true, language );
        }

		public static string ConvertItemName( string itemname, int amount, bool toLower, string language )
		{
			if( string.IsNullOrEmpty( itemname ) )
				return "";

            if( toLower )
                itemname = itemname.ToLower();

            string name;

            if( language != "ITA" )
            {
                bool plurals = false;

                if( itemname.IndexOf( "%s%" ) != -1 )
                {
                    plurals = true;
                    itemname = itemname.Substring( 0, itemname.IndexOf( "%s%" ) );
                }

                if( itemname.IndexOf( "%s" ) != -1 )
                {
                    plurals = true;
                    itemname = itemname.Substring( 0, itemname.IndexOf( "%s" ) );
                }

                if( amount > 1 )
                {
                    if( itemname.IndexOf( "an " ) == 0 )
                        itemname = itemname.TrimStart( ( "an " ).ToCharArray() );

                    if( itemname.IndexOf( "a " ) == 0 )
                        itemname = itemname.TrimStart( ( "a " ).ToCharArray() );

                    name = String.Format( "{0} {1}", amount, itemname );
                    if( plurals )
                        name = name + "s";
                }
                else if( itemname.Length >= 2 && ( ( itemname.Length >= 3 && itemname.IndexOf( "an " ) == 0 || itemname.IndexOf( "a " ) == 0 ) || ( itemname[ itemname.Length - 1 ] == 's' && itemname[ itemname.Length - 2 ] != 's' ) ) )
                {
                    name = itemname;
                }
                else
                {
                    bool beginwithvowel = false;

                    foreach( char t in m_Vowels )
                    {
                        if( itemname[ 0 ] != t )
                            continue;

                        beginwithvowel = true;
                        break;
                    }

                    name = beginwithvowel ? "an " + itemname : "a " + itemname;
                }
            }
            else
            {
                if( amount > 1 )
                {
                    //tranci%o% di bacon
                    //Muschi%o% Insanguinat%oi%
                    //Cappucci%o% da Boia
                    //Legn%oi% Marci%o%
                    if( itemname.IndexOf( "%o%" ) != -1 )
                        itemname = itemname.Replace( "%o%", "" );

                    //fett%ae% di bacon
                    //fett%ae% di pesce crudo
                    //fett%ae% di pesce
                    //form%ae% di formaggio
                    //per%ae%
                    //mel%ae%
                    //tort%ae%
                    //confettur%ae% di miele
                    if( itemname.IndexOf( "%ae%" ) != -1 )
                        itemname = itemname.Replace( "%ae%", "e" );

                    //pezz%oi% di formaggio
                    //barattol%oi%
                    //uccell%oi% cott%oi%
                    //uccell%oi% crud%oi%
                    //prosciutt%oi%
                    //grappol%oi% d'uva
                    //piatt%oi%
                    if( itemname.IndexOf( "%oi%" ) != -1 )
                        itemname = itemname.Replace( "%oi%", "i" );

                    //maial%ei% arrostit%oi%
                    //melon%ei%
                    //diamant%ei%
                    if( itemname.IndexOf( "%ei%" ) != -1 )
                        itemname = itemname.Replace( "%ei%", "i" );

                    //salsicc%iae%
                    //frecc%iae%
                    //torc%iae%
                    //pellicc%iae%
                    if( itemname.IndexOf( "%iae%" ) != -1 )
                        itemname = itemname.Replace( "%iae%", "e" );

                    //brocc%ahe% di sidro
                    //pesc%ahe%
                    //zucc%ahe%
                    //pergamen%ae% bianc%ahe%
                    if( itemname.IndexOf( "%ahe%" ) != -1 )
                        itemname = itemname.Replace( "%ahe%", "he" );

                    //Al%ai% di pipistrello
                    if( itemname.IndexOf( "%ai%" ) != -1 )
                        itemname = itemname.Replace( "%ai%", "i" );

                    //Oss%oa%
                    if( itemname.IndexOf( "%oa%" ) != -1 )
                        itemname = itemname.Replace( "%oa%", "a" );

                    //sacc%ohi% di farina
                    if( itemname.IndexOf( "%ohi%" ) != -1 )
                        itemname = itemname.Replace( "%ohi%", "hi" );


                    name = String.Format( "{0} {1}", amount, itemname );
                }
                else
                {
                    //tranci%o% di bacon
                    //Muschi%o% Insanguinat%oi%
                    //Cappucci%o% da Boia
                    //Legn%oi% Marci%o%
                    if( itemname.IndexOf( "%o%" ) != -1 )
                        itemname = itemname.Replace( "%o%", "o" );

                    //fett%ae% di bacon
                    if( itemname.IndexOf( "%ae%" ) != -1 )
                        itemname = itemname.Replace( "%ae%", "a" );

                    //pezz%oi% di formaggio
                    if( itemname.IndexOf( "%oi%" ) != -1 )
                        itemname = itemname.Replace( "%oi%", "o" );

                    //maial%ei% arrostit%oi%
                    if( itemname.IndexOf( "%ei%" ) != -1 )
                        itemname = itemname.Replace( "%ei%", "e" );

                    //salsicc%iae%
                    if( itemname.IndexOf( "%iae%" ) != -1 )
                        itemname = itemname.Replace( "%iae%", "ia" );

                    //brocc%ahe% di sidro
                    if( itemname.IndexOf( "%ahe%" ) != -1 )
                        itemname = itemname.Replace( "%ahe%", "a" );

                    //Al%ai% di pipistrello
                    if( itemname.IndexOf( "%ai%" ) != -1 )
                        itemname = itemname.Replace( "%ai%", "a" );

                    //Oss%oa%
                    if( itemname.IndexOf( "%oa%" ) != -1 )
                        itemname = itemname.Replace( "%oa%", "o" );

                    //sacc%ohi% di farina
                    if( itemname.IndexOf( "%ohi%" ) != -1 )
                        itemname = itemname.Replace( "%ohi%", "o" );

                    name = itemname;
                }
            }

            return name;
        }

        public static string Capitalize( string s )
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase( s );
        }

        public static string RemoveArticle( string name )
        {
            string temp = name;

            if( temp.IndexOf( "an " ) == 0 )
                temp = temp.TrimStart( ( "an " ).ToCharArray() );
            else if( temp.IndexOf( "a " ) == 0 )
                temp = temp.TrimStart( ( "a " ).ToCharArray() );

            return temp;
        }

        public static string AddArticle( string name )
        {
            bool beginwithvowel = false;

            foreach( char t in m_Vowels )
            {
                if( name[ 0 ] != t )
                    continue;

                beginwithvowel = true;
                break;
            }

            if( beginwithvowel )
                return "an " + name;
            else
                return "a " + name;
        }
    }
}