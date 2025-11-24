using System.Xml;

using Server.Engines.Craft;

namespace Midgard.Engines.OldCraftSystem
{
    internal class MakeLastChildNode : ChildNode
    {
        public MakeLastChildNode( CraftSystem system, XmlReader xml, ParentNode parent )
            : base( system, xml, parent )
        {
        }
    }
}