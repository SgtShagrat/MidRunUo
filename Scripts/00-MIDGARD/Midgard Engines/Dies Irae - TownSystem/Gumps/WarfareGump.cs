using System;
using System.Collections.Generic;

using Server;
using Server.Gumps;
using Server.Network;

namespace Midgard.Engines.MidgardTownSystem
{
    public class WarfareGump : TownGump
    {
        public enum Buttons
        {
            Close,

            Switch,
            AddWar,
            AddAlliance,
            RemoveWar,
            RemoveAlliance
        }

        public enum GumpType
        {
            Allies,
            Enemies
        }

        #region design variables
        protected override int NumLabels
        {
            get
            {
                if( m_Type == GumpType.Allies )
                    return Town.TownAllies.Count == 0 ? 2 : Town.TownAllies.Count + 1;
                else
                    return Town.TownEnemies.Count == 0 ? 2 : Town.TownEnemies.Count + 1;
            }
        }

        protected override int NumButtons { get { return 5; } }
        protected override int MainWindowWidth { get { return 310; } }
        #endregion

        private readonly GumpType m_Type;

        public WarfareGump( TownSystem system, Mobile owner )
            : this( system, owner, GumpType.Allies )
        {
        }

        public WarfareGump( TownSystem system, Mobile owner, GumpType gumpType )
            : base( system, owner, 50, 50 )
        {
            owner.CloseGump( typeof( WarfareGump ) );

            m_Type = gumpType;

            Design();

            base.RegisterUse( typeof( WarfareGump ) );
        }

        private void Design()
        {
            AddPage( 0 );

            AddMainBackground();
            AddMainTitle( 205, String.Format( "Warfare of {0}", Town.Definition.TownName ) );
            AddMainWindow();

            int labelOffsetX = GetMainWindowLabelsX();
            int labelOffsetY = GetMainWindowFirstLabelY();

            // labels
            string label = m_Type == GumpType.Enemies ? "towns you are at war with:" : "towns you are allied with:";

            AddLabel( labelOffsetX, labelOffsetY, GroupsHue, label );

            List<TownSystem> toDisplay = ( m_Type == GumpType.Enemies ) ? Town.TownEnemies : Town.TownAllies;

            if( toDisplay != null && toDisplay.Count > 0 )
            {
                for( int i = 0; i < toDisplay.Count; i++ )
                    AddLabel( labelOffsetX, labelOffsetY + LabelsOffset + ( LabelsOffset * i ), LabelsHue, toDisplay[ i ].Definition.TownName );
            }
            else
                AddLabel( labelOffsetX, labelOffsetY + LabelsOffset, LabelsHue, "- none -" );

            // buttons
            AddActionButton( 1, "switch", (int)Buttons.Switch );
            AddActionButton( 2, "add war", (int)Buttons.AddWar, Owner, (int)TownAccessFlags.CanEditWarFare );
            AddActionButton( 3, "add alliance", (int)Buttons.AddAlliance, Owner, (int)TownAccessFlags.CanEditWarFare );
            AddActionButton( 4, "remove war", (int)Buttons.RemoveWar, Owner, (int)TownAccessFlags.CanEditWarFare );
            AddActionButton( 5, "remove alliance", (int)Buttons.RemoveAlliance, Owner, (int)TownAccessFlags.CanEditWarFare );
            AddCloseButton();
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            Mobile from = sender.Mobile;

            switch( info.ButtonID )
            {
                case (int)Buttons.Switch:
                    GumpType newType = ( m_Type == GumpType.Allies ) ? GumpType.Enemies : GumpType.Allies;
                    from.SendGump( new WarfareGump( Town, Owner, newType ) );
                    break;
                case (int)Buttons.AddWar:
                    from.SendGump( new EditWarsGump( Town, Owner, EditWarsGump.GumpType.AddWar ) );
                    break;
                case (int)Buttons.AddAlliance:
                    from.SendGump( new EditWarsGump( Town, Owner, EditWarsGump.GumpType.AddAlliance ) );
                    break;
                case (int)Buttons.RemoveWar:
                    from.SendGump( new EditWarsGump( Town, Owner, EditWarsGump.GumpType.RemoveWar ) );
                    break;
                case (int)Buttons.RemoveAlliance:
                    from.SendGump( new EditWarsGump( Town, Owner, EditWarsGump.GumpType.RemoveAlliance ) );
                    break;
                default:
                    from.SendGump( new TownSystemInfoGump( Town, Owner ) );
                    break;
            }
        }
    }
}