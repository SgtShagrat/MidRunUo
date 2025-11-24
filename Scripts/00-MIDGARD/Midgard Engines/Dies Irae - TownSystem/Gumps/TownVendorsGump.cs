/***************************************************************************
 *                                  ItemPricesGump.cs
 *                            		-------------------
 *  begin                	: Marzo, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;
using Server;
using Server.Gumps;
using Server.Network;
using System.Collections.Generic;

namespace Midgard.Engines.MidgardTownSystem
{
    public class TownVendorsGump : TownGump
    {
        public enum Buttons
        {
            Close,

            Page = 10000,

            SortByVendorTypeName,
            SortByVendorStatus,
            SortByVendorBalance,
            SortByVendorCount
        }

        #region design variables
        protected override int NumLabels { get { return 10; } }
        protected override int NumButtons { get { return 4; } }
        protected override int MainWindowWidth { get { return 585; } }
        protected override bool HasSubtitles { get { return true; } }
        #endregion

        private readonly InternalComparerType m_OrderBy;
        private List<TownVendorState> m_InfoList;

        public TownVendorsGump( TownSystem system, Mobile owner )
            : this( system, owner, InternalComparerType.ByVendorTypeName )
        {
        }

        public TownVendorsGump( TownSystem system, Mobile owner, InternalComparerType orderBy )
            : base( system, owner, 50, 50 )
        {
            m_OrderBy = orderBy;

            owner.CloseGump( typeof( TownVendorsGump ) );

            Design();

            base.RegisterUse( typeof( TownVendorsGump ) );
        }

        private void Design()
        {
            m_InfoList = Town.CommercialStatus.States;

            InternalComparer comparer = new InternalComparer( m_OrderBy );
            m_InfoList.Sort( comparer );

            AddPage( 0 );

            AddMainBackground();
            AddMainTitle( 225, String.Format( "Vendors of {0}", Town.Definition.TownName ) );
            AddMainWindow();

            int labelOffsetX = GetMainWindowLabelsX();
            int labelOffsetY = GetMainWindowFirstLabelY();

            // Groups
            AddSubTitle( labelOffsetX, "Type" );
            AddSubTitle( labelOffsetX + 185, "Status" );
            AddSubTitle( labelOffsetX + 295, "Number" );
            AddSubTitle( labelOffsetX + 405, "Balance" );
            AddSubTitle( labelOffsetX + 520, "Edit" );

            // buttons
            AddActionButton( 1, "sort by name", (int)Buttons.SortByVendorTypeName, Owner, (int)TownAccessFlags.Citizen );
            AddActionButton( 2, "sort by status (and name)", (int)Buttons.SortByVendorStatus, Owner, (int)TownAccessFlags.Citizen );
            AddActionButton( 3, "sort by balance (and name)", (int)Buttons.SortByVendorBalance, Owner, (int)TownAccessFlags.Citizen );
            AddActionButton( 4, "sort by count (and name)", (int)Buttons.SortByVendorCount, Owner, (int)TownAccessFlags.Citizen );

            AddCloseButton();

            int hue = HuePrim;
            int i = 0;

            foreach( TownVendorState state in m_InfoList )
            {
                string typeName = MidgardUtility.GetFriendlyClassName( state.VendorType.Name );
                string status = state.Status.ToString();

                int page = i / NumLabels;
                int pos = i % NumLabels;

                if( pos == 0 )
                {
                    if( page > 0 )
                        AddButton( MainWindowWidth - 10, 10, 0x15E1, 0x15E5, (int)Buttons.Page, GumpButtonType.Page, page + 1 ); // Next

                    AddPage( page + 1 );

                    if( page > 0 )
                        AddButton( MainWindowWidth - 30, 10, 0x15E3, 0x15E7, (int)Buttons.Page, GumpButtonType.Page, page ); // Back
                }

                hue = GetHueFor( hue );

                int y = pos * LabelsOffset + labelOffsetY;

                AddLabel( labelOffsetX, y, hue, typeName );
                AddLabel( labelOffsetX + 185, y, hue, status );
                AddLabel( labelOffsetX + 295, y, hue, state.Instances == null ? "0" : state.Instances.Count.ToString() );
                AddLabel( labelOffsetX + 405, y, hue, state.Balance.ToString() );

                AddMainWindowButton( y + 4, i + 1, (int)( (int)TownAccessFlags.CanEditItemPrice ), false );

                i += 1;
            }
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            Mobile from = sender.Mobile;
            if( from == null )
                return;

            int id = info.ButtonID;

            if( id > 0 && id < m_InfoList.Count )
            {
                TownVendorState state = m_InfoList[ id - 1 ];

                if( state != null )
                    from.SendGump( new VendorStatusEditGump( Town, Owner, state ) );
                else
                    from.SendMessage( "The vendor state for this category is invalid." );
            }
            else if( id == (int)Buttons.SortByVendorTypeName )
            {
                from.SendGump( new TownVendorsGump( Town, Owner, InternalComparerType.ByVendorTypeName ) );
            }
            else if( id == (int)Buttons.SortByVendorStatus )
            {
                from.SendGump( new TownVendorsGump( Town, Owner, InternalComparerType.ByVendorStatus ) );
            }
            else if( id == (int)Buttons.SortByVendorBalance )
            {
                from.SendGump( new TownVendorsGump( Town, Owner, InternalComparerType.ByVendorBalance ) );
            }
            else if( id == (int)Buttons.SortByVendorCount )
            {
                from.SendGump( new TownVendorsGump( Town, Owner, InternalComparerType.ByVendorCount ) );
            }
            else
            {
                from.SendGump( new TownSystemInfoGump( Town, from ) );
            }
        }

        public enum InternalComparerType
        {
            ByVendorTypeName,
            ByVendorStatus,
            ByVendorBalance,
            ByVendorCount
        }

        private class InternalComparer : IComparer<TownVendorState>
        {
            private readonly InternalComparerType m_Type;

            public InternalComparer( InternalComparerType type )
            {
                m_Type = type;
            }

            public int Compare( TownVendorState x, TownVendorState y )
            {
                if( x == null || y == null )
                    return 0;

                switch( m_Type )
                {
                    case InternalComparerType.ByVendorTypeName:
                        {
                            return Insensitive.Compare( x.VendorType.Name, y.VendorType.Name );
                        }
                    case InternalComparerType.ByVendorStatus:
                        {
                            if( x.Status == y.Status )
                                return Insensitive.Compare( x.VendorType.Name, y.VendorType.Name );
                            else if( x.Status == VendorStatus.Active )
                                return -1;
                            else
                                return 1;
                        }
                    case InternalComparerType.ByVendorBalance:
                        {
                            if( x.Balance == y.Balance )
                                return Insensitive.Compare( x.VendorType.Name, y.VendorType.Name );
                            else if( x.Balance > y.Balance )
                                return -1;
                            else
                                return 1;
                        }
                    case InternalComparerType.ByVendorCount:
                        {
                            if( x.Instances == null )
                                return 0;
                            else if( y.Instances == null )
                                return 0;
                            else if( x.Instances.Count == y.Instances.Count )
                                return 0;
                            else if( x.Instances.Count > y.Instances.Count )
                                return -1;
                            else
                                return 1;
                        }
                    default:
                        return Insensitive.Compare( x.VendorType.Name, y.VendorType.Name );
                }
            }
        }
    }
}