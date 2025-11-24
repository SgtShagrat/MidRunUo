/***************************************************************************
 *                                      AdvancedFeaturesGump.cs
 *
 *   begin                : 24 aprile 2012
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using Server;
using Server.Gumps;
using Server.Network;
using Server.Regions;

namespace Midgard.Engines.RegionalSpawningSystem
{
    public class AdvancedFeaturesGump : RegionBaseSpawnGump
    {
        public enum Buttons
        {
            Close,

            RespawnAllRegions,
            DelAllRegionSpawns,
            StartAllRegionSpawns,
            StopAllRegionSpawns,
            ListRegionalsSpawns
        }

        #region design variables
        protected override int NumLabels { get { return 1; } }
        protected override int NumButtons { get { return 5; } }
        protected override int MainWindowWidth { get { return 310; } }
        #endregion

        public AdvancedFeaturesGump( BaseRegion region, Mobile owner )
            : base( region, owner )
        {
            owner.CloseGump( typeof( AdvancedFeaturesGump ) );

            Design();

            base.RegisterUse( typeof( AdvancedFeaturesGump ) );
        }

        private void Design()
        {
            AddPage( 0 );

            AddMainBackground();
            AddMainTitle( "Advanced features" );
            AddMainWindow();

            int labelOffsetX = GetMainWindowLabelsX();
            int labelOffsetY = GetMainWindowFirstLabelY();

            // labels
            AddLabel( labelOffsetX, labelOffsetY, GroupsHue, "Active Spawns:" );
            // AddLabel( labelOffsetX, labelOffsetY + LabelsOffset, GroupsHue, "" );

            // values
            AddLabel( labelOffsetX + 220, labelOffsetY, LabelsHue, Core.Table.Count.ToString() );
            // AddLabel( labelOffsetX + 220, labelOffsetY + LabelsOffset, LabelsHue, "" );

            // buttons
            AddActionButton( 1, "respawn all regions", (int)Buttons.RespawnAllRegions, Owner, (int)AccessLevel.Administrator );
            AddActionButton( 2, "delete all spawned objects", (int)Buttons.DelAllRegionSpawns, Owner, (int)AccessLevel.Administrator );
            AddActionButton( 3, "start all regional spawns", (int)Buttons.StartAllRegionSpawns, Owner, (int)AccessLevel.Administrator );
            AddActionButton( 4, "stop all regional spawns ", (int)Buttons.StopAllRegionSpawns, Owner, (int)AccessLevel.Administrator );
            AddActionButton( 5, "list all regional spawns", (int)Buttons.ListRegionalsSpawns, Owner, (int)AccessLevel.Administrator );

            AddCloseButton();
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            Mobile from = sender.Mobile;

            switch( info.ButtonID )
            {
                case (int)Buttons.RespawnAllRegions:
                    Core.RespawnAllRegions( from );
                    break;
                case (int)Buttons.DelAllRegionSpawns:
                    Core.DelAllRegionSpawns( from );
                    break;
                case (int)Buttons.StartAllRegionSpawns:
                    Core.StartAllRegionSpawns( from );
                    break;
                case (int)Buttons.StopAllRegionSpawns:
                    Core.StopAllRegionSpawns( from );
                    break;
                case (int)Buttons.ListRegionalsSpawns:
                    Core.ListRegionSpawns( from );
                    break;
                default:
                    from.SendGump( new RegionSpawnInfoGump( Region, Owner ) );
                    break;
            }
        }
    }
}