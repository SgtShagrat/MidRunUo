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
    public class ItemPricesGump : TownGump
    {
        public enum Buttons
        {
            Close,

            SetFilter,
            Page = 10000
        }

        #region design variables
        protected override int NumLabels { get { return 10; } }
        protected override int NumButtons { get { return 2; } }
        protected override int MainWindowWidth { get { return 585; } }
        protected override bool HasSubtitles { get { return true; } }
        #endregion

        private readonly List<ItemCommercialInfo> m_InfoList;

        public ItemPricesGump( TownSystem system, Mobile owner )
            : this( system, owner, BuildList( system, null ) )
        {
        }

        public ItemPricesGump( TownSystem system, Mobile owner, ItemFilter filter )
            : this( system, owner, BuildList( system, filter ) )
        {
        }

        public ItemPricesGump( TownSystem system, Mobile owner, List<ItemCommercialInfo> list )
            : base( system, owner, 50, 50 )
        {
            owner.CloseGump( typeof( ItemPricesGump ) );

            m_InfoList = list;
            Design();

            base.RegisterUse( typeof( ItemPricesGump ) );
        }

        private static List<ItemCommercialInfo> BuildList( TownSystem town, ItemFilter filter )
        {
            if( filter == null || filter.Flags == ItemFilterFlags.None )
                return town.EditableItemPrices;
            else
                return filter.ForTypes( town.EditableItemPrices );
        }

        private void Design()
        {
            List<ItemCommercialInfo> infoes = m_InfoList;

            AddPage( 0 );

            AddMainBackground();
            AddMainTitle( 225, String.Format( "Prices for {0}", Town.Definition.TownName ) );
            AddMainWindow();

            int labelOffsetX = GetMainWindowLabelsX();
            int labelOffsetY = GetMainWindowFirstLabelY();

            // Groups
            AddSubTitle( labelOffsetX, "Type" );
            AddSubTitle( labelOffsetX + 185, "Original Cost" );
            AddSubTitle( labelOffsetX + 295, "Actual Cost" );
            AddSubTitle( labelOffsetX + 405, "Quantity Sold" );
            AddSubTitle( labelOffsetX + 520, "Edit" );

            // buttons
            AddCloseButton();

            AddActionButton( 2, "set filter", (int)Buttons.SetFilter );

            int hue = HuePrim;

            if( HasAccess( (int) TownAccessFlags.CanEditItemPrice ) )
            {
                AddLabel( 15, GetFirstBtnY(), Colors.Indigo, "custom price" );
                AddBlackAlpha( 105, GetFirstBtnY(), 100, 20 );
                AddTextEntry( 105, GetFirstBtnY(), 100, 20, HuePrim, 1, "" );
            }

            if( infoes.Count > 0 )
            {
                ItemCommercialInfo info;
                TownItemPriceDefinition tcd;

                for( int i = 0; i < infoes.Count; ++i )
                {
                    int page = i / NumLabels;
                    int pos = i % NumLabels;

                    if( pos == 0 )
                    {
                        if( page > 0 )
                            AddButton( 575, 10, 0x15E1, 0x15E5, (int)Buttons.Page, GumpButtonType.Page, page + 1 ); // Next

                        AddPage( page + 1 );

                        if( page > 0 )
                            AddButton( 555, 10, 0x15E3, 0x15E7, (int)Buttons.Page, GumpButtonType.Page, page ); // Back
                    }

                    info = infoes[ i ];
                    tcd = TownItemPriceDefinition.GetDefFromType( info.ItemType );

                    hue = GetHueFor( hue );

                    if( tcd != null )
                    {
                        int mx, my, Mx, My;

                        Item.Measure( Item.GetBitmap( tcd.Figure ), out Mx, out My, out mx, out my );

                        int width = mx - Mx;
                        int height = my - My;

                        height = height > 20 ? height : 20;

                        int y = pos * LabelsOffset + labelOffsetY;

                        AddItem( labelOffsetX - ( width / 2 ), y - ( height / 2 ) + 15, tcd.Figure, tcd.Hue );

                        AddLabelCropped( labelOffsetX + 50, y, 130, 17, hue, tcd.ItemName );
                        AddLabel( labelOffsetX + 185, y, hue, tcd.OriginalPrice.ToString() );
                        AddLabel( labelOffsetX + 295, y, hue, info.ActualPrice.ToString() );
                        AddLabel( labelOffsetX + 405, y, hue, info.TotalSold.ToString() );

                        if( HasAccess( (int) TownAccessFlags.CanEditItemPrice ) )
                        {
                            AddButton( labelOffsetX + 515, y, UpBtnIdNormal, UpBtnIdPressed, i + 1 + 10000, GumpButtonType.Reply, 0 );
                            AddButton( labelOffsetX + 535, y, DownBtnIdNormal, DownBtnIdPressed, i + 1 + 20000, GumpButtonType.Reply, 0 );
                        }
                    }
                }
            }
            else
                AddLabel( labelOffsetX, labelOffsetY, hue, "- no price info found -" );
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            Mobile from = sender.Mobile;
            if( from == null )
                return;

            if( info.ButtonID == (int)Buttons.Close )
            {
                from.SendGump( new TownSystemInfoGump( Town, from ) );
                return;
            }
            else if( info.ButtonID == (int)Buttons.SetFilter )
            {
                from.SendGump( new ItemPricesSetFilterGump( Town, from, from, null ) );
                return;
            }

            if( m_InfoList == null || m_InfoList.Count == 0 )
                return;

            int id = info.ButtonID;
            int infoIndex = 0;

            if( id > 20000 )
                infoIndex = id - 20000 - 1;
            else if( id > 10000 )
                infoIndex = id - 10000 - 1;

            if( infoIndex > -1 && infoIndex < m_InfoList.Count )
            {
                TextRelay relayTxt;
                int customPrice = 0;

                if( info.TextEntries != null && info.TextEntries.Length > 0 )
                {
                    relayTxt = info.TextEntries[ 0 ];
                    customPrice = Utility.ToInt32( relayTxt.Text.Trim() );
                }

                int delta = customPrice == 0 ? 1 : customPrice;

                if( id > 10000 && id < 20000 )
                    m_InfoList[ infoIndex ].ActualPrice += delta;
                else
                {
                    if( m_InfoList[ infoIndex ].ActualPrice - delta > 1 )
                        m_InfoList[ infoIndex ].ActualPrice -= delta;
                    else
                        m_InfoList[ infoIndex ].ActualPrice = 1;
                }

                from.SendGump( new ItemPricesGump( Town, Owner, m_InfoList ) );
            }
        }
    }
}