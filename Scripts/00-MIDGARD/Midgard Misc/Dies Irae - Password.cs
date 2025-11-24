/***************************************************************************
 *                               Password.cs
 *
 *   begin                : 11 October, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Security.Cryptography;
using System.Text;

namespace Midgard.Misc
{
    public class Password
    {
        private const PasswordFlags DefaultFlag = PasswordFlags.Midgard;

        [Flags]
        private enum PasswordFlags
        {
            IncludeLetters = 1 << 0,
            IncludeMixedCase = 1 << 1,
            IncludeNumbers = 1 << 2,
            IncludePunctuaction = 1 << 3,
            NoSimilarCharacters = 1 << 4,

            Midgard = IncludeLetters | IncludeMixedCase | IncludeNumbers | NoSimilarCharacters
        }

        private static bool Get( PasswordFlags toGet )
        {
            return ( ( DefaultFlag & toGet ) != 0 );
        }

        public static string GetNewPassword( int length )
        {
            StringBuilder builder = new StringBuilder();

            char[] possibleChars = FormatChars( Get( PasswordFlags.IncludeLetters ), Get( PasswordFlags.IncludeMixedCase ),
            Get( PasswordFlags.IncludeNumbers ), Get( PasswordFlags.IncludePunctuaction ), Get( PasswordFlags.NoSimilarCharacters ) );

            for( int i = 0; i < length; i++ )
            {
                // Get our cryptographically random 32-bit integer & use as seed in Random class
                // random value generated PER ITERATION, meaning that the System.Random class
                // is re-instantiated every iteration with a new, crytographically random numeric seed.
                int randInt32 = RandomInt32Value.GetRandomInt();
                Random r = new Random( randInt32 );

                int nextInt = r.Next( possibleChars.Length );
                char c = possibleChars[ nextInt ];
                builder.Append( c );
            }

            return builder.ToString();
        }

        private static char[] FormatChars( bool includeLetters, bool includeMixedCase, bool includeNumbers, bool includePunctuaction, bool noSimilarCharacters )
        {
            StringBuilder sb = new StringBuilder();

            if( includeLetters )
                sb.Append( "ABCDEFGHIJKLMNOPQRSTUVWXYZ" );

            if( includeMixedCase )
                sb.Append( "abcdefghijklmnopqrstuvwxyz" );

            if( includeNumbers )
                sb.Append( "0123456789" );

            if( includePunctuaction )
                sb.Append( "!@#$%^&*()" );

            if( noSimilarCharacters )
                sb.Append( "" );

            if( sb.Length > 0 )
                return sb.ToString().ToCharArray();
            else
                return new char[ 0 ];
        }

        private static class RandomInt32Value
        {
            public static Int32 GetRandomInt()
            {
                byte[] randomBytes = new byte[ 4 ];
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                rng.GetBytes( randomBytes );
                Int32 randomInt = BitConverter.ToInt32( randomBytes, 0 );
                return randomInt;
            }
        }
    }
}