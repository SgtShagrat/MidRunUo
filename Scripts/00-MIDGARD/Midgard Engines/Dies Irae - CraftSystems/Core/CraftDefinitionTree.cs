using System.IO;
using System.Xml;
using Server.Engines.Craft;

namespace Midgard.Engines.OldCraftSystem
{
    public class CraftDefinitionTree
    {
        public CraftDefinitionTree( string fileName, CraftSystem system )
        {
            DefinitionCraftSystem = system;

            string path = Path.Combine( "Data/OldCraftDefinitions/", fileName );

            if( File.Exists( path ) )
            {
                XmlTextReader xml = new XmlTextReader( new StreamReader( path ) );

                xml.WhitespaceHandling = WhitespaceHandling.None;

                Root = Parse( xml );

                xml.Close();
            }
        }

        public CraftSystem DefinitionCraftSystem { get; private set; }

        public ParentNode Root { get; private set; }

        private ParentNode Parse( XmlTextReader xml )
        {
            xml.Read();
            xml.Read();
            xml.Read();

            return new ParentNode( DefinitionCraftSystem, xml, null );
        }
    }
}