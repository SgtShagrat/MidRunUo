using Server;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;

namespace Midgard.Items
{
    [Flipable( 0x1428, 0x1429 )]
    public class DippingStick : Item
    {
        public override int LabelNumber { get { return 1065253; } } 		// dipping stick

        [Constructable]
        public DippingStick()
            : base( 0x1428 )
        {
            Weight = 0.5;
        }

        public DippingStick( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }

    public class PileOfBlankCandles : Item
    {
        public override int LabelNumber { get { return 1065254; } } // Pile Of Blank Candles

        [Constructable]
        public PileOfBlankCandles()
            : base( 0x1BD6 )
        {
            Weight = 3.0;
            Hue = 1153;
        }

        public PileOfBlankCandles( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }

    [Flipable( 0x12CA, 0x12CB )]
    public class RawWaxBust : Item
    {
        private bool m_Pictured = false;

        public override int LabelNumber { get { return 1065255; } } // Raw Wax Bust

        [CommandProperty( AccessLevel.GameMaster )]
        public bool Pictured
        {
            get
            {
                return m_Pictured;
            }
            set
            {
                m_Pictured = value;
            }
        }

        [Constructable]
        public RawWaxBust()
            : base( 0x12CA )
        {
            Weight = 0.5;
            Hue = 1150;
        }

        public RawWaxBust( Serial serial )
            : base( serial )
        {
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( IsChildOf( from.Backpack ) )
            {
                if( m_Pictured == false )
                {
                    if( from.Skills[ SkillName.Anatomy ].Base < 100.0 )
                        from.SendLocalizedMessage( 1065345 ); // You do not possess enough knowledge of the human body to carve a face into the raw wax bust.
                    else
                        from.Target = new InternalTarget( this );
                }
                else
                    from.SendLocalizedMessage( 1065346 ); // You cannot change the bust without destroying it.
            }
            else
                from.SendLocalizedMessage( 1065347 ); // This must be in your backpack.
        }

        private class InternalTarget : Target
        {
            private RawWaxBust it_Bust;

            public InternalTarget( RawWaxBust bust )
                : base( 1, false, TargetFlags.None )
            {
                it_Bust = bust;
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                if( targeted != null && targeted is PlayerMobile )
                {
                    if( it_Bust != null )
                    {
                        from.SendLocalizedMessage( 1065348 ); // You carefully apply the face of your model to the bust.

                        it_Bust.m_Pictured = true;
                        it_Bust.Name = "Wax Bust Of " + ( (Mobile)targeted ).Name.ToString();
                    }
                    else
                        return;
                }
                else
                    from.SendLocalizedMessage( 1065349 ); // Invalid target.
            }
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)1 ); // version

            writer.Write( m_Pictured );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            m_Pictured = reader.ReadBool();
        }
    }

    public class SomeBlankCandles : Item
    {
        public override int LabelNumber { get { return 1065256; } } // Some Blank Candles

        [Constructable]
        public SomeBlankCandles()
            : base( 0x1BD5 )
        {
            Weight = 2.0;
            Hue = 1154;
        }

        public SomeBlankCandles( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }
}