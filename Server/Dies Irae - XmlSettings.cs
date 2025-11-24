/***************************************************************************
 *                               XmlSettings.cs
 *                            --------------------
 *   begin                : 16 dicembre, 2008
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Collections;
using System.Collections.Specialized;

namespace Server
{
    public class XMLUtility
    {
        private static readonly string ConfigFile = Path.Combine( Core.BaseDirectory, "Data/ServerSettings.xml" );
        private const string RootName = "Settings";

        #region props
        /// <summary>
        /// Asci messages for Mobile class
        /// </summary>
        public static bool MobileForceAscii = false;

        /// <summary>
        /// Asci messages for Item class
        /// </summary>
        public static bool ItemForceAscii = true;
        #endregion

        public static void Initialize()
        {
            Element element = ConfigParser.GetConfig( ConfigFile, RootName );
            if( null == element || element.ChildElements.Count <= 0 )
                return;

            foreach( Element child in element.ChildElements )
            {
                bool tempBool;
                if( child.TagName == "MobileForceAscii" && child.GetBoolValue( out tempBool ) )
                    MobileForceAscii = tempBool;

                if( child.TagName == "ItemForceAscii" && child.GetBoolValue( out tempBool ) )
                    ItemForceAscii = tempBool;
            }
        }
    }

    public class ConfigParser
    {
        /// <summary>
        /// Get an element for a config xml file
        /// </summary>
        /// <param name="filename">path to our xml config file</param>
        /// <param name="tag">node name</param>
        /// <returns>found element or null</returns>
        public static Element GetConfig( string filename, string tag )
        {
            Element element = GetConfig( filename );
            if( element != null )
                element = GetConfig( element, tag );

            return element;
        }

        /// <summary>
        /// Retrieve our xml config file
        /// </summary>
        /// <param name="filename">path to our xml config file</param>
        /// <returns>found element or null</returns>
        public static Element GetConfig( string filename )
        {
            XmlTextReader reader = null;
            Element element = null;
            DOMParser parser;

            try
            {
                reader = new XmlTextReader( filename );
                parser = new DOMParser();
                element = parser.Parse( reader );
            }
            catch( Exception exc )
            {
                Console.WriteLine( exc.ToString() );
                // Fail gracefully only on errors reading the file
                //if( !( exc is IOException ) )
                //    throw;
            }

            if( null != reader )
                reader.Close();

            return element;
        }

        /// <summary>
        /// Retrieve a value for given tag in our element
        /// </summary>
        /// <param name="element">our element</param>
        /// <param name="tag">our tag</param>
        /// <returns>found element or null</returns>
        public static Element GetConfig( Element element, string tag )
        {
            if( element.ChildElements.Count > 0 )
            {
                foreach( Element child in element.ChildElements )
                {
                    if( child.TagName == tag )
                        return child;
                }
            }
            return null;
        }
    }

    public class DOMParser
    {
        private Stack m_Elements;
        private Element m_CurrentElement;
        private Element m_RootElement;

        public DOMParser()
        {
            m_Elements = new Stack();
            m_CurrentElement = null;
            m_RootElement = null;
        }

        public Element Parse( XmlTextReader reader )
        {
            Element element = null;

            while( !reader.EOF )
            {
                reader.Read();
                switch( reader.NodeType )
                {
                    case XmlNodeType.Element:
                        element = new Element( reader.LocalName );
                        m_CurrentElement = element;
                        if( m_Elements.Count == 0 )
                        {
                            m_RootElement = element;
                            m_Elements.Push( element );
                        }
                        else
                        {
                            Element parent = (Element)m_Elements.Peek();
                            parent.ChildElements.Add( element );

                            if( reader.IsEmptyElement )
                                break;
                            else
                                m_Elements.Push( element );
                        }
                        if( reader.HasAttributes )
                        {
                            while( reader.MoveToNextAttribute() )
                            {
                                m_CurrentElement.SetAttribute( reader.Name, reader.Value );
                            }
                        }
                        break;
                    case XmlNodeType.Attribute:
                        if( element != null )
                            element.SetAttribute( reader.Name, reader.Value );
                        break;
                    case XmlNodeType.EndElement:
                        m_Elements.Pop();
                        break;
                    case XmlNodeType.Text:
                        m_CurrentElement.Text = reader.Value;
                        break;
                    case XmlNodeType.CDATA:
                        m_CurrentElement.Text = reader.Value;
                        break;
                    default:
                        // ignore
                        break;
                }
            }
            return m_RootElement;
        }
    }

    public class Elements : CollectionBase
    {
        public void Add( Element element )
        {
            List.Add( element );
        }

        public Element this[ int index ]
        {
            get { return (Element)List[ index ]; }
        }
    }

    public class Element
    {
        private const string DecimalSeparator = ".";

        private String m_TagName;
        private String m_Text;
        private StringDictionary m_Attributes;
        private Elements m_ChildElements;

        public Element( String tagName )
        {
            m_TagName = tagName;
            m_Attributes = new StringDictionary();
            m_ChildElements = new Elements();
            m_Text = "";
        }

        public String TagName
        {
            get { return m_TagName; }
            set { m_TagName = value; }
        }

        public string Text
        {
            get { return m_Text; }
            set { m_Text = value; }
        }

        public Elements ChildElements
        {
            get { return m_ChildElements; }
        }

        public StringDictionary Attributes
        {
            get { return m_Attributes; }
        }

        public String Attribute( String name )
        {
            return m_Attributes[ name ];
        }

        public void SetAttribute( String name, String value )
        {
            m_Attributes.Add( name, value );
        }

        private static void BroadCastSet( string format, params object[] args )
        {
            BroadCastSet( String.Format( format, args ) );
        }

        private static void BroadCastSet( string toSend )
        {
            if( Core.Debug )
                Console.WriteLine( toSend );
        }

        #region Xml to data type conversions

        /// <summary>
        /// Convert our element to boolean value
        /// </summary>
        /// <param name="val">parsed value</param>
        /// <returns>true if success, false otherwise</returns>
        public bool GetBoolValue( out bool val )
        {
            val = false;

            try
            {
                if( !string.IsNullOrEmpty( m_Text ) )
                {
                    val = bool.Parse( m_Text );

                    BroadCastSet( "GetBoolValue {0}: {1}", m_Text, val );
                    return true;
                }
            }
            catch( Exception exc ) { HandleError( exc ); }

            return false;
        }

        /// <summary>
        /// Convert our element to double value
        /// </summary>
        /// <param name="val">parsed value</param>
        /// <returns>true if success, false otherwise</returns>
        public bool GetDoubleValue( out double val )
        {
            val = 0;

            NumberFormatInfo provider = new NumberFormatInfo();
            provider.NumberDecimalSeparator = DecimalSeparator;

            try
            {
                if( !string.IsNullOrEmpty( m_Text ) )
                {
                    val = double.Parse( m_Text, provider );
                    BroadCastSet( "GetDoubleValue {0}: {1}", m_Text, val );
                    return true;
                }
            }
            catch( Exception exc ) { HandleError( exc ); }

            return false;
        }

        /// <summary>
        /// Convert our element to integer value
        /// </summary>
        /// <param name="val">parsed value</param>
        /// <returns>true if success, false otherwise</returns>
        public bool GetIntValue( out int val )
        {
            val = 0;

            try
            {
                if( !string.IsNullOrEmpty( m_Text ) )
                {
                    val = Int32.Parse( m_Text );
                    BroadCastSet( "GetIntValue {0}: {1}", m_Text, val );
                    return true;
                }
            }
            catch( Exception exc ) { HandleError( exc ); }

            return false;
        }

        /// <summary>
        /// Convert our element to access level enum value
        /// </summary>
        /// <param name="val">parsed value</param>
        /// <returns>true if success, false otherwise</returns>
        public bool GetAccessLevelValue( out AccessLevel val )
        {
            val = AccessLevel.Player;
            try
            {
                val = (AccessLevel)Enum.Parse( typeof( AccessLevel ), m_Text, true );
                BroadCastSet( "GetAccessLevelValue {0}: {1}", m_Text, val );
            }
            catch( Exception exc ) { HandleError( exc ); }

            return true;
        }

        /// <summary>
        /// Convert our element to map value
        /// </summary>
        /// <param name="val">parsed value</param>
        /// <returns>true if success, false otherwise</returns>
        public bool GetMapValue( out Map val )
        {
            val = null;
            try
            {
                val = Map.Parse( m_Text );
                BroadCastSet( "GetMapValue {0}: {1}", m_Text, val );

                if( null == val )
                    throw new ArgumentException( "Map expected" );
            }
            catch( Exception exc ) { HandleError( exc ); }

            return true;
        }

        /// <summary>
        /// Convert our element to type value
        /// </summary>
        /// <param name="val">parsed value</param>
        /// <returns>true if success, false otherwise</returns>
        public bool GetTypeValue( out Type val )
        {
            val = null;
            try
            {
                val = Type.GetType( m_Text );
                BroadCastSet( "GetTypeValue {0}: {1}", m_Text, val );

                if( null == val )
                    throw new ArgumentException( "Type expected" );
            }
            catch( Exception exc ) { HandleError( exc ); }

            return true;
        }

        /// <summary>
        /// Convert our element to a valid timespan
        /// NB: Format is [ws][-]{ d | d.hh:mm[:ss[.ff]] | hh:mm[:ss[.ff]] }[ws] 
        /// </summary>
        /// <param name="val">parsed value</param>
        /// <returns>true if success, false otherwise</returns>
        public bool GetTimeSpan( out TimeSpan val )
        {
            val = TimeSpan.Zero;

            try
            {
                if( !TimeSpan.TryParse( m_Text, out val ) )
                    throw new ArgumentException( "Time Span string expected" );
                BroadCastSet( "GetTimeSpan {0}: {1}", m_Text, val );
            }
            catch( Exception exc ) { HandleError( exc ); }

            return true;
        }

        /// <summary>
        /// Convert our element to a valid enumeration value.
        /// </summary>
        /// <param name="enumType">enum type to parse</param>
        /// <param name="val">parsed value</param>
        /// <returns>true if success, false otherwise</returns>
        public bool GetEnum( Type enumType, out int val )
        {
            val = 0;

            if( enumType == null )
            {
                Console.WriteLine( "GetEnum: enumType == null" );
                return false;
            }

            try
            {
                if( Enum.IsDefined( enumType, m_Text ) )
                    val = (int)Enum.Parse( enumType, m_Text );
                else
                    throw new ArgumentException( "Valid enum value expected" );

                BroadCastSet( "GetEnum {0}: {1}", m_Text, val );
            }
            catch( Exception exc ) { HandleError( exc ); }

            return true;
        }

        /// <summary>
        /// Convert our element to Point3D value
        /// </summary>
        /// <param name="val">parsed value</param>
        /// <returns>true if success, false otherwise</returns>
        public bool GetPoint3DValue( out Point3D val )
        {
            val = new Point3D();
            int elementsExpected = 3;

            try
            {
                if( null == ChildElements )
                    return false;

                if( elementsExpected != ChildElements.Count )
                    throw new IndexOutOfRangeException( string.Format( "{0} elements were expected", elementsExpected ) );

                int temp;

                if( ChildElements[ 0 ].GetIntValue( out temp ) )
                    val.X = temp;
                else
                    throw new ArrayTypeMismatchException( "Int expected" );

                if( ChildElements[ 1 ].GetIntValue( out temp ) )
                    val.Y = temp;
                else
                    throw new ArrayTypeMismatchException( "Int expected" );

                if( ChildElements[ 2 ].GetIntValue( out temp ) )
                    val.Z = temp;
                else
                    throw new ArrayTypeMismatchException( "Int expected" );

                BroadCastSet( "GetPoint3DValue {0}: {1}", m_Text, val );
            }
            catch( Exception exc ) { HandleError( exc ); }

            return true;
        }

        /// <summary>
        /// Convert our element to Array of booleans value
        /// </summary>
        /// <param name="val">parsed value</param>
        /// <returns>true if success, false otherwise</returns>
        public bool GetArray( out bool[] val )
        {
            return GetArray( 0, out val );
        }

        /// <summary>
        /// Convert our element to Array value
        /// </summary>
        /// <param name="elementsExpected">number of elements expected</param>
        /// <param name="val">parsed value</param>
        /// <returns>true if success, false otherwise</returns>
        public bool GetArray( int elementsExpected, out bool[] val )
        {
            val = null;

            if( null == ChildElements )
                return false;

            try
            {
                if( elementsExpected > 0 && elementsExpected != ChildElements.Count )
                    throw new IndexOutOfRangeException( string.Format( "{0} elements were expected", elementsExpected ) );

                bool[] array = new bool[ ChildElements.Count ];

                for( int i = 0; i < ChildElements.Count; i++ )
                {
                    bool temp;
                    if( ChildElements[ i ].GetBoolValue( out temp ) )
                        array[ i ] = temp;
                    else
                        throw new ArrayTypeMismatchException( "Bool expected" );
                }
                val = array;
            }
            catch( Exception exc ) { HandleError( exc ); }

            return true;
        }

        /// <summary>
        /// Convert our element to Array value
        /// </summary>
        /// <param name="val">parsed value</param>
        /// <returns>true if success, false otherwise</returns>
        public bool GetArray( out int[] val )
        {
            return GetArray( 0, out val );
        }

        /// <summary>
        /// Convert our element to Array value
        /// </summary>
        /// <param name="elementsExpected">number of elements expected</param>
        /// <param name="val">parsed value</param>
        /// <returns>true if success, false otherwise</returns>
        public bool GetArray( int elementsExpected, out int[] val )
        {
            val = null;

            if( null == ChildElements )
                return false;

            try
            {
                if( elementsExpected > 0 && elementsExpected != ChildElements.Count )
                    throw new IndexOutOfRangeException( string.Format( "{0} elements were expected", elementsExpected ) );

                int[] array = new int[ ChildElements.Count ];

                for( int i = 0; i < ChildElements.Count; i++ )
                {
                    int temp;
                    if( ChildElements[ i ].GetIntValue( out temp ) )
                        array[ i ] = temp;
                    else
                        throw new ArrayTypeMismatchException( "Int expected" );
                }
                val = array;
            }
            catch( Exception exc ) { HandleError( exc ); }

            return true;
        }

        /// <summary>
        /// Convert our element to Array of types value
        /// </summary>
        /// <param name="val">parsed value</param>
        /// <returns>true if success, false otherwise</returns>
        public bool GetArray( out Type[] val )
        {
            return GetArray( 0, out val );
        }

        /// <summary>
        /// Convert our element to Array value
        /// </summary>
        /// <param name="elementsExpected">number of elements expected</param>
        /// <param name="val">parsed value</param>
        /// <returns>true if success, false otherwise</returns>
        public bool GetArray( int elementsExpected, out Type[] val )
        {
            val = null;

            if( null == ChildElements )
                return false;

            try
            {
                if( elementsExpected > 0 && elementsExpected != ChildElements.Count )
                    throw new IndexOutOfRangeException( string.Format( "{0} elements were expected", elementsExpected ) );

                Type[] array = new Type[ ChildElements.Count ];

                for( int i = 0; i < ChildElements.Count; i++ )
                {
                    array[ i ] = Type.GetType( ChildElements[ i ].Text );
                }
                val = array;
            }
            catch( Exception exc ) { HandleError( exc ); }

            return true;
        }

        /// <summary>
        /// Convert our element to Array value
        /// </summary>
        /// <param name="val">parsed value</param>
        /// <returns>true if success, false otherwise</returns>
        public bool GetArray( out string[] val )
        {
            return GetArray( 0, out val );
        }

        /// <summary>
        /// Convert our element to Array value
        /// </summary>
        /// <param name="elementsExpected">number of elements expected</param>
        /// <param name="val">parsed value</param>
        /// <returns>true if success, false otherwise</returns>
        public bool GetArray( int elementsExpected, out string[] val )
        {
            val = null;

            if( null == ChildElements )
                return false;

            try
            {
                if( elementsExpected > 0 && elementsExpected != ChildElements.Count )
                    throw new IndexOutOfRangeException( string.Format( "{0} elements were expected", elementsExpected ) );

                string[] array = new string[ ChildElements.Count ];

                for( int i = 0; i < ChildElements.Count; i++ )
                {
                    if( null != ChildElements[ i ].Text )
                        array[ i ] = ChildElements[ i ].Text;
                    else
                        throw new ArrayTypeMismatchException( "String expected" );
                }
                val = array;
            }
            catch( Exception exc ) { HandleError( exc ); }

            return true;
        }

        #endregion

        private void HandleError( Exception exc )
        {
            Console.WriteLine( "\nConfigParser error:\n{0}\nElement: <{1}>{2}</{1}>\n", exc.Message, TagName, Text );
        }
    }
}