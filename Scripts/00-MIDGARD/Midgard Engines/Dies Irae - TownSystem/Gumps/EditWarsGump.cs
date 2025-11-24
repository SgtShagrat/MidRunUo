/***************************************************************************
 *                                  WarfareGump.cs
 *                            		-------------------
 *  begin                	: Marzo, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasiaalice.it
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;

using Server;
using Server.Gumps;
using Server.Network;

namespace Midgard.Engines.MidgardTownSystem
{
    public class EditWarsGump : TownGump
    {
        public enum Buttons
        {
            Close,
        }

        public enum GumpType
        {
            AddWar,
            AddAlliance,
            RemoveWar,
            RemoveAlliance
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

        private readonly GumpType m_Type;
        private List<TownSystem> m_Display;

        public EditWarsGump( TownSystem system, Mobile owner, GumpType gumpType )
            : base( system, owner, 50, 50 )
        {
            owner.CloseGump( typeof( EditWarsGump ) );

            m_Type = gumpType;

            Design();

            base.RegisterUse( typeof( EditWarsGump ) );
        }

        private void Design()
        {
            m_Display = BuildList();

            AddPage( 0 );

            AddMainBackground();
            AddMainTitle( 205, String.Format( "War editor of {0}", Town.Definition.TownName ) );
            AddMainWindow();

            int labelOffsetX = GetMainWindowLabelsX();
            int labelOffsetY = GetMainWindowFirstLabelY();

            string title = "";
            switch( m_Type )
            {
                case GumpType.AddWar:
                    title = "chose the town to war with:";
                    break;
                case GumpType.AddAlliance:
                    title = "chose the town you'd to be allied with:";
                    break;
                case GumpType.RemoveWar:
                    title = "chose the town to stop war with:";
                    break;
                case GumpType.RemoveAlliance:
                    title = "chose the town you'd not to be allied with:";
                    break;
            }

            AddLabel( labelOffsetX, labelOffsetY, GroupsHue, title );

            if( m_Display.Count > 0 )
            {
                for( int i = 0; i < TownSystem.TownSystems.Length; i++ )
                {
                    TownSystem sys = TownSystem.TownSystems[ i ];
                    if( sys == null )
                        continue;

                    int y = labelOffsetY + LabelsOffset + ( LabelsOffset * i );
                    bool enabled = m_Display.Contains( sys );
                    int hue = enabled ? LabelsHue : DisabledHue;

                    // the town friendly name
                    AddLabel( labelOffsetX + 20, y, hue, sys.Definition.TownName );

                    if( enabled )
                        AddMainWindowButton( labelOffsetX, y + 4, i + 1, (int) TownAccessFlags.CanEditWarFare );
                }
            }
            else
            {
                AddLabel( labelOffsetX, labelOffsetY + LabelsOffset, LabelsHue, "- none -" );
            }

            // buttons
            AddCloseButton();
        }

        public List<TownSystem> BuildList()
        {
            List<TownSystem> list = new List<TownSystem>();

            switch( m_Type )
            {
                case GumpType.AddWar:
                case GumpType.AddAlliance:
                    foreach( TownSystem system in TownSystem.TownSystems )
                    {
                        if( system != Town && Town.IsNeutralTo( system ) )
                            list.Add( system );
                    }
                    break;
                case GumpType.RemoveWar:
                    foreach( TownSystem system in TownSystem.TownSystems )
                    {
                        if( system != Town && Town.IsEnemyTo( system ) )
                            list.Add( system );
                    }
                    break;
                case GumpType.RemoveAlliance:
                    foreach( TownSystem system in TownSystem.TownSystems )
                    {
                        if( system != Town && Town.IsAlliedTo( system ) )
                            list.Add( system );
                    }
                    break;
            }

            list.Sort( InternalComparer.Instance );

            return list;
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            Mobile from = sender.Mobile;

            int realId = info.ButtonID - 1;
            TownSystem sys = realId >= 0 ? TownSystem.TownSystems[ realId ] : null;
            if( sys == null )
                return;

            switch( m_Type )
            {
                case GumpType.AddAlliance:
                    Town.RegisterAlly( sys );
                    break;
                case GumpType.AddWar:
                    Town.RegisterEnemy( sys );
                    break;
                case GumpType.RemoveAlliance:
                    Town.RemoveAlly( sys );
                    break;
                case GumpType.RemoveWar:
                    Town.RemoveEnemy( sys );
                    break;
            }

            from.SendGump( new TownSystemInfoGump( Town, Owner ) );
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