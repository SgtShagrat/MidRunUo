using System;
using Midgard.Engines.AdvancedCooking;
using Server.Targeting;

namespace Server.Items
{
    public class BananaCakeMix : CookableFood
    {
        public override int LabelNumber
        {
            get { return 1041002; }
        }

        [Constructable]
        public BananaCakeMix()
            : base( 0x103F, 75 )
        {
            Name = "banana cake mix";
            Hue = 354;
        }

        public BananaCakeMix( Serial serial )
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

        public override Food Cook()
        {
            return new BananaCake();
        }
    }

    public class CantaloupeCakeMix : CookableFood
    {
        public override int LabelNumber
        {
            get { return 1041002; }
        }

        [Constructable]
        public CantaloupeCakeMix()
            : base( 0x103F, 75 )
        {
            Name = "cantaloupe cake mix";
            Hue = 145;
        }

        public CantaloupeCakeMix( Serial serial )
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

        public override Food Cook()
        {
            return new CantaloupeCake();
        }
    }

    public class CarrotCakeMix : CookableFood
    {
        public override int LabelNumber
        {
            get { return 1041002; }
        }

        [Constructable]
        public CarrotCakeMix()
            : base( 0x103F, 75 )
        {
            Name = "carrot cake mix";
            Hue = 248;
        }

        public CarrotCakeMix( Serial serial )
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

        public override Food Cook()
        {
            return new CarrotCake();
        }
    }

    public class CoconutCakeMix : CookableFood
    {
        public override int LabelNumber
        {
            get { return 1041002; }
        }

        [Constructable]
        public CoconutCakeMix()
            : base( 0x103F, 75 )
        {
            Name = "coconut cake mix";
            Hue = 556;
        }

        public CoconutCakeMix( Serial serial )
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

        public override Food Cook()
        {
            return new CoconutCake();
        }
    }

    public class FruitCakeMix : CookableFood
    {
        public override int LabelNumber
        {
            get { return 1041002; }
        }

        [Constructable]
        public FruitCakeMix()
            : base( 0x103F, 75 )
        {
            Name = "fruit cake mix";
        }

        public FruitCakeMix( Serial serial )
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

        public override Food Cook()
        {
            return new FruitCake( Hue );
        }
    }

    public class GrapeCakeMix : CookableFood
    {
        public override int LabelNumber
        {
            get { return 1041002; }
        }

        [Constructable]
        public GrapeCakeMix()
            : base( 0x103F, 75 )
        {
            Name = "grape cake mix";
            Hue = 21;
        }

        public GrapeCakeMix( Serial serial )
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

        public override Food Cook()
        {
            return new GrapeCake();
        }
    }

    public class HoneydewMelonCakeMix : CookableFood
    {
        public override int LabelNumber
        {
            get { return 1041002; }
        }

        [Constructable]
        public HoneydewMelonCakeMix()
            : base( 0x103F, 75 )
        {
            Name = "honeydew melon cake mix";
            Hue = 61;
        }

        public HoneydewMelonCakeMix( Serial serial )
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

        public override Food Cook()
        {
            return new HoneydewMelonCake();
        }
    }

    public class KeyLimeCakeMix : CookableFood
    {
        public override int LabelNumber
        {
            get { return 1041002; }
        }

        [Constructable]
        public KeyLimeCakeMix()
            : base( 0x103F, 75 )
        {
            Name = "key lime cake mix";
            Hue = 71;
        }

        public KeyLimeCakeMix( Serial serial )
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

        public override Food Cook()
        {
            return new KeyLimeCake();
        }
    }

    public class LemonCakeMix : CookableFood
    {
        public override int LabelNumber
        {
            get { return 1041002; }
        }

        [Constructable]
        public LemonCakeMix()
            : base( 0x103F, 75 )
        {
            Name = "lemon cake mix";
            Hue = 53;
        }

        public LemonCakeMix( Serial serial )
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

        public override Food Cook()
        {
            return new LemonCake();
        }
    }

    public class MeatCakeMix : CookableFood
    {
        public override int LabelNumber
        {
            get { return 1041002; }
        }

        [Constructable]
        public MeatCakeMix()
            : base( 0x103F, 75 )
        {
            Name = "meat cake mix";
        }

        public MeatCakeMix( Serial serial )
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

        public override Food Cook()
        {
            return new MeatCake( Hue );
        }
    }

    public class PeachCakeMix : CookableFood
    {
        public override int LabelNumber
        {
            get { return 1041002; }
        }

        [Constructable]
        public PeachCakeMix()
            : base( 0x103F, 75 )
        {
            Name = "peach cake mix";
            Hue = 46;
        }

        public PeachCakeMix( Serial serial )
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

        public override Food Cook()
        {
            return new PeachCake();
        }
    }

    public class PumpkinCakeMix : CookableFood
    {
        public override int LabelNumber
        {
            get { return 1041002; }
        }

        [Constructable]
        public PumpkinCakeMix()
            : base( 0x103F, 75 )
        {
            Name = "pumpkin cake mix";
            Hue = 243;
        }

        public PumpkinCakeMix( Serial serial )
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

        public override Food Cook()
        {
            return new PumpkinCake();
        }
    }

    public class UnbakedChickenPotPie : CookableFood
    {
        [Constructable]
        public UnbakedChickenPotPie()
            : base( 0x1042, 25 )
        {
            Name = "unbaked chicken pot pie";
        }

        public UnbakedChickenPotPie( Serial serial )
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

        public override Food Cook()
        {
            return new ChickenPotPie();
        }
    }

    public class UnbakedKeyLimePie : CookableFood
    {
        [Constructable]
        public UnbakedKeyLimePie()
            : base( 0x1042, 25 )
        {
            Name = "unbaked key lime pie";
        }

        public UnbakedKeyLimePie( Serial serial )
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

        public override Food Cook()
        {
            return new KeyLimePie();
        }
    }

    public class UnbakedVegePie : CookableFood
    {
        [Constructable]
        public UnbakedVegePie()
            : this( null, 0 )
        {
        }

        [Constructable]
        public UnbakedVegePie( string Desc )
            : this( Desc, 0 )
        {
        }

        [Constructable]
        public UnbakedVegePie( int Color )
            : this( null, Color )
        {
        }

        [Constructable]
        public UnbakedVegePie( string desc, int hue )
            : base( 0x1042, 25 )
        {
            Name = string.IsNullOrEmpty( desc )
                       ? Name = "unbaked vegetable pie"
                       : string.Format( "unbaked {0} vegetable pie", desc );
            Hue = hue;
        }

        public UnbakedVegePie( Serial serial )
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

        public override Food Cook()
        {
            return new VegePie();
        }
    }

    public class RawBacon : CookableFood, IUncookedEatable
    {
        [Constructable]
        public RawBacon()
            : this( 1 )
        {
        }

        [Constructable]
        public RawBacon( int amount )
            : base( 0x979, 0 )
        {
            Name = "raw slice of bacon";
            Stackable = true;
            Amount = amount;
            Hue = 336;
        }

        public RawBacon( Serial serial )
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

        public override Food Cook()
        {
            return new Bacon();
        }
    }

    public class RawBaconSlab : CookableFood, ICarvable, IUncookedEatable
    {
        [Constructable]
        public RawBaconSlab()
            : this( 1 )
        {
        }

        [Constructable]
        public RawBaconSlab( int amount )
            : base( 0x976, 0 )
        {
            Name = "raw slab of bacon";
            Stackable = true;
            Hue = 41;
            Amount = amount;
        }

        public void Carve( Mobile from, Item item )
        {
            if( !Movable )
                return;

            base.ScissorHelper( from, new RawBacon(), 5 );
            from.SendMessage( "You cut the slab into slices." );
        }

        public RawBaconSlab( Serial serial )
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

        public override Food Cook()
        {
            return new BaconSlab();
        }
    }

    public class RawHam : CookableFood, ICarvable, IUncookedEatable
    {
        [Constructable]
        public RawHam()
            : this( 1 )
        {
        }

        [Constructable]
        public RawHam( int amount )
            : base( 0x9C9, 0 )
        {
            Name = "raw ham";
            Stackable = true;
            Hue = 41;
            Amount = amount;
        }

        public void Carve( Mobile from, Item item )
        {
            if( !Movable )
                return;

            base.ScissorHelper( from, new RawHamSlices(), 5 );
            from.SendMessage( "You slice the ham." );
        }

        public RawHam( Serial serial )
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

        public override Food Cook()
        {
            return new Ham();
        }
    }

    public class RawHamSlices : CookableFood, IUncookedEatable
    {
        [Constructable]
        public RawHamSlices()
            : this( 1 )
        {
        }

        [Constructable]
        public RawHamSlices( int amount )
            : base( 0x1E1F, 0 )
        {
            Name = "raw sliced ham";
            Stackable = true;
            Amount = amount;
            Hue = 336;
        }

        public RawHamSlices( Serial serial )
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

        public override Food Cook()
        {
            return new HamSlices();
        }
    }

    public class RawHeadlessFish : CookableFood, ICarvable, IUncookedEatable
    {
        public void Carve( Mobile from, Item item )
        {
            base.ScissorHelper( from, new RawFishSteak(), 4 );
        }

        [Constructable]
        public RawHeadlessFish()
            : this( 1 )
        {
        }

        [Constructable]
        public RawHeadlessFish( int amount )
            : base( Utility.Random( 7703, 2 ), 20 )
        {
            Stackable = true;
            Weight = 0.6;
            Amount = amount;
        }

        public override Food Cook()
        {
            return new CookedHeadlessFish();
        }

        public RawHeadlessFish( Serial serial )
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

    public class RawSteak : CookableFood, IUncookedEatable
    {
        [Constructable]
        public RawSteak()
            : this( 1 )
        {
        }

        [Constructable]
        public RawSteak( int amount )
            : base( 0x097A, 10 )
        {
            Name = "Raw Steak";
            Stackable = true;
            Amount = amount;
        }

        public RawSteak( Serial serial )
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

        public override Food Cook()
        {
            return new CookedSteak();
        }
    }

    public class RawHalibutSteak : CookableFood, IUncookedEatable
    {
        public override double DefaultWeight
        {
            get { return 0.1; }
        }

        [Constructable]
        public RawHalibutSteak()
            : this( 1 )
        {
        }

        [Constructable]
        public RawHalibutSteak( int amount )
            : base( 0x097A, 10 )
        {
            Stackable = true;
            Amount = amount;
            Name = "Raw Halibut Steak";
        }

        public RawHalibutSteak( Serial serial )
            : base( serial )
        {
        }

        public override Food Cook()
        {
            return new HalibutFishSteak();
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

    public class RawFlukeSteak : CookableFood, IUncookedEatable
    {
        public override double DefaultWeight
        {
            get { return 0.1; }
        }

        [Constructable]
        public RawFlukeSteak()
            : this( 1 )
        {
        }

        [Constructable]
        public RawFlukeSteak( int amount )
            : base( 0x097A, 10 )
        {
            Stackable = true;
            Amount = amount;
            Name = "Raw Fluke Steak";
        }

        public RawFlukeSteak( Serial serial )
            : base( serial )
        {
        }

        public override Food Cook()
        {
            return new FlukeFishSteak();
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

    public class RawMahiSteak : CookableFood, IUncookedEatable
    {
        public override double DefaultWeight
        {
            get { return 0.1; }
        }

        [Constructable]
        public RawMahiSteak()
            : this( 1 )
        {
        }

        [Constructable]
        public RawMahiSteak( int amount )
            : base( 0x097A, 10 )
        {
            Stackable = true;
            Amount = amount;
            Name = "Raw Mahi-Mahi Steak";
        }

        public RawMahiSteak( Serial serial )
            : base( serial )
        {
        }

        public override Food Cook()
        {
            return new MahiFishSteak();
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

    public class RawSalmonSteak : CookableFood, IUncookedEatable
    {
        public override double DefaultWeight
        {
            get { return 0.1; }
        }

        [Constructable]
        public RawSalmonSteak()
            : this( 1 )
        {
        }

        [Constructable]
        public RawSalmonSteak( int amount )
            : base( 0x097A, 10 )
        {
            Stackable = true;
            Amount = amount;
            Name = "Raw Salmon Steak";
        }

        public RawSalmonSteak( Serial serial )
            : base( serial )
        {
        }

        public override Food Cook()
        {
            return new SalmonFishSteak();
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

    public class RawRedSnapperSteak : CookableFood, IUncookedEatable
    {
        public override double DefaultWeight
        {
            get { return 0.1; }
        }

        [Constructable]
        public RawRedSnapperSteak()
            : this( 1 )
        {
        }

        [Constructable]
        public RawRedSnapperSteak( int amount )
            : base( 0x097A, 10 )
        {
            Stackable = true;
            Amount = amount;
            Name = "Raw Red Snapper Steak";
        }

        public RawRedSnapperSteak( Serial serial )
            : base( serial )
        {
        }

        public override Food Cook()
        {
            return new RedSnapperFishSteak();
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

    public class RawParrotFishSteak : CookableFood, IUncookedEatable
    {
        public override double DefaultWeight
        {
            get { return 0.1; }
        }

        [Constructable]
        public RawParrotFishSteak()
            : this( 1 )
        {
        }

        [Constructable]
        public RawParrotFishSteak( int amount )
            : base( 0x097A, 10 )
        {
            Stackable = true;
            Amount = amount;
            Name = "Raw Parrot Fish Steak";
        }

        public RawParrotFishSteak( Serial serial )
            : base( serial )
        {
        }

        public override Food Cook()
        {
            return new ParrotFishSteak();
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

    public class RawTroutSteak : CookableFood, IUncookedEatable
    {
        public override double DefaultWeight
        {
            get { return 0.1; }
        }

        [Constructable]
        public RawTroutSteak()
            : this( 1 )
        {
        }

        [Constructable]
        public RawTroutSteak( int amount )
            : base( 0x097A, 10 )
        {
            Stackable = true;
            Amount = amount;
            Name = "Raw Trout Steak";
        }

        public RawTroutSteak( Serial serial )
            : base( serial )
        {
        }

        public override Food Cook()
        {
            return new TroutFishSteak();
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

    public class RawShrimp : CookableFood, IUncookedEatable
    {
        public override double DefaultWeight
        {
            get { return 0.1; }
        }

        [Constructable]
        public RawShrimp()
            : this( 1 )
        {
        }

        [Constructable]
        public RawShrimp( int amount )
            : base( 0x097A, 10 )
        {
            Stackable = true;
            Amount = amount;
            Name = "Raw Shrimp";
        }

        public RawShrimp( Serial serial )
            : base( serial )
        {
        }

        public override Food Cook()
        {
            return new CookedShrimp();
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

    public class UncookedDonuts : CookableFood
    {
        [Constructable]
        public UncookedDonuts()
            : base( 6867, 0 )
        {
            Hue = 51;
            Name = "uncooked donuts";
        }

        public UncookedDonuts( Serial serial )
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

        public override Food Cook()
        {
            return new Donuts();
        }
    }

    public class UncookedFrenchBread : CookableFood
    {
        [Constructable]
        public UncookedFrenchBread()
            : base( 0x98C, 0 )
        {
            Hue = 51;
            Name = "uncooked french bread";
        }

        public UncookedFrenchBread( Serial serial )
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

        public override Food Cook()
        {
            return new FrenchBread();
        }
    }

    public class UncookedPizza : Item
    {
        public override int LabelNumber
        {
            get { return 1024227; }
        }

        private Food m_CookedFood
        {
            get { return new Pizza( Desc, Hue ); }
        }

        private int m_MinSkill;
        private int m_MaxSkill;

        private string m_Desc;
        private string m_Ingredients;

        public string Desc
        {
            get { return m_Desc; }
            set
            {
                m_Desc = value;
                if( !String.IsNullOrEmpty( m_Desc ) )
                    Name = string.Format( "uncooked {0} pizza", m_Desc );

                InvalidateProperties();
            }
        }

        public string Ingredients
        {
            get { return m_Ingredients; }
            set
            {
                m_Ingredients = value;

                InvalidateProperties();
            }
        }

        public Food CookedFood
        {
            get { return m_CookedFood; }
        }

        public int MinSkill
        {
            get { return m_MinSkill; }
        }

        public int MaxSkill
        {
            get { return m_MaxSkill; }
        }

        [Constructable]
        public UncookedPizza()
            : this( "cheese" )
        {
        }

        [Constructable]
        public UncookedPizza( string desc )
            : this( desc, 0 )
        {
        }

        [Constructable]
        public UncookedPizza( int hue )
            : this( "cheese", hue )
        {
        }

        [Constructable]
        public UncookedPizza( string desc, int hue )
            : base( 0x1083 )
        {
            if( hue != 0 )
                Hue = hue;

            if( !String.IsNullOrEmpty( desc ) )
                Desc = desc;

            m_MinSkill = 0;
            m_MaxSkill = 100;
        }

        #region serialization

        public UncookedPizza( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 1 );

            writer.Write( m_Desc );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            switch( version )
            {
                case 1:
                    {
                        m_Desc = reader.ReadString();
                        break;
                    }
            }
        }

        #endregion

        public override void AddNameProperties( ObjectPropertyList list )
        {
            base.AddNameProperties( list );

            if( !String.IsNullOrEmpty( m_Ingredients ) )
            {
                list.Add( 1060847, "With\t{0}", m_Ingredients );
            }
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( !Movable )
                return;

            from.Target = new InternalTarget( this );
        }

        public class InternalTarget : Target
        {
            private UncookedPizza m_Food;

            public InternalTarget( UncookedPizza item )
                : base( 1, false, TargetFlags.None )
            {
                m_Food = item;
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                if( m_Food.Deleted )
                    return;

                if( targeted is Item && !( (Item)targeted ).Movable )
                    return;

                if( FoodHelper.IsHeatSource( targeted ) )
                {
                    if( from.BeginAction( typeof( Item ) ) )
                    {
                        from.PlaySound( 0x225 );

                        m_Food.Consume();

                        InternalTimer t = new InternalTimer( from, targeted as IPoint3D, from.Map, m_Food.MinSkill,
                                                            m_Food.MaxSkill, m_Food.CookedFood );
                        t.Start();
                    }
                    else
                    {
                        from.SendLocalizedMessage( 500119 );
                    }
                }
                else if( m_Food.Ingredients != null && m_Food.Ingredients.Length > 30 )
                {
                    from.SendMessage( "The pizza has enough toppings already." );
                }
                else if( FoodHelper.IsNamedIngredient( targeted.GetType() ) )
                {
                    string name = FoodHelper.GetNameFromType( targeted.GetType() );

                    if( !String.IsNullOrEmpty( name ) )
                    {
                        from.SendMessage( "You add {0} to the pizza.", name );
                        if( m_Food.Ingredients != null )
                            m_Food.Ingredients += string.Format( ", {0}", name );
                        ( (Item)targeted ).Consume();
                    }
                }
                else if( targeted is Dough )
                {
                    from.SendMessage( "You also make the pizza a thick crust one." );
                }
                else if( targeted is BrightlyColoredEggs || targeted is EasterEggs || targeted is Eggs )
                {
                    if( ( (Item)targeted ).Hue != 0 )
                        m_Food.Hue = ( (Item)targeted ).Hue;
                    from.AddToBackpack( new Eggshells( m_Food.Hue ) );
                }
                else if( FoodHelper.IsTooLargeIngredient( targeted.GetType() ) )
                {
                    from.SendMessage( "That portion is too large. Use a bladed object to cut it up first." );
                }
            }

            private class InternalTimer : Timer
            {
                private Mobile m_From;
                private IPoint3D m_Point;
                private Map m_Map;
                private int Min;
                private int Max;
                private Food m_CookedFood;

                public InternalTimer( Mobile from, IPoint3D p, Map map, int min, int max, Food cookedFood )
                    : base( TimeSpan.FromSeconds( 3.0 ) )
                {
                    m_From = from;
                    m_Point = p;
                    m_Map = map;
                    Min = min;
                    Max = max;
                    m_CookedFood = cookedFood;
                }

                protected override void OnTick()
                {
                    m_From.EndAction( typeof( Item ) );

                    if( m_From.Map != m_Map || ( m_Point != null && m_From.GetDistanceToSqrt( m_Point ) > 3 ) )
                    {
                        m_From.SendLocalizedMessage( 500686 );
                        return;
                    }

                    if( m_From.CheckSkill( SkillName.Cooking, Min, Max ) )
                    {
                        if( m_From.AddToBackpack( m_CookedFood ) )
                            m_From.PlaySound( 0x57 );
                    }
                    else
                    {
                        m_From.PlaySound( 0x57 );
                        m_From.SendLocalizedMessage( 500686 );
                    }
                }
            }
        }
    }

    public class VegetableCakeMix : CookableFood
    {
        public override int LabelNumber
        {
            get { return 1041002; }
        }

        [Constructable]
        public VegetableCakeMix()
            : base( 0x103F, 75 )
        {
            Name = "vegetable cake mix";
        }

        public VegetableCakeMix( Serial serial )
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

        public override Food Cook()
        {
            return new VegetableCake( Hue );
        }
    }

    public class WatermelonCakeMix : CookableFood
    {
        public override int LabelNumber
        {
            get { return 1041002; }
        }

        [Constructable]
        public WatermelonCakeMix()
            : base( 0x103F, 75 )
        {
            Name = "watermelon cake mix";
            Hue = 34;
        }

        public WatermelonCakeMix( Serial serial )
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

        public override Food Cook()
        {
            return new WatermelonCake();
        }
    }
}