using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Server;
using Server.Commands;
using Server.Gumps;

namespace Midgard.Commands
{
    public class AddCustomGoLocation
    {
        private static readonly string m_Path = Path.Combine( Path.Combine( "Data", "Locations" ), "felucca.xml" );
        private static XDocument m_Document;

        public static void Initialize()
        {
            CommandSystem.Register( "AddCustomGoLocation", AccessLevel.GameMaster, new CommandEventHandler( AddCustomGoLocation_OnCommand ) );
        }

        [Usage( "AddCustomGoLocation <locationName>" )]
        [Description( "Add a custom location to the \"go\" system" )]
        private static void AddCustomGoLocation_OnCommand( CommandEventArgs e )
        {
            if( e.Length != 1 )
            {
                e.Mobile.SendMessage( "Usage: [AddCustomGoLocation <locationName>" );
                return;
            }

            string name = e.GetString( 0 );
            if( string.IsNullOrEmpty( name ) )
            {
                e.Mobile.SendMessage( "You entered an invalid name for this location" );
                return;
            }

            try
            {
                m_Document = m_Document ?? XDocument.Load( XmlReader.Create( m_Path ) );

                IEnumerable<XElement> query = from c in m_Document.Element( "places" ).Elements( "parent" ).Elements( "parent" )
                                              where c.Attribute( "name" ).Value == "Custom"
                                              select c;

                Point3D location = e.Mobile.Location;

                XElement elem = query.First();
                // <child name="Test" x="5826" y="2184" z="0" />
                elem.Add( new XElement( "child", new XAttribute( "name", name ),
                                        new XAttribute( "x", location.X ),
                                        new XAttribute( "y", location.Y ),
                                        new XAttribute( "z", location.Z ) ) );

                m_Document.Save( m_Path, SaveOptions.None );

               GoGump.Felucca = new LocationTree( "felucca.xml", Map.Felucca );
            }
            catch( Exception ex )
            {
                Console.WriteLine( ex.ToString() );
                e.Mobile.SendMessage( "An error occurred." );
            }
        }
    }
}