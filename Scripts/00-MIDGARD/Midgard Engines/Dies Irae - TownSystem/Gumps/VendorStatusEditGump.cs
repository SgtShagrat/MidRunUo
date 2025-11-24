/***************************************************************************
 *                                  TownAccessInfoGump.cs
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
    public class VendorStatusEditGump : TownGump
    {
        public enum Buttons
        {
            Close = 0,

            ToggleStatus = 1000,
            ToggleBuySell,
            RefundDebt,
            GetGain,
            ToggleRefundMode,
            EditScalar
        }

        #region design variables
        protected override int NumLabels { get { return 7; } }
        protected override int NumButtons { get { return 6; } }
        protected override int MainWindowWidth { get { return 290; } }
        #endregion

        private readonly TownVendorState m_State;

        public VendorStatusEditGump( TownSystem system, Mobile owner, TownVendorState state )
            : base( system, owner, 50, 50 )
        {
            owner.CloseGump( typeof( VendorStatusEditGump ) );

            m_State = state;

            Design();

            base.RegisterUse( typeof( VendorStatusEditGump ) );
        }

        private void Design()
        {
            TownPlayerState tps = TownPlayerState.Find( Owner );
            if( tps == null )
                return;

            AddPage( 0 );

            AddMainBackground();
            AddMainTitle( 190, String.Format( "{0} Details:", MidgardUtility.GetFriendlyClassName( m_State.VendorType.Name ) ) );
            AddMainWindow();

            int labelOffsetX = GetMainWindowLabelsX();
            int labelOffsetY = GetMainWindowFirstLabelY();

            AddLabel( labelOffsetX, labelOffsetY + ( 0 * LabelsOffset ), TitleHue, "Num. in town:" );
            AddLabel( labelOffsetX + 20 + 100, labelOffsetY + ( 0 * LabelsOffset ), DefaultValueHue, m_State.Instances == null ? "0" : m_State.Instances.Count.ToString() );

            AddLabel( labelOffsetX, labelOffsetY + ( 1 * LabelsOffset ), TitleHue, "Status:" );
            AddLabel( labelOffsetX + 20 + 100, labelOffsetY + ( 1 * LabelsOffset ), DefaultValueHue, m_State.Status.ToString() );

            AddLabel( labelOffsetX, labelOffsetY + ( 2 * LabelsOffset ), TitleHue, "Balance:" );
            AddLabel( labelOffsetX + 20 + 100, labelOffsetY + ( 2 * LabelsOffset ), DefaultValueHue, m_State.Balance.ToString() );

            string tmp = String.Format( "{0}/{1}", ( m_State.ActiveSeller ? "Yes" : "No" ), ( m_State.ActiveBuyer ? "Yes" : "No" ) );
            AddLabel( labelOffsetX, labelOffsetY + ( 3 * LabelsOffset ), TitleHue, "Vendors Buy/Sell:" );
            AddLabel( labelOffsetX + 20 + 100, labelOffsetY + ( 3 * LabelsOffset ), DefaultValueHue, tmp );

            tmp = String.Format( "{0}/{1}", m_State.NegativeRefundLevel, m_State.PositiveRefundLevel );
            AddLabel( labelOffsetX, labelOffsetY + ( 4 * LabelsOffset ), TitleHue, "Refund Levels:" );
            AddLabel( labelOffsetX + 20 + 100, labelOffsetY + ( 4 * LabelsOffset ), DefaultValueHue, tmp );

            tmp = String.Format( "{0}/{1}", m_State.AutoNegativeRefund ? "Automatic" : "Manual", m_State.AutoPositiveRefund ? "Automatic" : "Manual" );
            AddLabel( labelOffsetX, labelOffsetY + ( 5 * LabelsOffset ), TitleHue, "Refund Modes:" );
            AddLabel( labelOffsetX + 20 + 100, labelOffsetY + ( 5 * LabelsOffset ), DefaultValueHue, tmp );

            AddLabel( labelOffsetX, labelOffsetY + ( 6 * LabelsOffset ), TitleHue, "Scalar:" );
            AddLabel( labelOffsetX + 20 + 100, labelOffsetY + ( 6 * LabelsOffset ), DefaultValueHue, m_State.Scalar.ToString( "F2" ) );

            // buttons
            AddActionButton( 1, "toggle status", (int)Buttons.ToggleStatus, Owner, (int)TownAccessFlags.CanEditItemPrice );
            AddActionButton( 2, "toggle sell/buy status", (int)Buttons.ToggleBuySell, Owner, (int)TownAccessFlags.CanEditItemPrice );
            AddActionButton( 3, "refund debt", (int)Buttons.RefundDebt, Owner, (int)TownAccessFlags.CanEditItemPrice );
            AddActionButton( 4, "get gain", (int)Buttons.GetGain, Owner, (int)TownAccessFlags.CanEditItemPrice );
            AddActionButton( 5, "toggle refund mode", (int)Buttons.ToggleRefundMode, Owner, (int)TownAccessFlags.CanEditItemPrice );
            AddActionButton( 6, "edit scalar", (int)Buttons.EditScalar, Owner, (int)TownAccessFlags.CanEditItemPrice );

            AddCloseButton();
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            Mobile from = sender.Mobile;

            TownPlayerState tps = TownPlayerState.Find( Owner );
            if( tps == null )
                return;

            int id = info.ButtonID;

            switch( id )
            {
                case (int)Buttons.ToggleStatus:
                    m_State.Status = m_State.Status == VendorStatus.Active ? VendorStatus.Closed : VendorStatus.Active;

                    from.SendMessage( "You have {0} that town service", m_State.Status == VendorStatus.Active ? "enabled" : "disabled" );

                    from.SendGump( new VendorStatusEditGump( Town, Owner, m_State ) );
                    break;
                case (int)Buttons.ToggleBuySell:
                    switch( m_State.BSStatus )
                    {
                        case BuySellStatus.BothEnabled:
                        case BuySellStatus.SellEnabled:
                        case BuySellStatus.BuyEnabled:
                            m_State.BSStatus += 1;
                            break;
                        case BuySellStatus.BothDisabled:
                            m_State.BSStatus = BuySellStatus.BothEnabled;
                            break;
                        default:
                            break;
                    }

                    from.SendMessage( "Now this vendor service ({0}) {1}.", m_State.VendorType.Name, m_State.BSStatusString );

                    from.SendGump( new VendorStatusEditGump( Town, Owner, m_State ) );
                    break;
                case (int)Buttons.RefundDebt:
                    m_State.RefundDebit( Owner, true );

                    from.SendGump( new VendorStatusEditGump( Town, Owner, m_State ) );
                    break;
                case (int)Buttons.GetGain:
                    m_State.GetGain( Owner, true );

                    from.SendGump( new VendorStatusEditGump( Town, Owner, m_State ) );
                    break;
                case (int)Buttons.ToggleRefundMode:
                    switch( m_State.RefundMode )
                    {
                        case RefundModeStatus.BothAutomatic:
                        case RefundModeStatus.AutomaticPositive:
                        case RefundModeStatus.AutomaticNegative:
                            m_State.RefundMode += 1;
                            break;
                        case RefundModeStatus.BothManual:
                            m_State.RefundMode = RefundModeStatus.BothAutomatic;
                            break;
                        default:
                            break;
                    }

                    from.SendMessage( "Now this vendor service ({0}) {1}.", m_State.VendorType.Name, m_State.RefundModeString );

                    from.SendGump( new VendorStatusEditGump( Town, Owner, m_State ) );
                    break;
                case (int)Buttons.EditScalar:
                    {
                        from.SendStringQuery( new TownConfirmationQuery( "Would you like to edit the scalar for this vendor?",
                            "Enter a new integer scalar.",
                            true,
                            300,
                            new TownConfirmationQueryCallback( ConfirmScalarEditCallback ),
                            m_State ) );
                        break;
                    }
                default:
                    from.SendGump( new TownVendorsGump( Town, Owner ) );
                    break;
            }
        }

        private static void ConfirmScalarEditCallback( Mobile from, bool okay, string text, object state )
        {
            if( okay )
            {
                int newScalar;

                if( int.TryParse( text, out newScalar ) )
                {
                    TownVendorState vendorState = state as TownVendorState;

                    if( vendorState != null )
                    {
                        vendorState.Scalar = newScalar / 100.0;
                        from.SendMessage( "The new scalar is set to: " + newScalar );
                    }
                    else
                    {
                        from.SendMessage( "Vendor state is null for this mobile." );
                    }
                }
                else
                {
                    from.SendMessage( "That is not a valid value. Try with an interger." );
                }
            }
        }
    }
}