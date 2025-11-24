using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Server;
using Server.Engines.Craft;

namespace Midgard.Engines.OldCraftSystem
{
    public class ParentNode : IMenuItem
    {
        private List<Type> m_CustomResourceType;

        private readonly CraftSystem m_System;
        private bool m_UsesMainSubRes;
        private bool m_UsesSecondarySubRes;

        public ParentNode( CraftSystem system, XmlTextReader xml, ParentNode parent )
        {
            m_System = system;
            m_MainSkillReq = -1.0;

            Parent = parent;

            Parse( xml );
        }

        public ParentNode Parent { get; private set; }

        /// <summary>
        /// Array of ParentNode and ChildNode nodes
        /// </summary>
        public object[] Children { get; private set; }

        /// <summary>
        /// Get if our node has only ChildNode nodes in Children array
        /// </summary>
        public bool HasOnlyChildNodesForChildren
        {
            get
            {
                bool hasOnlyChildren = true;
                foreach( object o in Children )
                {
                    if( o is ParentNode )
                        hasOnlyChildren = false;
                }

                return hasOnlyChildren;
            }
        }

        /// <summary>
        /// Get if our node has only ParentNode nodes in Children array
        /// </summary>
        public bool HasOnlyParentNodesForChildren
        {
            get
            {
                bool hasOnlyParents = true;
                foreach( object o in Children )
                {
                    if( o is ChildNode )
                        hasOnlyParents = false;
                }

                return hasOnlyParents;
            }
        }

        /// <summary>
        /// Displayed name in OldCraftMenu
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Displayed ID in OldCraftMenu
        /// </summary>
        public int ItemID { get; private set; }

        /// <summary>
        /// The color of the item in the old craft menu
        /// </summary>
        public int Hue { get; private set; }

        /// <summary>
        /// The string displayed above the OldCraftMenu
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// If true force resource selection before child selection
        /// </summary>
        public bool SelectResource { get; private set; }

        /// <summary>
        /// Collection of CraftSkill for this node used to verify
        /// if a mobile can access this parent node
        /// </summary>
        public CraftSkillCol RequiredSkills { get; private set; }

        /// <summary>
        /// The craft system whom this node belongs to
        /// </summary>
        public CraftSystem System
        {
            get
            {
                if( m_System == null )
                    OldCraftMenu.DebugMessage( "Warning: null craft system in ParentNode {0}.", Name );

                return m_System;
            }
        }

        private double m_MainSkillReq;

        public double MainSkillReq
        {
            get
            {
                if( m_MainSkillReq == -1 )
                {
                    for( int i = 0; i < RequiredSkills.Count; i++ )
                    {
                        CraftSkill skill = RequiredSkills.GetAt( i );
                        if( skill.SkillToMake == System.MainSkill )
                        {
                            m_MainSkillReq = skill.MinSkill;
                        }
                    }
                }

                return m_MainSkillReq;
            }
        }

        /// <summary>
        /// Is true if our node supports the main resources of <see cref="System"/>
        /// </summary>
        public bool UsesMainSubRes
        {
            get { return m_UsesMainSubRes; }
            set
            {
                m_UsesMainSubRes = value;

                if( Parent != null )
                    Parent.UsesMainSubRes = value;
            }
        }

        /// <summary>
        /// Is true if our node supports the secondary resources of <see cref="System"/>
        /// </summary>
        public bool UsesSecondarySubRes
        {
            get { return m_UsesSecondarySubRes; }
            set
            {
                m_UsesSecondarySubRes = value;

                if( Parent != null )
                    Parent.UsesSecondarySubRes = value;
            }
        }

        /// <summary>
        /// Is true if our node supports custom resource types (for ex. Cloth for DefTailoring)
        /// </summary>
        public bool UsesCustomSubRes
        {
            get { return m_CustomResourceType != null && m_CustomResourceType.Count > 0; }
        }

        /// <summary>
        /// The custom resource type supported by this parent node
        /// </summary>
        public List<Type> CustomResourceTypes
        {
            get
            {
                if( m_CustomResourceType == null )
                    m_CustomResourceType = new List<Type>();

                return m_CustomResourceType;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine( string.Format( "Parent: {0}\n", Name ) );
            sb.AppendLine( string.Format( "MainSkillReq: {0:F3}\n", MainSkillReq ) );
            sb.AppendLine( string.Format( "\tHasOnlyChildNodesForChildren: {0}\n", HasOnlyChildNodesForChildren ) );
            sb.AppendLine( string.Format( "\tHasOnlyParentNodesForChildren: {0}\n", HasOnlyParentNodesForChildren ) );
            sb.AppendLine( string.Format( "\tItemID: {0}\n", ItemID ) );
            sb.AppendLine( string.Format( "\tHue: {0}\n", Hue ) );
            sb.AppendLine( string.Format( "\tTitle: {0}\n", Title ) );
            sb.AppendLine( string.Format( "\tSelectResource: {0}\n", SelectResource ) );
            sb.AppendLine( string.Format( "\tUsesMainSubRes: {0}\n", UsesMainSubRes ) );
            sb.AppendLine( string.Format( "\tUsesSecondarySubRes: {0}\n", UsesSecondarySubRes ) );
            sb.AppendLine( string.Format( "\tUsesCustomSubRes: {0}\n", UsesCustomSubRes ) );

            if( m_CustomResourceType != null && m_CustomResourceType.Count > 0 )
            {
                sb.AppendLine( string.Format( "\tCustomResourceTypes:" ) );
                foreach( Type type in m_CustomResourceType )
                    sb.AppendLine( string.Format( "\t\t{0}\n", type.Name ) );
            }

            if( RequiredSkills != null )
            {
                sb.AppendLine( string.Format( "\tRequiredSkills:" ) );
                for( int i = 0; i < RequiredSkills.Count; i++ )
                    sb.AppendLine( string.Format( "\t\t{0}-{1}\n", RequiredSkills.GetAt( i ).SkillToMake, RequiredSkills.GetAt( i ).MinSkill ) );
            }

            return sb.ToString();
        }

        private void Parse( XmlTextReader xml )
        {
            Name = xml.MoveToAttribute( "name" ) ? xml.Value : "empty";
            ItemID = xml.MoveToAttribute( "itemID" ) ? Utility.ToInt32( xml.Value ) : 0;
            Title = xml.MoveToAttribute( "title" ) ? xml.Value : "";
            SelectResource = xml.MoveToAttribute( "selectresource" ) && Utility.ToBoolean( xml.Value );

            if( xml.MoveToAttribute( "hue" ) )
                Hue = Utility.ToInt32( xml.Value );

            if( xml.IsEmptyElement )
                Children = new object[ 0 ];
            else
            {
                ArrayList children = new ArrayList();

                while( xml.Read() && xml.NodeType == XmlNodeType.Element )
                {
                    if( xml.Name == "child" )
                    {
                        ChildNode n = new ChildNode( m_System, xml, this );
                        children.Add( n );
                    }
                    else
                        children.Add( new ParentNode( m_System, xml, this ) );
                }

                Children = children.ToArray();
            }
        }

        /// <summary>
        /// Check if our mobile satisfy skill requirements for this node
        /// </summary>
        public bool CheckSkills( Mobile from )
        {
            OldCraftMenu.DebugMessage( "ParentNode.CheckSkills" );
            if( RequiredSkills == null )
                return true;

            OldCraftMenu.DebugMessage( "\tchecking skills..." );
            OldCraftMenu.DebugMessage( "\t\t{0} {1}", from.Skills[ System.MainSkill ].Value, MainSkillReq );
            // parent nodes checks only for main skill

            return from.Skills[ System.MainSkill ].Value >= MainSkillReq;

            /*
            for( int i = 0; i < RequiredSkills.Count; i++ )
            {
                CraftSkill craftSkill = RequiredSkills.GetAt( i );

                double minSkill = craftSkill.MinSkill;
                double valSkill = from.Skills[ craftSkill.SkillToMake ].Value;

                OldCraftMenu.DebugMessage( "\t{0} {1} {2}", craftSkill.SkillToMake, minSkill, valSkill );

                if( valSkill < minSkill )
                    return false;
            }

            return true;
            */
        }

        /// <summary>
        /// Check if our mobile satisfy resource requirements for this node
        /// </summary>
        public bool CheckResources( Mobile from )
        {
            return true;
        }

        /// <summary>
        /// Update our resource requirement according to a CraftItem passed
        /// </summary>
        public void InvalidateReqResources( ResReqTypes type, Type t )
        {
            switch( type )
            {
                case ResReqTypes.Primary:
                    UsesMainSubRes = true;
                    break;
                case ResReqTypes.Secondary:
                    UsesSecondarySubRes = true;
                    break;
                case ResReqTypes.Custom:
                    if( t != null )
                        RegisterCustomType( t );
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Add a new custom type to our custom type list
        /// and refresh parent supported types list.
        /// </summary>
        public void RegisterCustomType( Type t )
        {
            if( t == null )
                return;

            if( !CustomResourceTypes.Contains( t ) )
                CustomResourceTypes.Add( t );

            if( Parent != null )
                Parent.RegisterCustomType( t );
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

            OldCraftMenu.DebugMessage( "SupportResource for parentNode: {0} and type {1}", Name, t.Name );
            OldCraftMenu.DebugMessage( "\tm_UsesMainSubRes {0}, m_UsesSecondarySubRes {1}, m_UsesCustomSubRes {2}: ", m_UsesMainSubRes, m_UsesSecondarySubRes, UsesCustomSubRes );
            OldCraftMenu.DebugMessage( "\tCustomResourceTypes.Contains( t ): {0}", CustomResourceTypes.Contains( t ) );

            if( Config.Debug )
            {
                if( UsesCustomSubRes )
                {
                    Config.Pkg.LogInfoLine( "CustomSubRes list" );
                    foreach( Type type in CustomResourceTypes )
                    {
                        Config.Pkg.LogInfoLine( "->" + type.Name );
                    }
                }
            }

            if( UsesCustomSubRes && CustomResourceTypes.Contains( t ) )
                return true;

            if( m_UsesMainSubRes && IsValidResource( t, m_System.CraftSubRes ) )
                return true;

            if( m_UsesSecondarySubRes && IsValidResource( t, m_System.CraftSubRes2 ) )
                return true;

            return false;
        }

        /// <summary>
        /// Update required skills for item passed.
        /// If the skill is not present, just add the requirement.
        /// If the skill is found, check and update its minimum requirement value.
        /// </summary>
        /// <param name="item">item to check</param>
        public void InvalidateReqSkills( CraftItem item )
        {
            if( item == null )
                return;

            if( RequiredSkills == null )
                RequiredSkills = new CraftSkillCol();

            // OldCraftMenu.DebugMessage( "InvalidateReqSkills" );

            CraftSkillCol toTest = item.Skills;

            for( int i = 0; i < toTest.Count; i++ )
            {
                CraftSkill newSkill = toTest.GetAt( i );

                bool found = false;
                for( int j = 0; !found && j < RequiredSkills.Count; j++ )
                {
                    CraftSkill nodeSkill = RequiredSkills.GetAt( j );

                    if( newSkill.SkillToMake == nodeSkill.SkillToMake )
                    {
                        found = true;

                        if( newSkill.MinSkill < nodeSkill.MinSkill )
                            nodeSkill.MinSkill = newSkill.MinSkill;
                    }
                }

                if( !found )
                    RequiredSkills.Add( newSkill );
            }

            // OldCraftMenu.DebugMessage( "RequiredSkills for node: {0}", Name );
            //for( int j = 0; j < RequiredSkills.Count; j++ )
            //    OldCraftMenu.DebugMessage( "\t{0} {1:F2}", RequiredSkills.GetAt( j ).SkillToMake, RequiredSkills.GetAt( j ).MinSkill );

            if( Parent != null )
                Parent.InvalidateReqSkills( item );
        }
    }
}