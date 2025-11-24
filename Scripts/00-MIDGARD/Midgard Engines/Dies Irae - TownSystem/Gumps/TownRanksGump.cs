/***************************************************************************
 *                                  TownRanksGump.cs
 *                            		----------------
 *  begin                	: Maggio, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Network;

namespace Midgard.Engines.MidgardTownSystem
{
    public class TownRanksGump : TownGump
    {
        public enum Buttons
        {
            Close,
            Page = 10000
        }

        #region design variables
        protected override int NumLabels { get { return 20; } }
        protected override int NumButtons { get { return 0; } }
        protected override int MainWindowWidth { get { return 460; } }
        protected override bool HasSubtitles { get { return true; } }
        #endregion

        private readonly List<Mobile> m_Mobiles;

        public TownRanksGump( TownSystem system, Mobile owner )
            : this( system, owner, null )
        {
        }

        public TownRanksGump( TownSystem system, Mobile owner, IEnumerable<Mobile> list )
            : base( system, owner, 50, 50 )
        {
            owner.CloseGump( typeof( TownRanksGump ) );

            m_Mobiles = list == null ? BuildList( system ) : new List<Mobile>( list );

            Design();

            base.RegisterUse( typeof( TownRanksGump ) );
        }

        private class InternalComparer : IComparer<Mobile>
        {
            public static readonly IComparer<Mobile> Instance = new InternalComparer();

            public int Compare( Mobile x, Mobile y )
            {
                if( x == null || y == null )
                    return 0;

                if( x.Name == null || y.Name == null )
                    return 0;

                TownPlayerState tpsX = TownPlayerState.Find( x );
                TownPlayerState tpsY = TownPlayerState.Find( y );
                if( tpsX == null || tpsY == null )
                    return 0;

                if( tpsX.TownRankPoints > tpsY.TownRankPoints )
                    return -1;
                else if( tpsX.TownRankPoints == tpsY.TownRankPoints )
                    return Insensitive.Compare( x.Name, y.Name );
                else
                    return 1;
            }
        }

        private static List<Mobile> BuildList( TownSystem system )
        {
            List<Mobile> list = new List<Mobile>();

            foreach( TownPlayerState tps in system.Players )
            {
                if( tps != null && tps.Mobile != null && tps.TownRankPoints > 0 )
                    list.Add( tps.Mobile );
            }

            list.Sort( InternalComparer.Instance );

            return list;
        }

        private void Design()
        {
            AddPage( 0 );

            AddMainBackground();
            AddMainTitle( 310, String.Format( "{0} Town War Ranks List", Town.Definition.TownName ) );
            AddMainWindow();

            int labelOffsetX = GetMainWindowLabelsX();
            int labelOffsetY = GetMainWindowFirstLabelY();

            // Groups
            AddSubTitle( labelOffsetX, "Name" );
            AddSubTitle( labelOffsetX + 170, "Town" );
            AddSubTitle( labelOffsetX + 300, "Points" );
            AddSubTitleImage( labelOffsetX + 410, 4033 );

            AddCloseButton();

            int hue = HuePrim;

            for( int i = 0; i < m_Mobiles.Count; ++i )
            {
                int page = i / NumLabels;
                int pos = i % NumLabels;

                if( pos == 0 )
                {
                    if( page > 0 )
                        AddButton( 460, 10, 0x15E1, 0x15E5, (int)Buttons.Page, GumpButtonType.Page, page + 1 ); // Next

                    AddPage( page + 1 );

                    if( page > 0 )
                        AddButton( 440, 10, 0x15E3, 0x15E7, (int)Buttons.Page, GumpButtonType.Page, page ); // Back
                }

                int y = pos * LabelsOffset + labelOffsetY;

                Mobile m = m_Mobiles[ i ];
                if( m == null || m.Deleted )
                    continue;

                TownPlayerState tps = TownPlayerState.Find( m );
                if( tps == null || tps.Mobile == null )
                    continue;

                hue = GetHueFor( m, hue );

                AddLabel( labelOffsetX, y, hue, m.Name );
                AddLabel( labelOffsetX + 170, y, hue, tps.TownSystem.Definition.TownName );
                AddLabel( labelOffsetX + 300, y, hue, tps.TownRankPoints.ToString() );

                AddMainWindowButton( labelOffsetX + 410, y + 3, i + 1, (int) TownAccessFlags.Citizen );
            }
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            Mobile from = sender.Mobile;

            if( info.ButtonID == (int)Buttons.Close )
            {
                from.SendGump( new TownSystemInfoGump( Town, from ) );
                return;
            }

            if( info.ButtonID < 0 || info.ButtonID > m_Mobiles.Count )
                return;

            Mobile m = m_Mobiles[ info.ButtonID - 1 ];
            if( m == null || m.Deleted )
                return;

            from.SendMessage( "Advanced Infoes not impemented yet!" );
            from.SendGump( new TownRanksGump( Town, from ) );
        }
    }
}