using System;
using System.Collections.Generic;

using Midgard.Engines.AutoLoop;

using Server;
using Server.Engines.Craft;
using Server.Items;
using Server.Menus.ItemLists;
using Server.Network;

namespace Midgard.Engines.OldCraftSystem
{
    public interface IMenuItem
    {
        string Name { get; }
        int Hue { get; }
        int ItemID { get; }
    }

    public class OldCraftMenu : ItemListMenu
    {
        private readonly CraftSystem m_CraftSystem;
        private readonly BaseTool m_Tool;
        private readonly ParentNode m_Node;
        private readonly ItemListEntry[] m_ItemsList;

        public static void DisplayTo( Mobile from, CraftSystem craftSystem, BaseTool tool, object message )
        {
            if( message is int && (int)message > 0 )
		from.SendLocalizedMessage( (int)message );
            else if( message is string )
                from.SendMessage( (string)message );

            if( AutoLoopContext.HasPendingAutoLoopContext( from ) )
            {
                CraftAutoLoopContext cont = AutoLoopContext.GetContext( from ) as CraftAutoLoopContext;
                if( Config.Debug )
                    Config.Pkg.LogInfoLine( "Autoloop context is not null: " + ( cont != null ) );

                if( cont != null )
                {
                    if( Config.Debug )
                        Config.Pkg.LogInfoLine( "Autoloop context message is: " + ( (int)message == 0 ) );
                    cont.DoNextOrEnd();
                }
                return;
            }

            DisplayTo( from, craftSystem, tool, craftSystem.DefinitionTree.Root );
        }

        public static void DisplayTo( Mobile from, CraftSystem craftSystem, BaseTool tool )
        {
            from.RevealingAction( true );

            if( AutoLoopContext.HasPendingAutoLoopContext( from ) )
            {
                AutoLoopContext loopContext = AutoLoopContext.GetContext( from );
                if( !loopContext.IsExpired( from ) )
                {
                    from.SendMessage( from.Language == "ITA" ? "Sei impegnato..." : "You are busy..." );
                    return;
                }
            }

            CraftContext context = craftSystem.GetContext( from );

            if( context == null )
                return;
            else
            {
                context.LastResourceSelected = null;
                DisplayTo( from, craftSystem, tool, craftSystem.DefinitionTree.Root );
            }
        }

        private static void DisplayTo( Mobile from, CraftSystem craftSystem, BaseTool tool, ParentNode branch )
        {
            CraftContext context = craftSystem.GetContext( from );
            if( context == null )
                return;

            if( branch.SelectResource && context.LastResourceSelected == null )
            {
                from.SendMessage( from.Language == "ITA" ? "Su cosa vuoi usarlo?" : "What would you like to use that on?" );
                from.Target = new SelectionTarget( craftSystem, tool, branch );
            }
            else
            {
                ItemListEntry[] list = GetEntriesByNode( from, craftSystem, branch );

                if( list.Length > 0 )
                    from.SendMenu( new OldCraftMenu( from, craftSystem, tool, branch, list ) );
                else
                    from.SendMessage( from.Language == "ITA" ? "Non puoi crearne di questo tipo." : "You cannot make anything of that kind." );
            }
        }

        public OldCraftMenu( Mobile from, CraftSystem craftSystem, BaseTool tool, ParentNode node, ItemListEntry[] list )
            : base( node.Title, list )
        {
            CraftContext context = craftSystem.GetContext( from );
            if( context == null )
                return;

            m_CraftSystem = craftSystem;
            m_Tool = tool;
            m_ItemsList = list;
            m_Node = node;
        }

        public static ItemListEntry[] GetEntriesByNode( Mobile from, CraftSystem craftSystem, ParentNode node )
        {
            List<ItemListEntry> list = new List<ItemListEntry>();

            CraftContext context = craftSystem.GetContext( from );
            if( context == null )
                return null;

            DebugMessage( "GetEntriesByNode: node ->>" + node.Name );

            if( context.LastMade != null )
                list.Add( new ItemListEntry( "Make Last", craftSystem.MakeLastID, 0 ) );

            for( int i = 0; i < node.Children.Length; ++i )
            {
                string name = null;
                int itemID = 0;
                int hue = 0;

                string resourceFormatted = null;

                if( node.Children[ i ] is ParentNode )
                {
                    ParentNode parentNode = ( (ParentNode)node.Children[ i ] );
                    DebugMessage( "GetEntriesByNode: parent node " + parentNode.Name );
                    DebugMessage( "\tcontext.LastResourceSelected == null {0}", ( context.LastResourceSelected == null ).ToString() );

                    if( parentNode.SupportResource( context.LastResourceSelected ) && parentNode.CheckSkills( from ) /* && parentNode.CheckResources( from ) */ )
                    {
                        name = parentNode.Name;
                        itemID = parentNode.ItemID;
                        hue = parentNode.Hue;
                    }
                    else
                        DebugMessage( "GetEntriesByNode: parent node not supported: {0}.", parentNode.Name );
                }
                else if( node.Children[ i ] is ChildNode )
                {
                    ChildNode childNode = ( (ChildNode)node.Children[ i ] );
                    DebugMessage( "GetEntriesByNode: parent node " + childNode.Name );

                    if( childNode.ChildType == ChildNodeType.MakeLast || childNode.ItemID == craftSystem.MakeLastID )
                    {
                        if( context.LastMade != null )
                            list.Add( new ItemListEntry( "Make Last", craftSystem.MakeLastID, 0 ) );
                    }
                    else
                    {
                        if( childNode.ShouldHaveNodeCraftItem && childNode.NodeCraftItem == null )
                        {
                            DebugMessage( string.Format( "Error: NodeCraftItem is null for node: {0}", childNode.Name ) );
                            continue;
                        }

                        CraftItem item = craftSystem.CraftItems.SearchFor( childNode.NodeCraftItem.ItemType );

                        if( item == null )
                            DebugMessage( "Error: NodeCraftItem for node {0} is null.", childNode.Name );
                        else if( childNode.SupportResource( context.LastResourceSelected ) && childNode.CheckSkills( from ) && childNode.CheckResources( from ) && childNode.CanBeCraftedBy( from, context ) )
                        {
                            name = childNode.Name;
                            itemID = CraftItem.ItemIDOf( item.ItemType );
                            resourceFormatted = FormatResource( from, craftSystem, item );
                            hue = childNode.Hue;
                        }
                    }
                }

                if( name != null && itemID > 0 )
                {
                    if( resourceFormatted != null )
                        list.Add( new ItemListEntry( String.Format( "{0}: {1}", name, resourceFormatted ), itemID, hue ) );
                    else
                        list.Add( new ItemListEntry( name, itemID, hue ) );
                }
            }

            return list.ToArray();
        }

        public static string FormatResource( Mobile from, CraftSystem system, CraftItem item )
        {
            if( item == null )
                return String.Empty;

            CraftRes craftResource = item.Resources.GetAt( 0 );

            string nameString = craftResource.NameString;
            int nameNumber = craftResource.NameNumber;

            string resourceFormatted;

            if( nameNumber > 0 )
                resourceFormatted = String.Format( "{0} {1}", craftResource.Amount, StringList.GetClilocString( null, nameNumber, from.Language ) );
            else
                resourceFormatted = String.Format( "{0} {1}", craftResource.Amount, nameString );

            return resourceFormatted;
        }

        public object GetNodeByEntry( ItemListEntry entry )
        {
            if( m_Node == null )
                return null;

            if( Config.Debug )
                Config.Pkg.LogInfoLine( "Node: {0} Entry: {1} {2}", m_Node.Name, entry.Name, entry.ItemID );

            foreach( object o in m_Node.Children )
            {
                if( o is IMenuItem )
                {
                    IMenuItem menuItem = (IMenuItem)o;

                    if( Config.Debug )
                        Config.Pkg.LogInfoLine( "SubNode: {0} {1} {2}", menuItem.Name, menuItem.ItemID, menuItem.Hue );

                    if( menuItem.ItemID == entry.ItemID && Insensitive.StartsWith( entry.Name, menuItem.Name ) )
                        return o;
                }
            }

            return null;
        }

        public override void OnResponse( NetState state, int index )
        {
            Mobile from = state.Mobile;

            if( Config.Debug )
                from.SendMessage( "Your index is " + index );

            CraftContext context = m_CraftSystem.GetContext( from );
            if( context == null )
            {
                Config.Pkg.LogWarningLine( "Warning: null context on OldCraftMenu.OnResponse" );
                return;
            }

            if( index > -1 && index < m_ItemsList.Length )
            {
                object o = GetNodeByEntry( m_ItemsList[ index ] );

                if( Config.Debug && o != null && o is IMenuItem )
                {
                    IMenuItem menuItem = (IMenuItem)o;
                    from.SendMessage( "You choose: {0} (itemid {1}, hue {2})", menuItem.Name, menuItem.ItemID, menuItem.Hue );
                }

                if( o == null )
                {
                    Config.Pkg.LogWarningLine( "Warning: null node on OldCraftMenu.OnResponse" );
                }
                else if( o is ParentNode )
                {
                    ParentNode parentNode = (ParentNode)o;
                    from.SendMessage( from.Language == "ITA" ? "Hai selezionato: {0}" : "You selected: {0}", parentNode.Name );

                    if( parentNode.HasOnlyChildNodesForChildren && !parentNode.CheckSkills( from ) )
                        from.SendMessage( from.Language == "ITA" ? "Non puoi creare nulla di questa categoria." : "You can't make anything of this category." );
                    else
                    {
                        ItemListEntry[] list = GetEntriesByNode( from, m_CraftSystem, parentNode );

                        if( list.Length > 0 )
                            from.SendMenu( new OldCraftMenu( from, m_CraftSystem, m_Tool, parentNode, list ) );
                        else
                            from.SendMessage( from.Language == "ITA" ? "Non puoi crearne di questo tipo." : "You cannot make anything of that kind." );
                    }
                }
                else if( o is ChildNode )
                {
                    ChildNode childNode = (ChildNode)o;
                    from.SendMessage( from.Language == "ITA" ? "Hai selezionato: {0}" : "You selected: {0}", childNode.Name );

                    if( childNode.ChildType == ChildNodeType.MakeLast || childNode.ItemID == m_CraftSystem.MakeLastID )
                    {
                        CraftItem item = context.LastMade;

                        if( item != null )
                            DoCraftItem( from, m_CraftSystem, m_Tool, item );
                        else
                            from.SendMessage( from.Language == "ITA" ? "Non hai ancora fatto nulla." : "You haven't made anything yet." );
                    }
                    else
                    {
                        if( childNode.ShouldHaveNodeCraftItem && childNode.NodeCraftItem == null )
                            DebugMessage( "Error: NodeCraftItem for node {0} is null.", childNode.Name );
                        else
                            DoCraftItem( from, m_CraftSystem, m_Tool, childNode.NodeCraftItem );
                    }
                }
            }
        }

        public static void DoCraftItem( Mobile from, CraftSystem system, BaseTool tool, CraftItem item )
        {
            if( Config.Debug )
                from.SendMessage( "DoCraftItem: you are crafting {0}, from {1} with {2}.", item.NameString, system.Name, tool.GetType().Name );

            int num = system.CanCraft( from, tool, item.ItemType );

            if( num > 0 )
            {
                from.SendLocalizedMessage( num );
            }
            else
            {
                Type type = null;

                CraftContext context = system.GetContext( from );

                if( context != null )
                {
                    CraftSubResCol res = ( item.UseSubRes2 ? system.CraftSubRes2 : system.CraftSubRes );
                    int resIndex = ( item.UseSubRes2 ? context.LastResourceIndex2 : context.LastResourceIndex );

                    if( resIndex >= 0 && resIndex < res.Count )
                        type = res.GetAt( resIndex ).ItemType;

                    if( type == null )
                        type = item.Resources.GetAt( 0 ).ItemType;
                }
                else
                    DebugMessage( "Warning: DocraftItem but context is null." );

                if( type != null )
                    DebugMessage( "DocraftItem: type {0}, res {1}", item.ItemType, type.Name );
                else
                    DebugMessage( "Warning: DocraftItem with null resource." );

                if( system.SupportsAutoLoop( item.ItemType ) )
                    new CraftAutoLoopContext( system, from, item.ItemType, type, tool, item );
                else
                    system.CreateItem( from, item.ItemType, type, tool, item );
            }
        }

        public static void DebugMessage( string message, params object[] args )
        {
            if( Config.Debug )
                Config.Pkg.LogInfoLine( message, args );
        }

        public static void DebugMessage( string message )
        {
            if( Config.Debug )
                Config.Pkg.LogInfoLine( message );
        }

        public class CraftAutoLoopContext : AutoLoopContext
        {
            private CraftSystem m_System;
            private Mobile m_From;
            private Type m_Type;
            private Type m_TypeRes;
            private BaseTool m_Tool;
            private CraftItem m_Item;

            public CraftAutoLoopContext( CraftSystem system, Mobile from, Type type, Type typeRes, BaseTool tool, CraftItem item )
                : base( from )
            {
                m_System = system;
                m_From = from;
                m_Type = type;
                m_Item = item;
                m_Tool = tool;
                m_TypeRes = typeRes;
            }

            public override bool CheckLoop()
            {
                if( m_Item == null )
                    return false;

                if( !base.CheckLoop() )
                    return false;

                bool allRequiredSkills = true;
                double chance = m_Item.GetSuccessChance( m_From, m_TypeRes, m_System, false, ref allRequiredSkills );

                if( allRequiredSkills && chance >= 0.0 )
                {
                    int badCraft = m_System.CanCraft( m_From, m_Tool, m_Type );

                    if( badCraft <= 0 )
                    {
                        //int resHue = 0;
                        //int maxAmount = 0;
                        // object message = null;

                        //if( m_Item.ConsumeRes( m_From, m_TypeRes, m_System, ref resHue, ref maxAmount, ConsumeType.None, ref message ) )
                        //{
                        //    message = null;

                        //if( m_Item.ConsumeAttributes( m_From, ref message, false ) )
                        return true;
                        //}
                    }
                }

                return false;
            }

            public override void DoAction()
            {
                m_System.AutoMacroCheck( m_From );
                m_System.CreateItem( m_From, m_Type, m_TypeRes, m_Tool, m_Item );
            }
        }
    }
}