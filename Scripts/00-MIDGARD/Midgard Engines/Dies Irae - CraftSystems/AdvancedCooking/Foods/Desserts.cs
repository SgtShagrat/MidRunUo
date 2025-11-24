namespace Server.Items
{
    public class BlueberryMuffins : Food
    {
        [Constructable]
        public BlueberryMuffins()
            : this( 1 )
        {
        }

        [Constructable]
        public BlueberryMuffins( int amount )
            : base( amount, 0x9EB )
        {
            Weight = 1.0;
            FillFactor = 3;
            Hue = 0x1FC;
            Name = "Blueberry Muffins";
        }

        public BlueberryMuffins( Serial serial )
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

    public class PumpkinMuffins : Food
    {
        [Constructable]
        public PumpkinMuffins()
            : this( 1 )
        {
        }

        [Constructable]
        public PumpkinMuffins( int amount )
            : base( amount, 0x9EB )
        {
            Weight = 1.0;
            FillFactor = 3;
            Hue = 0x1C0;
            Name = "Pumpkin Muffins";
        }

        public PumpkinMuffins( Serial serial )
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

    public class ChocSunflowerSeeds : Food
    {
        [Constructable]
        public ChocSunflowerSeeds()
            : this( 1 )
        {
        }

        [Constructable]
        public ChocSunflowerSeeds( int amount )
            : base( amount, 0x9B4 )
        {
            Weight = 1.0;
            FillFactor = 2;
            Hue = 0x41B;
            Name = "Chocolate Sunflower Seeds";
        }

        public ChocSunflowerSeeds( Serial serial )
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

    public class RiceKrispTreat : Food
    {
        [Constructable]
        public RiceKrispTreat()
            : this( 1 )
        {
        }

        [Constructable]
        public RiceKrispTreat( int amount )
            : base( amount, 0x1044 )
        {
            Weight = 1.0;
            FillFactor = 2;
            Hue = 0x9B;
            Name = "Rice Krisp Treat";
        }

        public RiceKrispTreat( Serial serial )
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

    public class Brownies : Food
    {
        [Constructable]
        public Brownies()
            : this( 1 )
        {
        }

        [Constructable]
        public Brownies( int amount )
            : base( amount, 0x160B )
        {
            Weight = 1.0;
            FillFactor = 2;
            Hue = 0x47D;
            Name = "Brownies";
        }

        public Brownies( Serial serial )
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

    public class ChocolateCake : Food
    {
        [Constructable]
        public ChocolateCake()
            : this( 1 )
        {
        }

        [Constructable]
        public ChocolateCake( int amount )
            : base( amount, 0x9E9 )
        {
            Weight = 2.0;
            FillFactor = 3;
            Hue = 0x45D;
            Name = "Chocolate Cake";
        }

        public ChocolateCake( Serial serial )
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
}