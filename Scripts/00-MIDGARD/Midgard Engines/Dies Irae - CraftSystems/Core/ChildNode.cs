using System;
using System.Text;
using System.Xml;

using Server;
using Server.Engines.Craft;

namespace Midgard.Engines.OldCraftSystem
{
    public enum ChildNodeType
    {
        Normal,
        MakeLast
    }

    public enum ResReqTypes
    {
        Unknown,
        Primary,
        Secondary,
        Custom
    }

    public class ChildNode : IMenuItem
    {
        private CraftSystem m_CraftSystem;
        private ResReqTypes m_ReqResType = ResReqTypes.Unknown;

        public ChildNode( CraftSystem system, XmlReader xml, ParentNode parent )
        {
            Parent = parent;
            m_CraftSystem = system;

            Parse( xml );
        }

        /// <summary>
        ///  The ParentNode of this node
        /// </summary>
        public ParentNode Parent { get; private set; }

        /// <summary>
        /// Name displayed in OldCrafteMenu
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// CraftItem which this node refers to
        /// </summary>
        public CraftItem NodeCraftItem { get; private set; }

        /// <summary>
        /// Denotes if this node is a special type node (like MakeLast)
        /// </summary>
        public ChildNodeType ChildType { get; private set; }

        /// <summary>
        /// Denotes if our node requires main, secondary or custom resources
        /// </summary>
        public ResReqTypes ReqResType { get { return m_ReqResType; } }

        /// <summary>
        /// This is the type or custom resource required for this child.
        /// An ex. is Cloth for DefTailoring
        /// </summary>
        public Type CustomResourceType { get; private set; }

        /// <summary>
        /// Returns true if it is mandatory for this node to have a CraftItem linked
        /// </summary>
        public bool ShouldHaveNodeCraftItem { get { return ChildType == ChildNodeType.Normal; } }

        /// <summary>
        /// Return the image displayed in the OldCraftMenu for this node
        /// </summary>
        public int ItemID
        {
            get
            {
                if( ChildType == ChildNodeType.MakeLast )
                    return m_CraftSystem.MakeLastID;

                return NodeCraftItem != null ? CraftItem.ItemIDOf( NodeCraftItem.ItemType ) : 0;
            }
        }

        /// <summary>
        /// The color of the item in the old craft menu
        /// </summary>
        public int Hue { get; private set; }

        private void Parse( XmlReader xml )
        {
            Name = xml.MoveToAttribute( "name" ) ? xml.Value : "empty";

            if( Name == "MakeLast" )
                ChildType = ChildNodeType.MakeLast;
            else
            {
                if( xml.MoveToAttribute( "typename" ) && m_CraftSystem != null )
                {
                    Type t = ScriptCompiler.FindTypeByName( xml.Value );

                    if( t != null )
                    {
                        CraftItem item = m_CraftSystem.CraftItems.SearchFor( t );

                        if( item != null )
                            NodeCraftItem = item;
                        else
                        {
                            Config.Pkg.LogErrorLine( "Error: null m_CraftItem for childnode: {0}", Name );
                        }
                    }
                    else
                    {
                        Config.Pkg.LogErrorLine( "Error: null typeName for childnode: {0}", Name );
                    }
                }

                InvalidateReqType();
                if( Parent != null )
                    Parent.InvalidateReqSkills( NodeCraftItem );
            }

            if( xml.MoveToAttribute( "hue" ) )
                Hue = Utility.ToInt32( xml.Value );
        }

        /// <summary>
        /// Check if our mobile can access this node
        /// </summary>
        public bool CheckSkills( Mobile from )
        {
            if( NodeCraftItem == null )
                return true;

            for( int i = 0; i < NodeCraftItem.Skills.Count; i++ )
            {
                CraftSkill craftSkill = NodeCraftItem.Skills.GetAt( i );

                double minSkill = craftSkill.MinSkill;
                double valSkill = from.Skills[ craftSkill.SkillToMake ].Value;

                if( valSkill < minSkill )
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Check it our typte t is in a given collection 
        /// </summary>
        public bool IsValidResource( Type t, CraftSubResCol coll )
        {
            if( Config.Debug )
                foreach( CraftSubRes re in coll )
                    Config.Pkg.LogInfoLine( StringList.GetClilocString( re.NameString, re.NameNumber ) );

            return coll.SearchFor( t ) != null;
        }

        /// <summary>
        /// Usefull to check if our node supports a resource given by type
        /// </summary>
        /// <param name="t">type to check</param>
        /// <returns>true if this node supports t or even if a child do</returns>
        public bool SupportResource( Type t )
        {
            if( t == null )
            {
                OldCraftMenu.DebugMessage( "SupportResource t == null" );
                return false;
            }

            OldCraftMenu.DebugMessage( "SupportResource for childNode: {0} and type {1}", Name, t.Name );
            OldCraftMenu.DebugMessage( "\tReqResType {0}, ", ReqResType );
            if( CustomResourceType != null )
                OldCraftMenu.DebugMessage( "\tCustomResourceType: {0}", CustomResourceType.Name );

            switch( ReqResType )
            {
                case ResReqTypes.Custom:
                    return CustomResourceType == t;
                case ResReqTypes.Primary:
                    return IsValidResource( t, m_CraftSystem.CraftSubRes );
                case ResReqTypes.Secondary:
                    return IsValidResource( t, m_CraftSystem.CraftSubRes2 );
            }

            return false;
        }

        /// <summary>
        /// Check if our mobile satisfy resource requirements for this node
        /// </summary>
        public bool CheckResources( Mobile from )
        {
            return true;
        }

        /// <summary>
        /// Invalidate requirement type and update parent root
        /// </summary>
        public void InvalidateReqType()
        {
            // OldCraftMenu.DebugMessage( "Debug for InvalidateReqType " + Name );

            if( NodeCraftItem == null )
            {
                OldCraftMenu.DebugMessage( "\tm_CraftItem == null" );
                return;
            }

            if( m_CraftSystem.CraftSubRes2.Init && NodeCraftItem.UseSubRes2 )
                m_ReqResType = ResReqTypes.Secondary;
            else if( m_CraftSystem.CraftSubRes.Init )
                m_ReqResType = ResReqTypes.Primary;

            // OldCraftMenu.DebugMessage( "\t" + m_ReqResType );

            CraftRes craftResource = NodeCraftItem.Resources.GetAt( 0 );

            if( m_CraftSystem.CraftSubRes.Init || m_CraftSystem.CraftSubRes2.Init )
            {
                // OldCraftMenu.DebugMessage( "\tm_CraftSystem.CraftSubRes.Init: " + m_CraftSystem.CraftSubRes.Init );
                // OldCraftMenu.DebugMessage( "\tm_CraftSystem.CraftSubRes2.Init: " + m_CraftSystem.CraftSubRes2.Init );

                if( craftResource.ItemType != m_CraftSystem.CraftSubRes.ResType &&
                    craftResource.ItemType != m_CraftSystem.CraftSubRes2.ResType )
                {
                    // OldCraftMenu.DebugMessage( "\tItemType != CraftSubRes.ResType: " + ( craftResource.ItemType != m_CraftSystem.CraftSubRes.ResType ) );
                    // OldCraftMenu.DebugMessage( "\tItemType != CraftSubRes2.ResType: " + ( craftResource.ItemType != m_CraftSystem.CraftSubRes2.ResType ) );
                    CustomResourceType = craftResource.ItemType;
                }
            }
            else
            {
                // OldCraftMenu.DebugMessage( "\tCustom res type: " + craftResource.ItemType.Name );
                CustomResourceType = craftResource.ItemType;
            }

            if( CustomResourceType != null )
                m_ReqResType = ResReqTypes.Custom;

            if( m_ReqResType != ResReqTypes.Unknown && Parent != null )
                Parent.InvalidateReqResources( m_ReqResType, CustomResourceType );
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine( string.Format( "Child Node: {0}\n", Name ) );
            sb.AppendLine( string.Format( "\tNodeCraftItem.ItemType.Name: {0}\n", NodeCraftItem.ItemType.Name ) );
            sb.AppendLine( string.Format( "\tChildType: {0}\n", ChildType ) );
            sb.AppendLine( string.Format( "\tReqResType: {0}\n", ReqResType ) );
            sb.AppendLine( string.Format( "\tCustomResourceType: {0}\n", CustomResourceType == null ? "null" : CustomResourceType.Name ) );
            sb.AppendLine( string.Format( "\tShouldHaveNodeCraftItem: {0}\n", ShouldHaveNodeCraftItem ) );
            sb.AppendLine( string.Format( "\tItemID: {0}\n", ItemID ) );
            sb.AppendLine( string.Format( "\tHue: {0}\n", Hue ) );
            return sb.ToString();
        }

        public bool CanBeCraftedBy( Mobile from, CraftContext context )
        {
            if( from == null )
                return true;

            if( context == null )
                return false;

            Type itemType = NodeCraftItem.ItemType;
            Type lastResourceSelected = context.LastResourceSelected;

            Item i = OldDisplayCache.Cache.Lookup( itemType );

            bool response = i != null && i.CanBeCraftedWith( lastResourceSelected ) && i.CanBeCraftedBy( from );

            if( Config.Debug )
                Console.WriteLine( "{0} CanBeCraftedBy {1} with {2}: {3}", itemType.Name, from.Name ?? "", lastResourceSelected.Name, response );

            return response;
        }
    }
}