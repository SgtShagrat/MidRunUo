using Server;
using Server.Gumps;
using Server.Network;
using Server.Regions;

namespace Midgard.Engines.RegionalSpawningSystem
{
    public class SpawnEntryDetailsGump : RegionBaseSpawnGump
    {
        public enum Buttons
        {
            Close,

            Start,
            Stop,
            Respawn,
            DeleteObjects
        }

        #region design variables
        protected override int NumLabels { get { return 6; } }
        protected override int NumButtons { get { return 5; } }
        protected override int MainWindowWidth { get { return 325; } }

        private const int HorOffset = 100;
        #endregion

        private readonly SpawnEntry m_Entry;

        public SpawnEntryDetailsGump( BaseRegion region, Mobile owner, SpawnEntry entry )
            : base( region, owner )
        {
            m_Entry = entry;

            owner.CloseGump( typeof( SpawnEntryDetailsGump ) );

            Design();

            base.RegisterUse( typeof( SpawnEntryDetailsGump ) );
        }

        private void Design()
        {
            AddPage( 0 );

            AddMainBackground();
            AddMainTitle( "Entry details" );
            AddMainWindow();

            int labelOffsetX = GetMainWindowLabelsX();
            int labelOffsetY = GetMainWindowFirstLabelY();

            int tabbedLabelX = labelOffsetX + 20;

            int hue;
            string typeName;
            string label = SpawnEntriesInfoGump.FormatEntry( m_Entry, out hue, out typeName );

            // Spawn definition type (group, etc)
            labelOffsetY += 0 * LabelsOffset;
            AddLabel( tabbedLabelX, labelOffsetY, LabelsHue, "Type:" );
            AddLabel( tabbedLabelX + HorOffset, labelOffsetY, DefaultValueHue, label );

            // Spawn definition type (group, etc)
            labelOffsetY += LabelsOffset;
            AddLabel( tabbedLabelX, labelOffsetY, LabelsHue, "Name:" );
            AddLabel( tabbedLabelX + HorOffset, labelOffsetY, DefaultValueHue, typeName );

            // Maximum spawn amount
            labelOffsetY += LabelsOffset;
            AddLabel( tabbedLabelX, labelOffsetY, LabelsHue, "Max amount:" );
            AddLabel( tabbedLabelX + HorOffset, labelOffsetY, DefaultValueHue, m_Entry.Max.ToString() );

            // Spawned objects count
            labelOffsetY += LabelsOffset;
            AddLabel( tabbedLabelX, labelOffsetY, LabelsHue, "Actual spawns:" );
            AddLabel( tabbedLabelX + HorOffset, labelOffsetY, DefaultValueHue, m_Entry.SpawnedObjects.Count.ToString() );

            // Min spawn time
            labelOffsetY += LabelsOffset;
            AddLabel( tabbedLabelX, labelOffsetY, LabelsHue, "Min spawn time:" );
            AddLabel( tabbedLabelX + HorOffset, labelOffsetY, DefaultValueHue, m_Entry.MinSpawnTime.TotalMinutes.ToString( "F1" ) + " min" );

            // Max spawn time
            labelOffsetY += LabelsOffset;
            AddLabel( tabbedLabelX, labelOffsetY, LabelsHue, "Max spawn time:" );
            AddLabel( tabbedLabelX + HorOffset, labelOffsetY, DefaultValueHue, m_Entry.MinSpawnTime.TotalMinutes.ToString( "F1" ) + " min" );

            // buttons
            AddActionButton( 1, "start spawns", (int)Buttons.Start, Owner, (int)AccessLevel.Seer );
            AddActionButton( 2, "stop spawns", (int)Buttons.Stop, Owner, (int)AccessLevel.Seer );
            AddActionButton( 3, "respawn", (int)Buttons.Respawn, Owner, (int)AccessLevel.Seer );
            AddActionButton( 4, "delete spawned", (int)Buttons.DeleteObjects, Owner, (int)AccessLevel.Seer );

            AddCloseButton();
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            Mobile from = sender.Mobile;

            switch( info.ButtonID )
            {
                case (int)Buttons.Start:
                    Core.StartEntrySpawns( from, m_Entry );
                    from.SendGump( new SpawnEntriesInfoGump( Region, from ) );
                    break;
                case (int)Buttons.Stop:
                    Core.StopEntrySpawns( from, m_Entry );
                    from.SendGump( new SpawnEntriesInfoGump( Region, from ) );
                    break;
                case (int)Buttons.Respawn:
                    Core.RespawnEntry( from, m_Entry );
                    from.SendGump( new SpawnEntriesInfoGump( Region, from ) );
                    break;
                case (int)Buttons.DeleteObjects:
                    Core.DelEntrySpawns( from, m_Entry );
                    from.SendGump( new SpawnEntriesInfoGump( Region, from ) );
                    break;
                default:
                    from.SendGump( new SpawnEntriesInfoGump( Region, from ) );
                    break;
            }
        }
    }
}