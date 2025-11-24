// Speech Fragments
// Using the frg speech delaration from files from UODemo
// Derrick Slopey: derrick@alienseed.com
// Date: Sept 5, 2009

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Server.Mobiles
{
    public class SpeechFragment
    {
        #region Global Fragment Instances
        /// <summary>
        /// Absolute default, will never be called
        /// #Fragment Global, Default, Global_Default
        /// </summary>
        public static SpeechFragment SFDefault = new SpeechFragment( "default.frg", null );

        /// <summary>
        /// Default personal responces, not used
        /// #Fragment Global, Default, Global_Default 
        /// </summary>
        public static SpeechFragment SFGDefault = new SpeechFragment( "default2.frg", SFDefault );

        /// <summary>
        /// Default responces (Default Regional Responce)
        /// #Fragment Britannia, Default, Britannia_Default
        /// </summary>
        public static SpeechFragment SFBDefault = new SpeechFragment( "bdefault.frg", SFDefault );

        /// <summary>
        /// Britainnia City Responses
        /// </summary>
        public static SpeechFragment SFBritainnia = new SpeechFragment( "britanni.frg", SFBDefault );
        #endregion

        private readonly SpeechFragment m_Parent;
        private readonly Section m_Tree;

        public string Name { get; private set; }

        public SpeechFragment( SpeechFragment primary, SpeechFragment secondary )
        {
            m_Tree = primary.m_Tree;
            m_Parent = secondary;
        }

        public SpeechFragment( string filename, SpeechFragment parent )
        {
            m_Parent = parent;
            Name = filename.Substring( 0, filename.Length - 4 );

            if( Core.Debug )
                Utility.Log( "SpeechFragment.log", "Loading speech fragment: {0}", Name );

            m_Tree = LoadTree( filename );
        }

        public virtual string GetResponseFragment( SophisticationLevel sophistication, AttitudeLevel attitude, NotorietyLevel notolevel, string speech, List<int> keywords, out string keyFound )
        {
            string resp = m_Tree.GetResponseFragment( sophistication, attitude, notolevel, speech, keywords, out keyFound );

            if( resp == null && m_Parent != null )
                resp = m_Parent.GetResponseFragment( sophistication, attitude, notolevel, speech, keywords, out keyFound );

            if( Core.Debug )
                Utility.Log( "SpeechFragment.log", "GetResponseFragment from {0}: {1} {2}", Name, resp, keyFound ?? "" );

            return resp;
        }

        private static Section LoadTree( string filename )
        {
            string fileContents;

            filename = Path.Combine( Core.BaseDirectory, Path.Combine( "Data", Path.Combine( "SpeechFragments", filename ) ) );
            using( StreamReader reader = new StreamReader( filename ) )
            {
                fileContents = reader.ReadToEnd().Replace( "\r\n", "" ).Replace( "\t", "" );
            }

            List<Section> sections = ReadSections( fileContents );
            if( sections != null && sections.Count == 1 )
                return sections[ 0 ];

            Console.WriteLine( "Failed to load Fragment file: {0}", filename );
            return null;
        }

        #region FindNodeValue(...)
        private static Section FindNodeValue( IEnumerable<Section> sections, int desiredvalue, string textcompare )
        {
            return FindNodeValue( sections, desiredvalue, -1, false, textcompare );
        }

        /*
                private static Section FindNodeValue( IEnumerable<Section> sections, int desiredvalue, bool bitwise )
                {
                    return FindNodeValue( sections, desiredvalue, -1, bitwise, null );
                }
        */

        private static Section FindNodeValue( IEnumerable<Section> sections, int desiredvalue, int defaultvalue, bool bitwise )
        {
            return FindNodeValue( sections, desiredvalue, defaultvalue, bitwise, null );
        }

        /// <summary>
        /// Searches for a integer value (intval) in a list of nodes
        /// </summary>
        /// <param name="sections">Sections to search for value</param>
        /// <param name="desiredvalue">The value to search for</param>
        /// <param name="defaultvalue">Secondary value to search for is primary not found, -1 to ignore</param>
        /// <param name="bitwise">Does a byte bit-flag comparison on value if true</param>
        /// <param name="textcompare">For non bitwise comparisons, such at Keys, do a comparison on string values</param>
        /// <returns>the section found, or null if not found.</returns>
        private static Section FindNodeValue( IEnumerable<Section> sections, int desiredvalue, int defaultvalue, bool bitwise, string textcompare )
        {
            Section def = null;
            if( bitwise )
            {
                foreach( Section sect in sections )
                {
                    if( ( sect.IntValue & desiredvalue ) != 0 )
                        return sect;
                    if( ( sect.IntValue & defaultvalue ) != 0 )
                        def = sect;
                }
            }
            else
            {
                foreach( Section sect in sections )
                {
                    if( sect.IntValue < 0 )
                    {
                        if( textcompare == null )
                            continue;
                        else // check sect.Value against textcompare. Examples:
                        // "*mapmaker*", "*cartographer*", "* map*"   <- Note space between * and map
                        // "*carpent*" "*woodcarving*"  "*joining*"   <- No commas
                        // "*where am i*", "*m lost*"                 <- nicer format
                        {
                            try
                            {
                                if( sect.Regex != null )
                                {
                                    Match match = sect.Regex.Match( textcompare );
                                    if( match.Success )
                                    {
                                        sect.KeyString = match.Value;
                                        return sect;
                                    }
                                }
                            }
                            catch( Exception ex )
                            {
                                Console.WriteLine( ex.ToString() );
                                throw;
                            }
                        }
                    }
                    else
                    {
                        if( sect.IntValue == desiredvalue )
                        {
                            if( textcompare != null && sect.Regex != null )
                            {
                                Match match = sect.Regex.Match( textcompare );
                                if( match.Success )
                                    sect.KeyString = match.Value;
                            }

                            return sect;
                        }
                        if( sect.IntValue == defaultvalue )
                            def = sect;
                    }
                }
            }
            return def;
        }
        #endregion

        private static List<Section> ReadSections( string contents )
        {
            int pos1 = 0;
            List<Section> sections = new List<Section>();
            Section cSection;

            do
            {
                int pos2 = FragmentHelper.FindNextSection( contents, pos1 );

                if( pos2 > pos1 )
                {
                    cSection = new Section( contents.Substring( pos1, pos2 - pos1 ) );
                    if( !cSection.IsValid )
                        return null;
                    sections.Add( cSection );
                }
                pos1 = pos2;
            } while( pos1 > 0 );

            return sections;
        }

        private class Section
        {
            private readonly NodeType ContentType = NodeType.Invalid;
            public readonly int IntValue;
            private readonly List<Section> SubSections;
            private readonly string Value;

            public Regex Regex { get; private set; }
            public string KeyString { private get; set; }

            private Section( NodeType contenttype, string value, int intvalue )
            {
                ContentType = contenttype;
                Value = value;
                IntValue = intvalue;
                SubSections = null;
            }

            private static readonly Dictionary<string, Regex> m_RegExCache = new Dictionary<string, Regex>();

            public Section( string strConstructor )
            {
                int pos1 = 0;

                //read section name
                while( pos1 < strConstructor.Length && strConstructor[ pos1 ] != '#' )
                    pos1++;

                if( pos1 >= strConstructor.Length )
                    return; // no sections

                int pos2 = strConstructor.IndexOf( ' ', pos1 );
                if( pos2 < pos1 || pos2 > strConstructor.Length )
                    throw new ArgumentException( "Not a valid fragment file." ); // no content declaration
                string name = strConstructor.Substring( pos1 + 1, pos2 - pos1 ).Trim();

                // read section data
                pos1 = pos2;
                pos2 = strConstructor.IndexOf( '{', pos1 );
                if( pos2 < pos1 || pos2 > strConstructor.Length )
                    throw new ArgumentException( "Not a valid fragment file." ); // no data
                Value = strConstructor.Substring( pos1, pos2 - pos1 ).Trim();

                // find contents
                int bc = 1;
                pos1 = pos2;

                while( ++pos2 < strConstructor.Length && bc > 0 ) // look for end of block
                {
                    if( strConstructor[ pos2 ] == '{' )
                        bc++;
                    if( strConstructor[ pos2 ] == '}' )
                        bc--;
                }

                if( pos2 < pos1 || pos2 > strConstructor.Length )
                    throw new ArgumentException( "Not a valid fragment file." ); // no contents

                string contents = strConstructor.Substring( pos1 + 1, pos2 - pos1 - 2 ).Trim();

                string[] values = Value.Replace( " ", "" ).Split( ',' );

                IntValue = 0;

                switch( name.ToLower() )
                {
                    case "fragment":
                        ContentType = NodeType.Fragment;
                        IntValue = 0;
                        break;
                    case "sophistication":
                        ContentType = NodeType.Sophistication;
                        foreach( string val in values )
                        {
                            IntValue |= (byte)Enum.Parse( typeof( SophisticationLevel ), val );
                        }
                        break;
                    case "key":
                        ContentType = NodeType.Key;
                        IntValue = FragmentHelper.FindKey( Value );
                        string exp = FragmentHelper.SplitToRegExKeys( Value );
                        if( !string.IsNullOrEmpty( exp ) )
                        {
                            if( m_RegExCache.ContainsKey( exp ) )
                                Regex = m_RegExCache[ exp ];
                            else
                            {
                                Regex = new Regex( exp, RegexOptions.Compiled | RegexOptions.IgnoreCase );
                                m_RegExCache.Add( exp, Regex );

                                if( Core.Debug )
                                    Utility.Log( "SpeechFragment.log", "Complied Key: ({0}) to Regex: ({1})", Value, Regex.ToString() );
                            }
                        }
                        break;
                    case "attitude":
                        ContentType = NodeType.Attitude;
                        foreach( string val in values )
                            IntValue |= (byte)Enum.Parse( typeof( AttitudeLevel ), val );
                        break;
                    case "notoriety":
                        ContentType = NodeType.Notoriety;
                        foreach( string val in values )
                            IntValue |= (byte)Enum.Parse( typeof( NotorietyLevel ), val );
                        break;
                    default:
                        throw new ArgumentException( "Not a valid fragment file. (" + Value + ")" ); // invalid type
                }

                SubSections = ReadSections( contents );
                if( SubSections == null || SubSections.Count == 0 ) // Phrases
                    AddPhrases( contents );
            }

            public bool IsValid
            {
                get { return ContentType != NodeType.Invalid; }
            }

            public string GetResponseFragment( SophisticationLevel sophistication, AttitudeLevel attitude, NotorietyLevel notolevel, string speech, List<int> keywords, out string keyFound )
            {
                string keyword = null;
                keyFound = null;

                if( IsValid && SubSections.Count > 0 )
                {
                    Section cur = this;
                    while( cur != null && cur.SubSections != null && cur.SubSections.Count > 0 && cur.SubSections[ 0 ].ContentType != NodeType.Phrases )
                    {
                        switch( cur.SubSections[ 0 ].ContentType ) // it is assumed that all subsections have the same content type
                        {
                            case NodeType.Sophistication:
                                {
                                    cur = FindNodeValue( cur.SubSections, (int)sophistication, (int)SophisticationLevel.Medium, true );
                                    break;
                                }
                            case NodeType.Key:
                                {
                                    // TODO: String comparisons if keywords is empty, and action keys such as "@InternalAcceptItem" 
                                    // TODO: "No key" and "*" should match anything
                                    Section found = null;
                                    List<Section> foundlist = new List<Section>();
                                    if( keywords != null && keywords.Count > 0 )
                                    {
                                        foreach( int key in keywords )
                                        {
                                            found = FindNodeValue( cur.SubSections, key, speech );
                                            if( found != null )
                                                foundlist.Add( found );
                                        }
                                    }
                                    else
                                    {
                                        found = FindNodeValue( cur.SubSections, -1, speech );
                                        if( found != null )
                                            foundlist.Add( found );
                                    }
                                    if( foundlist.Count == 1 )
                                        found = foundlist[ 0 ];
                                    else if( foundlist.Count > 1 )
                                        found = foundlist[ Utility.Random( foundlist.Count ) ];
                                    cur = found;
                                    if( cur != null )
                                        keyword = cur.KeyString;
                                    break;
                                }
                            case NodeType.Attitude:
                                {
                                    cur = FindNodeValue( cur.SubSections, (int)attitude, (int)AttitudeLevel.Neutral, true );
                                    break;
                                }
                            case NodeType.Notoriety:
                                {
                                    cur = FindNodeValue( cur.SubSections, (int)notolevel, (int)NotorietyLevel.Anonymous, true );
                                    break;
                                }
                            // should not see these:
                            case NodeType.Fragment:
                            case NodeType.Invalid:
                                return null;
                            default:
                                return null;
                        }
                    }

                    // TODO: parse out result phrase ($milord/milady$), and apply actions, like [LEAVE]
                    if( cur != null && cur.SubSections != null && cur.SubSections.Count > 0 && cur.SubSections[ 0 ].ContentType == NodeType.Phrases )
                    {
                        keyFound = cur.KeyString;

                        if( Core.Debug )
                            Utility.Log( "SpeechFragment.log", "Speech debug: regex: {0} keyString: {1}", cur.Regex, keyFound );

                        string ret = cur.SubSections[ Utility.Random( cur.SubSections.Count ) ].Value;
                        return ret.Replace( "%0", string.IsNullOrEmpty( keyword ) ? "this" : keyword ); // Assuming %0 is probably a noun in the case of a null keyword
                    }
                }
                return null;
            }

            private void AddPhrases( string phraselist )
            {
                string[] phrases = phraselist.Split( new string[] { "\"," }, StringSplitOptions.RemoveEmptyEntries );
                foreach( string phrase in phrases )
                    SubSections.Add( new Section( NodeType.Phrases, phrase.Trim( ' ', '"' ), 0 ) );
            }
        }
    }
}