using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Midgard.Engines.TalkingVendors
{
    internal class StringHelper
    {
        internal static bool CompareWildcards( string wildString, string mask, bool ignoreCase )
        {
            int i = 0;

            if( String.IsNullOrEmpty( mask ) )
                return false;
            if( mask == "*" )
                return true;

            while( i != mask.Length )
            {
                if( CompareWildcard( wildString, mask.Substring( i ), ignoreCase ) )
                    return true;

                while( i != mask.Length && mask[ i ] != ';' )
                    i += 1;

                if( i != mask.Length && mask[ i ] == ';' )
                {
                    i += 1;

                    while( i != mask.Length && mask[ i ] == ' ' )
                        i += 1;
                }
            }

            return false;
        }

        internal static bool CompareWildcard( string wildString, string mask, bool ignoreCase )
        {
            int i = 0, k = 0;

            while( k != wildString.Length )
            {
                switch( mask[ i ] )
                {
                    case '*':

                        while( k != wildString.Length )
                        {
                            if( CompareWildcard( wildString.Substring( k + 1 ), mask.Substring( i + 1 ), ignoreCase ) )
                                return true;

                            k += 1;
                        }

                        return false;

                    case '?':

                        break;

                    default:

                        if( ignoreCase == false && wildString[ k ] != mask[ i ] )
                            return false;
                        if( ignoreCase && Char.ToLower( wildString[ k ] ) != Char.ToLower( mask[ i ] ) )
                            return false;

                        break;
                }

                i += 1;
                k += 1;
            }

            if( k == wildString.Length )
            {
                if( i == mask.Length || mask[ i ] == ';' || mask[ i ] == '*' )
                    return true;
            }

            return false;
        }

        internal static string WildcardToRegex( string wildcard )
        {
            StringBuilder sb = new StringBuilder( wildcard.Length + 8 );

            sb.Append( "^" );

            for( int i = 0; i < wildcard.Length; i++ )
            {
                char c = wildcard[ i ];
                switch( c )
                {
                    case '*':
                        sb.Append( ".*" );
                        break;
                    case '?':
                        sb.Append( "." );
                        break;
                    case '\\':
                        if( i < wildcard.Length - 1 )
                            sb.Append( Regex.Escape( wildcard[ ++i ].ToString() ) );
                        break;
                    default:
                        sb.Append( Regex.Escape( wildcard[ i ].ToString() ) );
                        break;
                }
            }

            sb.Append( "$" );

            return sb.ToString();
        }

        internal bool IsWildcardMatch( String wildcard, String text, bool casesensitive )
        {
            StringBuilder sb = new StringBuilder( wildcard.Length + 10 );
            sb.Append( "^" );
            for( int i = 0; i < wildcard.Length; i++ )
            {
                char c = wildcard[ i ];
                switch( c )
                {
                    case '*':
                        sb.Append( ".*" );
                        break;
                    default:
                        sb.Append( Regex.Escape( wildcard[ i ].ToString() ) );
                        break;
                }
            }
            sb.Append( "$" );

            Regex regex;
            if( casesensitive )
                regex = new Regex( sb.ToString(), RegexOptions.None );
            else
                regex = new Regex( sb.ToString(), RegexOptions.IgnoreCase );

            return regex.IsMatch( text );
        }

        internal static bool Match( string strWithWildCards, string myString )
        {
            if( strWithWildCards.Length == 0 )
                return myString.Length == 0;

            if( myString.Length == 0 )
                return false;

            if( strWithWildCards[ 0 ] == '*' && strWithWildCards.Length > 1 )
                for( int index = 0; index < myString.Length; index++ )
                {
                    if( Match( strWithWildCards.Substring( 1 ), myString.Substring( index ) ) )
                        return true;
                }
            else if( strWithWildCards[ 0 ] == '*' )
                return true;
            else if( strWithWildCards[ 0 ] == myString[ 0 ] )
                return Match( strWithWildCards.Substring( 1 ), myString.Substring( 1 ) );
            return false;
        }

        internal static bool Wildcard( string pattern, string input )
        {
            return Wildcard( pattern, 0, input, 0, false );
        }

        internal static bool Wildcard( string pattern, string input, bool insensitive )
        {
            return Wildcard( pattern, 0, input, 0, insensitive );
        }

        internal static bool Wildcard( string pattern, int p, string input, int i, bool insensitive )
        {
            for( ; ; )
            {
                char ic = input[ i ];
                char pc = pattern[ p ];
                switch( pc )
                {
                    case '?':
                        break;

                    case '*':
                        p++;
                        for( int j = i; j < input.Length; j++ )
                        {
                            if( Wildcard( pattern, p, input, j, insensitive ) )
                            {
                                return true;
                            }
                        }
                        return false;

                    default:
                        if( insensitive )
                        {
                            ic = char.ToLower( ic );
                            pc = char.ToLower( pc );
                        }
                        if( ic != pc )
                        {
                            return false;
                        }
                        break;
                }
                i++;
                p++;
                if( p >= pattern.Length )
                {
                    if( i >= input.Length )
                    {
                        return true;
                    }
                    return false;
                }
                else if( i >= input.Length )
                {
                    return false;
                }
            }
        }
    }
}