/***************************************************************************
 *                                  AdvancedFeaturesGump.cs
 *                            		-----------------------
 *  begin                	: Aprile, 2008
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
    public class AdvancedFeaturesGump : TownGump
    {
        public enum Buttons
        {
            Close,

            EditBanList,
            EditPermaBanList,
            ManageAccessLevel,
            ClearInactiveStates,
            ClearNews,
            ManageGuards
        }

        #region design variables
        protected override int NumLabels { get { return 2; } }
        protected override int NumButtons { get { return 5; } }
        protected override int MainWindowWidth { get { return 310; } }
        #endregion

        public AdvancedFeaturesGump( TownSystem system, Mobile owner )
            : base( system, owner, 50, 50 )
        {
            owner.CloseGump( typeof( AdvancedFeaturesGump ) );

            Design();

            base.RegisterUse( typeof( AdvancedFeaturesGump ) );
        }

        private void Design()
        {
            AddPage( 0 );

            AddMainBackground();
            AddMainTitle( 250, String.Format( "Advanced feat. of {0}", Town.Definition.TownName ) );
            AddMainWindow();

            int labelOffsetX = GetMainWindowLabelsX();
            int labelOffsetY = GetMainWindowFirstLabelY();

            // labels
            AddLabel( labelOffsetX, labelOffsetY, GroupsHue, "exiled players:" );
            AddLabel( labelOffsetX, labelOffsetY + LabelsOffset, GroupsHue, "inactive players:" );

            // values
            AddLabel( labelOffsetX + 220, labelOffsetY, LabelsHue, Town.ExiledPlayers.Count.ToString() );
            AddLabel( labelOffsetX + 220, labelOffsetY + LabelsOffset, LabelsHue, Town.GetInactiveStatesCount().ToString() );

            // buttons
            AddActionButton( 1, "manage exile list", (int)Buttons.EditBanList, Owner, (int)TownAccessFlags.BanCitizen );
            AddActionButton( 2, "manage permaexile list", (int)Buttons.EditPermaBanList, Owner, (int)TownAccessFlags.PermaBanCitizen );
            AddActionButton( 3, "manage town access", (int)Buttons.ManageAccessLevel, Owner, (int)TownAccessFlags.SetTownAccess );
            AddActionButton( 4, "remove inactive players", (int)Buttons.ClearInactiveStates, Owner, (int)TownAccessFlags.RemoveInactiveStates );
            AddActionButton( 5, "clear town news", (int)Buttons.ClearNews, Owner, (int)TownAccessFlags.ClearNews );
            AddActionButton( 6, "manage guards", (int)Buttons.ManageGuards, Owner, (int)TownAccessFlags.CommandGuards );

            AddCloseButton();
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            Mobile from = sender.Mobile;

            switch( info.ButtonID )
            {
                case (int)Buttons.ManageAccessLevel:
                    from.SendGump( new EditTownAccessGump( Town, Owner ) );
                    break;
                case (int)Buttons.EditBanList:
                    from.SendGump( new EditBanListGump( Town, Owner ) );
                    break;
                case (int)Buttons.EditPermaBanList:
                    from.SendGump( new EditPermaBanListGump( Town, Owner ) );
                    break;
                case (int)Buttons.ClearInactiveStates:
                    Town.ClearInactiveStates();
                    from.SendMessage( "You have removed all inactive players from {0} town system.", Town.Definition.TownName );
                    from.SendGump( new TownSystemInfoGump( Town, Owner ) );
                    break;
                case (int)Buttons.ClearNews:
                    Town.ClearNews();
                    from.SendMessage( "You have removed all news from {0} news system.", Town.Definition.TownName );
                    from.SendGump( new TownSystemInfoGump( Town, Owner ) );
                    break;
                case (int)Buttons.ManageGuards:
                    from.SendGump( new GuardsMainGump( Town, Owner ) );
                    break;
                default:
                    from.SendGump( new TownSystemInfoGump( Town, Owner ) );
                    break;
            }
        }
    }
}