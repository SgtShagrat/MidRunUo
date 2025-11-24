/***************************************************************************
 *                                  CitizenManagementGump.cs
 *                            		------------------------
 *  begin                	: Marzo, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Midgard.Engines.MidgardTownSystem
{
    public class CitizenManagementGump : TownGump
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

        public CitizenManagementGump( TownSystem system, Mobile owner )
            : this( system, owner, null )
        {
        }

        public CitizenManagementGump( TownSystem system, Mobile owner, IEnumerable<Mobile> list )
            : base( system, owner, 50, 50 )
        {
            owner.CloseGump( typeof( CitizenManagementGump ) );

            if( list == null )
                m_Mobiles = BuildList( system, false );
            else
                m_Mobiles = new List<Mobile>( list );

            Design();

            base.RegisterUse( typeof( CitizenManagementGump ) );
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

        private static List<Mobile> BuildList( TownSystem system, bool online )
        {
            List<Mobile> list = new List<Mobile>();

            foreach( TownPlayerState t in system.Players )
            {
                Mobile m = t.Mobile;
                if( m != null && !m.Deleted )
                {
                    if( list.Contains( m ) )
                        Console.WriteLine( "Warning: duplicate player {0} in townsystem {1}", m.Name, system.Definition.TownName );

                    if( !online || ( m.NetState != null ) )
                        list.Add( m );
                }
            }

            list.Sort( InternalComparer.Instance );

            return list;
        }

        private void Design()
        {
            AddPage( 0 );

            AddMainBackground();
            AddMainTitle( 315, String.Format( "{0} Citizens List", Town.Definition.TownName ) );
            AddMainWindow();

            int labelOffsetX = GetMainWindowLabelsX();
            int labelOffsetY = GetMainWindowFirstLabelY();

            // Groups
            AddSubTitle( labelOffsetX, "Name" );
            AddSubTitle( labelOffsetX + 150, "Office" );
            AddSubTitle( labelOffsetX + 280, "Profession" );
            AddSubTitleImage( labelOffsetX + 410, 4033 );

            int hue = HuePrim;

            for( int i = 0; i < m_Mobiles.Count; ++i )
            {
                Mobile m = m_Mobiles[ i ];
                if( m == null || m.Deleted )
                    continue;

                TownPlayerState tps = TownPlayerState.Find( m );
                if( tps == null || tps.Mobile == null )
                    continue;

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

                hue = GetHueFor( m, hue );

                AddLabelCropped( labelOffsetX, y, 150, 21, hue, m.Name );

                if( !String.IsNullOrEmpty( tps.CustomTownOffice ) )
                    AddLabelCropped( labelOffsetX + 150, y, 150, 21, hue, tps.CustomTownOffice );
                else if( tps.TownOffice != TownOffices.None )
                    AddLabelCropped( labelOffsetX + 150, y, 150, 21, hue, MidgardUtility.GetFriendlyClassName( tps.TownOffice.ToString() ) );

                if( !String.IsNullOrEmpty( tps.CustomProfession ) )
                    AddLabelCropped( labelOffsetX + 280, y, 150, 21, hue, tps.CustomProfession );
                else if( tps.TownProfession != Professions.None )
                    AddLabelCropped( labelOffsetX + 280, y, 150, 21, hue, MidgardUtility.GetFriendlyClassName( tps.TownProfession.ToString() ) );

                AddMainWindowButton( labelOffsetX + 410, y + 3, i + 1, (int)TownAccessFlags.Citizen );
            }

            AddCloseButton();
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

            from.SendGump( new CitizenInfoGump( Town, m, from ) );
        }
    }
}