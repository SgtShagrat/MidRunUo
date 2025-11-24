using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Regions;

namespace Midgard.Engines.RegionalSpawningSystem
{
    public class SpawnGroupsInfoGump : RegionBaseSpawnGump
    {
        public enum Buttons
        {
            Close = 0,

            Page = 10000,

            AddGroup,
            SaveModifications
        }

        #region design variables
        protected override int NumLabels { get { return 10; } }
        protected override int NumButtons { get { return 2; } }
        protected override int MainWindowWidth { get { return 300; } }
        #endregion

        private readonly List<SpawnGroup> m_Groups;
        private static readonly int[] m_HorOffset = new int[] { 0, 225, 265 };

        public SpawnGroupsInfoGump( BaseRegion region, Mobile owner )
            : base( region, owner )
        {
            owner.CloseGump( typeof( SpawnGroupsInfoGump ) );

            m_Groups = Core.GetSpawnGroups();
            Design();

            base.RegisterUse( typeof( SpawnGroupsInfoGump ) );
        }

        private void Design()
        {
            AddPage( 0 );

            AddMainBackground();
            AddMainTitle( "Spawn Groups" );
            AddMainWindow();

            int labelOffsetX = GetMainWindowLabelsX();
            int labelOffsetY = GetMainWindowFirstLabelY();

            // Groups
            AddSubTitle( labelOffsetX + m_HorOffset[ 0 ], "Name" );
            AddSubTitle( labelOffsetX + m_HorOffset[ 1 ] - 5, "Entries" );

            // buttons
            AddActionButton( 1, "add group", (int)Buttons.AddGroup, Owner, (int)AccessLevel.Seer );
            AddActionButton( 2, "save modifications", (int)Buttons.SaveModifications, Owner, (int)AccessLevel.Seer );

            AddCloseButton();

            if ( m_Groups.Count <= 0 )
                return;

            int hue = HuePrim;

            for( int i = 0; i < m_Groups.Count; ++i )
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

                SpawnGroup group = m_Groups[ i ];
                if ( group == null )
                    continue;

                hue = GetHueFor( hue );
                int y = pos * LabelsOffset + labelOffsetY;

                AddLabel( labelOffsetX + m_HorOffset[ 0 ], y, hue, MidgardUtility.GetFriendlyClassName( group.Name ) );
                AddLabel( labelOffsetX + m_HorOffset[ 1 ], y, hue, group.Elements.Length.ToString() );
                AddMainWindowButton( labelOffsetX + m_HorOffset[ 2 ], y + 4, i + 1, (int)AccessLevel.Seer );
            }
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            Mobile from = sender.Mobile;

            int id = info.ButtonID;

            if( id > 0 && id <= m_Groups.Count + 1 )
            {
                SpawnGroup spawnGroup = m_Groups[ id - 1 ];

                from.SendGump( new SpawnGroupDetailsGump( Region, Owner, spawnGroup ) );
                return;
            }

            switch( id )
            {
                case (int)Buttons.AddGroup:
                    from.SendMessage( "Not implemented yet." );
                    // from.SendGump( new AddSpawnGroupGump( Region, Owner ) );
                    break;
                case (int)Buttons.SaveModifications:
                    Core.Snapshot();
                    from.SendMessage( "New spawn groups file saved. Contact Dies Irae for file upload." );
                    break;
                default:
                    from.SendGump( new RegionSpawnInfoGump( Region, Owner ) );
                    break;
            }
        }
    }
}