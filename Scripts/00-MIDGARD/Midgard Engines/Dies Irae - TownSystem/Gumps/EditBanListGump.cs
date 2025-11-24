/***************************************************************************
 *                                  EditBanListGump.cs
 *                            		------------------
 *  begin                	: Aprile, 2008
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
    public class EditBanListGump : TownGump
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

        public EditBanListGump( TownSystem system, Mobile owner )
            : this( system, owner, null )
        {
        }

        public EditBanListGump( TownSystem system, Mobile owner, IEnumerable<Mobile> list )
            : base( system, owner, 50, 50 )
        {
            owner.CloseGump( typeof( EditBanListGump ) );

            m_Mobiles = list == null ? BuildList( system ) : new List<Mobile>( list );

            Design( owner.Language );

            base.RegisterUse( typeof( EditBanListGump ) );
        }

        private class InternalComparer : IComparer<Mobile>
        {
            public static readonly IComparer<Mobile> Instance = new InternalComparer();

            private TownSystem m_Town;

            public TownSystem Town
            {
                set { m_Town = value; }
            }

            public int Compare( Mobile x, Mobile y )
            {
                if( x == null || y == null )
                    throw new ArgumentException();

                if( m_Town != null && m_Town.ExiledPlayers != null )
                {
                    if( m_Town.ExiledPlayers.Contains( x ) && !m_Town.ExiledPlayers.Contains( y ) )
                        return -1;
                    else if( m_Town.ExiledPlayers.Contains( y ) && !m_Town.ExiledPlayers.Contains( x ) )
                        return 1;
                }

                return Insensitive.Compare( x.Name, y.Name );
            }
        }

        private static List<Mobile> BuildList( TownSystem system )
        {
            List<Mobile> list = new List<Mobile>();

            try
            {
                List<Mobile> mobs = new List<Mobile>( World.Mobiles.Values );

                foreach( Mobile m in mobs )
                {
                    if( m != null && m is Midgard2PlayerMobile )
                    {
                        list.Add( m );
                    }
                }

                InternalComparer comparer = InternalComparer.Instance as InternalComparer;
                if( comparer != null )
                    comparer.Town = system;

                list.Sort( comparer );
            }
            catch( Exception ex )
            {
                Console.WriteLine( ex );
            }

            return list;
        }

        private void Design( string lang )
        {
            AddPage( 0 );

            AddMainBackground();
            AddMainTitle( 315, String.Format( lang == "ITA" ? "Lista Esiliati da {0}" : "{0} Citizens Ban List", Town.Definition.TownName ) );
            AddMainWindow();

            int labelOffsetX = GetMainWindowLabelsX();
            int labelOffsetY = GetMainWindowFirstLabelY();

            // Groups
            AddSubTitle( labelOffsetX, lang == "ITA" ? "Nome" : "Name" );
            AddSubTitle( labelOffsetX + 150, "Account" );
            AddSubTitle( labelOffsetX + 280, lang == "ITA" ? "Città" : "Town" );
            AddSubTitleImage( labelOffsetX + 410, 4033 );

            int hue = HuePrim;

            for( int i = 0; i < m_Mobiles.Count; ++i )
            {
                Mobile m = m_Mobiles[ i ];
                if( m == null || m.Deleted )
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

                TownPlayerState tps = TownPlayerState.Find( m );

                AddLabel( labelOffsetX, y, hue, m.Name );
                AddLabel( labelOffsetX + 150, y, hue, HasAccess( (int) TownAccessFlags.BanCitizen ) ? FormatPrivateInfo( m.Account.Username ) : "--private info--" );

                if( tps != null )
                    AddLabel( labelOffsetX + 280, y, hue, tps.TownSystem.Definition.TownName );

                AddMainWindowButton( labelOffsetX + 410, y + 3, i + 1, (int) TownAccessFlags.BanCitizen );
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

            bool shouldBan = !TownHelper.IsTownBanned( (Midgard2PlayerMobile)m, Town );

            TownHelper.DoTownBan( m, Town, shouldBan );

            from.SendMessage( from.Language == "ITA" ? "Il giocatore {0} ora{1} è esiliato da {2}." : "Player {0} is now {1} exiled from {2}.", m.Name,
                shouldBan ? "" : (from.Language == "ITA" ? " NON" : "NOT"), Town.Definition.TownName );
        }

        public override int GetHueFor( Mobile m, int hue )
        {
            if( Town.ExiledPlayers.Contains( m ) )
                return 0x90;

            return base.GetHueFor( m, hue );
        }
    }
}