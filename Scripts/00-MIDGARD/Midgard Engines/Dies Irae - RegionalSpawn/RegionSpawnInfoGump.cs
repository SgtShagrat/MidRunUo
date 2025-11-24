/***************************************************************************
 *                               RegionSpawnInfoGump.cs
 *
 *   begin                : Aprile 2012
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Regions;

namespace Midgard.Engines.RegionalSpawningSystem
{
    public class RegionSpawnInfoGump : RegionBaseSpawnGump
    {
        public enum Buttons
        {
            Close,

            RespawnRegion,
            DelRegionSpawns,
            StartRegionSpawns,
            StopRegionSpawns,
            SpawnsInfo,
            AdvancedCommands,
            EditSpawnGroups,

            Guide
        }

        #region design variables
        protected override int NumLabels { get { return Labels.Length; } }
        protected override int NumButtons { get { return 7; } }
        protected override int MainWindowWidth { get { return 380; } }
        #endregion

        private static readonly string[] Labels = new string[]
                                                      {
                                                          "Region Name",
                                                          "Area",
                                                          "Spawn Entries",
                                                          "-  Running",
                                                          "-  Spawning"
                                                      };

        public RegionSpawnInfoGump( BaseRegion region, Mobile owner )
            : base( region, owner )
        {
            if( region == null )
                return;

            Owner.CloseGump( typeof( RegionSpawnInfoGump ) );

            Design();

            base.RegisterUse( typeof( RegionSpawnInfoGump ) );
        }

        private void Design()
        {
            AddPage( 0 );

            AddMainBackground();
            AddMainTitle( 225, "Midgard - Regional Spawn Info" );
            AddMainWindow();

            int labelOffsetX = GetMainWindowLabelsX();
            int labelOffsetY = GetMainWindowFirstLabelY();

            for( int i = 0; i < Labels.Length; i++ )
                AddLabel( labelOffsetX, labelOffsetY + ( LabelsOffset * i ), GroupsHue, String.Format( "{0}:", Labels[ i ] ) );

            List<string> values = new List<string>();
            values.Add( Region.Name ?? "-unamed region-" );
            values.Add( Region.SizeOfRegion.ToString() );
            values.Add( Region.Spawns == null ? "0" : Region.Spawns.Length.ToString() );
            values.Add( Core.GetRunningSpawnEntries( Region.Spawns ).ToString() );
            values.Add( Core.GetSpawningSpawnEntries( Region.Spawns ).ToString() );

            for( int i = 0; i < values.Count; i++ )
                AddLabel( labelOffsetX + 100, labelOffsetY + ( 20 * i ), DefaultValueHue, values[ i ] );

            // Buttons
            AddActionButton( 1, "respawn region", (int)Buttons.RespawnRegion, Owner, (int)AccessLevel.Seer );
            AddActionButton( 2, "delete spawn objects", (int)Buttons.DelRegionSpawns, Owner, (int)AccessLevel.Seer );
            AddActionButton( 3, "start spawns", (int)Buttons.StartRegionSpawns, Owner, (int)AccessLevel.Seer );
            AddActionButton( 4, "stop respawn", (int)Buttons.StopRegionSpawns, Owner, (int)AccessLevel.Seer );
            AddActionButton( 5, "spawns info", (int)Buttons.SpawnsInfo, Owner, (int)AccessLevel.Seer );
            AddActionButton( 6, "advanced commands", (int)Buttons.AdvancedCommands, Owner, (int)AccessLevel.Administrator );
            AddActionButton( 7, "spawn groups", (int)Buttons.EditSpawnGroups, Owner, (int)AccessLevel.Seer );

            AddCloseButton();
            AddGuideButton( (int)Buttons.Guide );
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            if( Region == null )
                return;

            Mobile from = sender.Mobile;
            if( from == null )
                return;

            switch( info.ButtonID )
            {
                case (int)Buttons.RespawnRegion:
                    Core.RespawnRegion( from, Region );
                    from.SendGump( new RegionSpawnInfoGump( Region, from ) );
                    break;
                case (int)Buttons.DelRegionSpawns:
                    Core.DelRegionSpawns( from, Region );
                    from.SendGump( new RegionSpawnInfoGump( Region, from ) );
                    break;
                case (int)Buttons.StartRegionSpawns:
                    Core.StartRegionSpawns( from, Region );
                    from.SendGump( new RegionSpawnInfoGump( Region, from ) );
                    break;
                case (int)Buttons.StopRegionSpawns:
                    Core.StopRegionSpawns( from, Region );
                    from.SendGump( new RegionSpawnInfoGump( Region, from ) );
                    break;
                case (int)Buttons.SpawnsInfo:
                    from.SendGump( new SpawnEntriesInfoGump( Region, from ) );
                    break;
                case (int)Buttons.AdvancedCommands:
                    from.SendGump( new AdvancedFeaturesGump( Region, from ) );
                    break;
                case (int)Buttons.EditSpawnGroups:
                    from.SendGump( new SpawnGroupsInfoGump( Region, from ) );
                    break;
                case (int)Buttons.Guide:
                    from.LaunchBrowser( Config.Guide );
                    break;
                default:
                    from.SendMessage( "You closed that menu." );
                    break;
            }
        }
    }
}