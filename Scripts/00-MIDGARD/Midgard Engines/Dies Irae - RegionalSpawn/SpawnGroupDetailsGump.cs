using Server;
using Server.Gumps;
using Server.Network;
using Server.Regions;

namespace Midgard.Engines.RegionalSpawningSystem
{
    public class SpawnGroupDetailsGump : RegionBaseSpawnGump
    {
        public enum Buttons
        {
            Close,

            Page = 10000
        }

        #region design variables
        protected override int NumLabels { get { return 6; } }
        protected override int NumButtons { get { return 1; } }
        protected override int MainWindowWidth { get { return 325; } }
        #endregion

        private readonly SpawnGroup m_Group;
        private readonly SpawnGroupElement[] m_Elements;
        private static readonly int[] m_HorOffset = new int[] { 0, 150, 200, 250 };

        public SpawnGroupDetailsGump( BaseRegion region, Mobile owner, SpawnGroup group )
            : base( region, owner )
        {
            m_Group = group;
            m_Elements = m_Group.Elements;

            owner.CloseGump( typeof( SpawnEntryDetailsGump ) );

            Design();

            base.RegisterUse( typeof( SpawnEntryDetailsGump ) );
        }

        private void Design()
        {
            AddPage( 0 );

            AddMainBackground();
            AddMainTitle( string.Format( "Details for {0}", MidgardUtility.GetFriendlyClassName( m_Group.Name ) ) );
            AddMainWindow();

            // buttons
            // none

            AddCloseButton();

            if( m_Elements.Length <= 0 )
                return;

            int hue = HuePrim;
            int labelOffsetX = GetMainWindowLabelsX();
            int labelOffsetY = GetMainWindowFirstLabelY();

            // Groups
            AddSubTitle( labelOffsetX + m_HorOffset[ 0 ], "Name" );
            AddSubTitle( labelOffsetX + m_HorOffset[ 1 ], "Type" );
            AddSubTitle( labelOffsetX + m_HorOffset[ 2 ], "Weight" );
            AddSubTitle( labelOffsetX + m_HorOffset[ 3 ], "Edit" );

            for( int i = 0; i < m_Elements.Length; ++i )
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

                SpawnGroupElement element = m_Elements[ i ];
                if( element == null )
                    continue;

                int y = pos * LabelsOffset + labelOffsetY;

                hue = GetHueFor( hue );
                string typeName;
                string label = FormatGroup( element, out typeName );

                AddLabel( labelOffsetX + m_HorOffset[ 0 ], y, hue, typeName );
                AddLabel( labelOffsetX + m_HorOffset[ 1 ], y, hue, label );
                AddLabel( labelOffsetX + m_HorOffset[ 2 ], y, hue, element.Weight.ToString() );

                AddMainWindowButton( labelOffsetX + m_HorOffset[ 3 ], y + 3, i + 1, (int)AccessLevel.Seer );
            }
        }

        public static string FormatGroup( SpawnGroupElement element, out string typeName )
        {
            string name;

            SpawnDefinition def = element.SpawnDefinition;
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
                name = element.ToString();
                typeName = def.ToString();
            }

            typeName = MidgardUtility.GetFriendlyClassName( typeName );
            return name;
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            Mobile from = sender.Mobile;

            switch( info.ButtonID )
            {
                case 0:
                    from.SendGump( new SpawnGroupsInfoGump( Region, from ) );
                    break;
            }
        }
    }
}