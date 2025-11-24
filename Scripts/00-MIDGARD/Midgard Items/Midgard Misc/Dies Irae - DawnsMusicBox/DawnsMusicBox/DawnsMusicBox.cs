/***************************************************************************
 *                                   DawnsMusicBox.cs
 *                            		------------------
 *  begin                	: August, 2007
 *  version					: 2.0 **VERSION FOR RUNUO 2.0**
 *  copyright            	: Matteo Visintin
 *  email                	: tocasia@alice.it
 *  msn						: Matteo_Visintin@hotmail.com
 ***************************************************************************/

/***************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 2 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;

using Server;
using Server.ContextMenus;
using Server.Engines.VeteranRewards;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Multis;
using Server.Network;

namespace Midgard.Items.MusicBox
{
    [Flipable( 0x2AF9, 0x2AFD )]
    public class DawnsMusicBox : Item, ISecurable, IRewardItem
    {
        private List<MusicName> m_Tracks;
        private Timer m_PlayingTimer;
        private MusicName m_ActualSong;

        public static readonly int MusicRange = 10;

        public override int LabelNumber { get { return 1075198; } } // Dawn’s Music Box

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public bool IsPlaying
        {
            get { return m_PlayingTimer != null; }
        }

        public List<MusicName> Tracks
        {
            get { return m_Tracks; }
        }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public MusicName ActualSong
        {
            get { return m_ActualSong; }
            set { m_ActualSong = value; InvalidateProperties(); }
        }

        [Constructable]
        public DawnsMusicBox()
            : base( 0x2AF9 )
        {
            Weight = 1.0;

            m_Tracks = new List<MusicName>();
            m_ActualSong = MusicName.Invalid;

            while( Tracks.Count < 4 )
                AddSong( TrackInfo.RandomSong( TrackRarity.Common ) );
        }

        public DawnsMusicBox( Serial serial )
            : base( serial )
        {
        }

        #region ISecurable members
        private SecureLevel m_Level;

        [CommandProperty( AccessLevel.GameMaster )]
        public SecureLevel Level
        {
            get { return m_Level; }
            set { m_Level = value; }
        }
        #endregion

        #region IRewardItem members
        private bool m_IsRewardItem;

        [CommandProperty( AccessLevel.GameMaster )]
        public bool IsRewardItem
        {
            get { return m_IsRewardItem; }
            set { m_IsRewardItem = value; }
        }
        #endregion

        public override void GetProperties( ObjectPropertyList list )
        {
            base.GetProperties( list );

            int commonSongs = 0;
            int unCommonSongs = 0;
            int rareSongs = 0;

            for( int i = 0; i < m_Tracks.Count; i++ )
            {
                TrackInfo ti = TrackInfo.GetInfo( m_Tracks[ i ] );
                switch( ti.Rarity )
                {
                    case TrackRarity.Common: commonSongs++; break;
                    case TrackRarity.UnCommon: unCommonSongs++; break;
                    case TrackRarity.Rare: rareSongs++; break;
                }
            }

            if( commonSongs > 0 )
                list.Add( 1075234, commonSongs.ToString() ); // ~1_NUMBER~ Common Tracks
            if( unCommonSongs > 0 )
                list.Add( 1075235, unCommonSongs.ToString() ); // ~1_NUMBER~ Uncommon Tracks
            if( rareSongs > 0 )
                list.Add( 1075236, rareSongs.ToString() ); // ~1_NUMBER~ Rare Tracks
        }

        public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
        {
            base.GetContextMenuEntries( from, list );

            SetSecureLevelEntry.AddTo( from, this, list ); // Set secure level
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( m_Tracks.Count < 1 )
            {
                from.SendMessage( "This music box is empty." );
            }
            else if( IsOwner( from ) )
            {
                if( !IsLockedDown && from.AccessLevel == AccessLevel.Player )
                    from.SendLocalizedMessage( 502692 ); // This must be in a house and be locked down to work.
                else
                {
                    if( from.HasGump( typeof( MusicGump ) ) )
                        from.CloseGump( typeof( MusicGump ) );

                    from.SendGump( new MusicGump( this ) );
                }
            }
            else
            {
                from.SendLocalizedMessage( 502691 ); // You must be the owner to use this.
            }
        }

        public bool AddSong( MusicName song )
        {
            if( m_Tracks.Contains( song ) )
            {
                return false;
            }
            else
            {
                m_Tracks.Add( song );
                return true;
            }
        }

        public void Animate()
        {
            switch( ItemID )
            {
                case 0x2AF9: ItemID = 0x2AFA; break;
                case 0x2AFA: ItemID = 0x2AFB; break;
                case 0x2AFB: ItemID = 0x2AFC; break;
                case 0x2AFC: ItemID = 0x2AF9; break;

                case 0x2AFD: ItemID = 0x2AFE; break;
                case 0x2AFE: ItemID = 0x2AFF; break;
                case 0x2AFF: ItemID = 0x2B00; break;
                case 0x2B00: ItemID = 0x2AFD; break;
            }
        }

        public bool IsOwner( Mobile mob )
        {
            if( mob.AccessLevel >= AccessLevel.GameMaster )
                return true;

            BaseHouse house = BaseHouse.FindHouseAt( this );

            return ( house != null && house.IsOwner( mob ) );
        }

        public void ToggleMusic( Mobile m, bool play )
        {
            if( m_ActualSong != MusicName.Invalid && m.NetState != null )
            {
                m.Send( new StopMusic() ); // Stop actual music

                if( play )
                    m.Send( PlayMusic.GetInstance( m_ActualSong ) );
            }
        }

        public void TogglePlaying( bool hasToStart )
        {
            ToggleTimer( hasToStart );

            string message = hasToStart ? "* The music box starts playing a song *" : "* The music box stops *";

            PublicOverheadMessage( MessageType.Regular, 0x5D, true, message );
            StopBoxesInRange();
            Map boxMap = Map;

            if( boxMap != Map.Internal )
            {
                Point3D boxLoc = Location;
                IPooledEnumerable mobsEable = boxMap.GetMobilesInRange( boxLoc, MusicRange );

                foreach( Mobile m in mobsEable )
                {
                    if( m is PlayerMobile )
                        ToggleMusic( m, hasToStart );
                }

                mobsEable.Free();
            }
        }

        public void ToggleTimer( bool hasToStart )
        {
            if( IsPlaying && !hasToStart )
            {
                if( m_PlayingTimer != null && m_PlayingTimer.Running )	// remove correctly the timer...
                    m_PlayingTimer.Stop();
                m_PlayingTimer = null;
            }
            else if( !IsPlaying && hasToStart )
            {
                TrackInfo ti = TrackInfo.GetInfo( m_ActualSong );

                m_PlayingTimer = new PlayingTimer( (double)ti.Duration, this );	// add a new timer
                m_PlayingTimer.Start();
            }
        }

        public void StopBoxesInRange()
        {
            Map boxMap = Map;

            if( boxMap != Map.Internal )
            {
                Point3D boxLoc = Location;
                IPooledEnumerable itemsEable = boxMap.GetItemsInRange( boxLoc, MusicRange );

                foreach( Item i in itemsEable )
                {
                    if( i is DawnsMusicBox && i != this )
                    {
                        DawnsMusicBox mb = (DawnsMusicBox)i;
                        if( mb.IsPlaying )
                        {
                            mb.ToggleTimer( false );
                            mb.PublicOverheadMessage( MessageType.Regular, 0x3B2, true, "* The music box stops *" );
                        }
                    }
                }

                itemsEable.Free();
            }
        }

        #region PlayingTimer
        internal class PlayingTimer : Timer
        {
            private readonly DawnsMusicBox m_Box;
            private readonly DateTime m_Until;

            public PlayingTimer( double duration, DawnsMusicBox box )
                : base( TimeSpan.FromSeconds( 1.0 ), TimeSpan.FromSeconds( 1.0 ) )
            {
                m_Box = box;
                m_Until = DateTime.Now + TimeSpan.FromSeconds( duration );

                Priority = TimerPriority.TwoFiftyMS;
            }

            protected override void OnTick()
            {
                if( DateTime.Now > m_Until )
                {
                    if( m_Box != null && !m_Box.Deleted )
                        m_Box.TogglePlaying( false );
                    else
                        Stop();
                }
                else if( m_Box != null && !m_Box.Deleted )
                    m_Box.Animate();
            }
        }
        #endregion

        #region MusicGump
        private class MusicGump : Gump
        {
            private static readonly int m_Fields = 9;
            private static readonly int m_HueTit = 32767;
            private static readonly int m_HueEnt = 32767;
            private static readonly int m_DeltaBut = 2;
            private static readonly int m_FieldsDist = 25;

            private DawnsMusicBox m_Box;
            private List<int> m_Songs;
            private int m_Page;
            private bool m_HasStopSongEntry;

            public MusicGump( DawnsMusicBox box )
                : this( box, null, 1 )
            {
            }

            private MusicGump( DawnsMusicBox box, List<int> songs, int page )
                : base( 50, 50 )
            {
                Closable = false;
                Disposable = true;
                Dragable = true;
                Resizable = false;

                m_Box = box;
                m_Songs = songs;
                m_Page = page;

                m_HasStopSongEntry = m_Box.IsPlaying;

                if( m_Songs == null )
                    m_Songs = BuildList( box, m_HasStopSongEntry );

                Design();
            }

            private static List<int> BuildList( DawnsMusicBox box, bool hasStopSongEntry )
            {
                List<int> list = new List<int>();

                for( int i = 0; i < box.Tracks.Count; i++ )
                {
                    TrackInfo ti = TrackInfo.GetInfo( box.Tracks[ i ] );
                    list.Add( ti.Label );
                }

                if( hasStopSongEntry )
                    list.Add( 1075207 ); // Stop Song

                return list;
            }

            private void Design()
            {
                AddPage( 0 );

                AddBackground( 0, 0, 275, 325, 9200 );

                AddImageTiled( 10, 10, 255, 25, 2624 );
                AddImageTiled( 10, 45, 255, 240, 2624 );
                AddImageTiled( 40, 295, 225, 20, 2624 );

                AddButton( 10, 295, 4017, 4018, 0, GumpButtonType.Reply, 0 );
                AddHtmlLocalized( 45, 295, 75, 20, 1011012, m_HueTit, false, false ); // CANCEL

                AddAlphaRegion( 10, 10, 255, 285 );
                AddAlphaRegion( 40, 295, 225, 20 );

                AddHtmlLocalized( 14, 12, 255, 25, 1075130, m_HueTit, false, false ); // Choose a track to play

                if( m_Page > 1 )
                    AddButton( 225, 297, 5603, 5607, 200, GumpButtonType.Reply, 0 ); // Previous page

                if( m_Page < Math.Ceiling( m_Songs.Count / (double)m_Fields ) )
                    AddButton( 245, 297, 5601, 5605, 300, GumpButtonType.Reply, 0 ); // Next Page

                int indMax = ( m_Page * m_Fields ) - 1;
                int indMin = ( m_Page * m_Fields ) - m_Fields;
                int indTemp = 0;

                for( int i = 0; i < m_Songs.Count; i++ )
                {
                    if( i >= indMin && i <= indMax )
                    {
                        AddHtmlLocalized( 35, 52 + ( indTemp * m_FieldsDist ), 225, 20, m_Songs[ i ], m_HueEnt, false, false );
                        AddButton( 15, 52 + m_DeltaBut + ( indTemp * m_FieldsDist ), 1209, 1210, i + 1, GumpButtonType.Reply, 0 );
                        indTemp++;
                    }
                }
            }

            public override void OnResponse( NetState sender, RelayInfo info )
            {
                Mobile from = sender.Mobile;

                if( info.ButtonID == 0 )
                    return;
                else if( info.ButtonID == 200 ) // Previous page
                {
                    m_Page--;
                    from.SendGump( new MusicGump( m_Box, m_Songs, m_Page ) );
                }
                else if( info.ButtonID == 300 )  // Next Page
                {
                    m_Page++;
                    from.SendGump( new MusicGump( m_Box, m_Songs, m_Page ) );
                }
                else if( m_HasStopSongEntry && info.ButtonID == m_Songs.Count )
                {
                    m_Box.TogglePlaying( false );
                }
                else
                {
                    TrackInfo ti = TrackInfo.GetInfo( m_Songs[ info.ButtonID - 1 ] );
                    m_Box.ActualSong = ti.Name;
                    m_Box.TogglePlaying( true );
                }
            }
        }
        #endregion

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version

            writer.Write( m_Tracks.Count );

            for( int i = 0; i < m_Tracks.Count; i++ )
                writer.Write( (int)m_Tracks[ i ] );

            writer.Write( (int)m_Level );
            writer.Write( m_IsRewardItem );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            switch( version )
            {
                case 0:
                    {
                        if( m_Tracks == null )
                            m_Tracks = new List<MusicName>();

                        int numSongs = reader.ReadInt();
                        for( int i = 0; i < numSongs; i++ )
                            m_Tracks.Add( (MusicName)reader.ReadInt() );

                        m_Level = (SecureLevel)reader.ReadInt();
                        m_IsRewardItem = reader.ReadBool();

                        ToggleTimer( false );

                        break;
                    }
            }
        }
        #endregion
    }
}