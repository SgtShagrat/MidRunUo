using System;

using Midgard.Mobiles;

using Server;
using Server.Commands;
using Server.Mobiles;
using Server.Targeting;

namespace Midgard.Engines.JailSystem
{
    public class WraithJailEffect
    {
        private readonly PlayerMobile m_Jailor;

        public WraithJailEffect( PlayerMobile prisoner, PlayerMobile jailor )
        {
            m_Jailor = jailor;
            Prisoner = prisoner;
            Prisoner.CantWalk = true;
            Prisoner.Squelched = true;
            Effects.PlaySound( jailor.Location, jailor.Map, 0x1DD );

            var loc = new Point3D( prisoner.X, prisoner.Y, prisoner.Z );

            var firstFlamea = new InternalItem( prisoner.Location, prisoner.Map );
            int mushx = loc.X - 2;
            int mushy = loc.Y - 2;
            int mushz = loc.Z;
            var mushxyz = new Point3D( mushx, mushy, mushz );
            firstFlamea.MoveToWorld( mushxyz, prisoner.Map );

            var firstFlamec = new InternalItem( prisoner.Location, prisoner.Map );
            mushx = loc.X;
            mushy = loc.Y - 3;
            mushz = loc.Z;
            var mushxyzb = new Point3D( mushx, mushy, mushz );
            firstFlamec.MoveToWorld( mushxyzb, prisoner.Map );

            var firstFlamed = new InternalItem( prisoner.Location, prisoner.Map );
            firstFlamed.ItemID = 0x3709;
            mushx = loc.X + 2;
            mushy = loc.Y - 2;
            mushz = loc.Z;
            var mushxyzc = new Point3D( mushx, mushy, mushz );
            firstFlamed.MoveToWorld( mushxyzc, prisoner.Map );
            var firstFlamee = new InternalItem( prisoner.Location, prisoner.Map );
            mushx = loc.X + 3;
            firstFlamee.ItemID = 0x3709;
            mushy = loc.Y;
            mushz = loc.Z;
            var mushxyzd = new Point3D( mushx, mushy, mushz );
            firstFlamee.MoveToWorld( mushxyzd, prisoner.Map );
            var firstFlamef = new InternalItem( prisoner.Location, prisoner.Map );
            firstFlamef.ItemID = 0x3709;
            mushx = loc.X + 2;
            mushy = loc.Y + 2;
            mushz = loc.Z;
            var mushxyze = new Point3D( mushx, mushy, mushz );
            firstFlamef.MoveToWorld( mushxyze, prisoner.Map );
            var firstFlameg = new InternalItem( prisoner.Location, prisoner.Map );
            mushx = loc.X;
            firstFlameg.ItemID = 0x3709;
            mushy = loc.Y + 3;
            mushz = loc.Z;
            var mushxyzf = new Point3D( mushx, mushy, mushz );
            firstFlameg.MoveToWorld( mushxyzf, prisoner.Map );
            var firstFlameh = new InternalItem( prisoner.Location, prisoner.Map );
            mushx = loc.X - 2;
            firstFlameh.ItemID = 0x3709;
            mushy = loc.Y + 2;
            mushz = loc.Z;
            var mushxyzg = new Point3D( mushx, mushy, mushz );
            firstFlameh.MoveToWorld( mushxyzg, prisoner.Map );
            var firstFlamei = new InternalItem( prisoner.Location, prisoner.Map );
            mushx = loc.X - 3;
            firstFlamei.ItemID = 0x3709;
            mushy = loc.Y;
            mushz = loc.Z;
            var mushxyzh = new Point3D( mushx, mushy, mushz );
            firstFlamei.MoveToWorld( mushxyzh, prisoner.Map );
            new JailWraith( this, prisoner.X + 15, prisoner.Y + 15, m_Jailor );
        }

        public PlayerMobile Prisoner { get; private set; }

        public void Jail()
        {
            JailSystem.Jail( Prisoner, TimeSpan.FromDays( 2 ), "Interefering with a Role-Playing event.", true,
                             m_Jailor.Name, AccessLevel.Seer );
            Prisoner.CantWalk = false;
            Prisoner.Squelched = false;
            Prisoner.SendMessage(
                "You are now in jail for disrupting an event.  Do not expect to see the staff member who jailed you until after the event has ended." );
        }

        public static void Initialize()
        {
            CommandSystem.Register( "Jailwraith", AccessLevel.GameMaster, new CommandEventHandler( JailOnCommand ) );
        }

        [Usage( "Jailwraith" )]
        [Description( "Places the selected player in jail by a wraith." )]
        public static void JailOnCommand( CommandEventArgs e )
        {
            if( e.Mobile is PlayerMobile )
            {
                e.Mobile.Target = new InternalTarget();
                e.Mobile.SendLocalizedMessage( 3000218 );
            }
        }

        private class InternalItem : Item
        {
            private DateTime m_End;
            private Timer m_Timer;

            public InternalItem( Point3D loc, Map map )
                : base( 0x3709 )
            {
                Visible = false;
                Movable = false;
                Light = LightType.Circle150;
                MoveToWorld( loc, map );
                Visible = true;
                m_Timer = new InternalTimer( this, TimeSpan.FromSeconds( 30.0 ) );
                m_Timer.Start();

                m_End = DateTime.Now + TimeSpan.FromSeconds( 30.0 );
            }

            public InternalItem( Serial serial )
                : base( serial )
            {
            }

            public override bool BlocksFit
            {
                get { return true; }
            }

            public override void Serialize( GenericWriter writer )
            {
                base.Serialize( writer );

                writer.Write( 1 ); // version

                writer.Write( m_End - DateTime.Now );
            }

            public override void Deserialize( GenericReader reader )
            {
                base.Deserialize( reader );

                int version = reader.ReadInt();

                switch( version )
                {
                    case 1:
                        {
                            TimeSpan duration = reader.ReadTimeSpan();

                            m_Timer = new InternalTimer( this, duration );
                            m_Timer.Start();

                            m_End = DateTime.Now + duration;

                            break;
                        }
                    case 0:
                        {
                            TimeSpan duration = TimeSpan.FromSeconds( 10.0 );

                            m_Timer = new InternalTimer( this, duration );
                            m_Timer.Start();

                            m_End = DateTime.Now + duration;

                            break;
                        }
                }
            }

            public override void OnAfterDelete()
            {
                base.OnAfterDelete();

                if( m_Timer != null )
                    m_Timer.Stop();
            }

            private class InternalTimer : Timer
            {
                private readonly InternalItem m_Item;

                public InternalTimer( InternalItem item, TimeSpan duration )
                    : base( duration )
                {
                    m_Item = item;
                }

                protected override void OnTick()
                {
                    m_Item.Delete();
                }
            }
        }

        private class InternalTarget : Target
        {
            public InternalTarget()
                : base( -1, false, TargetFlags.None )
            {
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                if( from is PlayerMobile && targeted is PlayerMobile )
                {
                    new WraithJailEffect( targeted as PlayerMobile, from as PlayerMobile );
                }
            }
        }
    }
}