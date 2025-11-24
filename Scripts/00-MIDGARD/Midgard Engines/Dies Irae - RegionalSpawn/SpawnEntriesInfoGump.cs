using System;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Regions;

namespace Midgard.Engines.RegionalSpawningSystem
{
    public class SpawnEntriesInfoGump : RegionBaseSpawnGump
    {
        public enum Buttons
        {
            Close = 0,

            Page = 10000,

            AddEntry,
            StopAll,
            StartAll
        }

        #region design variables
        protected override int NumLabels { get { return 10; } }
        protected override int NumButtons { get { return 3; } }
        protected override int MainWindowWidth { get { return 600; } }
        #endregion

        private readonly List<SpawnEntry> m_Entries;
        private static readonly int[] m_HorOffset = new int[] { 0, 50, 275, 400, 450, 550 };

        public SpawnEntriesInfoGump( BaseRegion region, Mobile owner )
            : base( region, owner )
        {
            owner.CloseGump( typeof( SpawnEntriesInfoGump ) );

            m_Entries = Core.GetSpawnEntries( Region );
            Design();

            base.RegisterUse( typeof( SpawnEntriesInfoGump ) );
        }

        private void Design()
        {
            AddPage( 0 );

            AddMainBackground();
            AddMainTitle( String.Format( "Spawns of {0}", Region.Name ) );
            AddMainWindow();

            int labelOffsetX = GetMainWindowLabelsX();
            int labelOffsetY = GetMainWindowFirstLabelY();

            // Groups
            AddSubTitle( labelOffsetX + m_HorOffset[ 0 ], "Type" );
            AddSubTitle( labelOffsetX + m_HorOffset[ 1 ], "Name" );
            AddSubTitle( labelOffsetX + m_HorOffset[ 2 ], "Spawn" );
            AddSubTitle( labelOffsetX + m_HorOffset[ 3 ], "m Time" );
            AddSubTitle( labelOffsetX + m_HorOffset[ 4 ], "M Time" );
            AddSubTitle( labelOffsetX + m_HorOffset[ 5 ], "Info" );

            AddCloseButton();

            if( m_Entries.Count > 0 )
            {
                for( int i = 0; i < m_Entries.Count; ++i )
                {
                    int page = i / NumLabels;
                    int pos = i % NumLabels;

                    if( pos == 0 )
                    {
                        if( page > 0 )
                            AddNextPageButton( page, (int)Buttons.Page );

                        AddPage( page + 1 );

                        if( page > 0 )
                            AddPrevPageButton( page, (int)Buttons.Page );
                    }

                    SpawnEntry entry = m_Entries[ i ];
                    if( entry == null )
                        continue;

                    int hue;
                    string typeName;
                    string label = FormatEntry( entry, out hue, out typeName );

                    int y = pos * LabelsOffset + labelOffsetY;

                    AddLabel( labelOffsetX + m_HorOffset[ 0 ], y, hue, !String.IsNullOrEmpty( label ) ? label : "spawn entry" );
                    AddLabel( labelOffsetX + m_HorOffset[ 1 ], y, hue, typeName );
                    AddLabel( labelOffsetX + m_HorOffset[ 2 ], y, hue, string.Format( "m {0} - M {1}", entry.SpawnedObjects.Count, entry.Max ) );
                    AddLabel( labelOffsetX + m_HorOffset[ 3 ], y, hue, entry.MinSpawnTime.TotalMinutes.ToString( "F1" ) );
                    AddLabel( labelOffsetX + m_HorOffset[ 4 ], y, hue, entry.MaxSpawnTime.TotalMinutes.ToString( "F1" ) );
                    AddMainWindowButton( labelOffsetX + m_HorOffset[ 5 ], y + 3, i + 1, (int)AccessLevel.Seer );
                }
            }

            // buttons
            AddActionButton( 1, "add entry", (int)Buttons.AddEntry, Owner, (int)AccessLevel.Seer );
            AddActionButton( 2, "stop all entries", (int)Buttons.StopAll, Owner, (int)AccessLevel.Seer );
            AddActionButton( 3, "start all entries", (int)Buttons.StartAll, Owner, (int)AccessLevel.Seer );

            AddCloseButton();
        }

        public static string FormatEntry( SpawnEntry entry, out int hue, out string typeName )
        {
            string name;

            SpawnDefinition def = entry.Definition;
            if( def is SpawnMobile )
            {
                name = "mobile";
                typeName = ( (SpawnType)def ).Type.Name;
            }
            else if( def is SpawnItem )
            {
                name = "item";
                typeName = ( (SpawnType)def ).Type.Name;
            }
            else if( def is SpawnGroup )
            {
                name = "group";
                typeName = ( (SpawnGroup)def ).Name;
            }
            else
            {
                name = entry.ToString();
                typeName = def.ToString();
            }

            typeName = MidgardUtility.GetFriendlyClassName( typeName );
            hue = entry.Running ? 0x3F : 0x25;
            return name;
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            Mobile from = sender.Mobile;

            int id = info.ButtonID;

            if( id > 0 && id <= m_Entries.Count + 1 )
            {
                SpawnEntry entry = m_Entries[ id - 1 ];

                from.SendGump( new SpawnEntryDetailsGump( Region, Owner, entry ) );
                return;
            }

            switch( id )
            {
                case (int)Buttons.AddEntry:
                    from.SendMessage( "Not implemented yet." );
                    // from.SendGump( new AddSpawnEntryGump( Region, Owner ) );
                    break;
                case (int)Buttons.StopAll:
                    Core.StopRegionSpawns( from, Region );
                    break;
                case (int)Buttons.StartAll:
                    Core.StartRegionSpawns( from, Region );
                    break;
                default:
                    from.SendGump( new RegionSpawnInfoGump( Region, Owner ) );
                    break;
            }
        }
    }
}