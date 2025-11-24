namespace Server.Items
{
    public class BasePie : Food
    {
        [Constructable]
        public BasePie()
            : this( null, 0 )
        {
        }

        [Constructable]
        public BasePie( string desc )
            : this( desc, 0 )
        {
        }

        [Constructable]
        public BasePie( int hue )
            : this( null, hue )
        {
        }

        [Constructable]
        public BasePie( string desc, int hue )
            : base( 0x1041 )
        {
            Weight = 1.0;
            FillFactor = 5;
            if( string.IsNullOrEmpty( desc ) )
                Name = string.Format( "a {0} pie", desc );
            else
                Name = "a pie";

            Hue = hue;
        }

        #region serialization
        public BasePie( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }

    public class BananaCake : Food
    {
        [Constructable]
        public BananaCake()
            : base( 0x9E9 )
        {
            Name = "a banana cake";
            Weight = 1.0;
            FillFactor = 15;
            Hue = 354;
        }

        public BananaCake( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }

    public class CantaloupeCake : Food
    {
        [Constructable]
        public CantaloupeCake()
            : base( 0x9E9 )
        {
            Name = "a cantaloupe cake";
            Weight = 1.0;
            FillFactor = 15;
            Hue = 145;
        }

        public CantaloupeCake( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }

    public class CarrotCake : Food
    {
        [Constructable]
        public CarrotCake()
            : base( 0x9E9 )
        {
            Name = "a carrot cake";
            Weight = 1.0;
            FillFactor = 15;
            Hue = 248;
        }

        public CarrotCake( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }

    public class ChickenPotPie : Food
    {
        [Constructable]
        public ChickenPotPie()
            : base( 0x1041 )
        {
            Name = "baked chicken pot pie";
            Weight = 1.0;
            FillFactor = 10;
        }

        public ChickenPotPie( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }

    public class CoconutCake : Food
    {
        [Constructable]
        public CoconutCake()
            : base( 0x9E9 )
        {
            Name = "a coconut cake";
            Weight = 1.0;
            FillFactor = 15;
            Hue = 556;
        }

        public CoconutCake( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }

    public class CookedHeadlessFish : Food, ICarvable
    {
        public void Carve( Mobile from, Item item )
        {
            base.ScissorHelper( from, new FishSteak(), 4 );
        }

        [Constructable]
        public CookedHeadlessFish()
            : this( 1 )
        {
        }

        [Constructable]
        public CookedHeadlessFish( int amount )
            : base( Utility.Random( 7708, 2 ) )
        {
            Stackable = true;
            Weight = 0.4;
            Amount = amount;
            FillFactor = 12;
        }

        public CookedHeadlessFish( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }

    public class Donuts : Food
    {
        [Constructable]
        public Donuts()
            : this( 1 )
        {
        }

        [Constructable]
        public Donuts( int amount )
            : base( amount, 6867 )
        {
            Weight = 2.0;
            FillFactor = 3;
        }

        public Donuts( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }

    public class FishHeads : Food
    {
        [Constructable]
        public FishHeads()
            : this( 1 )
        {
        }

        [Constructable]
        public FishHeads( int amount )
            : base( Utility.Random( 7705, 2 ) )
        {
            Weight = 0.1;
            Amount = amount;
            FillFactor = 0;
        }

        public override void OnDoubleClick( Mobile from )
        {
            from.SendMessage( "*ugh*! That's cat food!" );
            return;
        }

        public FishHeads( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }

    public class FruitCake : Food
    {
        [Constructable]
        public FruitCake()
            : this( 0 )
        {
        }

        [Constructable]
        public FruitCake( int Color )
            : base( 0x9E9 )
        {
            Name = "a fruit cake";
            Weight = 1.0;
            FillFactor = 15;
            Hue = Color;
        }

        public FruitCake( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }

    public class GrapeCake : Food
    {
        [Constructable]
        public GrapeCake()
            : base( 0x9E9 )
        {
            Name = "a grape cake";
            Weight = 1.0;
            FillFactor = 15;
            Hue = 21;
        }

        public GrapeCake( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }

    public class HoneydewMelonCake : Food
    {
        [Constructable]
        public HoneydewMelonCake()
            : base( 0x9E9 )
        {
            Name = "a honeydew melon cake";
            Weight = 1.0;
            FillFactor = 15;
            Hue = 61;
        }

        public HoneydewMelonCake( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }

    public class KeyLimeCake : Food
    {
        [Constructable]
        public KeyLimeCake()
            : base( 0x9E9 )
        {
            Name = "a key lime cake";
            Weight = 1.0;
            FillFactor = 15;
            Hue = 71;
        }

        public KeyLimeCake( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }

    public class KeyLimePie : Food
    {
        [Constructable]
        public KeyLimePie()
            : base( 0x1041 )
        {
            Name = "baked key lime pie";
            Weight = 1.0;
            FillFactor = 5;
        }

        public KeyLimePie( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }

    public class LemonCake : Food
    {
        [Constructable]
        public LemonCake()
            : base( 0x9E9 )
        {
            Name = "a lemon cake";
            Weight = 1.0;
            FillFactor = 15;
            Hue = 53;
        }

        public LemonCake( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }

    public class MeatCake : Food
    {
        [Constructable]
        public MeatCake()
            : this( 0 )
        {
        }

        [Constructable]
        public MeatCake( int Color )
            : base( 0x9E9 )
        {
            Name = "a meat cake";
            Weight = 1.0;
            FillFactor = 15;
            Hue = Color;
        }

        public MeatCake( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }

    public class PeachCake : Food
    {
        [Constructable]
        public PeachCake()
            : base( 0x9E9 )
        {
            Name = "a peach cake";
            Weight = 1.0;
            FillFactor = 15;
            Hue = 46;
        }

        public PeachCake( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }

    public class Pizza : Food
    {
        private string m_Desc;

        [CommandProperty( AccessLevel.Counselor )]
        public string Desc
        {
            get { return m_Desc; }
            set
            {
                m_Desc = value;

                if( m_Desc != null )
                    Name = string.Format( "cooked {0} pizza", m_Desc );

                InvalidateProperties();
            }
        }

        [Constructable]
        public Pizza()
            : this( "cheese", 0 )
        {
        }

        [Constructable]
        public Pizza( int color )
            : this( "cheese", color )
        {
        }

        [Constructable]
        public Pizza( string desc )
            : this( desc, 0 )
        {
        }

        [Constructable]
        public Pizza( string desc, int color )
            : base( 0x1040 )
        {
            Weight = 1.0;
            FillFactor = 6;
            Desc = string.IsNullOrEmpty( Desc ) ? "cheese" : desc;
            Hue = color;
        }

        public Pizza( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)1 );

            writer.Write( (string)m_Desc );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            switch( version )
            {
                case 1:
                    m_Desc = reader.ReadString();
                    break;
            }
        }
    }

    public class PlateOfCookies : Food
    {
        [Constructable]
        public PlateOfCookies()
            : this( 1 )
        {
        }

        [Constructable]
        public PlateOfCookies( int amount )
            : base( amount, 0x160C )
        {
            Weight = 0.2;
            FillFactor = 1;
            Name = "a plate of cookies";
        }

        public PlateOfCookies( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)1 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }

    public class PumpkinCake : Food
    {
        [Constructable]
        public PumpkinCake()
            : base( 0x9E9 )
        {
            Name = "a pumpkin cake";
            Weight = 1.0;
            FillFactor = 15;
            Hue = 243;
        }

        public PumpkinCake( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }

    public class VegePie : Food
    {
        [Constructable]
        public VegePie()
            : base( 0x1041 )
        {
            Weight = 1.0;
            FillFactor = 5;
            Name = "vegetable pie";
        }

        public VegePie( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }

    public class VegetableCake : Food
    {
        [Constructable]
        public VegetableCake()
            : this( 0 )
        {
        }

        [Constructable]
        public VegetableCake( int Color )
            : base( 0x9E9 )
        {
            Name = "a vegetable cake";
            Weight = 1.0;
            FillFactor = 15;
            Hue = Color;
        }

        public VegetableCake( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }

    public class WatermelonCake : Food
    {
        [Constructable]
        public WatermelonCake()
            : base( 0x9E9 )
        {
            Name = "a watermelon cake";
            Weight = 1.0;
            FillFactor = 15;
            Hue = 34;
        }

        public WatermelonCake( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }
}