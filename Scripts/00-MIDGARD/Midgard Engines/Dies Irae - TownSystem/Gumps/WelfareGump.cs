/***************************************************************************
 *                                  WelfareGump.cs
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

namespace Midgard.Engines.MidgardTownSystem
{
    public class WelfareGump : TownGump
    {
        public enum Buttons
        {
            Close,

            EditPrices,
            ManageVendors,
            ToggleGlobalVendorState
        }

        #region design variables
        protected override int NumLabels { get { return 4; } }
        protected override int NumButtons { get { return 3; } }
        protected override int MainWindowWidth { get { return 310; } }
        #endregion

        public WelfareGump( TownSystem system, Mobile owner )
            : base( system, owner, 50, 50 )
        {
            owner.CloseGump( typeof( WelfareGump ) );

            Design();

            base.RegisterUse( typeof( WelfareGump ) );
        }

        private void Design()
        {
            AddPage( 0 );

            AddMainBackground();
            AddMainTitle( 205, String.Format( "Welfare of {0}", Town.Definition.TownName ) );
            AddMainWindow();

            int labelOffsetX = GetMainWindowLabelsX();
            int labelOffsetY = GetMainWindowFirstLabelY();

            // labels
            AddLabel( labelOffsetX, labelOffsetY + ( LabelsOffset * 0 ), GroupsHue, "general tax rate:" );
            AddLabel( labelOffsetX, labelOffsetY + ( LabelsOffset * 1 ), GroupsHue, "player vendor tax:" );
            AddLabel( labelOffsetX, labelOffsetY + ( LabelsOffset * 2 ), GroupsHue, "land cost:" );
            AddLabel( labelOffsetX, labelOffsetY + ( LabelsOffset * 3 ), GroupsHue, "access cost:" );

            // values
            AddLabel( labelOffsetX + 220, labelOffsetY + ( LabelsOffset * 0 ), LabelsHue, Town.PercMercTaxes.ToString() );
            AddLabel( labelOffsetX + 220, labelOffsetY + ( LabelsOffset * 1 ), LabelsHue, Town.PercPlayerVendorTaxes.ToString() );
            AddLabel( labelOffsetX + 220, labelOffsetY + ( LabelsOffset * 2 ), LabelsHue, Town.LandCost.ToString() );
            AddLabel( labelOffsetX + 220, labelOffsetY + ( LabelsOffset * 3 ), LabelsHue, Town.ServiceAccessCost.ToString() );

            // buttons
            AddActionButton( 1, "manage item prices", (int)Buttons.EditPrices, Owner, (int)TownAccessFlags.Citizen );
            AddActionButton( 2, "manage vendors", (int)Buttons.ManageVendors, Owner, (int)TownAccessFlags.Citizen );
            AddActionButton( 3, "toggle global vendor states", (int)Buttons.ToggleGlobalVendorState, Owner, (int)TownAccessFlags.CanEditItemPrice );
            AddCloseButton();
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            Mobile from = sender.Mobile;

            switch( info.ButtonID )
            {
                case (int)Buttons.EditPrices:
                    from.SendGump( new ItemPricesGump( Town, Owner ) );
                    break;
                case (int)Buttons.ManageVendors:
                    from.SendGump( new TownVendorsGump( Town, Owner ) );
                    break;
                case (int)Buttons.ToggleGlobalVendorState:
                    Town.CommercialStatus.ToggleGlobalVendorStates( Owner );
                    from.SendGump( new WelfareGump( Town, Owner ) );
                    break;
                default:
                    from.SendGump( new TownSystemInfoGump( Town, Owner ) );
                    break;
            }
        }
    }
}