/***************************************************************************
 *                               BaseScoutThrowingWeapon.cs
 *
 *   begin                : 08 October, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;

using Midgard.Engines.Classes;

using Server;
using Server.Items;
using Server.Targeting;

namespace Midgard.Items
{
    public abstract class BaseScoutThrowingWeapon : BaseWeapon
    {
        public abstract int AnimID { get; }
        public abstract int AnimHue { get; }

        public BaseScoutThrowingWeapon( int itemID )
            : base( itemID )
        {
        }

        #region serialization
        public BaseScoutThrowingWeapon( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion

        public override void OnDoubleClick( Mobile from )
        {
            if( from == null || !from.Alive )
                return;

            if( !ClassSystem.IsScout( from ) )
                from.SendMessage( "Thou cannot use this weapon." );
            else if( !from.Items.Contains( this ) )
                from.SendMessage( "You must be holding that weapon to use it." );
            else
                from.Target = new InternalTarget( this );
        }

        public virtual void DoTarget( Mobile from, object targeted )
        {
            if( Deleted )
                return;

            if( !from.Items.Contains( this ) )
                from.SendMessage( "You must be holding that weapon to use it." );
            else if( targeted is Mobile )
            {
                Mobile m = (Mobile)targeted;

                if( m != from && from.HarmfulCheck( m ) )
                {
                    Direction to = from.GetDirectionTo( m );

                    from.Direction = to;
                    from.Animate( from.Mounted ? 26 : 9, 7, 1, true, false, 0 );

                    if( Utility.RandomDouble() >= ( Math.Sqrt( m.Dex / 100.0 ) * 0.8 ) )
                    {
                        from.MovingEffect( m, AnimID, 7, 1, false, false, AnimHue, 0 );
                        AOS.Damage( m, from, Utility.Random( 5, from.Str / 10 ), 100, 0, 0, 0, 0 );
                        MoveToWorld( m.Location, m.Map );
                    }
                    else
                    {
                        int x = 0, y = 0;

                        switch( to & Direction.Mask )
                        {
                            case Direction.North:
                                --y;
                                break;
                            case Direction.South:
                                ++y;
                                break;
                            case Direction.West:
                                --x;
                                break;
                            case Direction.East:
                                ++x;
                                break;
                            case Direction.Up:
                                --x;
                                --y;
                                break;
                            case Direction.Down:
                                ++x;
                                ++y;
                                break;
                            case Direction.Left:
                                --x;
                                ++y;
                                break;
                            case Direction.Right:
                                ++x;
                                --y;
                                break;
                        }

                        x += Utility.Random( -1, 3 );
                        y += Utility.Random( -1, 3 );

                        x += m.X;
                        y += m.Y;

                        MoveToWorld( new Point3D( x, y, m.Z ), m.Map );
                        from.MovingEffect( this, AnimID, 7, 1, false, false, AnimHue, 0 );
                        from.SendMessage( "You miss." );
                    }
                }
            }
        }

        private class InternalTarget : Target
        {
            private BaseScoutThrowingWeapon m_Weapon;

            public InternalTarget( BaseScoutThrowingWeapon weapon )
                : base( 10, false, TargetFlags.Harmful )
            {
                m_Weapon = weapon;
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                if( m_Weapon != null && !m_Weapon.Deleted )
                    m_Weapon.DoTarget( from, targeted );
            }
        }
    }
}