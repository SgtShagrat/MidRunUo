using Server;
using Server.Items;

namespace Midgard.Items
{
    public class Blackberry : Food
    {
        [Constructable]
        public Blackberry()
            : this( 1 )
        {
        }

        [Constructable]
        public Blackberry( int amount )
            : base( amount, 0x9D1 )
        {
            FillFactor = 1;
            Hue = 0x25A;
            Name = "Blackberry";
        }

        public Blackberry( Serial serial )
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

    public class Blueberry : Food
    {
        [Constructable]
        public Blueberry()
            : this( 1 )
        {
        }

        [Constructable]
        public Blueberry( int amount )
            : base( amount, 0x9D1 )
        {
            FillFactor = 1;
            Hue = 0x62;
            Name = "Blueberry";
        }

        public Blueberry( Serial serial )
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

    public class ChiliPepper : Food
    {
        [Constructable]
        public ChiliPepper()
            : this( 1 )
        {
        }

        [Constructable]
        public ChiliPepper( int amount )
            : base( amount, 0xC6D )
        {
            Weight = 0.5;
            FillFactor = 1;
            Hue = 0x10C;
            Name = "Chili Pepper";
        }

        public ChiliPepper( Serial serial )
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

    [Flipable( 0xc80, 0xc81 )]
    public class Cucumber : Food
    {
        [Constructable]
        public Cucumber()
            : this( 1 )
        {
        }

        [Constructable]
        public Cucumber( int amount )
            : base( amount, 0xc81 )
        {
            Name = "Cucumber";
            Hue = 0x2FF;
            FillFactor = 1;
        }

        public Cucumber( Serial serial )
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

    /* Elderberries
    public class Elderberries : Food
    {
        [Constructable]
        public Elderberries()
            : this( 1 )
        {
        }

        [Constructable]
        public Elderberries( int amount )
            : base( amount, 0x9D1 )
        {
            FillFactor = 1;
            Hue = 0x200;
            Name = "Elderberries";
        }

        public Elderberries( Serial serial )
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

    /* GreenPepper
    public class GreenPepper : Food
    {
        [Constructable]
        public GreenPepper()
            : this( 1 )
        {
        }

        [Constructable]
        public GreenPepper( int amount )
            : base( amount, 0xC75 )
        {
            FillFactor = 1;
            Hue = 0x1D3;
            Name = "Green Pepper";
        }

        public GreenPepper( Serial serial )
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

    /* GreenSquash
    public class GreenSquash : Food
    {
        [Constructable]
        public GreenSquash()
            : this( 1 )
        {
        }

        [Constructable]
        public GreenSquash( int amount )
            : base( amount, 0xC66 )
        {
            FillFactor = 2;
            Name = "Green Squash";
            Hue = 0x1D8;
        }

        public GreenSquash( Serial serial )
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

    /* OrangePepper
    public class OrangePepper : Food
    {
        [Constructable]
        public OrangePepper()
            : this( 1 )
        {
        }

        [Constructable]
        public OrangePepper( int amount )
            : base( amount, 0xC75 )
        {
            FillFactor = 1;
            Hue = 0x30;
            Name = "Orange Pepper";
        }

        public OrangePepper( Serial serial )
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

    public class Peanut : Food
    {
        [Constructable]
        public Peanut()
            : this( 1 )
        {
        }

        [Constructable]
        public Peanut( int amount )
            : base( amount, 0x14FD )
        {
            Weight = 0.1;
            FillFactor = 1;
            Hue = 0x224;
            Name = "Peanut";
        }

        public Peanut( Serial serial )
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

    /* RedRaspberry
    public class RedRaspberry : Food
    {
        [Constructable]
        public RedRaspberry()
            : this( 1 )
        {
            Weight = 0.5;
            Hue = 0x26;
            Name = "red raspberry";
        }

        [Constructable]
        public RedRaspberry( int amount )
            : base( amount, 0x9D1 )
        {
            Weight = 0.5;
            FillFactor = 2;
            Hue = 0x26;
            Name = "Red Raspberry";
        }

        public RedRaspberry( Serial serial )
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

    /* BlackRaspberry
    public class BlackRaspberry : Food
    {
        [Constructable]
        public BlackRaspberry()
            : this( 1 )
        {
            Weight = 0.5;
            Hue = 1175;
            Name = "black raspberry";
        }

        [Constructable]
        public BlackRaspberry( int amount )
            : base( amount, 0x9D1 )
        {
            Weight = 0.5;
            FillFactor = 2;
            Hue = 1175;
            Name = "Black Raspberry";
        }

        public BlackRaspberry( Serial serial )
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

    /* RedPepper
    public class RedPepper : Food
    {
        [Constructable]
        public RedPepper()
            : this( 1 )
        {
        }

        [Constructable]
        public RedPepper( int amount )
            : base( amount, 0xC75 )
        {
            FillFactor = 1;
            Hue = 0xE9;
            Name = "Red Pepper";
        }

        public RedPepper( Serial serial )
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

    /* Strawberry
    public class Strawberry : Food
    {
        [Constructable]
        public Strawberry()
            : this( 1 )
        {
        }

        [Constructable]
        public Strawberry( int amount )
            : base( amount, 0xF2A )
        {
            FillFactor = 1;
            Hue = 0x85;
            Name = "Strawberry";
        }

        public Strawberry( Serial serial )
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

    public class Tomato : Food
    {
        [Constructable]
        public Tomato()
            : this( 1 )
        {
        }

        [Constructable]
        public Tomato( int amount )
            : base( amount, 0x9D0 )
        {
            Name = "a tomato";
            Hue = 0x8F;
            FillFactor = 1;
        }

        public Tomato( Serial serial )
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

    /* YellowPepper
    public class YellowPepper : Food
    {
        [Constructable]
        public YellowPepper()
            : this( 1 )
        {
        }

        [Constructable]
        public YellowPepper( int amount )
            : base( amount, 0xC75 )
        {
            FillFactor = 1;
            Hue = 0x000;
            Name = "Yellow Pepper";
        }

        public YellowPepper( Serial serial )
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