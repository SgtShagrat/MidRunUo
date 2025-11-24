using System;
using Server.Targeting;

namespace Server.Items
{
    public class ThrowPillow : Item
    {
        [Constructable]
        public ThrowPillow()
            : base( 0x1944 )
        {
            LootType = LootType.Blessed;
        }

        #region serialization
        public ThrowPillow( Serial serial )
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
            if( !IsChildOf( from.Backpack ) )
            {
                from.SendLocalizedMessage( 1042010 ); // You must have the object in your backpack to use it.
            }
            else if( from.CanBeginAction( typeof( ThrowPillow ) ) )
            {
                from.SendMessage( "Select your victim!" );
                from.Target = new PillowTarget( from, this );
            }
            else
            {
                from.SendMessage( "Keep trying..." );
            }
        }

        private class InternalTimer : Timer
        {
            private Mobile m_From;

            public InternalTimer( Mobile from )
                : base( TimeSpan.FromSeconds( 5.0 ) )
            {
                m_From = from;
            }

            protected override void OnTick()
            {
                m_From.EndAction( typeof( ThrowPillow ) );
            }
        }

        private class PillowTarget : Target
        {
            private Item m_Pillow;

            public PillowTarget( Mobile thrower, Item pillow )
                : base( 10, false, TargetFlags.None )
            {
                m_Pillow = pillow;
            }

            protected override void OnTarget( Mobile from, object target )
            {
                if( target == from )
                {
                    from.SendLocalizedMessage( 1005576 ); // You can't throw this at yourself.
                }
                else if( target is Mobile )
                {
                    Mobile targ = (Mobile)target;
                    Container pack = targ.Backpack;

                    if( pack != null && pack.FindItemByType( new Type[] { typeof( ThrowPillow ) } ) != null )
                    {
                        if( from.BeginAction( typeof( ThrowPillow ) ) )
                        {
                            new InternalTimer( from ).Start();

                            from.PlaySound( 0x145 );

                            from.Animate( 9, 1, 1, true, false, 0 );

                            targ.SendMessage( "You have just been hit by a pillow!" );
                            from.SendMessage( "You throw the pillow and hit the target!" );

                            Effects.SendMovingEffect( from, targ, m_Pillow.ItemID, 7, 0, false, true, 0x480, 0 );
                        }
                        else
                        {
                            from.SendMessage( "Keep trying..." );
                        }
                    }
                    else
                    {
                        from.SendMessage( "You can only throw a pillow at something that can throw one back." );
                    }
                }
                else
                {
                    from.SendMessage( "You can only throw a pillow at something that can throw one back." );
                }
            }
        }
    }
}