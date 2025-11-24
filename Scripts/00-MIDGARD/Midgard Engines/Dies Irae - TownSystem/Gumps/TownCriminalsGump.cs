/***************************************************************************
 *                                  TownCriminalsGump.cs
 *                            		--------------------
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
using Server.Mobiles;
using Server.Network;

namespace Midgard.Engines.MidgardTownSystem
{
    public class TownCriminalsGump : TownGump
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

        public TownCriminalsGump( TownSystem system, Mobile owner )
            : this( system, owner, null )
        {
        }

        public TownCriminalsGump( TownSystem system, Mobile owner, IEnumerable<Mobile> list )
            : base( system, owner, 50, 50 )
        {
            owner.CloseGump( typeof( TownCriminalsGump ) );

            if( list == null )
                m_Mobiles = BuildList( system );
            else
                m_Mobiles = new List<Mobile>( list );

            Design( owner.Language );

            base.RegisterUse( typeof( TownCriminalsGump ) );
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

                if( tpsX.CitizenKills > tpsY.CitizenKills )
                    return -1;
                else if( tpsX.CitizenKills == tpsY.CitizenKills )
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
                if( tps != null && tps.Mobile != null && tps.CitizenKills > 0 )
                    list.Add( tps.Mobile );
            }

            list.Sort( InternalComparer.Instance );

            return list;
        }

        private void Design( string lang )
        {
            AddPage( 0 );

            AddMainBackground();
            AddMainTitle( 315, String.Format( lang == "ITA" ? "Lista Criminali di {0}" : "{0} Town Criminal List", Town.Definition.TownName ) );
            AddMainWindow();

            int labelOffsetX = GetMainWindowLabelsX();
            int labelOffsetY = GetMainWindowFirstLabelY();

            // Groups
            AddSubTitle( labelOffsetX, lang == "ITA" ? "Nome" : "Name" );
            AddSubTitle( labelOffsetX + 170, "Account" );
            AddSubTitle( labelOffsetX + 300, lang == "ITA" ? "Omicidi" : "Kills" );
            AddSubTitleImage( labelOffsetX + 410, 4033 );

            AddCloseButton();

            int hue = HuePrim;

            for( int i = 0; i < m_Mobiles.Count; i++ )
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

                AddLabel( labelOffsetX, y, hue, m.Name );
                AddLabel( labelOffsetX + 170, y, hue, FormatPrivateInfo( m.Account.Username ) );
                AddLabel( labelOffsetX + 300, y, hue, tps.CitizenKills.ToString() );

                AddMainWindowButton( labelOffsetX + 410, y + 3, i + 1, (int)TownAccessFlags.BanCitizen );
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

            bool shouldBan = !TownHelper.IsTownBanned( (Midgard2PlayerMobile)m, Town );

            TownHelper.DoTownBan( m, Town, shouldBan );

            from.SendMessage( from.Language == "ITA" ? "Il giocatore {0} ora{1} è esiliato da {2}" : "Player {0} is now {1} exiled from {2}.", m.Name, shouldBan ? "" : (from.Language == "ITA" ? " NON" : "NOT"), Town.Definition.TownName );
            from.SendGump( new TownCriminalsGump( Town, from ) );
        }
    }
}