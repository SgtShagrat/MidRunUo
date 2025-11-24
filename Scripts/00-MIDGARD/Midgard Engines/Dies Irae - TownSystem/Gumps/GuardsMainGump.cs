/***************************************************************************
 *                               GuardsMainGump.cs
 *
 *   revision             : 13 January, 2010
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using Server;
using Server.Gumps;
using Server.Network;

namespace Midgard.Engines.MidgardTownSystem
{
    public class GuardsMainGump : TownGump
    {
        public enum Buttons
        {
            Close,

            HireGuard = 1,
            ShowWayPoints,
            HideWayPoints,
            ResetWayPoints
        }

        #region design variables
        protected override int NumLabels { get { return 4; } }
        protected override int NumButtons { get { return 4; } }
        protected override int MainWindowWidth { get { return 325; } }
        #endregion

        public GuardsMainGump( TownSystem system, Mobile owner )
            : base( system, owner, null, 50, 50 )
        {
            owner.CloseGump( typeof( GuardsMainGump ) );

            Design();

            base.RegisterUse( typeof( GuardsMainGump ) );
        }

        private void Design()
        {
            AddPage( 0 );

            AddMainBackground();
            AddMainTitle( 190, "Town Militia" );
            AddMainWindow();

            int labelOffsetX = GetMainWindowLabelsX();
            int labelOffsetY = GetMainWindowFirstLabelY();

            int tabbedLabelX = labelOffsetX + 20;

            AddLabel( labelOffsetX, labelOffsetY + ( 0 * LabelsOffset ), GroupsHue, "Financial Infos:" );

            AddLabel( tabbedLabelX, labelOffsetY + ( 1 * LabelsOffset ), LabelsHue, "Gold:" );
            AddLabel( tabbedLabelX + 150, labelOffsetY + ( 1 * LabelsOffset ), DefaultValueHue, Town.TownTreasure.ToString( "N0" ) );

            AddLabel( tabbedLabelX, labelOffsetY + ( 2 * LabelsOffset ), LabelsHue, "Militia cost per day:" );
            AddLabel( tabbedLabelX + 150, labelOffsetY + ( 2 * LabelsOffset ), DefaultValueHue, Town.MilitiaCost.ToString( "N0" ) );

            AddLabel( tabbedLabelX, labelOffsetY + ( 3 * LabelsOffset ), LabelsHue, "Cashflow: " );
            AddLabel( tabbedLabelX + 150, labelOffsetY + ( 3 * LabelsOffset ), DefaultValueHue, Town.NetCashFlow.ToString( "N0" ) );

            // buttons
            AddActionButton( 1, "hire guard", (int)Buttons.HireGuard, Owner, (int)TownAccessFlags.CommandGuards );
            AddActionButton( 2, "show way points", (int)Buttons.ShowWayPoints, Owner, (int)TownAccessFlags.CommandGuards );
            AddActionButton( 3, "hide way points", (int)Buttons.HideWayPoints, Owner, (int)TownAccessFlags.CommandGuards );
            AddActionButton( 4, "reset way points", (int)Buttons.ResetWayPoints, Owner, (int)TownAccessFlags.CommandGuards );

            AddCloseButton();
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            Mobile from = sender.Mobile;

            switch( info.ButtonID )
            {
                case (int)Buttons.HireGuard:
                    from.SendGump( new SelectGuardGump( Town, Owner ) );
                    break;
                case (int)Buttons.ShowWayPoints:
                    if( Town != null )
                        Town.ToggleWayPointsVisibility( true );
                    from.SendMessage( "All town waypoints are now visible." );
                    break;
                case (int)Buttons.HideWayPoints:
                    if( Town != null )
                        Town.ToggleWayPointsVisibility( false );
                    from.SendMessage( "All town waypoints are now hidden." );
                    break;
                case (int)Buttons.ResetWayPoints:
                    if( Town != null )
                        Town.ResetWayPoints();
                    from.SendMessage( "All town waypoints have been deleted." );
                    break;
                default:
                    from.SendGump( new TownSystemInfoGump( Town, Owner ) );
                    break;
            }
        }
    }
}