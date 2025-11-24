using Server;
using Server.Items;
using Server.Targeting;

namespace Midgard.Items
{
    /* Asparagus
    public class Asparagus : Food
    {
        [Constructable]
        public Asparagus()
            : this( 1 )
        {
        }

        [Constructable]
        public Asparagus( int amount )
            : base( amount, 0xC77 )
        {
            FillFactor = 2;
            Name = "Asparagus";
            Hue = 0x1D3;
        }

        public Asparagus( Serial serial )
            : base( serial )
        {
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
    }
    */

    public class Beet : Food
    {
        [Constructable]
        public Beet()
            : this( 1 )
        {
        }

        [Constructable]
        public Beet( int amount )
            : base( amount, 0xD39 )
        {
            Weight = 0.5;
            FillFactor = 1;
            Hue = 0x1B;
            Name = "Beet";
        }

        public Beet( Serial serial )
            : base( serial )
        {
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
    }

    public class Broccoli : Food
    {
        [Constructable]
        public Broccoli()
            : this( 1 )
        {
        }

        [Constructable]
        public Broccoli( int amount )
            : base( amount, 0xC78 )
        {
            Weight = 0.3;
            FillFactor = 1;
            Hue = 0x48F;
            Name = "Broccoli";
        }

        public Broccoli( Serial serial )
            : base( serial )
        {
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
    }

    public class Cauliflower : Food
    {
        [Constructable]
        public Cauliflower()
            : this( 1 )
        {
        }

        [Constructable]
        public Cauliflower( int amount )
            : base( amount, 0xC7B )
        {
            FillFactor = 2;
            Hue = 0x47E;
            Name = "Cauliflower";
        }

        public Cauliflower( Serial serial )
            : base( serial )
        {
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
    }

    public class Celery : Food
    {
        [Constructable]
        public Celery()
            : this( 1 )
        {
        }

        [Constructable]
        public Celery( int amount )
            : base( amount, 0xC77 )
        {
            Weight = 0.5;
            FillFactor = 1;
            Name = "Celery";
            Hue = 0xAA;
        }

        public Celery( Serial serial )
            : base( serial )
        {
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
    }

    /* Eggplant
    public class Eggplant : Food
    {
        [Constructable]
        public Eggplant()
            : this( 1 )
        {
        }

        [Constructable]
        public Eggplant( int amount )
            : base( amount, 0xD3A )
        {
            FillFactor = 2;
            Name = "Eggplant";
            Hue = 0x72;
        }

        public Eggplant( Serial serial )
            : base( serial )
        {
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
    }
    */

    public class GreenBean : Food
    {
        [Constructable]
        public GreenBean()
            : this( 1 )
        {
        }

        [Constructable]
        public GreenBean( int amount )
            : base( amount, 0xF80 )
        {
            Weight = 0.1;
            FillFactor = 1;
            Hue = 0x483;
            Name = "Green Bean";
        }

        public GreenBean( Serial serial )
            : base( serial )
        {
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
    }

    public class Peas : Food
    {
        [Constructable]
        public Peas()
            : this( 1 )
        {
        }

        [Constructable]
        public Peas( int amount )
            : base( amount, 0xF2F )
        {
            Weight = 0.1;
            FillFactor = 1;
            Hue = 0x42;
            Name = "Peas";
        }

        public Peas( Serial serial )
            : base( serial )
        {
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
    }

    public class Potato : Food
    {
        [Constructable]
        public Potato()
            : this( 1 )
        {
        }

        [Constructable]
        public Potato( int amount )
            : base( amount, 0xC5D )
        {
            FillFactor = 3;
            Hue = 0x6C0;
            Name = "Potato";
        }

        public Potato( Serial serial )
            : base( serial )
        {
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
    }

    /*
    public class Radish : Food
    {
        [Constructable]
        public Radish()
            : this( 1 )
        {
        }

        [Constructable]
        public Radish( int amount )
            : base( amount, 0xD39 )
        {
            Weight = 0.5;
            FillFactor = 1;
            Name = "Radish";
            Hue = 0x1B9;
        }

        public Radish( Serial serial )
            : base( serial )
        {
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
    }
    */

    public class SnowPeas : Food
    {
        [Constructable]
        public SnowPeas()
            : this( 1 )
        {
        }

        [Constructable]
        public SnowPeas( int amount )
            : base( amount, 0xF2F )
        {
            Weight = 0.1;
            FillFactor = 1;
            Hue = 0x29A;
            Name = "Snow Peas";
        }

        public SnowPeas( Serial serial )
            : base( serial )
        {
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
    }

    public class Soybean : Item
    {
        [Constructable]
        public Soybean()
            : this( 1 )
        {
        }

        [Constructable]
        public Soybean( int amount )
            : base( 0x1EBD )
        {
            Amount = amount;
            Weight = 3.0;
            Stackable = true;
            Name = "Soybean";
            Hue = 0x292;
        }

        public Soybean( Serial serial )
            : base( serial )
        {
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

        public override void OnDoubleClick( Mobile from )
        {
            if( !Movable )
                return;

            from.Target = new InternalTarget( this );
        }

        private class InternalTarget : Target
        {
            private Soybean m_Item;

            public InternalTarget( Soybean item )
                : base( 1, false, TargetFlags.None )
            {
                m_Item = item;
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                if( m_Item.Deleted )
                    return;

                else if( IsFlourMill( targeted ) )
                {
                    if( m_Item.Amount >= 20 )
                    {
                        m_Item.Consume();
                        from.SendMessage( "You made a bag of soy." );
                        from.AddToBackpack( new BagOfSoy() );
                    }
                    else
                        from.SendMessage( "You don't have enough soybeans." );
                }
            }
        }

        public static bool IsFlourMill( object targeted )
        {
            int itemID;

            if( targeted is Item )
                itemID = ( (Item)targeted ).ItemID & 0x3FFF;
            else if( targeted is StaticTarget )
                itemID = ( (StaticTarget)targeted ).ItemID & 0x3FFF;
            else
                return false;

            if( itemID >= 0x1883 && itemID <= 0x1893 )
                return true;
            else if( itemID >= 0x1920 && itemID <= 0x1937 )
                return true;

            return false;
        }
    }

    public class Spinach : Food
    {
        [Constructable]
        public Spinach()
            : this( 1 )
        {
        }

        [Constructable]
        public Spinach( int amount )
            : base( amount, 0xD09 )
        {
            Weight = 0.1;
            Stackable = true;
            FillFactor = 1;
            Hue = 0x29D;
            Name = "Spinach Leaf";
        }

        public Spinach( Serial serial )
            : base( serial )
        {
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
    }

    /* SweetPotato
    public class SweetPotato : Food
    {
        [Constructable]
        public SweetPotato()
            : this( 1 )
        {
        }

        [Constructable]
        public SweetPotato( int amount )
            : base( amount, 0xC64 )
        {
            FillFactor = 2;
            Name = "Sweet Potato";
            Hue = 0x45E;
        }

        public SweetPotato( Serial serial )
            : base( serial )
        {
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
    }
     */
}