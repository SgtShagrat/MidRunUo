using System;
using System.Collections.Generic;

using Server;
using Server.Gumps;
using Server.Network;

namespace Midgard.Engines.MidgardTownSystem
{
    public class TownSelectionGump : TownGump
    {
        public enum Buttons
        {
            Close,
        }

        #region design variables
        protected override int NumLabels
        {
            get { return TownSystem.TownSystems.Length + 1; }
        }

        protected override int NumButtons
        {
            get { return 1; }
        }

        protected override int MainWindowWidth
        {
            get { return 310; }
        }
        #endregion

        private List<TownSystem> m_Display;

        public TownSelectionGump( TownSystem system, Mobile owner )
            : base( system, owner, 50, 50 )
        {
            owner.CloseGump( typeof( TownSelectionGump ) );

            Design();

            base.RegisterUse( typeof( TownSelectionGump ) );
        }

        private void Design()
        {
            m_Display = BuildList();

            AddPage( 0 );

            AddMainBackground();
            AddMainTitle( 205, String.Format( "Town selection" ) );
            AddMainWindow();

            int labelOffsetX = GetMainWindowLabelsX();
            int labelOffsetY = GetMainWindowFirstLabelY();

            string title = "choose the town:";

            AddLabel( labelOffsetX, labelOffsetY, GroupsHue, title );

            for( int i = 0; i < m_Display.Count; i++ )
            {
                int y = labelOffsetY + LabelsOffset + ( LabelsOffset * i );
                AddLabel( labelOffsetX + 20, y, LabelsHue, m_Display[ i ].Definition.TownName );
                AddMainWindowButton( labelOffsetX, y + 4, i + 1, (int) TownAccessFlags.Citizen );
            }

            // buttons
            AddCloseButton();
        }

        public List<TownSystem> BuildList()
        {
            List<TownSystem> list = new List<TownSystem>();

            foreach( TownSystem system in TownSystem.TownSystems )
            {
                list.Add( system );
            }

            list.Sort( InternalComparer.Instance );

            return list;
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            int realId = info.ButtonID - 1;
            TownSystem sys = realId >= 0 ? TownSystem.TownSystems[ realId ] : null;
            if( sys == null )
                return;
        }

        private class InternalComparer : IComparer<TownSystem>
        {
            public static readonly IComparer<TownSystem> Instance = new InternalComparer();

            public int Compare( TownSystem x, TownSystem y )
            {
                if( x == null || y == null )
                    throw new ArgumentException();

                return Insensitive.Compare( x.Definition.TownName, y.Definition.TownName );
            }
        }
    }
}