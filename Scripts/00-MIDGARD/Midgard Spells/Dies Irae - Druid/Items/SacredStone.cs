/***************************************************************************
 *                               SacredStone.cs
 *
 *   begin                : 27 September, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;

using Server;
using Server.Mobiles;

namespace Midgard.Engines.SpellSystem
{
    public class SacredStone : Item
    {
        private Timer m_CampSecureTimer;
        private Timer m_Timer;

        public override bool HandlesOnMovement
        {
            get { return true; }
        }

        public static int CampingRange
        {
            get { return 5; }
        }

        [Constructable]
        public SacredStone( Mobile owner )
            : base( 0x8E3 )
        {
            Movable = false;
            Name = "Sacred Stone";

            Camper = owner;

            m_Timer = new DecayTimer( this );
            m_Timer.Start();

            m_CampSecureTimer = new SecureTimer( Camper, this );
            m_CampSecureTimer.Start();
        }

        public SacredStone( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( 0 ); // version

            writer.Write( Camper );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();

            switch( version )
            {
                case 0:
                    {
                        Camper = reader.ReadMobile();
                        m_Timer = new DecayTimer( this );
                        m_Timer.Start();
                        m_CampSecureTimer = new SecureTimer( Camper, this );
                        m_CampSecureTimer.Start();
                        break;
                    }
            }
        }

        public override void OnMovement( Mobile m, Point3D oldLocation )
        {
            if( ( m is PlayerMobile ) && ( m == Camper ) )
            {
                bool inOldRange = Utility.InRange( oldLocation, Location, CampingRange );
                bool inNewRange = Utility.InRange( m.Location, Location, CampingRange );

                if( inNewRange && !inOldRange )
                    OnEnter( m );
                else if( inOldRange && !inNewRange )
                    OnExit( m );
            }
        }

        public virtual void OnEnter( Mobile m )
        {
            StartSecureTimer();
        }

        public virtual void OnExit( Mobile m )
        {
            StopSecureTimer();
            m.SendMessage( "You have left the grove." );
        }

        public override void OnAfterDelete()
        {
            if( m_Timer != null )
                m_Timer.Stop();
        }

        private class DecayTimer : Timer
        {
            private SacredStone m_Owner;

            public DecayTimer( SacredStone owner )
                : base( TimeSpan.FromMinutes( 2.0 ) )
            {
                Priority = TimerPriority.FiveSeconds;
                m_Owner = owner;
            }

            protected override void OnTick()
            {
                m_Owner.Delete();
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool CampSecure { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public Mobile Camper { get; private set; }

        public void StartSecureTimer()
        {
            Camper.SendMessage( "You start to feel secure" ); // You feel it would take a few moments to secure your camp.
            if( m_CampSecureTimer.Running == false )
            {
                CampSecure = false;
                m_CampSecureTimer.Start();
            }
        }

        public void StopSecureTimer()
        {
            CampSecure = false;
            m_CampSecureTimer.Stop();
        }

        public override void OnDelete()
        {
            base.OnDelete();
            if( m_CampSecureTimer != null )
                StopSecureTimer();
        }

        private class SecureTimer : Timer
        {
            private Mobile m_Owner;
            private SacredStone m_SacredStone;

            public SecureTimer( Mobile owner, SacredStone sacredStone )
                : base( TimeSpan.FromSeconds( 30.0 ) )
            {
                Priority = TimerPriority.FiveSeconds;
                m_SacredStone = sacredStone;
                m_Owner = owner;
            }

            protected override void OnTick()
            {
                m_SacredStone.CampSecure = true;
                m_Owner.SendMessage( "The power of the grove washes over you." );
            }
        }
    }
}