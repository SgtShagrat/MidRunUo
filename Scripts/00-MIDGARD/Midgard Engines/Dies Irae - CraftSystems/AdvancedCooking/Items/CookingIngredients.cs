namespace Server.Items
{
    public class Batter : Item
    {
        [Constructable]
        public Batter()
            : base( 0xE23 )
        {
            Weight = 0.5;
            Stackable = true;
            Name = "Batter";
            Hue = 0x227;
        }

        public Batter( Serial serial )
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

    public class Butter : Item
    {
        [Constructable]
        public Butter()
            : base( 0x1044 )
        {
            Weight = 0.5;
            Stackable = true;
            Name = "Butter";
            Hue = 0x161;
        }

        public Butter( Serial serial )
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

    public class ChocolateMix : Item
    {
        [Constructable]
        public ChocolateMix()
            : base( 0xE23 )
        {
            Weight = 0.5;
            Stackable = true;
            Name = "Chocolate Mix";
            Hue = 0x414;
        }

        public ChocolateMix( Serial serial )
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

    public class PastaNoodles : Item
    {
        [Constructable]
        public PastaNoodles()
            : base( 0x1038 )
        {
            Weight = 0.5;
            Stackable = true;
            Name = "Pasta Noodles";
            Hue = 0x100;
        }

        public PastaNoodles( Serial serial )
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

    public class PieMix : Item
    {
        [Constructable]
        public PieMix()
            : base( 0x103F )
        {
            Weight = 0.5;
            Stackable = true;
            Name = "Butter";
            Hue = 0x45A;
        }

        public PieMix( Serial serial )
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

    public class Tortilla : Item
    {
        [Constructable]
        public Tortilla()
            : base( 0x973 )
        {
            Weight = 0.5;
            Stackable = true;
            Name = "Tortilla";
            Hue = 0x22C;
        }

        public Tortilla( Serial serial )
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

    public class WaffleMix : Item
    {
        [Constructable]
        public WaffleMix()
            : base( 0x103F )
        {
            Weight = 0.5;
            Stackable = true;
            Name = "Waffle Mix";
            Hue = 0x227;
        }

        public WaffleMix( Serial serial )
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

    public class GroundBeef : Item
    {
        [Constructable]
        public GroundBeef()
            : this( 1 )
        {
        }

        [Constructable]
        public GroundBeef( int amount )
            : base( 9908 )
        {
            Stackable = true;
            Name = "Ground Beef";
            Hue = 0x21B;
            Amount = amount;
        }

        public GroundBeef( Serial serial )
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

    public class GroundPork : Item
    {
        [Constructable]
        public GroundPork()
            : this( 1 )
        {
        }

        [Constructable]
        public GroundPork( int amount )
            : base( 9908 )
        {
            Weight = 0.5;
            Stackable = true;
            Name = "Ground Pork";
            Hue = 0x221;
            Amount = amount;
        }

        public GroundPork( Serial serial )
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

    public class Tofu : Item
    {
        [Constructable]
        public Tofu()
            : base( 0x1044 )
        {
            Weight = 0.5;
            Stackable = true;
            Name = "Tofu";
            Hue = 0x38F;
        }

        public Tofu( Serial serial )
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

    public class BagOfCocoa : Item
    {
        [Constructable]
        public BagOfCocoa()
            : base( 0x1045 )
        {
            Weight = 5.0;
            Stackable = true;
            Hue = 0x475;
            Name = "Bag of Cocoa";
        }

        public BagOfCocoa( Serial serial )
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

    public class BagOfCoffee : Item
    {
        [Constructable]
        public BagOfCoffee()
            : base( 0x1045 )
        {
            Weight = 5.0;
            Stackable = true;
            Hue = 0x46A;
            Name = "Bag of Coffee";
        }

        public BagOfCoffee( Serial serial )
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

    public class BagOfCornmeal : Item
    {
        [Constructable]
        public BagOfCornmeal()
            : base( 0x1045 )
        {
            Weight = 5.0;
            Stackable = true;
            Hue = 0x466;
            Name = "Bag of Cornmeal";
        }

        public BagOfCornmeal( Serial serial )
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

    public class BagOfOats : Item
    {
        [Constructable]
        public BagOfOats()
            : base( 0x1045 )
        {
            Weight = 5.0;
            Stackable = true;
            Hue = 0x45E;
            Name = "Bag of Oats";
        }

        public BagOfOats( Serial serial )
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

    public class BagOfRicemeal : Item
    {
        [Constructable]
        public BagOfRicemeal()
            : base( 0x1045 )
        {
            Weight = 5.0;
            Stackable = true;
            Hue = 0x303;
            Name = "Bag of Ricemeal";
        }

        public BagOfRicemeal( Serial serial )
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

    public class BagOfSoy : Item
    {
        [Constructable]
        public BagOfSoy()
            : base( 0x1045 )
        {
            Weight = 5.0;
            Stackable = true;
            Hue = 0x3D5;
            Name = "Bag of Soy";
        }

        public BagOfSoy( Serial serial )
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

    public class BagOfSugar : Item
    {
        [Constructable]
        public BagOfSugar()
            : base( 0x1045 )
        {
            Weight = 5.0;
            Stackable = true;
            Hue = 0x430;
            Name = "Bag of Sugar";
        }

        public BagOfSugar( Serial serial )
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