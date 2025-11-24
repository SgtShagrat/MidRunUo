using System;

using Server;
using Server.Items;
using Server.Targeting;

namespace Midgard.Engines.Events.Items
{
    public class ChristmasPileOfGlacialSnow : Item
    {
        #region IEventItem
        [CommandProperty( AccessLevel.GameMaster )]
        public int Year { get; set; }

        public EventType Event
        {
            get { return EventType.Christmas; }
        }
        #endregion

        [Constructable]
        public ChristmasPileOfGlacialSnow()
            : base( 0x913 )
        {
            Hue = 0x480;
            Weight = 1.0;
            LootType = LootType.Blessed;

            Year = ChristmasHelper.GetCurrentYear;
        }

        public override int LabelNumber { get { return 1070874; } } // a Pile of Glacial Snow

        public ChristmasPileOfGlacialSnow( Serial serial )
            : base( serial )
        {
        }

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version

            writer.WriteEncodedInt( Year );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            Year = reader.ReadEncodedInt();

            Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( ChristmasHelper.VerifyEndPeriod_Callback ), this );
        }
        #endregion

        public override void OnSingleClick( Mobile from )
        {
            base.OnSingleClick( from );

            LabelTo( from, "Christmas 2010" ); // Winter 2004
        }

        public override bool DisplayLootType { get { return false; } }
        public override bool DisplayWeight { get { return false; } }

        public override void OnDoubleClick( Mobile from )
        {
            if( !IsChildOf( from.Backpack ) )
            {
                from.SendLocalizedMessage( 1042010 ); // You must have the object in your backpack to use it.
            }
            else if( from.CanBeginAction( typeof( SnowPile ) ) )
            {
                from.SendLocalizedMessage( 1005575 ); // You carefully pack the snow into a ball...
                from.Target = new SnowTarget();
            }
            else
            {
                from.SendLocalizedMessage( 1005574 ); // The snow is not ready to be packed yet.  Keep trying.
            }
        }

        private class InternalTimer : Timer
        {
            private readonly Mobile m_From;

            public InternalTimer( Mobile from )
                : base( TimeSpan.FromSeconds( 5.0 ) )
            {
                m_From = from;
            }

            protected override void OnTick()
            {
                m_From.EndAction( typeof( SnowPile ) );
            }
        }

        private class SnowTarget : Target
        {
            public SnowTarget()
                : base( 10, false, TargetFlags.None )
            {
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

                    if( pack != null && pack.FindItemByType( new Type[] { typeof( SnowPile ), typeof( ChristmasPileOfGlacialSnow ) } ) != null )
                    {
                        if( from.BeginAction( typeof( SnowPile ) ) )
                        {
                            new InternalTimer( from ).Start();

                            from.PlaySound( 0x145 );

                            from.Animate( 9, 1, 1, true, false, 0 );

                            targ.SendLocalizedMessage( 1010572 ); // You have just been hit by a snowball!
                            from.SendLocalizedMessage( 1010573 ); // You throw the snowball and hit the target!

                            Effects.SendMovingEffect( from, targ, 0x36E4, 7, 0, false, true, 0x47F, 0 );
                        }
                        else
                        {
                            from.SendLocalizedMessage( 1005574 ); // The snow is not ready to be packed yet.  Keep trying.
                        }
                    }
                    else
                    {
                        from.SendLocalizedMessage( 1005577 ); // You can only throw a snowball at something that can throw one back.
                    }
                }
                else
                {
                    from.SendLocalizedMessage( 1005577 ); // You can only throw a snowball at something that can throw one back.
                }
            }
        }
    }
}