using System;
using System.Collections;
using Server;
using Server.Commands;
using Server.Commands.Generic;
using Server.Gumps;
using Server.Items;
using Server.Network;
using Server.Targeting;
using Server.Targets;

namespace Midgard.Engines.GroupsHandler
{
    public enum GroupsGumpPage
    {
        Main,
        Details,
        Groups,
        GroupsChange,
        Actions
    }

    public class GroupsGump : Gump
    {
        private Mobile m_From;
        private GroupsGumpPage m_PageType;

        private string m_Message;
        private string m_MarkedMessage;

        private ArrayList m_List;
        private ArrayList m_Checked;
        private ArrayList m_OrigList;
        private ArrayList m_Groups;

        private int m_EntryCount = 12;
        private int m_ListPage;
        private int m_GroupsPage;
        private int m_Index;

        private bool m_Secure;

        private string m_Language = String.Empty;

        // change this if you want to override prevention of grabbing not movable items
        private static bool m_GrabNotMovable = false;

        private ItemsGroup m_FilterGroup;

        private const int LabelHue = 0x480;
        private const int GreenHue = 0x40;
        private const int RedHue = 0x20;
        private const int GrayHue = 0x76C;
        private const int YellowHue = 0x33;
        private const int SecureHue = 0x841;
        private const int ItemHue = 0x516;

        private const int StandardButtonIDUp = 0xFA5;
        private const int StandardButtonIDDown = 0xFA7;

        private bool CheckBaseLevel()
        {
            if( m_From.AccessLevel < GroupsHandler.BaseLevel )
            {
                m_From.SendMessage( 0x23,
                                   m_Language == "ITA" ? "Non hai i diritti per compiere questa operazione." : "You do not have access to do this." );
                ResendGump( m_PageType );
                return false;
            }
            return true;
        }

        public GroupsGump( Mobile from, ArrayList list, ArrayList rads )
            : this( from, GroupsGumpPage.Main, list, rads, null, null, null )
        {
        }

        public GroupsGump( Mobile from, GroupsGumpPage page, ArrayList list, ArrayList toDelete, ArrayList original, ArrayList groups, object state )
            : base( 50, 40 )
        {
            m_From = from;
            m_PageType = page;
            m_List = ( list ?? new ArrayList() );
            m_Checked = ( toDelete ?? new ArrayList() );
            m_OrigList = ( original ?? new ArrayList( list ) );
            m_Groups = ( groups ?? new ArrayList( GroupsHandler.GroupsTable.Values ) );

            if( m_From.Language != null )
                m_Language = m_From.Language.ToUpper();

            m_Groups.Sort( new ItemsGroupComparer() );

            if( state != null )
            {
                object[] states = (object[])state;
                m_ListPage = (int)states[ 0 ];
                m_Index = (int)states[ 1 ];
                m_GroupsPage = (int)states[ 2 ];
                m_FilterGroup = (ItemsGroup)states[ 3 ];
            }

            m_Secure = m_From.AccessLevel >= GroupsHandler.SecureLevel;

            m_Message = String.Format( m_Language == "ITA" ? "{0} oggett{1} trovat{1}." : "{0} item{1} found.",
                                      m_List.Count == 1 ? ( m_Language == "ITA" ? "Un" : "One" ) : m_List.Count.ToString(),
                                      m_List.Count == 1 ? ( m_Language == "ITA" ? "o" : "" ) : ( m_Language == "ITA" ? "i" : "s" ) );
            m_MarkedMessage = String.Format( m_Language == "ITA" ? "Selezionati: {0}/{1}" : "Selected: {0}/{1}", m_Checked.Count, m_OrigList.Count );

            InitializeGump();
        }

        private void AddPrevNextButtons( ICollection list, int page, int index )
        {
            if( page > 0 )
                AddButton( 475, 102, 0x15E3, 0x15E7, 31, GumpButtonType.Reply, 0 ); // Previous ( 31 )
            else
                AddImage( 475, 102, 0x25EA );

            if( index < list.Count )
                AddButton( 492, 102, 0x15E1, 0x15E5, 32, GumpButtonType.Reply, 0 ); // Next ( 32 )
            else
                AddImage( 492, 102, 0x25E6 );
        }

        private void AddBottomButtons()
        {
            AddBottomButtons( false, false );
        }

        private void AddBottomButtons( bool secure, bool onlyBack )
        {
            int height = m_EntryCount * 20;
            if( m_PageType == GroupsGumpPage.Details || m_PageType == GroupsGumpPage.Actions )
                height = 240;

            int startY = 130 + height;
            int offset = 22;
            int labelLength = 210;
            int x1 = 10;
            int x2 = 265;
            int xdiff = 35;

            switch( m_PageType )
            {
                case GroupsGumpPage.Main:
                    {
                        if( m_From.AccessLevel < GroupsHandler.BaseLevel )
                            break;

                        AddLabelCropped( x1 + xdiff, startY, labelLength, offset, LabelHue, m_Language == "ITA" ? "Seleziona tutti" : "Mark all" );
                        AddButton( x1, startY, StandardButtonIDUp, StandardButtonIDDown, 1, GumpButtonType.Reply, 0 ); // Mark all ( 1 )

                        AddLabelCropped( x2 + xdiff, startY, labelLength, offset, LabelHue,
                                        m_Language == "ITA" ? "Inserisci o rimuovi da un gruppo" : "Toggle grouped state" );
                        AddButton( x2, startY, StandardButtonIDUp, StandardButtonIDDown, 5, GumpButtonType.Reply, 0 ); // Toggle from list ( 5 )

                        AddLabelCropped( x1 + xdiff, startY + offset, labelLength, offset, LabelHue,
                                        m_Language == "ITA" ? "Deseleziona tutti" : "Unmark all" );
                        AddButton( x1, startY + offset, StandardButtonIDUp, StandardButtonIDDown, 2, GumpButtonType.Reply, 0 ); // Unmark all ( 2 )

                        AddLabelCropped( x2 + xdiff, startY + offset, labelLength, offset, LabelHue,
                                        m_Language == "ITA" ? "Imposta o cambia gruppo" : "Set or change group" );
                        AddButton( x2, startY + offset, StandardButtonIDUp, StandardButtonIDDown, 6, GumpButtonType.Reply, 0 ); // Change group ( 6 )

                        AddLabelCropped( x1 + xdiff, startY + 2 * offset, labelLength, offset, LabelHue,
                                        m_Language == "ITA" ? "Seleziona oggetti in nessun gruppo" : "Mark non grouped items" );
                        AddButton( x1, startY + 2 * offset, StandardButtonIDUp, StandardButtonIDDown, 3, GumpButtonType.Reply, 0 );
                        // Mark not grouped items ( 3 )

                        AddLabelCropped( x2 + xdiff, startY + 2 * offset, labelLength, offset, LabelHue,
                                        m_Language == "ITA" ? "Esegui azioni avanzate" : "Execute commands on selection" ); // Execute commands ( 7 )
                        AddButton( x2, startY + 2 * offset, StandardButtonIDUp, StandardButtonIDDown, 7, GumpButtonType.Reply, 0 );

                        AddLabelCropped( x1 + xdiff, startY + 3 * offset, labelLength, offset, LabelHue,
                                        m_Language == "ITA" ? "Aggiungi un oggetto all'elenco" : "Add an item to the current list" );
                        AddButton( x1, startY + 3 * offset, StandardButtonIDUp, StandardButtonIDDown, 4, GumpButtonType.Reply, 0 );
                        // Add an item to the current list ( 4 )

                        AddLabelCropped( x2 + xdiff, startY + 3 * offset, labelLength, offset, LabelHue,
                                        m_Language == "ITA" ? "Cancella selezionati" : "Delete selected" );
                        AddButton( x2, startY + 3 * offset, StandardButtonIDUp, StandardButtonIDDown, 8, GumpButtonType.Reply, 0 ); // Delete ( 8 )

                        break;
                    }
                case GroupsGumpPage.Details:
                    {
                        if( onlyBack )
                            goto default;

                        AddButton( x1, startY, StandardButtonIDUp, StandardButtonIDDown, 111, GumpButtonType.Reply, 0 ); // Props ( 111 )
                        AddLabelCropped( x1 + xdiff, startY, labelLength, offset, LabelHue, m_Language == "ITA" ? "Proprietà" : "Properties" );

                        AddButton( x2, startY, StandardButtonIDUp, StandardButtonIDDown, 114, GumpButtonType.Reply, 0 ); // GoTo ( 114 )
                        AddLabelCropped( x2 + xdiff, startY, labelLength, offset, LabelHue, m_Language == "ITA" ? "Vai all'oggetto" : "Go to this item" );

                        if( m_From.AccessLevel < GroupsHandler.BaseLevel )
                            goto default;

                        if( !secure )
                        {
                            AddButton( x2, startY + offset, StandardButtonIDUp, StandardButtonIDDown, 116, GumpButtonType.Reply, 0 );
                            // Move to location ( 116 )
                            AddLabelCropped( x2 + xdiff, startY + offset, labelLength, offset, LabelHue,
                                            m_Language == "ITA" ? "Sposta oggetto" : "Move this item" );

                            AddButton( x2, startY + 2 * offset, StandardButtonIDUp, StandardButtonIDDown, 112, GumpButtonType.Reply, 0 );
                            // Grab item ( 112 )
                            AddLabelCropped( x2 + xdiff, startY + 2 * offset, labelLength, offset, LabelHue,
                                            m_Language == "ITA" ? "Cattura oggetto" : "Grab this item" );
                        }

                        AddButton( x1, startY + offset, StandardButtonIDUp, StandardButtonIDDown, 113, GumpButtonType.Reply, 0 );
                        // Change group ( 113 )
                        AddLabelCropped( x1 + xdiff, startY + offset, labelLength, offset, LabelHue,
                                        m_Language == "ITA" ? "Imposta o cambia gruppo" : "Set or change group" );

                        AddButton( x1, startY + 2 * offset, StandardButtonIDUp, StandardButtonIDDown, 115, GumpButtonType.Reply, 0 ); // Delete ( 115 )
                        AddLabelCropped( x1 + xdiff, startY + 2 * offset, labelLength, offset, LabelHue,
                                        m_Language == "ITA" ? "Cancella oggetto" : "Delete this item" );

                        goto default;
                    }
                default:
                    {
                        AddLabelCropped( x1 + xdiff, startY + 3 * offset, labelLength, 22, LabelHue, m_Language == "ITA" ? "Indietro" : "Back" );
                        AddButton( x1, startY + 3 * offset, 0xFAE, 0xFB0, 101, GumpButtonType.Reply, 0 ); // Back ( 101 )
                        break;
                    }
            }
        }

        private void AddSearchButtons()
        {
            switch( m_PageType )
            {
                case GroupsGumpPage.Groups:
                case GroupsGumpPage.GroupsChange:
                    {
                        AddLabelCropped( 285, 13, 50, 22, LabelHue, "Name" );
                        AddImageTiled( 335, 13, 170, 25, 0x248A );
                        AddAlphaRegion( 336, 14, 168, 23 );
                        AddTextEntry( 340, 18, 160, 16, LabelHue, 0, "" ); // insert text here

                        AddLabelCropped( 285, 40, 50, 22, LabelHue, "Descr." );
                        AddImageTiled( 335, 40, 170, 25, 0x248A );
                        AddAlphaRegion( 336, 41, 168, 23 );
                        AddTextEntry( 340, 45, 160, 16, LabelHue, 1, "" ); // insert text here

                        AddButton( 285, 65, StandardButtonIDUp, StandardButtonIDDown, 21, GumpButtonType.Reply, 0 ); // Add ( 21 )
                        AddButton( 380, 65, StandardButtonIDUp, StandardButtonIDDown, 22, GumpButtonType.Reply, 0 ); // Search ( 22 )

                        AddLabelCropped( 320, 65, 75, 22, LabelHue, m_Language == "ITA" ? "Aggiungi" : "Add" );
                        AddLabelCropped( 415, 65, 75, 22, LabelHue, m_Language == "ITA" ? "Cerca" : "Search" );
                        break;
                    }
                default:
                    {
                        AddImageTiled( 285, 15, 220, 25, 0x248A );
                        AddAlphaRegion( 286, 16, 218, 23 );
                        AddTextEntry( 290, 20, 210, 16, LabelHue, 0, "" ); // insert text here

                        AddLabelCropped( 285, 40, 220, 22, LabelHue, m_Language == "ITA" ? "Cerca per:" : "Search for:" );

                        AddButton( 285, 65, StandardButtonIDUp, StandardButtonIDDown, 11, GumpButtonType.Reply, 0 ); // Search for Type ( 11 )
                        AddButton( 360, 65, StandardButtonIDUp, StandardButtonIDDown, 12, GumpButtonType.Reply, 0 ); // Search for Serial ( 12 )
                        AddButton( 435, 65, StandardButtonIDUp, StandardButtonIDDown, 13, GumpButtonType.Reply, 0 ); // Search for ItemID ( 13 )

                        AddLabelCropped( 315, 65, 40, 22, LabelHue, "Type" );
                        AddLabelCropped( 390, 65, 40, 22, LabelHue, "Serial" );
                        AddLabelCropped( 465, 65, 40, 22, LabelHue, "ItemID" );
                        break;
                    }
            }
        }

        private void AddTitleBox()
        {
            AddLabelCropped( 15, 10, 120, 19, GreenHue, m_Language == "ITA" ? "Oggetti in lista" : "Grouped items" );
            AddLabelCropped( 140, 10, 120, 19, RedHue, m_Language == "ITA" ? "Oggetti non in lista" : "Free items" );
            AddLabelCropped( 15, 30, 250, 19, LabelHue, m_Message );
            AddLabelCropped( 15, 50, 250, 19, LabelHue, m_MarkedMessage );

            switch( m_PageType )
            {
                case GroupsGumpPage.Main:
                    {
                        string filter = ( m_FilterGroup == null ? ( m_Language == "ITA" ? "(disattivo)" : "(disabled)" ) : m_FilterGroup.Name );

                        AddButton( 15, 68, StandardButtonIDUp, StandardButtonIDDown, 103, GumpButtonType.Reply, 0 ); // Show groups ( 103 )
                        AddLabelCropped( 50, 69, 98, 20, LabelHue, m_Language == "ITA" ? "Filtro gruppo:" : "Filter group:" );
                        AddLabelCropped( 150, 69, 115, 20, m_FilterGroup == null ? GrayHue : ( m_FilterGroup.Secure ? SecureHue : YellowHue ), filter );
                        break;
                    }
                case GroupsGumpPage.Details:
                    {
                        AddLabelCropped( 15, 69, 250, 20, GrayHue,
                                        m_Language == "ITA" ? "Dettagli sull'oggetto selezionato" : "Selected item's details" );
                        break;
                    }
                case GroupsGumpPage.Groups:
                    {
                        AddButton( 15, 68, StandardButtonIDUp, StandardButtonIDDown, 102, GumpButtonType.Reply, 0 ); // All groups ( 102 )
                        AddLabelCropped( 50, 69, 215, 20, LabelHue,
                                        m_Language == "ITA" ? "Mostra oggetti di tutti i gruppi" : "Show items from all groups" );
                        break;
                    }
                case GroupsGumpPage.GroupsChange:
                    {
                        AddLabelCropped( 15, 69, 250, 20, GrayHue,
                                        m_Language == "ITA" ? "Seleziona il nuovo gruppo dall'elenco" : "Select a group from the list" );
                        break;
                    }
                case GroupsGumpPage.Actions:
                    {
                        AddLabelCropped( 15, 69, 250, 20, GrayHue,
                                        m_Language == "ITA" ? "Operazioni avanzate sulla selezione" : "Advanced tasks on selection" );
                        break;
                    }
                default:
                    break;
            }
        }

        public virtual void InitializeGump()
        {
            int height = m_EntryCount * 20;
            if( m_PageType == GroupsGumpPage.Details || m_PageType == GroupsGumpPage.Actions )
                height = 240;

            AddPage( 0 );

            AddBackground( 0, 0, 520, 228 + height, 0x13BE );

            AddAlphaRegion( 10, 10, 260, 80 ); // top left
            AddAlphaRegion( 280, 10, 230, 80 ); // top right
            AddAlphaRegion( 10, 100, 500, 20 + height ); // middle
            AddAlphaRegion( 10, 130 + height, 500, 88 ); // bottom

            AddTitleBox();

            AddSearchButtons();

            switch( m_PageType )
            {
                case GroupsGumpPage.Main:
                    {
                        AddLabelCropped( 32, 100, 100, 20, LabelHue, "Type" );
                        AddLabelCropped( 134, 100, 70, 20, LabelHue, "Group" );
                        AddLabelCropped( 206, 100, 90, 20, LabelHue, "Serial" );
                        AddLabelCropped( 298, 100, 190, 20, LabelHue, "Location" );

                        if( m_List.Count == 0 )
                            AddLabel( 12, 120, LabelHue, m_Language == "ITA" ? "Non è stato trovato alcun oggetto." : "No items found." );

                        int index = ( m_ListPage * m_EntryCount );
                        for( int i = 0; i < m_EntryCount && index >= 0 && index < m_List.Count; i++, index++ )
                        {
                            Item item = m_List[ index ] as Item;
                            ItemsGroup group = GroupsHandler.GetGroup( item );

                            if( item == null )
                                continue;

                            int offset = 120 + ( i * 20 );
                            string type = item.GetType().Name;
                            string serial = item.Serial.ToString();
                            int serialHue = group != null ? GreenHue : RedHue;
                            string loc = item.GetWorldLocation().ToString();
                            string location = String.Format( "{0} [{1}]", loc, ( ( item.Map == null ) ? "N/A" : item.Map.ToString() ) );

                            if( item.Deleted )
                                location = "N/A (deleted)";

                            AddLabelCropped( 32, offset, 100, 20, ItemHue, type ); // Type
                            AddLabelCropped( 134, offset, 70, 20, group == null ? LabelHue : ( group.Secure ? SecureHue : YellowHue ),
                                            group == null ? "(no group)" : group.Name ); // Group
                            AddLabelCropped( 206, offset, 90, 20, serialHue, serial ); // Serial
                            AddLabelCropped( 298, offset, 190, 20, LabelHue, location ); // Location

                            if( item.Deleted || group != null && !group.IsAccessible( m_Secure ) )
                                continue;

                            if( m_From.AccessLevel >= GroupsHandler.BaseLevel )
                                AddCheck( 10, offset, 0xD2, 0xD3, m_Checked.Contains( item ), index );
                            AddButton( 480, offset - 1, 0xFAB, 0xFAD, 1000 + index, GumpButtonType.Reply, 0 ); // Show details ( 1000 + index )
                        }

                        AddPrevNextButtons( m_List, m_ListPage, index );
                        AddBottomButtons();
                        break;
                    }

                case GroupsGumpPage.Details:
                    {
                        bool onlyBack = false;
                        int offset = 100;
                        int suboffset = 21;
                        int dimX1 = 220;
                        int dimX2 = 250;
                        int dimY = 20;
                        int x1 = 20;
                        int x2 = 250;

                        int index = m_Index;
                        Item item = m_List[ index ] as Item;
                        ItemsGroup group = GroupsHandler.GetGroup( item );

                        if( item == null || item.Deleted )
                        {
                            AddLabel( 12, offset, LabelHue,
                                     m_Language == "ITA"
                                         ? "L'oggetto non esiste o è stato cancellato." : "The item does not exist or has been deleted." );
                            onlyBack = true;
                        }
                        else if( group != null && !group.IsAccessible( m_Secure ) )
                        {
                            AddLabel( 12, offset, LabelHue,
                                     m_Language == "ITA" ? "L'oggetto non è accessibile da qui." : "The item is not accessible from here." );
                            onlyBack = true;
                        }
                        else
                        {
                            string type = item.GetType().Name;
                            string name = item.Name;
                            string itemID = String.Format( "{0} (0x{1})", item.ItemID, item.ItemID.ToString( "X4" ) );
                            string serial = item.Serial.ToString();
                            bool root = item.RootParent != null;
                            string rootParent = !root
                                                    ? null
                                                    : ( item.RootParent is Mobile
                                                           ? "Mobile " + ( item.RootParent as Mobile ).Serial + " (" + ( item.RootParent as Mobile ).Name +
                                                             ")"
                                                           : ( item.RootParent is Item ? "Item " + ( item.RootParent as Item ).Serial : "-unknown-" ) );
                            string location = item.GetWorldLocation().ToString();
                            string map = " [" + ( ( item.Map == null ) ? "N/A" : item.Map.ToString() ) + "]";
                            string hue = String.Format( "{0} (0x{1})", item.Hue, item.Hue.ToString( "X4" ) );
                            string lootType = item.LootType.ToString();

                            AddLabel( 230, offset, LabelHue, m_Language == "ITA" ? "Informazioni" : "Informations" );
                            offset += suboffset;
                            AddLabelCropped( x1, offset, dimX1, dimY, LabelHue, "Type:" );
                            AddLabelCropped( x2, offset, dimX2, dimY, ItemHue, type );
                            offset += suboffset;
                            AddLabelCropped( x1, offset, dimX1, dimY, LabelHue, "Name:" );
                            AddLabelCropped( x2, offset, dimX2, dimY, LabelHue, name );
                            offset += suboffset;
                            AddLabelCropped( x1, offset, dimX1, dimY, LabelHue, "ItemID:" );
                            AddLabelCropped( x2, offset, dimX2, dimY, LabelHue, itemID );
                            offset += suboffset;
                            AddLabelCropped( x1, offset, dimX1, dimY, LabelHue, "Serial:" );
                            AddLabelCropped( x2, offset, dimX2, dimY, GroupsHandler.InGroup( item ) ? GreenHue : RedHue, serial );
                            offset += suboffset;
                            AddLabelCropped( x1, offset, dimX1, dimY, LabelHue, "Location:" );
                            AddLabelCropped( x2, offset, dimX2, dimY, LabelHue, location + map );
                            offset += suboffset;
                            AddLabelCropped( x1, offset, dimX1, dimY, LabelHue, "RootParent:" );
                            AddLabelCropped( x2, offset, dimX2, dimY, LabelHue, rootParent );
                            offset += suboffset;
                            AddLabelCropped( x1, offset, dimX1, dimY, LabelHue, "Hue:" );
                            AddLabelCropped( x2, offset, dimX2, dimY, item.Hue, hue );
                            offset += suboffset;
                            AddLabelCropped( x1, offset, dimX1, dimY, LabelHue, "LootType:" );
                            AddLabelCropped( x2, offset, dimX2, dimY,
                                            lootType == "Blessed"
                                                ? GreenHue : ( lootType == "Cursed" ? RedHue : ( lootType == "Newbied" ? 0x30 : LabelHue ) ), lootType );
                            offset += suboffset;
                            AddLabelCropped( x1, offset, dimX1, dimY, LabelHue, "Group:" );
                            AddLabelCropped( x2, offset, dimX2, dimY, group == null ? LabelHue : ( group.Secure ? SecureHue : YellowHue ),
                                            group == null ? "(no group)" : group.Name );
                            offset += suboffset;
                            AddLabelCropped( x1, offset, dimX1, dimY, LabelHue, "Visible:" );
                            AddLabelCropped( x2, offset, dimX2, dimY, LabelHue, item.Visible.ToString() );
                            offset += suboffset;
                            AddLabelCropped( x1, offset, dimX1, dimY, LabelHue, "Movable:" );
                            AddLabelCropped( x2, offset, dimX2, dimY, LabelHue, item.Movable.ToString() );

                            try
                            {
                                Rectangle2D bounds = ItemBounds.Table[ item.ItemID ];
                                if( item.ItemID != 1 && bounds.Height <= 140 && bounds.Width <= 180 )
                                {
                                    AddItem( 100, 130, item.ItemID );
                                }
                            }
                            catch
                            {
                            }
                        }

                        AddPrevNextButtons( m_List, index, index + 1 );
                        AddBottomButtons( group != null && group.Secure, onlyBack );
                        break;
                    }

                case GroupsGumpPage.Groups:
                case GroupsGumpPage.GroupsChange:
                    {
                        AddLabelCropped( 32, 100, 130, 20, LabelHue, "Name" );
                        AddLabelCropped( 154, 100, 268, 20, LabelHue, "Description" );
                        AddLabelCropped( 424, 100, 54, 20, LabelHue, "Items" );

                        if( m_Groups.Count == 0 )
                            AddLabel( 12, 120, LabelHue, m_Language == "ITA" ? "Non è stato trovato alcun gruppo" : "No groups found" );

                        int index = ( m_GroupsPage * m_EntryCount );
                        for( int i = 0; i < m_EntryCount && index >= 0 && index < m_Groups.Count; i++, index++ )
                        {
                            ItemsGroup group = m_Groups[ index ] as ItemsGroup;

                            if( group == null || !group.IsAccessible( m_Secure ) )
                            {
                                i--;
                                continue;
                            }

                            int offset = 120 + ( i * 20 );
                            string name = group.Name;
                            string description = group.Description;
                            string count = group.Count.ToString();

                            if( m_Secure && m_PageType == GroupsGumpPage.Groups )
                                AddButton( 12, offset + 2, group.Secure ? 0x2C92 : 0x2C88, group.Secure ? 0x2C94 : 0x2C8A, -1000 - index,
                                          GumpButtonType.Reply, 0 ); // Change secure state ( -1000 - index )

                            AddLabelCropped( 32, offset, 120, 20, group.Secure ? SecureHue : YellowHue, name ); // Name
                            AddLabelCropped( 154, offset, 268, 20, LabelHue, description ); // Description
                            AddLabelCropped( 424, offset, 54, 20, LabelHue, count ); // Items count

                            AddButton( 480, offset - 1, StandardButtonIDUp, StandardButtonIDDown, 1000 + index, GumpButtonType.Reply, 0 );
                            // Select or apply group ( 1000 + index )
                        }

                        AddPrevNextButtons( m_Groups, m_GroupsPage, index );
                        AddBottomButtons();
                        break;
                    }

                case GroupsGumpPage.Actions:
                    {
                        AddHtml( 20, 100, 480, 40,
                                m_Language == "ITA"
                                    ? "<basefont color=yellow>Assicurati che gli oggetti che hai selezionato siano quelli desiderati, quindi scegli una delle opzioni qui sotto elencate.</basefont>"
                                    : "<basefont color=yellow>Please ensure you have selected the desired items, then choose one of the following options.</basefont>",
                                false, false );

                        AddHtml( 20, 160, 480, 55,
                                m_Language == "ITA"
                                    ? "<basefont color=white>Scrivi nella casella sottostante un comando (eventualmente con condizione) da eseguire sugli oggetti attualmente selezionati.<br><i>Esempio: set movable false</i></basefont>"
                                    : "<basefont color=white>Type in this textbox a command (with a condition, if you want) you wish to execute on all currently selected items.<br><i>Example: set movable false</i></basefont>",
                                false, false );
                        AddImageTiled( 20, 215, 480, 62, 0x248A );
                        AddAlphaRegion( 21, 216, 478, 60 );
                        AddTextEntry( 25, 216, 470, 60, LabelHue, 2, "" );
                        AddButton( 20, 280, StandardButtonIDUp, StandardButtonIDDown, 41, GumpButtonType.Reply, 0 ); // Execute advanced command ( 41 )
                        AddLabel( 55, 281, LabelHue, m_Language == "ITA" ? "Esegui il comando sopra indicato" : "Run the command typed above" );

                        AddButton( 20, 320, StandardButtonIDUp, StandardButtonIDDown, 42, GumpButtonType.Reply, 0 ); // Mass move ( 42 )
                        AddLabel( 55, 321, LabelHue, m_Language == "ITA" ? "Spostamento di massa degli oggetti" : "Mass move of selected items" );
                        AddLabelCropped( 285, 321, 50, 22, LabelHue, "Range:" );
                        AddImageTiled( 335, 316, 30, 25, 0x248A );
                        AddAlphaRegion( 336, 317, 28, 23 );
                        AddTextEntry( 340, 321, 20, 16, LabelHue, 3, "10" ); // max range

                        AddBottomButtons();
                        break;
                    }
                default:
                    {
                        AddBottomButtons();
                        break;
                    }
            }
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            // set selected items
            if( m_List != null && m_Checked != null && m_PageType == GroupsGumpPage.Main )
            {
                for( int i = 0, index = m_ListPage * m_EntryCount; i < m_EntryCount && index < m_List.Count; i++, index++ )
                {
                    Item item = m_List[ index ] as Item;

                    if( info.IsSwitched( index ) )
                    {
                        if( item == null || item.Deleted )
                        {
                            m_Checked.Remove( item );
                        }
                        else if( !m_Checked.Contains( item ) )
                        {
                            m_Checked.Add( item );
                        }
                    }
                    else
                    {
                        m_Checked.Remove( item );
                    }
                }
            }

            switch( info.ButtonID )
            {
                case 1: // Mark all
                    {
                        ArrayList list = m_List;
                        ArrayList rads = new ArrayList( list );

                        m_From.SendGump( new GroupsGump( m_From, GroupsGumpPage.Main, list, rads, m_OrigList, m_Groups,
                                                       new object[] { m_ListPage, m_Index, m_GroupsPage, m_FilterGroup } ) );
                        break;
                    }
                case 2: // Unmark all
                    {
                        ArrayList list = m_List;
                        ArrayList rads = new ArrayList();

                        m_From.SendGump( new GroupsGump( m_From, GroupsGumpPage.Main, list, rads, m_OrigList, m_Groups,
                                                       new object[] { m_ListPage, m_Index, m_GroupsPage, m_FilterGroup } ) );
                        break;
                    }
                case 3: // Mark not grouped items
                    {
                        ArrayList list = m_List;
                        ArrayList rads = new ArrayList();

                        if( list != null )
                        {
                            foreach( Item item in list )
                            {
                                if( GroupsHandler.GetGroup( item ) == null )
                                {
                                    rads.Add( item );
                                }
                            }

                            m_From.SendGump( new GroupsGump( m_From, GroupsGumpPage.Main, list, rads, m_OrigList, m_Groups,
                                                           new object[] { m_ListPage, m_Index, m_GroupsPage, m_FilterGroup } ) );
                        }
                        break;
                    }
                case 4: // Add an item to the list
                    {
                        m_From.SendMessage( m_Language == "ITA"
                                               ? "Seleziona un oggetto da inserire nell'elenco corrente."
                                               : "Select an item you wish to add to current list." );
                        m_From.BeginTarget( -1, false, TargetFlags.None, new TargetStateCallback( AddItem_Callback ),
                                           new object[] { m_List, m_Checked, m_OrigList, m_ListPage, m_FilterGroup } );
                        break;
                    }
                case 5: // Toggle from list
                    {
                        if( !CheckBaseLevel() )
                            break;

                        ArrayList list = m_List;
                        ArrayList rads = m_Checked;

                        string message;
                        if( m_Language == "ITA" )
                            message =
                                String.Format(
                                    "Stai per modificare l'appartenenza a un gruppo di {0} oggett{1}.<br><br>{2}oggett{1} che ora f{4} parte di un gruppo, sar{3} rimoss{1} da esso e non potr{3} più essere elencat{1} da questo strumento, ma dovr{3} essere cercat{1} manualmente.<br><br>{2}oggett{1} che non f{4} parte di un gruppo saranno aggiunt{1} al gruppo {5} e sar{3} elencati nella finestra dell'HandleGroups.<br><br>Desideri continuare?",
                                    rads.Count, rads.Count == 1 ? "o" : "i", rads.Count == 1 ? "L'" : "Gli ", rads.Count == 1 ? "à" : "anno",
                                    rads.Count == 1 ? "a" : "anno", m_FilterGroup == null ? GroupsHandler.DefaultGroup.Name : m_FilterGroup.Name );
                        else
                            message =
                                String.Format(
                                    "You are about to change the grouped state of {0} item{1}.<br><br>Currently grouped item{1} will be removed from {2} group. You will no more be able to quickly find {3} later by using this tool, but instead you'll need to find {3} manually.<br><br>Currently ungrouped item{1} will be added to group {4}. You will be able to quickly find {3} later by using HandleGroups.<br><br>Would you like to continue?",
                                    rads.Count, rads.Count == 1 ? "" : "s", rads.Count == 1 ? "its" : "their", rads.Count == 1 ? "it" : "them",
                                    m_FilterGroup == null ? GroupsHandler.DefaultGroup.Name : m_FilterGroup.Name );

                        if( rads.Count > 0 )
                            m_From.SendGump( new WarningGump( 1060635, 30720, message, 0xFFC000, 420, 280, new WarningGumpCallback( Toggle_Callback ),
                                                            new object[] { list, rads, m_OrigList, m_ListPage, m_FilterGroup } ) );
                        else
                            m_From.SendGump( new NoticeGump( 1060637, 30720,
                                                           m_Language == "ITA"
                                                               ? "Non hai selezionato alcun oggetto. Seleziona le caselle di spunta accanto agli oggetti che desideri aggiungere nel gruppo selezionato o rimuovere dal proprio gruppo, quindi prova di nuovo."
                                                               : "You didn't select any item. Please check the boxes left to the items you wish to add/remove from their group, then try again.",
                                                           0xFFC000, 420, 280, new NoticeGumpCallback( ResendGump_Callback ),
                                                           new object[] { list, rads, m_OrigList, GroupsGumpPage.Main, m_ListPage, 0, m_FilterGroup } ) );

                        break;
                    }
                case 6: // Change group (multiple --> m_Index = -1)
                case 113: // change group (single)
                    {
                        if( !CheckBaseLevel() )
                            break;

                        ArrayList list = m_List;
                        ArrayList rads = m_Checked;

                        if( info.ButtonID == 6 )
                        {
                            if( rads != null )
                                if( rads.Count == 0 )
                                {
                                    m_From.SendGump( new NoticeGump( 1060637, 30720,
                                                                   m_Language == "ITA"
                                                                       ? "Non hai selezionato alcun oggetto. Seleziona le caselle di spunta accanto agli oggetti a cui desideri impostare o cambiare gruppo, quindi prova di nuovo."
                                                                       : "You didn't select any item. Please check the boxes left to the items you wish to insert in a group or move into another group, then try again.",
                                                                   0xFFC000, 420, 280, new NoticeGumpCallback( ResendGump_Callback ),
                                                                   new object[] { list, rads, m_OrigList, GroupsGumpPage.Main, m_ListPage, 0, m_FilterGroup } ) );
                                    break;
                                }
                        }

                        m_From.SendGump( new GroupsGump( m_From, GroupsGumpPage.GroupsChange, list, rads, m_OrigList, m_Groups,
                                                       new object[] { m_ListPage, m_PageType == GroupsGumpPage.Main ? -1 : m_Index, 0, m_FilterGroup } ) );
                        break;
                    }
                case 7: // Execute commands
                    {
                        if( !CheckBaseLevel() )
                            break;

                        if( m_Checked != null )
                            if( m_Checked.Count > 0 )
                                ResendGump( GroupsGumpPage.Actions );
                            else
                                m_From.SendGump( new NoticeGump( 1060637, 30720,
                                                               m_Language == "ITA"
                                                                   ? "Non hai selezionato alcun oggetto. Seleziona le caselle di spunta accanto agli oggetti ai quali desideri eseguire un comando, quindi prova di nuovo."
                                                                   : "You didn't select any item. Please check the boxes left to the items on which you wish to run a command, then try again.",
                                                               0xFFC000, 420, 280, new NoticeGumpCallback( ResendGump_Callback ),
                                                               new object[] { m_List, m_Checked, m_OrigList, GroupsGumpPage.Main, m_ListPage, 0, m_FilterGroup } ) );

                        break;
                    }
                case 8: // Delete marked
                    {
                        if( !CheckBaseLevel() )
                            break;

                        ArrayList list = m_List;
                        ArrayList rads = m_Checked;

                        string message;
                        if( m_Language == "ITA" )
                            message =
                                String.Format(
                                    "Stai per <em><basefont color=red>cancellare permanentemente</basefont></em> {0} oggett{1}. {2}oggett{1} cancellat{1} non sar{3} più recuperabil{1} in alcun modo!<br><br>Desideri continuare?",
                                    rads.Count, rads.Count == 1 ? "o" : "i", rads.Count == 1 ? "L'" : "Gli ", rads.Count == 1 ? "à" : "anno" );
                        else
                            message =
                                String.Format(
                                    "You are about to <em><basefont color=red>permanently delete</basefont></em> {0} item{1}. Deleted item{1} won't be recoverable by any means!<br><br>Would you like to continue?",
                                    rads.Count, rads.Count == 1 ? "" : "s" );

                        if( rads.Count > 0 )
                            m_From.SendGump( new WarningGump( 1060635, 30720, message, 0xFFC000, 420, 280, new WarningGumpCallback( Delete_Callback ),
                                                            new object[] { list, rads, m_OrigList, m_ListPage, m_FilterGroup } ) );
                        else
                            m_From.SendGump( new NoticeGump( 1060637, 30720,
                                                           m_Language == "ITA"
                                                               ? "Non hai selezionato alcun oggetto. Seleziona le caselle di spunta accanto agli oggetti che desideri cancellare, quindi prova di nuovo."
                                                               : "You didn't select any item. Please check the boxes left to the items you wish to delete, then try again.",
                                                           0xFFC000, 420, 280, new NoticeGumpCallback( ResendGump_Callback ),
                                                           new object[] { list, rads, m_OrigList, GroupsGumpPage.Main, m_ListPage, 0, m_FilterGroup } ) );

                        break;
                    }
                case 11: // Search for type/name
                case 12: // Search for serial/description
                case 13: // Search for itemid
                    {
                        ArrayList results = new ArrayList();
                        ArrayList list = m_OrigList;

                        TextRelay matchEntry = info.GetTextEntry( 0 );
                        string match = ( matchEntry == null ? null : matchEntry.Text.Trim().ToLower() );

                        if( string.IsNullOrEmpty( match ) )
                        {
                            m_From.SendGump( new GroupsGump( m_From, GroupsGumpPage.Main, list, m_Checked, m_OrigList, m_Groups,
                                                           new object[] { 0, 0, 0, m_FilterGroup } ) );
                            break;
                        }

                        for( int i = 0; i < list.Count; i++ )
                        {
                            Item item = list[ i ] as Item;
                            if( item == null )
                                continue;

                            bool isMatch = false;

                            switch( info.ButtonID )
                            {
                                case 11: // type
                                    {
                                        string type = item.GetType().Name;
                                        isMatch = type.ToLower().IndexOf( match ) >= 0;
                                        break;
                                    }
                                case 12: // serial (string search)
                                    {
                                        string serial = item.Serial.ToString();
                                        isMatch = serial.ToLower().IndexOf( match ) >= 0;
                                        break;
                                    }
                                case 13: // itemid
                                    {
                                        int itemid = 0;
                                        try
                                        {
                                            if( match.StartsWith( "0x" ) )
                                                itemid = Convert.ToInt32( match, 16 );
                                            else
                                                itemid = Convert.ToInt32( match );
                                        }
                                        catch
                                        {
                                        }
                                        isMatch = itemid == item.ItemID;
                                        break;
                                    }
                            }

                            if( isMatch )
                                results.Add( item );
                        }

                        m_From.SendGump( new GroupsGump( m_From, GroupsGumpPage.Main, results, m_Checked, list, m_Groups,
                                                       new object[] { 0, 0, 0, m_FilterGroup } ) );
                        break;
                    }
                case 21: // Add new group
                    {
                        if( !CheckBaseLevel() )
                            break;

                        string name = info.GetTextEntry( 0 ).Text;
                        string description = info.GetTextEntry( 1 ).Text;

                        if( !string.IsNullOrEmpty( name ) )
                            GroupsHandler.AddGroup( name, description, false );

                        m_From.SendGump( new GroupsGump( m_From, m_PageType, m_List, m_Checked, m_OrigList, null,
                                                       new object[] { m_ListPage, m_Index, 0, m_FilterGroup } ) );
                        break;
                    }
                case 22: // Search a group
                    {
                        bool forName = true;

                        ArrayList results = new ArrayList();
                        ArrayList list = new ArrayList( GroupsHandler.GroupsTable.Values );

                        TextRelay matchEntry = info.GetTextEntry( 0 );
                        string match = ( matchEntry == null ? null : matchEntry.Text.Trim().ToLower() );

                        if( string.IsNullOrEmpty( match ) )
                        {
                            forName = false;

                            matchEntry = info.GetTextEntry( 1 );
                            match = ( matchEntry == null ? null : matchEntry.Text.Trim().ToLower() );

                            if( string.IsNullOrEmpty( match ) )
                            {
                                m_From.SendGump( new GroupsGump( m_From, m_PageType, m_List, m_Checked, m_OrigList, null,
                                                               new object[] { m_ListPage, m_Index, 0, m_FilterGroup } ) );
                                break;
                            }
                        }

                        for( int i = 0; i < list.Count; i++ )
                        {
                            ItemsGroup group = list[ i ] as ItemsGroup;
                            bool isMatch;

                            if( forName )
                            {
                                isMatch = ( group != null && group.Name.ToLower().IndexOf( match ) >= 0 );
                            }
                            else
                            {
                                isMatch = ( group != null && group.Description.ToLower().IndexOf( match ) >= 0 );
                            }

                            if( isMatch )
                                results.Add( group );
                        }

                        m_From.SendGump( new GroupsGump( m_From, m_PageType, m_List, m_Checked, m_OrigList, results,
                                                       new object[] { m_ListPage, m_Index, 0, m_FilterGroup } ) );
                        break;
                    }
                case 31: // Previous
                    {
                        switch( m_PageType )
                        {
                            case GroupsGumpPage.Main:
                                m_ListPage--;
                                break;

                            case GroupsGumpPage.Groups:
                            case GroupsGumpPage.GroupsChange:
                                m_GroupsPage--;
                                break;

                            default:
                                m_Index--;
                                break;
                        }

                        m_From.SendGump( new GroupsGump( m_From, m_PageType, m_List, m_Checked, m_OrigList, m_Groups,
                                                       new object[] { m_ListPage, m_Index, m_GroupsPage, m_FilterGroup } ) );
                        break;
                    }
                case 32: // Next
                    {
                        switch( m_PageType )
                        {
                            case GroupsGumpPage.Main:
                                m_ListPage++;
                                break;

                            case GroupsGumpPage.Groups:
                            case GroupsGumpPage.GroupsChange:
                                m_GroupsPage++;
                                break;

                            default:
                                m_Index++;
                                break;
                        }

                        m_From.SendGump( new GroupsGump( m_From, m_PageType, m_List, m_Checked, m_OrigList, m_Groups,
                                                       new object[] { m_ListPage, m_Index, m_GroupsPage, m_FilterGroup } ) );
                        break;
                    }
                case 41: // Execute advanced command
                    {
                        if( !CheckBaseLevel() )
                            break;

                        ResendGump( m_PageType );
                        try
                        {
                            string commandString, argString;
                            string[] args;
                            bool contained = GetCommandDetails( info.GetTextEntry( 2 ).Text, out commandString, out argString, out args );

                            BaseCommand command = GroupsHandler.CommandImplementor.Commands[ commandString ];

                            if( command == null || command.ObjectTypes == ObjectTypes.Mobiles )
                                throw new Exception(
                                    String.Format(
                                        m_Language == "ITA"
                                            ? "Questo comando non è valido oppure non supporta una lista di oggetti: {0}"
                                            : "That is either an invalid command name or one that does not support a list of items: {0}",
                                        commandString ) );

                            if( m_From.AccessLevel < command.AccessLevel )
                                throw new Exception(
                                    String.Format(
                                        m_Language == "ITA"
                                            ? "Non hai accesso a questo comando: {0}." : "You do not have access to that command: {0}.", commandString ) );

                            Extensions ext = Extensions.Parse( m_From, ref args );

                            bool items, mobiles;
                            if( !GroupsHandler.CommandImplementor.CheckObjectTypes( command, ext, out items, out mobiles ) || !items )
                                throw new Exception( m_Language == "ITA"
                                                        ? "Non è stato trovato alcun oggetto su cui eseguire questo comando."
                                                        : "Nothing was found to use this command on." );

                            ArrayList list = new ArrayList();
                            if( contained )
                            {
                                if( m_Checked != null )
                                    foreach( Item item in m_Checked )
                                    {
                                        if( item is Container && !item.Deleted )
                                        {
                                            Container cont = (Container)item;

                                            Item[] found = cont.FindItemsByType( typeof( Item ), true );

                                            for( int i = 0; i < found.Length; ++i )
                                            {
                                                if( ext.IsValid( found[ i ] ) && GroupsHandler.IsAccessible( m_From, found[ i ] ) )
                                                    list.Add( found[ i ] );
                                            }
                                        }
                                    }

                                GroupsHandler.CommandImplementor.RunCommand( m_From, list, command, args );
                            }
                            else
                            {
                                if( m_Checked != null )
                                    foreach( Item item in m_Checked )
                                    {
                                        if( item != null && !item.Deleted && ext.IsValid( item ) && GroupsHandler.IsAccessible( m_From, item ) )
                                            list.Add( item );
                                    }

                                GroupsHandler.CommandImplementor.RunCommand( m_From, list, command, args );
                            }
                        }
                        catch( Exception ex )
                        {
                            m_From.SendMessage( ex.Message );
                        }

                        break;
                    }
                case 42: // Mass move
                    {
                        if( !CheckBaseLevel() )
                            break;

                        ArrayList list = new ArrayList( m_Checked );
                        int range = 10;
                        try
                        {
                            range = Convert.ToInt32( info.GetTextEntry( 3 ).Text );
                            if( range > 100 )
                                range = 100;
                            if( range < 0 )
                                range = 0;
                        }
                        catch
                        {
                        }

                        if( list.Count == 0 )
                        {
                            m_From.SendMessage( m_Language == "ITA" ? "Non è stato trovato alcun oggetto." : "No items found." );
                        }
                        else
                        {
                            if( list.Count > 1000 )
                                m_From.SendMessage( 0x23,
                                                   m_Language == "ITA"
                                                       ? "Attenzione, hai selezionato più di 1000 oggetti. Assicurati che la selezione sia corretta prima di fare clic sul punto di destinazione."
                                                       : "Warning: you selected more than 1000 items. Please ensure your selection is correct before targeting on destination spot." );
                            m_From.SendMessage( m_Language == "ITA"
                                                   ? "Seleziona il punto centrale di destinazione" : "Select the central destination point" );
                            m_From.Target = new MassMoveTarget( list, range );
                        }
                        ResendGump( m_PageType );
                        break;
                    }
                case 101: // Back
                    {
                        ResendGump( GroupsGumpPage.Main );
                        break;
                    }
                case 102: // All groups
                    {
                        ArrayList list = GroupsHandler.ListItems( m_From, null );

                        m_From.SendGump( new GroupsGump( m_From, list, null ) );
                        break;
                    }
                case 103: // Show groups
                    {
                        ResendGump( GroupsGumpPage.Groups );
                        break;
                    }
                case 111: // Props
                    {
                        ArrayList list = m_List;
                        if( list != null )
                        {
                            Item item = list[ m_Index ] as Item;

                            ResendGump( m_PageType );

                            if( item != null && !item.Deleted )
                            {
                                m_From.SendGump( new PropertiesGump( m_From, item ) );
                            }
                            else
                            {
                                m_From.SendMessage( 0x23,
                                                   m_Language == "ITA"
                                                       ? "L'oggetto non esiste o è stato cancellato." : "The item does not exist or has been deleted." );
                            }
                        }
                        break;
                    }
                case 112: // Grab item
                    {
                        if( !CheckBaseLevel() )
                            break;

                        ArrayList list = m_List;
                        if( list != null )
                        {
                            Item item = list[ m_Index ] as Item;
                            ItemsGroup group = GroupsHandler.GetGroup( item );
                            if( item != null && !item.Deleted &&
                                ( group == null || group.IsAccessible( m_From.AccessLevel >= GroupsHandler.SecureLevel ) ) )
                            {
                                if( m_GrabNotMovable || item.Movable )
                                {
                                    if( ( item.Parent as Container ) != m_From.Backpack )
                                    {
                                        item.Movable = true;
                                        m_From.AddToBackpack( item );
                                        m_From.SendMessage( m_Language == "ITA"
                                                               ? "L'oggetto è stato spostato nel tuo backpack."
                                                               : "The item has been moved inside your backpack." );
                                    }
                                    else
                                        m_From.SendMessage( m_Language == "ITA"
                                                               ? "L'oggetto è già all'interno del tuo backpack."
                                                               : "The item is already in your backpack." );
                                }
                                else
                                {
                                    m_From.SendMessage( 0x23,
                                                       m_Language == "ITA"
                                                           ? "L'oggetto non è movable. Imposta la proprietà Movable a true prima di spostare l'oggetto."
                                                           : "The item is not movable. Set its Movable property to true before moving it." );
                                }
                            }
                            else
                            {
                                m_From.SendMessage( 0x23,
                                                   m_Language == "ITA"
                                                       ? "L'oggetto non esiste o è stato cancellato." : "The item does not exist or has been deleted." );
                            }
                        }

                        ResendGump( m_PageType );
                        break;
                    }
                case 114: // GoTo
                    {
                        ArrayList list = m_List;
                        if( list != null )
                        {
                            Item item = list[ m_Index ] as Item;

                            if( item != null && !item.Deleted && item.Map != null && item.Map != Map.Internal )
                            {
                                m_From.MoveToWorld( item.GetWorldLocation(), item.Map );
                                m_From.SendMessage( m_Language == "ITA"
                                                       ? "Sei stato teletrasportato presso l'oggetto." : "You have been teleported to the item." );
                            }
                            else
                            {
                                m_From.SendMessage( 0x23, m_Language == "ITA" ? "L'oggetto è irraggiungibile." : "The item is unreachable." );
                            }
                        }

                        ResendGump( m_PageType );
                        break;
                    }
                case 115: // Delete
                    {
                        if( !CheckBaseLevel() )
                            break;

                        ArrayList rads = new ArrayList();
                        if( m_List != null )
                        {
                            Item item = m_List[ m_Index ] as Item;
                            rads.Add( item );
                        }

                        if( GroupsHandler.DeleteItems( m_From, rads ) == 0 )
                        {
                            m_From.SendGump( new NoticeGump( 1060637, 30720,
                                                           m_Language == "ITA"
                                                               ? "Non è stato possibile cancellare permanentemente l'oggetto selezionato.<br><br>Probabilmente l'oggetto selezionato è stato precedentemente cancellato dal server, oppure non disponi dei privilegi richiesti per compiere questa operazione."
                                                               : "It was not possible to permanently delete the selected item.<br><br>Maybe the selected item has already been deleted, or you lack required privileges to perform this task.",
                                                           0xFFC000, 420, 280, new NoticeGumpCallback( ResendGump_Callback ),
                                                           new object[] { m_List, m_Checked, m_OrigList, m_PageType, m_ListPage, m_Index, m_FilterGroup } ) );
                            break;
                        }

                        ResendGump( m_PageType );
                        break;
                    }
                case 116: // Move to target
                    {
                        if( !CheckBaseLevel() )
                            break;

                        if( m_List != null )
                        {
                            Item item = m_List[ m_Index ] as Item;
                            if( item != null && !item.Deleted )
                                m_From.Target = new MoveTarget( item );
                        }

                        ResendGump( m_PageType );
                        break;
                    }
                default: // Show details - Select group - Apply group
                    {
                        if( info.ButtonID == 0 )
                            break;

                        int index;
                        if( info.ButtonID < 0 ) // Change secure setting
                        {
                            index = -1000 - info.ButtonID;

                            ItemsGroup group = m_Groups[ index ] as ItemsGroup;
                            if( group == null )
                            {
                                ResendGump( m_PageType );
                                break;
                            }

                            m_From.SendGump( new WarningGump( 1060635, 30720,
                                                            String.Format(
                                                                m_Language == "ITA"
                                                                    ? "Stai per impostare il gruppo <em><basefont color=red>{0}</basefont></em> come {1}sicuro.<br><br>Desideri continuare?"
                                                                    : "You are about to set the group <em><basefont color=red>{0}</basefont></em> as {1}secure.<br><br>Would you like to continue?",
                                                                group.Name, group.Secure ? ( m_Language == "ITA" ? "NON " : "NOT " ) : "" ), 0xFFC000,
                                                            420, 280, new WarningGumpCallback( ToggleSecurity_Callback ),
                                                            new object[] { m_List, m_Checked, m_ListPage, m_OrigList, group, m_FilterGroup } ) );
                            break;
                        }

                        index = info.ButtonID - 1000;

                        switch( m_PageType )
                        {
                            case GroupsGumpPage.Main: // Show details
                                {
                                    m_From.SendGump( new GroupsGump( m_From, GroupsGumpPage.Details, m_List, m_Checked, m_OrigList, m_Groups,
                                                                   new object[] { m_ListPage, index, m_GroupsPage, m_FilterGroup } ) );
                                    break;
                                }
                            case GroupsGumpPage.Groups: // Select group
                                {
                                    ItemsGroup group = m_Groups[ index ] as ItemsGroup;
                                    if( group == null )
                                        group = GroupsHandler.DefaultGroup;

                                    m_FilterGroup = group;
                                    ArrayList list = group.Items;

                                    m_From.SendGump( new GroupsGump( m_From, GroupsGumpPage.Main, list, null, null, null,
                                                                   new object[] { 0, 0, 0, m_FilterGroup } ) );
                                    break;
                                }
                            case GroupsGumpPage.GroupsChange: // Apply group
                                {
                                    ArrayList list = m_List;
                                    ArrayList rads = m_Checked;

                                    ItemsGroup newGroup = m_Groups[ index ] as ItemsGroup;
                                    if( newGroup == null )
                                        newGroup = GroupsHandler.DefaultGroup;

                                    if( m_Index == -1 ) // from main gump page
                                    {
                                        string message;
                                        if( m_Language == "ITA" )
                                            message =
                                                String.Format(
                                                    "Stai per impostare il gruppo <em><basefont color=red>{2}</basefont></em> a {0} oggett{1}.<br><br>Desideri continuare?",
                                                    rads.Count, rads.Count == 1 ? "o" : "i", newGroup.Name );
                                        else
                                            message =
                                                String.Format(
                                                    "You are about to set the group <em><basefont color=red>{2}</basefont></em> to {0} item{1}.<br><br>Would you like to continue?",
                                                    rads.Count, rads.Count == 1 ? "" : "s", newGroup.Name );
                                        m_From.SendGump( new WarningGump( 1060635, 30720, message, 0xFFC000, 420, 280,
                                                                        new WarningGumpCallback( ApplyGroup_Callback ),
                                                                        new object[] { list, rads, m_ListPage, m_OrigList, newGroup, m_FilterGroup } ) );
                                    }
                                    else
                                    {
                                        string message;
                                        if( m_Language == "ITA" )
                                            message = "Non è stato possibile modificare il gruppo di appartenenza dell'oggetto selezionato.";
                                        else
                                            message = "It was not possible to change the group for the selected item.";
                                        if( m_List != null )
                                        {
                                            Item item = m_List[ m_Index ] as Item;
                                            ItemsGroup oldGroup = GroupsHandler.GetGroup( item );

                                            if( oldGroup == newGroup )
                                            {
                                                if( m_Language == "ITA" )
                                                    message = "Seleziona un gruppo diverso da quello originale.";
                                                else
                                                    message = "Select a group different from the original one.";
                                            }
                                            else if( newGroup.IsAccessible( m_Secure ) )
                                            {
                                                if( ( oldGroup == null || oldGroup.RemoveItem( item, m_Secure ) ) && newGroup.AddItem( item, m_Secure ) )
                                                {
                                                    message =
                                                        String.Format(
                                                            m_Language == "ITA"
                                                                ? "Un oggetto è stato spostato nel gruppo <em><basefont color=red>{0}</basefont></em>."
                                                                : "One item has been moved to group <em><basefont color=red>{0}</basefont></em>.",
                                                            newGroup.Name );
                                                }
                                            }
                                        }

                                        m_From.SendGump( new NoticeGump( 1060637, 30720, message, 0xFFC000, 420, 280,
                                                                       new NoticeGumpCallback( ResendGump_Callback ),
                                                                       new object[]
                                                                           {
                                                                               list, rads, m_OrigList, GroupsGumpPage.Details, m_ListPage, m_Index,
                                                                               m_FilterGroup
                                                                           } ) );
                                    }

                                    break;
                                }
                            default:
                                break;
                        }
                        break;
                    }
            }
        }

        private void ResendGump( GroupsGumpPage page )
        {
            m_From.SendGump( new GroupsGump( m_From, page, m_List, m_Checked, m_OrigList, m_Groups,
                                           new object[] { m_ListPage, m_Index, m_GroupsPage, m_FilterGroup } ) );
        }

        private static void ResendGump_Callback( Mobile from, object state )
        {
            object[] states = (object[])state;
            ArrayList list = (ArrayList)states[ 0 ];
            ArrayList rads = (ArrayList)states[ 1 ];
            ArrayList original = (ArrayList)states[ 2 ];
            GroupsGumpPage gumpPage = (GroupsGumpPage)states[ 3 ];
            int page = (int)states[ 4 ];
            int index = (int)states[ 5 ];
            ItemsGroup filter = (ItemsGroup)states[ 6 ];

            from.SendGump( new GroupsGump( from, gumpPage, list, rads, original, null, new object[] { page, index, 0, filter } ) );
        }

        private static void Delete_Callback( Mobile from, bool ok, object state )
        {
            object[] states = (object[])state;
            ArrayList list = (ArrayList)states[ 0 ];
            ArrayList rads = (ArrayList)states[ 1 ];
            ArrayList original = (ArrayList)states[ 2 ];
            int page = (int)states[ 3 ];
            ItemsGroup filter = (ItemsGroup)states[ 4 ];

            string language = ( from.Language == null ? String.Empty : from.Language.ToUpper() );

            int deleted = 0;

            if( ok )
            {
                deleted = GroupsHandler.DeleteItems( from, rads );

                GroupsHandler.RemoveInvalid( list, true );
                GroupsHandler.RemoveInvalid( original, true );

                if( deleted > 0 )
                {
                    string message;
                    if( language == "ITA" )
                        message = String.Format( "Cancellazione completata: {0} oggett{1} selezionat{1} {2} stat{1} rimoss{1} dal server.",
                                                deleted == 1 ? "un" : deleted.ToString(), deleted == 1 ? "o" : "i", deleted == 1 ? "è" : "sono" );
                    else
                        message = String.Format( "Delete complete: {0} selected item{1} {2} been deleted from server.",
                                                deleted == 1 ? "One" : deleted.ToString(), deleted == 1 ? "" : "s", deleted == 1 ? "has" : "have" );
                    from.SendGump( new NoticeGump( 1060637, 30720, message, 0xFFC000, 420, 280, new NoticeGumpCallback( ResendGump_Callback ),
                                                 new object[] { list, null, original, GroupsGumpPage.Main, 0, 0, filter } ) );
                }
                else
                    from.SendGump( new NoticeGump( 1060637, 30720,
                                                 language == "ITA"
                                                     ? "Non è stato possibile cancellare permanentemente gli oggetti selezionati.<br><br>Probabilmente gli oggetti selezionati sono stati precedentemente cancellati dal server, oppure non disponi dei privilegi richiesti per compiere questa operazione."
                                                     : "It was not possible to permanently delete the selected items.<br><br>Maybe the selected items have already been deleted, or you lack required privileges to perform this task.",
                                                 0xFFC000, 420, 280, new NoticeGumpCallback( ResendGump_Callback ),
                                                 new object[] { list, rads, original, GroupsGumpPage.Main, 0, 0, filter } ) );
            }
            else
            {
                from.SendGump( new NoticeGump( 1060637, 30720, language == "ITA" ? "Operazione annullata." : "Delete aborted.", 0xFFC000, 420, 280,
                                             new NoticeGumpCallback( ResendGump_Callback ),
                                             new object[] { list, rads, original, GroupsGumpPage.Main, 0, 0, filter } ) );
            }
        }

        private static void Toggle_Callback( Mobile from, bool ok, object state )
        {
            object[] states = (object[])state;
            ArrayList list = (ArrayList)states[ 0 ];
            ArrayList rads = (ArrayList)states[ 1 ];
            ArrayList original = (ArrayList)states[ 2 ];
            int page = (int)states[ 3 ];
            ItemsGroup filter = (ItemsGroup)states[ 4 ];

            string language = ( from.Language == null ? String.Empty : from.Language.ToUpper() );

            int added = 0;
            int deleted = 0;
            string message = "";

            bool secure = from.AccessLevel >= GroupsHandler.SecureLevel;

            if( ok )
            {
                for( int i = 0; i < rads.Count; i++ )
                {
                    Item item = rads[ i ] as Item;

                    if( item == null || item.Deleted )
                        continue;

                    ItemsGroup group = GroupsHandler.GetGroup( item );

                    if( group == null )
                    {
                        group = ( filter ?? GroupsHandler.DefaultGroup );

                        if( group.AddItem( item, secure ) )
                            added++;
                    }
                    else
                    {
                        if( group.RemoveItem( item, secure ) )
                            deleted++;
                    }
                }

                if( deleted > 0 )
                    message += ( language == "ITA"
                                    ? String.Format(
                                          "{0} oggett{1} {2} stat{1} rimoss{1} dal loro gruppo di appartenenza. Quest{1} oggett{1} non comparir{3} nella finestra dell'HandleGroups e pertanto dovr{3} essere cercat{1} manualmente.<br><br>",
                                          deleted, deleted == 1 ? "o" : "i", deleted == 1 ? "è" : "sono", deleted == 1 ? "à" : "anno" )
                                    : String.Format(
                                          "{0} item{1} {2} been removed from their group. {3} item{1} will no more show on HandleGroups' gump, and so you'll need to find {4} manually if you want to track {4} again.<br><br>",
                                          deleted, deleted == 1 ? "" : "s", deleted == 1 ? "has" : "have", deleted == 1 ? "This" : "These",
                                          deleted == 1 ? "it" : "them" ) );
                if( added > 0 )
                    message += ( language == "ITA"
                                    ? String.Format(
                                          "{0} oggett{1} {2} stat{1} aggiunt{1} al gruppo {4}. Quest{1} oggett{1} comparir{3} nella finestra dell'HandleGroups e potr{3} essere controllat{1} tramite essa.",
                                          added, added == 1 ? "o" : "i", added == 1 ? "è" : "sono", added == 1 ? "à" : "anno",
                                          filter == null ? GroupsHandler.DefaultGroup.Name : filter.Name )
                                    : String.Format(
                                          "{0} item{1} {2} been added to the group {4}. {3} item{1} will be available on HandleGroups' gump, and so will be easily trackable from there.",
                                          added, added == 1 ? "" : "s", added == 1 ? "has" : "have", added == 1 ? "This" : "These",
                                          filter == null ? GroupsHandler.DefaultGroup.Name : filter.Name ) );

                if( deleted == 0 && added == 0 )
                    message += ( language == "ITA"
                                    ? "Non è stato possibile modificare l'appartenenza ai gruppi degli oggetti selezionati.<br><br>Probabilmente gli oggetti selezionati sono stati precedentemente spostati in un altro gruppo oppure cancellati dal server, oppure non disponi dei privilegi richiesti per compiere questa operazione."
                                    : "It was not possible to change group to the selected items.<br><br>Maybe the selected items have previously been moved into another group, or deleted, or you lack required privileges to perform this task." );

                from.SendGump( new NoticeGump( 1060637, 30720, message, 0xFFC000, 420, 280, new NoticeGumpCallback( ResendGump_Callback ),
                                             new object[] { list, rads, original, GroupsGumpPage.Main, page, 0, filter } ) );
            }
            else
            {
                from.SendGump( new NoticeGump( 1060637, 30720, language == "ITA" ? "Operazione annullata." : "Aborted.", 0xFFC000, 420, 280,
                                             new NoticeGumpCallback( ResendGump_Callback ),
                                             new object[] { list, rads, original, GroupsGumpPage.Main, 0, 0, filter } ) );
            }
        }

        private static void ApplyGroup_Callback( Mobile from, bool ok, object state )
        {
            object[] states = (object[])state;
            ArrayList list = (ArrayList)states[ 0 ];
            ArrayList rads = (ArrayList)states[ 1 ];
            int page = (int)states[ 2 ];
            ArrayList original = (ArrayList)states[ 3 ];
            ItemsGroup newGroup = (ItemsGroup)states[ 4 ];
            ItemsGroup filter = (ItemsGroup)states[ 5 ];

            string language = ( from.Language == null ? String.Empty : from.Language.ToUpper() );

            string message = ( language == "ITA" ? "Operazione annullata." : "Aborted." );
            int done = 0;

            bool secure = from.AccessLevel >= GroupsHandler.SecureLevel;

            if( newGroup == null )
                newGroup = GroupsHandler.DefaultGroup;

            if( !newGroup.IsAccessible( secure ) )
            {
                message = ( language == "ITA"
                               ? "Impossibile modificare il gruppo di appartenenza.<br><br>Non disponi dei privilegi richiesti per compiere questa operazione."
                               : "Unable to change group.<br><br>You lack required privileges to perform this task." );
                ok = false;
            }

            if( ok )
            {
                for( int i = 0; i < rads.Count; i++ )
                {
                    Item item = rads[ i ] as Item;
                    ItemsGroup oldGroup = GroupsHandler.GetGroup( item );

                    if( item == null || item.Deleted || newGroup == oldGroup )
                        continue;

                    if( ( oldGroup == null || oldGroup.RemoveItem( item, secure ) ) && newGroup.AddItem( item, secure ) )
                        done++;
                }

                if( done > 0 )
                    message = ( language == "ITA"
                                   ? String.Format( "{0} oggett{1} {2} stat{1} spostat{1} nel gruppo <em><basefont color=red>{3}</basefont></em>.",
                                                   done, done == 1 ? "o" : "i", done == 1 ? "è" : "sono", newGroup.Name )
                                   : String.Format( "{0} item{1} {2} been moved to group <em><basefont color=red>{3}</basefont></em>.", done,
                                                   done == 1 ? "" : "s", done == 1 ? "has" : "have", newGroup.Name ) );
                else
                    message = ( language == "ITA"
                                   ? "Non è stato possibile modificare il gruppo di appartenenza degli oggetti selezionati.<br><br>Probabilmente il gruppo di destinazione coincide con quello di origine, oppure gli oggetti sono stati precedentemente cancellati dal server, oppure non disponi dei privilegi richiesti per compiere questa operazione."
                                   : "It was not possible to change group to the selected items.<br><br>Maybe the destination group is the same as source group, or the selected items have been deleted, or you lack required privileges to perform this task." );
            }

            from.SendGump( new NoticeGump( 1060637, 30720, message, 0xFFC000, 420, 280, new NoticeGumpCallback( ResendGump_Callback ),
                                         new object[] { list, rads, original, GroupsGumpPage.Main, page, 0, filter } ) );
        }

        private static void ToggleSecurity_Callback( Mobile from, bool ok, object state )
        {
            object[] states = (object[])state;
            ArrayList list = (ArrayList)states[ 0 ];
            ArrayList rads = (ArrayList)states[ 1 ];
            int page = (int)states[ 2 ];
            ArrayList original = (ArrayList)states[ 3 ];
            ItemsGroup group = (ItemsGroup)states[ 4 ];
            ItemsGroup filter = (ItemsGroup)states[ 5 ];

            string language = ( from.Language == null ? String.Empty : from.Language.ToUpper() );

            string message = ( language == "ITA" ? "Operazione annullata." : "Aborted." );
            bool secure = from.AccessLevel >= GroupsHandler.SecureLevel;

            if( group == null || group == GroupsHandler.DefaultGroup || group == GroupsHandler.DecorationGroup || group == GroupsHandler.DoorGenGroup ||
                group == GroupsHandler.MoonGenGroup || group == GroupsHandler.TelGenGroup || group == GroupsHandler.SignGenGroup ||
                group == GroupsHandler.UOAMVendorsGroup )
            {
                message = ( language == "ITA"
                               ? "Impossibile modificare lo stato di sicurezza di questo gruppo." : "Unable to change security level of this group." );
                ok = false;
            }
            else if( !group.IsAccessible( secure ) )
            {
                message = ( language == "ITA"
                               ? "Impossibile modificare lo stato di sicurezza di questo gruppo.<br><br>Non disponi dei privilegi richiesti per compiere questa operazione."
                               : "It was not possible to change security level of this group.<br><br>You lack required privileges to perform this task." );
                ok = false;
            }

            if( ok )
            {
                group.Secure = !group.Secure;
                message = ( language == "ITA"
                               ? String.Format( "Il gruppo <em><basefont color=red>{0}</basefont></em> è stato impostato come {1}sicuro.", group.Name,
                                               group.Secure ? "" : "NON " )
                               : String.Format( "The group <em><basefont color=red>{0}</basefont></em> has been set as {1}secure.", group.Name,
                                               group.Secure ? "" : "NOT " ) );
            }

            from.SendGump( new NoticeGump( 1060637, 30720, message, 0xFFC000, 420, 280, new NoticeGumpCallback( ResendGump_Callback ),
                                         new object[] { list, rads, original, GroupsGumpPage.Groups, page, 0, filter } ) );
        }

        private static void AddItem_Callback( Mobile from, object targeted, object state )
        {
            bool secure = from.AccessLevel >= GroupsHandler.SecureLevel;
            string language = ( from.Language == null ? String.Empty : from.Language.ToUpper() );

            object[] states = (object[])state;
            ArrayList list = (ArrayList)states[ 0 ];
            ArrayList rads = (ArrayList)states[ 1 ];
            ArrayList original = (ArrayList)states[ 2 ];
            int page = (int)states[ 3 ];
            ItemsGroup filter = (ItemsGroup)states[ 4 ];

            Item item = GroupsHandler.CheckAddon( targeted as Item );

            if( item == null )
                from.SendMessage( language == "ITA" ? "Quello non è un oggetto." : "That is not an item." );
            else if( !GroupsHandler.IsAccessible( from, item ) )
                from.SendMessage( language == "ITA" ? "Quello non è accessibile" : "That is not accessible." );
            else
            {
                if( !GroupsHandler.IsInvalid( item ) )
                {
                    if( list.Contains( item ) )
                    {
                        from.SendMessage( language == "ITA" ? "L'oggetto è già elencato." : "That item is already listed." );
                    }
                    else
                    {
                        list.Add( item );

                        if( !GroupsHandler.InGroup( item ) && !rads.Contains( item ) )
                            rads.Add( item );

                        if( !original.Contains( item ) )
                            original.Add( item );

                        from.SendMessage( language == "ITA"
                                             ? "L'oggetto è stato aggiunto alla lista corrente." : "The item has been added to current list." );
                    }
                }
            }

            from.CloseGump( typeof( GroupsGump ) );
            from.SendGump( new GroupsGump( from, GroupsGumpPage.Main, list, rads, original, null, new object[] { page, 0, 0, filter } ) );
        }

        private static bool GetCommandDetails( string commandString, out string command, out string argString, out string[] args )
        {
            bool contained = false;
            int indexOf = commandString.IndexOf( ' ' );

            if( indexOf >= 0 )
            {
                argString = commandString.Substring( indexOf + 1 );
                command = commandString.Substring( 0, indexOf );

                if( command.ToLower() == "contained" )
                {
                    argString = argString.TrimStart();
                    indexOf = argString.IndexOf( ' ' );
                    if( indexOf >= 0 )
                    {
                        command = argString.Substring( 0, indexOf );
                        argString = argString.Substring( indexOf + 1 );
                    }
                    else
                    {
                        command = argString;
                        argString = "";
                        args = new string[ 0 ];
                    }
                    contained = true;
                }

                args = CommandSystem.Split( argString );
            }
            else
            {
                argString = "";
                command = commandString;
                args = new string[ 0 ];
            }

            return contained;
        }
    }
}