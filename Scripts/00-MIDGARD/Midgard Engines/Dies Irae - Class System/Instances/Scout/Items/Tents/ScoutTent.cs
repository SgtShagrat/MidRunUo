/***************************************************************************
 *                               ScoutTent.cs
 *
 *   begin                : 08 October, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;

using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Multis;
using Server.Multis.Deeds;
using Server.Items;
using Server.Network;

namespace Midgard.Items
{
    public class ScoutTent : BaseHouse
    {
        private Timer m_Timer;
        private DateTime m_Created;
        private List<ScoutTentEntry> m_Entries;

        private double GetTentDuration()
        {
            if( Owner != null )
                return 1.0;

            return 1.0;
        }

        public static Rectangle2D[] AreaArray = new Rectangle2D[] { new Rectangle2D( -3, -3, 8, 8 ) };

        public override Rectangle2D[] Area
        {
            get { return AreaArray; }
        }

        public override int DefaultPrice
        {
            get { return 0; }
        }

        public override Point3D BaseBanLocation
        {
            get { return new Point3D( 1, 4, 0 ); }
        }

        public ScoutTent( Mobile owner, int id )
            : base( id, owner, 1, 0 )
        {
            SetSign( -1, 5, 9 );
            ChangeSignType( 0x0bd1 );
            Sign.Visible = false;

            m_Entries = new List<ScoutTentEntry>();

            m_Created = DateTime.Now;
            m_Timer = Timer.DelayCall( TimeSpan.FromSeconds( 1.0 ), TimeSpan.FromSeconds( 1.0 ), new TimerCallback( OnTick ) );
        }

        public override bool IsInside( Point3D p, int height )
        {
            if( Deleted )
                return false;

            foreach( Rectangle2D rect in Area )
            {
                if( rect.Contains( new Point2D( p.X - X, p.Y - Y ) ) )
                    return true;
            }

            return false;
        }

        public override HouseDeed GetDeed()
        {
            switch( ItemID ^ 0x4000 )
            {
                case 0x70:
                    return new ScoutBlueTentRoll();
                case 0x72:
                    return new ScoutGreenTentRoll();
                default:
                    return new ScoutBlueTentRoll();
            }
        }

        public override void OnAfterDelete()
        {
            if( m_Timer != null )
                m_Timer.Stop();

            HouseDeed deed = GetDeed();
            if( deed != null )
            {
                if( Owner != null && Owner.Backpack != null && Owner.InRange( BanLocation, 10 ) && Owner.Backpack.TryDropItem( Owner, deed, false ) )
                    deed.MoveToWorld( BanLocation );
            }

            if( !Sign.Deleted )
                Sign.Delete();

            ClearEntries();
        }

        private static readonly Dictionary<Mobile, ScoutTentEntry> m_Dictionary = new Dictionary<Mobile, ScoutTentEntry>();

        public static ScoutTentEntry GetEntry( Mobile player )
        {
            ScoutTentEntry entry;
            m_Dictionary.TryGetValue( player, out entry );
            return entry;
        }

        private static void RemoveEntry( ScoutTentEntry entry )
        {
            if( entry != null && entry.Player != null && m_Dictionary.ContainsKey( entry.Player ) )
                m_Dictionary.Remove( entry.Player );

            if( entry != null && entry.Tent != null )
                entry.Tent.m_Entries.Remove( entry );
        }

        private void OnTick()
        {
            if( DateTime.Now - m_Created >= TimeSpan.FromMinutes( GetTentDuration() ) )
            {
                Delete();
                return;
            }

            if( m_Entries != null )
            {
                for( int i = 0; i < m_Entries.Count; i++ )
                {
                    ScoutTentEntry entry = m_Entries[ i ];

                    if( !entry.Valid || entry.Player.NetState == null )
                        RemoveEntry( entry );
                    else if( !entry.Safe && DateTime.Now - entry.Start >= TimeSpan.FromSeconds( 30.0 ) )
                    {
                        entry.Safe = true;
                        entry.Player.SendLocalizedMessage( 500621 ); // The camp is now secure.
                    }
                }
            }

            List<Mobile> eable = GetMobiles();

            foreach( Mobile m in eable )
            {
                PlayerMobile pm = m as PlayerMobile;

                if( pm != null && GetEntry( pm ) == null )
                {
                    ScoutTentEntry entry = new ScoutTentEntry( pm, this );

                    m_Dictionary[ pm ] = entry;

                    if( m_Entries == null )
                        m_Entries = new List<ScoutTentEntry>();

                    m_Entries.Add( entry );

                    pm.SendMessage( "You feel it would take a few moments to be secure in this tent." );
                }
            }
        }

        private void ClearEntries()
        {
            if( m_Entries == null )
                return;

            for( int i = 0; i < m_Entries.Count; i++ )
            {
                ScoutTentEntry entry = m_Entries[ i ];
                RemoveEntry( entry );
            }
        }

        public static void SendLogoutGump( ScoutTentEntry entry, Bedroll bedroll, Mobile m )
        {
            m.SendGump( new InternalGump( entry, bedroll ) );
        }

        #region serialization
        public ScoutTent( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); //version

            writer.Write( m_Created );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            m_Created = reader.ReadDateTime();

            m_Entries = new List<ScoutTentEntry>();
            m_Timer = Timer.DelayCall( TimeSpan.FromSeconds( 1.0 ), TimeSpan.FromSeconds( 1.0 ), new TimerCallback( OnTick ) );
        }
        #endregion

        public class ScoutTentEntry
        {
            private bool m_Safe;

            public PlayerMobile Player { get; private set; }
            public ScoutTent Tent { get; private set; }
            public DateTime Start { get; private set; }

            public bool Valid
            {
                get { return !Tent.Deleted && Player.Map == Tent.Map && Tent.IsInside( Player ); }
            }

            public bool Safe
            {
                get { return Valid && m_Safe; }
                set { m_Safe = value; }
            }

            public ScoutTentEntry( PlayerMobile player, ScoutTent tent )
            {
                Player = player;
                Tent = tent;
                Start = DateTime.Now;

                m_Safe = false;
            }
        }

        private class InternalGump : Gump
        {
            private Timer m_CloseTimer;
            private ScoutTentEntry m_Entry;
            private Bedroll m_Bedroll;

            public InternalGump( ScoutTentEntry entry, Bedroll bedroll )
                : base( 100, 0 )
            {
                m_Entry = entry;
                m_Bedroll = bedroll;

                m_CloseTimer = Timer.DelayCall( TimeSpan.FromSeconds( 10.0 ), new TimerCallback( CloseGump ) );

                AddBackground( 0, 0, 400, 350, 0xA28 );

                AddHtmlLocalized( 100, 20, 200, 35, 1011015, false, false ); // <center>Logging out via camping</center>

                /* Using a bedroll in the safety of a camp will log you out of the game safely.
                             * If this is what you wish to do choose CONTINUE and you will be logged out.
                             * Otherwise, select the CANCEL button to avoid logging out at this time.
                             * The camp will remain secure for 10 seconds at which time this window will close
                             * and you not be logged out.
                             */
                AddHtmlLocalized( 50, 55, 300, 140, 1011016, true, true );

                AddButton( 45, 298, 0xFA5, 0xFA7, 1, GumpButtonType.Reply, 0 );
                AddHtmlLocalized( 80, 300, 110, 35, 1011011, false, false ); // CONTINUE

                AddButton( 200, 298, 0xFA5, 0xFA7, 0, GumpButtonType.Reply, 0 );
                AddHtmlLocalized( 235, 300, 110, 35, 1011012, false, false ); // CANCEL
            }

            public override void OnResponse( NetState sender, RelayInfo info )
            {
                PlayerMobile pm = m_Entry.Player;

                m_CloseTimer.Stop();

                if( GetEntry( pm ) != m_Entry )
                    return;

                if( info.ButtonID == 1 && m_Entry.Safe && m_Bedroll.Parent == null && m_Bedroll.IsAccessibleTo( pm )
                && m_Bedroll.VerifyMove( pm ) && m_Bedroll.Map == pm.Map && pm.InRange( m_Bedroll, 2 ) )
                {
                    pm.PlaceInBackpack( m_Bedroll );

                    pm.BedrollLogout = true;
                    sender.Dispose();
                }

                RemoveEntry( m_Entry );
            }

            private void CloseGump()
            {
                RemoveEntry( m_Entry );
                m_Entry.Player.CloseGump( typeof( InternalGump ) );
            }
        }
    }
}