using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using Server.Commands;
using Server.Regions;

namespace Server.Mobiles
{
    public static class FragmentHelper
    {
        private static readonly Regex RxMaleFemale = new Regex( @"\$[a-z\ \'+\/[a-z\ \']+\$", RegexOptions.Compiled | RegexOptions.IgnoreCase );
        
        public static string DoGenderReplacement( string resp, bool female )
        {
            // ex: $good sir/good lady$
            // $sir/lady$
            Match match = RxMaleFemale.Match( resp );
            if( match.Success )
            {
                string result = ( match.Value.Substring( 1, match.Value.Length - 2 ).Split( '/' ) )[ female ? 1 : 0 ];

                Console.WriteLine( "DoGenderReplacement success.");
                Console.WriteLine( "response: {0}.", resp );
                Console.WriteLine( "result: {0}.", result );

                return DoGenderReplacement( resp.Replace( match.Value, result ), female );
            }

            return resp;
        }

        public static string ParseResponceMacros( string resp, ISpeaker speaker, Mobile focus, string job )
        {
            // Action Macros: [Attack] [Leave]
            // Value Macros: [getHint] [GetNeed] <- not implemented
            // Literals: _Job_ _Town_
            // Options: $<male>/<female>$

            string ret = DoGenderReplacement( resp, !( focus == null || !focus.Female ) );
            ret = ret.Replace( "_Job_", String.IsNullOrEmpty( job ) ? "llama" : job );
            ret = ret.Replace( "_Town_", ( speaker.Region is TownRegion ) ? speaker.Region.Name : "the wilderness" );

            BaseCreature bc = speaker as BaseCreature;
            if( bc != null ) // actions
            {
                if( ret.Contains( "[Attack]" ) )
                {
                    bc.Combatant = focus;
                    ret = ret.Replace( "[Attack]", "" );
                }
                else if( resp.Contains( "[Leave]" ) && focus != null && bc.CanSee( focus ) ) // TODO: this should be implemented for all ISpeakers
                {
                    bc.FocusMob = null;
                    bc.Direction = (Direction)( ( ( (int)bc.GetDirectionTo( focus ) ) + 4 ) & (int)Direction.Mask );
                }
            }
            else
                ret = ret.Replace( "[Attack]", "" );

            ret = ret.Replace( "[Leave]", "" );

            // TODO:
            ret = ret.Replace( "[GetNeed]", "stuff" );
            ret = ret.Replace( "[getHint]", "" );

            return ret;
        }

        /// <summary>
        /// Tranform keyword format into regex one
        /// </summary>
        /// <param name="keys">the keys string</param>
        /// <returns>the regex formula</returns>
        /// "*mapmaker*", "*cartographer*", "* map*"   Note space between * and map
        /// "*carpent*" "*woodcarving*"  "*joining*"   No commas, extra spaces
        /// "*where am i*", "*m lost*"                 nicer format
        public static string SplitToRegExKeys( string keys )
        {
            if( keys == "\"No key\"" )
                return ".*";

            StringBuilder ret = new StringBuilder( String.Empty );

            int qc = 0;
            int pos1 = 0;
            while( pos1 < keys.Length )
            {
                //find string begin
                while( pos1 < keys.Length && qc == 0 )
                {
                    if( keys[ pos1 ] == '"' )
                        qc++;
                    pos1++;
                }
                int pos2 = pos1;
                while( pos2 < keys.Length && qc > 0 )
                {
                    if( keys[ pos2 ] == '"' )
                        qc--;
                    else
                        pos2++;
                }
                if( qc == 0 && pos2 < keys.Length )
                {
                    if( ret.Length > 0 )
                        ret.Append( '|' );
                    ret.Append( /*WildcardToRegex*/ TrimRegEx( keys.Substring( pos1, pos2 - pos1 ) ) );
                }
                pos1 = pos2 + 1;
            }

            //ret.Insert(0, "(?i)(");
            //ret.Append(')');

            return ret.ToString(); //+ "$" ;
        }

        public static string TrimRegEx( string pattern )
        {
            int i1 = pattern.IndexOf( '*' );
            if( i1 == 0 && pattern.Length > 2 )
            {
                int i2 = pattern.LastIndexOf( '*' );
                if( i2 == pattern.Length - 1 )
                {
                    return '(' + WildcardToRegex( pattern.Substring( 1, pattern.Length - 2 ) ) + ')';
                }
            }

            return WildcardToRegex( pattern );
        }

        public static string WildcardToRegex( string pattern )
        {
            //return "^" + Regex.Escape(pattern).
            //Replace("\\*", ".*").
            //Replace("\\?", ".") + "$";

            return Regex.Escape( pattern ).
                Replace( "\\*", ".*" ).
                Replace( "\\?", "." );
        }

        /// <summary>
        /// returns sectionend or -1 if not found
        /// </summary>
        public static int FindNextSection( string contents, int begin )
        {
            int pos = begin;

            // seek beginning
            bool withinquotes = false;
            while( pos < contents.Length && ( withinquotes || contents[ pos ] != '#' ) )
            {
                if( contents[ pos ] == '"' )
                    withinquotes = !withinquotes;
                pos++;
            }

            if( pos >= contents.Length )
                return -1; // no contents

            while( pos < contents.Length && contents[ pos ] != '{' )
            {
                pos++;
            }

            if( pos >= contents.Length )
                throw new ArgumentException( "Not a valid fragment." ); // no content declaration

            int bc = 1;
            while( ++pos < contents.Length && bc > 0 ) // look for end of block
            {
                if( contents[ pos ] == '{' )
                    bc++;
                if( contents[ pos ] == '}' )
                    bc--;
            }
            if( bc != 0 )
                throw new ArgumentException( "Not a valid fragment file, no closing bracket" ); // no contents

            return pos;
        }

        public static Dictionary<string, int> m_SpeechID;

        public static int FindKey( string speech )
        {
            if( m_SpeechID == null )
                LoadSpeechLookup();

            Dictionary<int, int> matches = new Dictionary<int, int>(); // best match <key,count>

            List<string> sa = new List<string>( speech.Replace( "\"", "" ).Split( ',' ) );
            if( m_SpeechID != null )
            {
                foreach( string s in sa )
                {
                    string key = s.Trim();
                    if( m_SpeechID.ContainsKey( key ) )
                    {
                        if( matches.ContainsKey( m_SpeechID[ key ] ) )
                            matches[ m_SpeechID[ key ] ]++;
                        else
                            matches[ m_SpeechID[ key ] ] = 1;
                        //return m_SpeechID[key];
                    }
                }
            }

            if( matches.Count == 0 )
                return -1;

            int max = 0;
            int maxkey = -1;
            foreach( int key in matches.Keys )
            {
                if( matches[ key ] >= max )
                {
                    max = matches[ key ];
                    maxkey = key;
                }
            }
            return maxkey;
        }

        public static void LoadSpeechLookup()
        {
            m_SpeechID = new Dictionary<string, int>();

            List<Dictionary<int, Docs.SpeechEntry>> entries = Docs.LoadSpeechFile();

            foreach( Dictionary<int, Docs.SpeechEntry> table in entries )
            {
                foreach( int id in table.Keys )
                {
                    foreach( string s in table[ id ].Strings )
                    {
                        if( !m_SpeechID.ContainsKey( s ) )
                            m_SpeechID.Add( s, id );
                    }
                }
            }
        }
    }
}