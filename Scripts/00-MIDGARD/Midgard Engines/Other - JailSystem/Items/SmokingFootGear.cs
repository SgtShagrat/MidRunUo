using System;

using Server;
using Server.Items;

namespace Midgard.Items
{
    public class SmokingFootGear : Item
    {
        public SmokingFootGear()
            : base( 5899 )
        {
            Name = "Smoking boots";
        }

        public new TimeSpan DecayTime
        {
            get { return TimeSpan.FromSeconds( 5 ); }
        }

        public new bool Movable
        {
            get { return false; }
        }

        public new bool Decays
        {
            get { return true; }
        }

        public SmokingFootGear( Mobile m )
            : base( FindFootGear( m ) )
        {
            Name = m.Name + "'s smoking boots";
            MoveToWorld( m.Location, m.Map );
            new InternalTimer( this );
        }

        public static int FindFootGear( Mobile m )
        {
            try
            {
                foreach( Item i in m.Items )
                {
                    if( i is BaseShoes )
                    {
                        if( i.Parent.Equals( m ) )
                            //return i.ItemID;
                            return 5899;
                    }
                }
                return 5899;
            }
            catch
            {
                m.SendMessage( "Flying Monkeys ate your shoes" );
                return 5899;
            }
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
        }

        public SmokingFootGear( Serial s )
            : base( s )
        {
        }

        private class InternalTimer : Timer
        {
            private SmokingFootGear footwear;
            private int m_TI = 1;

            public InternalTimer( SmokingFootGear i )
                : base( TimeSpan.FromSeconds( 1 ), TimeSpan.FromSeconds( 5 ) )
            {
                footwear = i;
                Start();
            }

            protected override void OnTick()
            {
                if( m_TI == 1 )
                {
                    Point3D p = new Point3D( footwear.Location.X, footwear.Location.Y, footwear.Location.Z + 2 );
                    Effects.SendLocationEffect( p, footwear.Map, 0x3735, 30 );
                    Effects.PlaySound( p, footwear.Map, 0x5C );
                    m_TI++;
                }
                else
                {
                    Effects.SendLocationEffect( footwear.Location, footwear.Map, 0x36BD, 10 );
                    Effects.PlaySound( footwear.Location, footwear.Map, 0x307 );
                    Stop();
                    footwear.Delete();
                }
            }
        }
    }
}