/***************************************************************************
 *                                  EditTownAccess.cs
 *                            		------------------
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
    public class EditTownAccessGump : TownGump
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

        public EditTownAccessGump( TownSystem system, Mobile owner )
            : this( system, owner, null )
        {
        }

        public EditTownAccessGump( TownSystem system, Mobile owner, IEnumerable<Mobile> list )
            : base( system, owner, 50, 50 )
        {
            owner.CloseGump( typeof( EditTownAccessGump ) );

            if( list == null )
                m_Mobiles = BuildList( system );
            else
                m_Mobiles = new List<Mobile>( list );

            Design();

            base.RegisterUse( typeof( EditTownAccessGump ) );
        }

        private class InternalComparer : IComparer<Mobile>
        {
            public static readonly IComparer<Mobile> Instance = new InternalComparer();

            public int Compare( Mobile x, Mobile y )
            {
                if( x == null || y == null )
                    return 0;

                return Insensitive.Compare( x.Name, y.Name );
            }
        }

        private static List<Mobile> BuildList( TownSystem system )
        {
            List<Mobile> list = new List<Mobile>();

            try
            {
                foreach( TownPlayerState m in system.Players )
                {
                    if( m.Mobile != null )
                        list.Add( m.Mobile );
                }

                list.Sort( InternalComparer.Instance );
            }
            catch( Exception ex )
            {
                Console.WriteLine( ex );
            }

            return list;
        }

        private void Design()
        {
            AddPage( 0 );

            AddMainBackground();
            AddMainTitle( 315, String.Format( "Town Access Levels for {0}", Town.Definition.TownName ) );
            AddMainWindow();

            int labelOffsetX = GetMainWindowLabelsX();
            int labelOffsetY = GetMainWindowFirstLabelY();

            // Groups
            AddSubTitle( labelOffsetX, "Name" );
            AddSubTitle( labelOffsetX + 150, "Account" );
            AddSubTitle( labelOffsetX + 280, "Town Access Level" );
            AddSubTitleImage( labelOffsetX + 410, 4033 );

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
                Mobile m = m_Mobiles[ i ];

                hue = GetHueFor( m, hue );

                if( m != null && !m.Deleted )
                {
                    TownPlayerState tps = TownPlayerState.Find( m );
                    if( tps == null || tps.Mobile == null )
                        continue;

                    int y = pos * LabelsOffset + labelOffsetY;

                    AddLabel( labelOffsetX, y, hue, m.Name );
                    AddLabel( labelOffsetX + 150, y, hue, FormatPrivateInfo( m.Account.Username ) );
                    AddLabel( labelOffsetX + 280, y, hue, FormatAccessLevel( tps ) );
                    AddButton( labelOffsetX + 410, y + 3, 0x837, 0x837, i + 1, GumpButtonType.Reply, 0 );
                }
            }

            AddCloseButton();
        }

        private static string FormatAccessLevel( TownPlayerState state )
        {
            if( state == null || state.Mobile == null )
                return String.Empty;

            TownAccessLevel level = state.TownLevel;
            if( level == TownAccessLevel.Citizen || level == TownAccessLevel.Staff )
                return MidgardUtility.GetFriendlyClassName( level.ToString() );
            else
                return String.Empty;
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

            from.SendGump( new TownAccessInfoGump( Town, m, Owner ) );
        }
    }
}