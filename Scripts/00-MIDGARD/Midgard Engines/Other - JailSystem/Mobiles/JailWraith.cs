using System;
using Server.Mobiles;
using Server.Regions;

using Midgard.Engines.JailSystem;

using Server;

namespace Midgard.Mobiles
{
    [CorpseName( "a errie corpse" )]
    public class JailWraith : BaseCreature
    {
        private readonly WraithJailEffect m_Effect;
        private readonly int m_EndPointX;
        private readonly int m_EndPointY;
        private bool m_Drag;

        public JailWraith( WraithJailEffect effect, int endPointX, int endPointY, IEntity jailor )
            : base( AIType.AI_Use_Default, FightMode.None, 10, 1, 0.2, 0.4 )
        {
            m_EndPointY = endPointY;
            m_EndPointX = endPointX;
            m_Effect = effect;
            Name = "a soulless demon";
            Body = 26;
            Hue = 0x4001;
            BaseSoundID = 0x482;
            Blessed = true;

            Fame = 4000;
            Karma = -4000;

            VirtualArmor = 28;
            CantWalk = true;
            new InternalTimer( this );
            X = jailor.X;
            Y = jailor.Y;
            Map = jailor.Map;
        }

        public JailWraith( Serial serial )
            : base( serial )
        {
        }

        protected override void OnLocationChange( Point3D loc )
        {
            base.OnLocationChange( loc );
            m_Effect.Prisoner.Hidden = false;
            Stam = 1000;
            if( m_Drag )
            {
                Direction = GetDirectionTo( m_EndPointX, m_EndPointY );
                if( ( X == m_EndPointX ) && ( Y == m_EndPointY ) )
                {
                    m_Effect.Jail();
                    Delete();
                }
                else
                {
                    if( !( m_Effect.Prisoner.Region is Jail ) )
                    {
                        m_Effect.Prisoner.Location = loc;
                    }
                }
            }
            else
            {
                if( ( X == m_Effect.Prisoner.X ) && ( Y == m_Effect.Prisoner.Y ) )
                {
                    Direction = GetDirectionTo( m_EndPointX, m_EndPointY );
                    m_Effect.Prisoner.Kill();
                    m_Effect.Prisoner.Hidden = false;
                    m_Drag = true;
                    PlaySound( GetAngerSound() );
                }
                else
                    Direction = GetDirectionTo( m_Effect.Prisoner.Location );
            }
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }

        private class InternalTimer : Timer
        {
            private readonly JailWraith m_Item;

            public InternalTimer( JailWraith item )
                : base( TimeSpan.FromMilliseconds( 200 ), TimeSpan.FromMilliseconds( 200 ) )
            {
                m_Item = item;
                Start();
                Priority = TimerPriority.FiftyMS;
            }

            protected override void OnTick()
            {
                //yeah baby move me
                if( m_Item.Deleted )
                    Stop();
                int x = m_Item.X;
                int y = m_Item.Y;
                switch( m_Item.Direction )
                {
                    case Direction.North:
                        x = x - 1;
                        break;
                    case Direction.Left:
                        y++;
                        x = x - 1;
                        break;
                    case Direction.West:
                        y++;
                        break;
                    case Direction.Down:
                        y++;
                        x++;
                        break;
                    case Direction.South:
                        x++;
                        break;
                    case Direction.Right:
                        y = y - 1;
                        x++;
                        break;
                    case Direction.East:
                        y = y - 1;
                        break;
                    case Direction.Up:
                        y = y - 1;
                        x = x - 1;
                        break;
                    default:
                        break;
                }
                m_Item.Location = new Point3D( x, y, m_Item.Z );
            }
        }
    }
}