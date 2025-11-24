/***************************************************************************
 *                               HireSelectedGuardGump.cs
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
    public class HireSelectedGuardGump : TownGump
    {
        public enum Buttons
        {
            Close = 0,

            HireGuard = 1
        }

        #region design variables
        protected override int NumLabels { get { return 7; } }
        protected override int NumButtons { get { return 1; } }
        protected override int MainWindowWidth { get { return 325; } }
        #endregion

        private readonly GuardList m_List;

        public HireSelectedGuardGump( GuardList guardList, TownSystem system, Mobile owner )
            : base( system, owner, null, 50, 50 )
        {
            owner.CloseGump( typeof( HireSelectedGuardGump ) );

            m_List = guardList;

            Design();

            base.RegisterUse( typeof( HireSelectedGuardGump ) );
        }

        private void Design()
        {
            if( m_List == null )
                return;

            AddPage( 0 );

            AddMainBackground();
            AddMainTitle( 190, m_List.Definition.Header.String );
            AddMainWindow();

            int labelOffsetX = GetMainWindowLabelsX();
            int labelOffsetY = GetMainWindowFirstLabelY();

            int tabbedLabelX = labelOffsetX + 20;

            AddLabel( labelOffsetX, labelOffsetY + ( 0 * LabelsOffset ), GroupsHue, "Guard Infoes:" );

            AddLabel( tabbedLabelX, labelOffsetY + ( 1 * LabelsOffset ), LabelsHue, "You have:" );
            AddLabel( tabbedLabelX + 110, labelOffsetY + ( 1 * LabelsOffset ), DefaultValueHue, m_List.Guards.Count.ToString() );

            AddLabel( tabbedLabelX, labelOffsetY + ( 2 * LabelsOffset ), LabelsHue, "Maximum:" );
            AddLabel( tabbedLabelX + 110, labelOffsetY + ( 2 * LabelsOffset ), DefaultValueHue, m_List.Definition.Maximum.ToString() );

            AddLabel( tabbedLabelX, labelOffsetY + ( 3 * LabelsOffset ), LabelsHue, "Price:" );
            AddLabel( tabbedLabelX + 110, labelOffsetY + ( 3 * LabelsOffset ), DefaultValueHue, m_List.Definition.Price.ToString( "N0" ) );

            AddLabel( tabbedLabelX, labelOffsetY + ( 4 * LabelsOffset ), LabelsHue, "Daily Pay:" );
            AddLabel( tabbedLabelX + 110, labelOffsetY + ( 4 * LabelsOffset ), DefaultValueHue, m_List.Definition.Upkeep.ToString( "N0" ) );

            AddLabel( tabbedLabelX, labelOffsetY + ( 5 * LabelsOffset ), LabelsHue, "Treasure:" );
            AddLabel( tabbedLabelX + 110, labelOffsetY + ( 5 * LabelsOffset ), DefaultValueHue, Town.TownTreasure.ToString( "N0" ) );

            AddLabel( tabbedLabelX, labelOffsetY + ( 6 * LabelsOffset ), LabelsHue, "Militia Cost:" );
            AddLabel( tabbedLabelX + 110, labelOffsetY + ( 6 * LabelsOffset ), DefaultValueHue, Town.MilitiaCost.ToString( "N0" ) );

            // buttons
            AddActionButton( 1, "hire guard", (int)Buttons.HireGuard, Owner, (int) TownAccessFlags.CommandGuards );

            AddCloseButton();
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            Mobile from = sender.Mobile;
            if( from == null )
                return;

            if( !Town.CanCommandGuards( from ) )
            {
                from.SendMessage( "You cannot do that." );
                return;
            }

            if( info.ButtonID == (int) Buttons.HireGuard )
            {
                TownSystem system = TownSystem.Find( Owner.Region );
                if( system == null )
                    return;

                if( system != Town )
                {
                    from.SendMessage( "You must be in your town to hire a guard." );
                }
                else if( m_List.Guards.Count >= m_List.Definition.Maximum )
                {
                    from.SendMessage( "You have reached the maximum guards available for that type." );
                }
                else if( Town.TownTreasure >= m_List.Definition.Price )
                {
                    BaseTownGuard guard = m_List.Construct();
                    if( guard == null )
                        return;

                    guard.Town = Town.Definition.Town;

                    Town.EditTownTreasure( -m_List.Definition.Price );

                    guard.MoveToWorld( Owner.Location, Owner.Map );
                    guard.Home = guard.Location;
                }
                else
                {
                    from.SendMessage( "You lack the required funds." );
                }
            }
        }
    }
}