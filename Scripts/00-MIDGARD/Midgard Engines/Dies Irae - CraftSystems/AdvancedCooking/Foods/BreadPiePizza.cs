namespace Server.Items
{
    public class BananaBread : Food
    {
        [Constructable]
        public BananaBread()
            : this( 1 )
        {
        }

        [Constructable]
        public BananaBread( int amount )
            : base( amount, 0x103B )
        {
            Weight = 1.0;
            FillFactor = 3;
            Name = "Banana Bread";
        }

        public BananaBread( Serial serial )
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

    public class BlackberryCobbler : Food
    {
        [Constructable]
        public BlackberryCobbler()
            : this( 1 )
        {
        }

        [Constructable]
        public BlackberryCobbler( int amount )
            : base( amount, 0x1041 )
        {
            Weight = 1.0;
            FillFactor = 3;
            Name = "Blackberry Cobbler";
        }

        public BlackberryCobbler( Serial serial )
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

    public class BlueberryPie : Food
    {
        [Constructable]
        public BlueberryPie()
            : this( 1 )
        {
        }

        [Constructable]
        public BlueberryPie( int amount )
            : base( amount, 0x1041 )
        {
            Weight = 1.0;
            FillFactor = 3;
            Name = "Blueberry Pie";
        }

        public BlueberryPie( Serial serial )
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

    public class CherryPie : Food
    {
        [Constructable]
        public CherryPie()
            : this( 1 )
        {
        }

        [Constructable]
        public CherryPie( int amount )
            : base( amount, 0x1041 )
        {
            Weight = 1.0;
            FillFactor = 3;
            Name = "Cherry Pie";
        }

        public CherryPie( Serial serial )
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

    public class ChickenPie : Food
    {
        [Constructable]
        public ChickenPie()
            : this( 1 )
        {
        }

        [Constructable]
        public ChickenPie( int amount )
            : base( amount, 0x1042 )
        {
            Weight = 1.0;
            FillFactor = 3;
            Name = "Chicken Pie";
        }

        public ChickenPie( Serial serial )
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

    public class BeefPie : Food
    {
        [Constructable]
        public BeefPie()
            : this( 1 )
        {
        }

        [Constructable]
        public BeefPie( int amount )
            : base( amount, 0x1042 )
        {
            Weight = 1.0;
            FillFactor = 3;
            Name = "Beef Pie";
        }

        public BeefPie( Serial serial )
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

    public class CornBread : Food
    {
        [Constructable]
        public CornBread()
            : this( 1 )
        {
        }

        [Constructable]
        public CornBread( int amount )
            : base( amount, 0x103C )
        {
            Weight = 1.0;
            FillFactor = 3;
            Name = "Corn Bread";
            Hue = 0x1C7;
        }

        public CornBread( Serial serial )
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

    public class HamPineapplePizza : Food
    {
        [Constructable]
        public HamPineapplePizza()
            : this( 1 )
        {
        }

        [Constructable]
        public HamPineapplePizza( int amount )
            : base( amount, 0x1040 )
        {
            Weight = 1.0;
            FillFactor = 3;
            Name = "Ham and Pineapple Pizza";
        }

        public HamPineapplePizza( Serial serial )
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

    public class KeyLimePie2 : Food
    {
        [Constructable]
        public KeyLimePie2()
            : this( 1 )
        {
        }

        [Constructable]
        public KeyLimePie2( int amount )
            : base( amount, 0x1042 )
        {
            Weight = 1.0;
            FillFactor = 3;
            Name = "Key Lime Pie 2";
        }

        public KeyLimePie2( Serial serial )
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

    public class LemonMerenguePie : Food
    {
        [Constructable]
        public LemonMerenguePie()
            : this( 1 )
        {
        }

        [Constructable]
        public LemonMerenguePie( int amount )
            : base( amount, 0x1042 )
        {
            Weight = 1.0;
            FillFactor = 3;
            Name = "Lemon Merengue Pie";
        }

        public LemonMerenguePie( Serial serial )
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

    public class MushroomOnionPizza : Food
    {
        [Constructable]
        public MushroomOnionPizza()
            : this( 1 )
        {
        }

        [Constructable]
        public MushroomOnionPizza( int amount )
            : base( amount, 0x1040 )
        {
            Weight = 1.0;
            FillFactor = 3;
            Name = "Mushroom and Onion Pizza";
        }

        public MushroomOnionPizza( Serial serial )
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

    public class PumpkinBread : Food
    {
        [Constructable]
        public PumpkinBread()
            : this( 1 )
        {
        }

        [Constructable]
        public PumpkinBread( int amount )
            : base( amount, 0x103B )
        {
            Weight = 1.0;
            FillFactor = 3;
            Name = "Pumpkin Bread";
            Hue = 0x1C1;
        }

        public PumpkinBread( Serial serial )
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

    public class SausOnionMushPizza : Food
    {
        [Constructable]
        public SausOnionMushPizza()
            : this( 1 )
        {
        }

        [Constructable]
        public SausOnionMushPizza( int amount )
            : base( amount, 0x1040 )
        {
            Weight = 1.0;
            FillFactor = 5;
            Name = "Sausage Onion and Mushroom Pizza";
        }

        public SausOnionMushPizza( Serial serial )
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

    public class ShepherdsPie : Food
    {
        [Constructable]
        public ShepherdsPie()
            : this( 1 )
        {
        }

        [Constructable]
        public ShepherdsPie( int amount )
            : base( amount, 0x1042 )
        {
            Weight = 1.0;
            FillFactor = 3;
            Name = "Shepherds Pie";
        }

        public ShepherdsPie( Serial serial )
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

    public class TacoPizza : Food
    {
        [Constructable]
        public TacoPizza()
            : this( 1 )
        {
        }

        [Constructable]
        public TacoPizza( int amount )
            : base( amount, 0x1040 )
        {
            Weight = 1.0;
            FillFactor = 3;
            Name = "Taco Pizza";
        }

        public TacoPizza( Serial serial )
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

    public class TurkeyPie : Food
    {
        [Constructable]
        public TurkeyPie()
            : this( 1 )
        {
        }

        [Constructable]
        public TurkeyPie( int amount )
            : base( amount, 0x1042 )
        {
            Weight = 1.0;
            FillFactor = 3;
            Name = "Turkey Pie";
        }

        public TurkeyPie( Serial serial )
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

    public class VeggiePizza : Food
    {
        [Constructable]
        public VeggiePizza()
            : this( 1 )
        {
        }

        [Constructable]
        public VeggiePizza( int amount )
            : base( amount, 0x1040 )
        {
            Weight = 1.0;
            FillFactor = 3;
            Name = "Vegetable Pizza";
        }

        public VeggiePizza( Serial serial )
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

    public class GarlicBread : Food
    {
        [Constructable]
        public GarlicBread()
            : this( 1 )
        {
        }

        [Constructable]
        public GarlicBread( int amount )
            : base( amount, 0x98C )
        {
            Weight = 1.0;
            FillFactor = 3;
            Name = "Garlic Bread";
            Hue = 0x1C8;
        }

        public GarlicBread( Serial serial )
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