/***************************************************************************
 *                               SelectGuardGump.cs
 *
 *   revision             : 13 January, 2010
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System.Collections.Generic;

using Server;
using Server.Gumps;
using Server.Network;

namespace Midgard.Engines.MidgardTownSystem
{
    public class SelectGuardGump : TownGump
    {
        public enum Buttons
        {
            Close = 0
        }

        #region design variables
        protected override int NumLabels { get { return 4; } }
        protected override int NumButtons { get { return 0; } }
        protected override int MainWindowWidth { get { return 325; } }
        #endregion

        public SelectGuardGump( TownSystem system, Mobile owner )
            : base( system, owner, null, 50, 50 )
        {
            owner.CloseGump( typeof( SelectGuardGump ) );

            Design();

            base.RegisterUse( typeof( SelectGuardGump ) );
        }

        private void Design()
        {
            AddPage( 0 );

            AddMainBackground();
            AddMainTitle( 190, "Guard Hiring" );
            AddMainWindow();

            int labelOffsetX = GetMainWindowLabelsX();
            int labelOffsetY = GetMainWindowFirstLabelY();

            int tabbedLabelX = labelOffsetX + 20;

            List<GuardList> guardLists = Town.GuardLists;

            for( int i = 0; i < guardLists.Count; ++i )
            {
                GuardList guardList = guardLists[ i ];

                AddMainWindowButton( labelOffsetX, labelOffsetY + ( i * LabelsOffset ) + 3, i + 1000, (int)TownAccessFlags.CommandGuards, false );
                // CenterItem( guardList.Definition.ItemID, 20, labelOffsetY + ( 1 * LabelsOffset ) - 20, 70, 60 );
                AddLabel( tabbedLabelX, labelOffsetY + ( i * LabelsOffset ), GroupsHue, guardList.Definition.Header.String );
            }

            AddCloseButton();
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            Mobile from = sender.Mobile;
            List<GuardList> guardLists = Town.GuardLists;

            int index = info.ButtonID - 1000;
            if( index < Town.GuardLists.Count && index > -1 )
            {
                GuardList guardList = guardLists[ index ];
                from.SendGump( new HireSelectedGuardGump( guardList, Town, Owner ) );
            }
        }

        /*
        private void CenterItem( int itemID, int x, int y, int w, int h )
        {
            Rectangle2D rc = ItemBounds.Table[ itemID ];
            AddItem( x + ( ( w - rc.Width ) / 2 ) - rc.X, y + ( ( h - rc.Height ) / 2 ) - rc.Y, itemID );
        }
        */
    }
}