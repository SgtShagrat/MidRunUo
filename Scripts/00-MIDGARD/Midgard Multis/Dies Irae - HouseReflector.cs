/***************************************************************************
 *                               HouseReflector.cs
 *                            -----------------------
 *   begin                : 01 April, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using Server;
using Server.Commands;
using Server.Multis;
using Server.Multis.Deeds;

namespace Midgard.Multis
{
    public class HouseReflector
    {
        private static List<Type> m_Types = new List<Type>();

        public static void Initialize()
        {
            CommandSystem.Register( "GenHouseXml", AccessLevel.Developer, GenHouseXml_OnCommand );
        }

        [Usage( "[GenHouseXml" )]
        [Description( "Generates xml data for house system." )]
        private static void GenHouseXml_OnCommand(CommandEventArgs e)
        {
            ProcessTypes();
            GenerateXml();
            LogSBHouseDeed();
            LogHouses();
        }

        private static void LogHouses()
        {
            string path = "Logs/midgardHouses.log";
            using( StreamWriter sw = new StreamWriter( path ) )
            {
                Mobile owner = new Mobile();
                BaseHouse house = null;
                HouseDeed deed = null;
                Type houseType = null;

                sw.WriteLine( string.Format( "Building Name\tLockdowns\tSecure\tCost" ) );

                foreach( Type type in m_Types )
                {
                    if( type.IsAbstract )
                        continue;

                    try
                    {
                        deed = Activator.CreateInstance( type ) as HouseDeed;
                        house = deed.GetHouse( owner );
                        houseType = house.GetType();
                    }
                    catch( Exception ex )
                    {
                        Console.WriteLine( ex.ToString() );
                    }

                    if( house == null )
                    {
                        Console.WriteLine( "Warning: null house for type: " + type.Name );
                        continue;
                    }

                    string name = MidgardUtility.GetFriendlyClassName( deed.GetType().Name );
                    name = name.Replace( "Deed", "" );

                    int lockDowns = house.MaxLockDowns;
                    int secures = house.MaxSecures;
                    int price = house.DefaultPrice;

                    sw.WriteLine( string.Format( "{0}\t{1}\t{2}\t{3}", name, lockDowns, secures, price ) );
                }
            }
        }

        private static void GenerateXml()
        {
            try
            {
                string path = "midgardHouses.xml";

                // Creazione dell'xml vuoto.
                XmlDocument doc = new XmlDocument();

                using( XmlTextWriter textWriter = new XmlTextWriter( path, null ) )
                {
                    textWriter.Formatting = Formatting.Indented;
                    textWriter.WriteStartElement( "houses" );
                    textWriter.WriteEndElement();
                    textWriter.Flush();
                }

                using( FileStream fileStream = new FileStream( path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite ) )
                {
                    doc.Load( fileStream );
                }

                Mobile owner = new Mobile();
                BaseHouse house = null;
                HouseDeed deed = null;
                Type houseType = null;

                foreach( Type type in m_Types )
                {
                    try
                    {
                        deed = Activator.CreateInstance( type ) as HouseDeed;
                        house = deed.GetHouse( owner );
                        houseType = house.GetType();
                    }
                    catch( Exception ex )
                    {
                        Console.WriteLine( ex.ToString() );
                    }

                    if( house == null )
                    {
                        Console.WriteLine( "Warning: null house for type: " + type.Name );
                        continue;
                    }

                    XmlElement houseElement = doc.CreateElement( "house" );

                    //<house id="0x23">
                    houseElement.SetAttribute( "id", String.Format( "0x{0:X}", house.ItemID & 0x3FFF ) );
                    houseElement.SetAttribute( "group", "General" );

                    //<name>Fortresses</name>
                    XmlElement name = doc.CreateElement( "name" );
                    name.InnerText = HouseFriendlyNameAttribute.GetHouseFriendlyNameFor( houseType );
                    houseElement.InsertAfter( name, houseElement.LastChild );

                    //<footPrint>16 x 14</footPrint>
                    XmlElement footPrint = doc.CreateElement( "footPrint" );
                    footPrint.InnerText = FormatFootPrint( house );
                    houseElement.InsertAfter( footPrint, houseElement.LastChild );

                    //<stories>3 + Roof</stories>
                    XmlElement stories = doc.CreateElement( "stories" );
                    stories.InnerText = HouseStoriesAttribute.GetHouseStoriesFor( houseType );
                    houseElement.InsertAfter( stories, houseElement.LastChild );

                    //<rooms>3 Stairway Rooms</rooms>
                    XmlElement rooms = doc.CreateElement( "rooms" );
                    rooms.InnerText = HouseRoomsAttribute.GetHouseRoomsFor( houseType );
                    houseElement.InsertAfter( rooms, houseElement.LastChild );

                    //<secures>12</secures>
                    XmlElement secures = doc.CreateElement( "secures" );
                    secures.InnerText = house.MaxSecures.ToString();
                    houseElement.InsertAfter( secures, houseElement.LastChild );

                    //<lockdowns>244</lockdowns>
                    XmlElement lockdowns = doc.CreateElement( "lockdowns" );
                    lockdowns.InnerText = house.MaxLockDowns.ToString();
                    houseElement.InsertAfter( lockdowns, houseElement.LastChild );

                    //<includes>-</includes>
                    XmlElement includes = doc.CreateElement( "includes" );
                    includes.InnerText = HouseIncludesAttribute.GetHouseIncludesFor( houseType );
                    houseElement.InsertAfter( includes, houseElement.LastChild );

                    //<cost>-</cost>
                    XmlElement cost = doc.CreateElement( "cost" );
                    cost.InnerText = house.DefaultPrice.ToString();
                    houseElement.InsertAfter( cost, houseElement.LastChild );

                    //<description>
                    //  This building is quite large, and has plenty of room for an entire Guild.
                    //  The addition of a useable roof makes this building quite unique.
                    //  It's one shortcoming is that from outside, you can see into Floor 3 
                    //  if you stand under one of the Roof Supports (seen on the Third Floor Image).
                    //</description>
                    XmlElement description = doc.CreateElement( "description" );
                    description.InnerText = HouseDescriptionAttribute.GetHouseDescriptionFor( houseType );
                    houseElement.InsertAfter( description, houseElement.LastChild );

                    //<images>
                    //<image stage="0" stagename="floor">0x23_0.bmp</image>
                    //</images>
                    XmlElement images = doc.CreateElement( "images" );

                    XmlElement image = doc.CreateElement( "image" );
                    image.SetAttribute( "stage", "..." );
                    image.SetAttribute( "stagename", "..." );

                    image.InnerText = String.Format( "0x{0:X}_0.bmp", house.ItemID & 0x3FFF );
                    images.InsertAfter( image, images.LastChild );

                    houseElement.InsertAfter( images, houseElement.LastChild );

                    doc.DocumentElement.InsertAfter( houseElement, doc.DocumentElement.LastChild );

                    if( !house.Deleted )
                        house.Delete();
                }

                using( FileStream outStream = new FileStream( path, FileMode.Truncate, FileAccess.Write, FileShare.Write ) )
                {
                    doc.Save( outStream );
                }
            }
            catch( Exception ex )
            {
                Console.WriteLine( ex.ToString() );
            }
        }

        private static void LogSBHouseDeed()
        {
            Mobile owner = new Mobile();
            BaseHouse house = null;
            HouseDeed deed = null;
            Type houseType = null;

            using( StreamWriter op = new StreamWriter( "Logs/SBHouseDeed.log", true ) )
            {
                foreach( Type type in m_Types )
                {
                    try
                    {
                        deed = Activator.CreateInstance( type ) as HouseDeed;
                        house = deed.GetHouse( owner );
                        houseType = house.GetType();
                    }
                    catch( Exception ex )
                    {
                        Console.WriteLine( ex.ToString() );
                    }

                    if( house == null )
                    {
                        Console.WriteLine( "Warning: null house for type: " + type.Name );
                        continue;
                    }

                    op.WriteLine( "Add( new GenericBuyInfo( \"{0}\", typeof( {1} ), {2}, 20, 0x14F0, 0 ) );", deed.DefaultName, deed.GetType().Name, house.DefaultPrice );
                }
            }
        }

        private static string FormatFootPrint( BaseHouse house )
        {
            StringBuilder builder = new StringBuilder();

            foreach( Rectangle2D area in house.Area )
                builder.AppendFormat( "{0} x {1} ", area.Width, area.Height );

            builder.Remove( builder.Length - 1, 1 ); // remove the trailing space

            return builder.ToString();
        }

        private static void ProcessTypes()
        {
            Assembly[] asms = ScriptCompiler.Assemblies;

            for( int i = 0; i < asms.Length; ++i )
            {
                Assembly asm = asms[ i ];
                TypeCache tc = ScriptCompiler.GetTypeCache( asm );
                Type[] types = tc.Types;

                for( int j = 0; j < types.Length; ++j )
                {
                    Type type = types[ j ];

                    if( type.IsSubclassOf( typeof( HouseDeed ) ) )
                        m_Types.Add( type );
                }
            }

            m_Types.Sort( InternalComparer.Instance );
        }

        #region Nested type: InternalComparer
        private class InternalComparer : IComparer<Type>
        {
            public static readonly IComparer<Type> Instance = new InternalComparer();

            private InternalComparer()
            {
            }

            #region IComparer<Type> Members
            public int Compare( Type x, Type y )
            {
                if( x == null || y == null )
                    throw new ArgumentException();

                return Insensitive.Compare( x.Name, y.Name );
            }
            #endregion
        }
        #endregion
    }

    public class HouseDescriptionAttribute : Attribute
    {
        private string m_Description;

        public HouseDescriptionAttribute( string description )
        {
            m_Description = description;
        }

        public string Description
        {
            get { return m_Description; }
        }

        public static string GetHouseDescriptionFor( Type t )
        {
            if( t.IsDefined( typeof( HouseDescriptionAttribute ), false ) )
            {
                object[] objs = t.GetCustomAttributes( typeof( HouseDescriptionAttribute ), false );

                if( objs != null && objs.Length > 0 )
                {
                    HouseDescriptionAttribute attribute = objs[ 0 ] as HouseDescriptionAttribute;

                    if( attribute != null )
                        return attribute.Description;
                }
            }

            return "...";
        }
    }

    public class HouseStoriesAttribute : Attribute
    {
        private string m_Stories;

        public HouseStoriesAttribute( string stories )
        {
            m_Stories = stories;
        }

        public string Stories
        {
            get { return m_Stories; }
        }

        public static string GetHouseStoriesFor( Type t )
        {
            if( t.IsDefined( typeof( HouseStoriesAttribute ), false ) )
            {
                object[] objs = t.GetCustomAttributes( typeof( HouseStoriesAttribute ), false );

                if( objs != null && objs.Length > 0 )
                {
                    HouseStoriesAttribute attribute = objs[ 0 ] as HouseStoriesAttribute;

                    if( attribute != null )
                        return attribute.Stories;
                }
            }

            return "...";
        }
    }

    public class HouseRoomsAttribute : Attribute
    {
        private string m_Rooms;

        public HouseRoomsAttribute( string rooms )
        {
            m_Rooms = rooms;
        }

        public string Rooms
        {
            get { return m_Rooms; }
        }

        public static string GetHouseRoomsFor( Type t )
        {
            if( t.IsDefined( typeof( HouseRoomsAttribute ), false ) )
            {
                object[] objs = t.GetCustomAttributes( typeof( HouseRoomsAttribute ), false );

                if( objs != null && objs.Length > 0 )
                {
                    HouseRoomsAttribute attribute = objs[ 0 ] as HouseRoomsAttribute;

                    if( attribute != null )
                        return attribute.Rooms;
                }
            }

            return "...";
        }
    }

    public class HouseIncludesAttribute : Attribute
    {
        private string m_Includes;

        public HouseIncludesAttribute( string includes )
        {
            m_Includes = includes;
        }

        public string Includes
        {
            get { return m_Includes; }
        }

        public static string GetHouseIncludesFor( Type t )
        {
            if( t.IsDefined( typeof( HouseIncludesAttribute ), false ) )
            {
                object[] objs = t.GetCustomAttributes( typeof( HouseIncludesAttribute ), false );

                if( objs != null && objs.Length > 0 )
                {
                    HouseIncludesAttribute attribute = objs[ 0 ] as HouseIncludesAttribute;

                    if( attribute != null )
                        return attribute.Includes;
                }
            }

            return "...";
        }
    }

    public class HouseFriendlyNameAttribute : Attribute
    {
        private string m_Name;

        public HouseFriendlyNameAttribute( string stories )
        {
            m_Name = stories;
        }

        public string Name
        {
            get { return m_Name; }
        }

        public static string GetHouseFriendlyNameFor( Type t )
        {
            if( t.IsDefined( typeof( HouseFriendlyNameAttribute ), false ) )
            {
                object[] objs = t.GetCustomAttributes( typeof( HouseFriendlyNameAttribute ), false );

                if( objs != null && objs.Length > 0 )
                {
                    HouseFriendlyNameAttribute attribute = objs[ 0 ] as HouseFriendlyNameAttribute;

                    if( attribute != null )
                        return attribute.Name;
                }
            }

            return t.Name;
        }
    }
}