namespace Server.Items
{
    public class BeefHock : Food
    {
        [Constructable]
        public BeefHock()
            : this( 1 )
        {
        }

        [Constructable]
        public BeefHock( int amount )
            : base( amount, 0x9D3 )
        {
            Stackable = true;
            Weight = 1.0;
            Amount = amount;
            Name = "Beef Hock";
            Hue = 0x459;
        }

        public BeefHock( Serial serial )
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

    public class SmokedRibs : Food
    {
        [Constructable]
        public SmokedRibs()
            : this( 1 )
        {
        }

        [Constructable]
        public SmokedRibs( int amount )
            : base( amount, 0x9F2 )
        {
            Weight = 0.2;
            FillFactor = 4;
            Name = "smoked ribs";
            Hue = 0x340;
        }

        public SmokedRibs( Serial serial )
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
    }

    public class SpicedRibs : Food
    {
        [Constructable]
        public SpicedRibs()
            : this( 1 )
        {
        }

        [Constructable]
        public SpicedRibs( int amount )
            : base( amount, 0x9F2 )
        {
            Weight = 1.0;
            FillFactor = 5;
            Name = "spiced ribs";
            Hue = 0x2F4;
        }

        public SpicedRibs( Serial serial )
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
    }

    public class EnergeticRibs : Food
    {
        [Constructable]
        public EnergeticRibs()
            : this( 1 )
        {
        }

        [Constructable]
        public EnergeticRibs( int amount )
            : base( amount, 0x9F2 )
        {
            Weight = 1.0;
            FillFactor = 6;
            Name = "energetic ribs";
            Hue = 0xED;
        }

        public EnergeticRibs( Serial serial )
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
    }

    public class SmokedFishSteak : Food
    {
        [Constructable]
        public SmokedFishSteak()
            : this( 1 )
        {
        }

        [Constructable]
        public SmokedFishSteak( int amount )
            : base( amount, 0x97B )
        {
            FillFactor = 3;
            Name = "smoked fish steak";
            Hue = 0x340;
        }

        public SmokedFishSteak( Serial serial )
            : base( serial )
        {
        }

        public override double DefaultWeight
        {
            get { return 0.1; }
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
    }

    public class SpicedFishSteak : Food
    {
        [Constructable]
        public SpicedFishSteak()
            : this( 1 )
        {
        }

        [Constructable]
        public SpicedFishSteak( int amount )
            : base( amount, 0x97B )
        {
            FillFactor = 4;
            Name = "spiced fish steak";
            Hue = 0x2F4;
        }

        public SpicedFishSteak( Serial serial )
            : base( serial )
        {
        }

        public override double DefaultWeight
        {
            get { return 0.1; }
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
    }

    public class EnergeticFishSteak : Food
    {
        [Constructable]
        public EnergeticFishSteak()
            : this( 1 )
        {
        }

        [Constructable]
        public EnergeticFishSteak( int amount )
            : base( amount, 0x97B )
        {
            FillFactor = 5;
            Name = "energetic fish steak";
            Hue = 0xED;
        }

        public EnergeticFishSteak( Serial serial )
            : base( serial )
        {
        }

        public override double DefaultWeight
        {
            get { return 0.1; }
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
    }

    /* Candy
    [Flipable( 0x9B5, 0x9B5 )]
    public class Candy : Food
    {
        [Constructable]
        public Candy()
            : this( 1 )
        {
        }

        [Constructable]
        public Candy( int amount )
            : base( amount, 0x9B5 )
        {
            Weight = 1.0;
            FillFactor = 1;
            Hue = 24;
            Name = "Candy";
        }

        public Candy( Serial serial )
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

    public class CheeseWedgeSmall : Food
    {
        [Constructable]
        public CheeseWedgeSmall()
            : this( 1 )
        {
        }

        [Constructable]
        public CheeseWedgeSmall( int amount )
            : base( amount, 0x97C )
        {
            Weight = 0.1;
            FillFactor = 3;
        }

        public CheeseWedgeSmall( Serial serial )
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

    public class PorkHock : Food
    {
        [Constructable]
        public PorkHock()
            : this( 1 )
        {
        }

        [Constructable]
        public PorkHock( int amount )
            : base( amount, 0x9D3 )
        {
            Stackable = true;
            Weight = 2.0;
            Amount = amount;
            Name = "Pork Hock";
            Hue = 0x457;
        }

        public PorkHock( Serial serial )
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

    /* Silverleaf
    public class Silverleaf : Food
    {
        [Constructable]
        public Silverleaf()
            : this( 1 )
        {
        }

        [Constructable]
        public Silverleaf( int amount )
            : base( amount, 0x9B6 )
        {
            Name = "Silverleaf meal";
            Hue = 96;
            Weight = 0.5;
            FillFactor = 0;
        }

        public Silverleaf( Serial serial )
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

    public class SlicedTurkey : Food
    {
        [Constructable]
        public SlicedTurkey()
            : this( 1 )
        {
        }

        [Constructable]
        public SlicedTurkey( int amount )
            : base( amount, 0x1E1F )
        {
            Weight = 0.2;
            FillFactor = 3;
            Name = "Sliced Turkey";
            Hue = 0x457;
            Stackable = true;
        }

        public SlicedTurkey( Serial serial )
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

    /* Silverleaf
    public class Spam : Food
    {
        [Constructable]
        public Spam()
            : this( 1 )
        {
        }

        [Constructable]
        public Spam( int amount )
            : base( amount, 0x1044 )
        {
            FillFactor = 7;
        }

        public Spam( Serial serial )
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

    [Flipable( 0xf2a, 0xf2b )]
    public class Strawberries : Food
    {
        [Constructable]
        public Strawberries()
            : this( 1 )
        {
        }

        [Constructable]
        public Strawberries( int amount )
            : base( amount, 0xf2a )
        {
            Name = "strawberries";
            Hue = 33;
            Weight = 1.0;
            FillFactor = 1;
        }

        public Strawberries( Serial serial )
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

    public class TurkeyHock : Food
    {
        [Constructable]
        public TurkeyHock()
            : this( 1 )
        {
        }

        [Constructable]
        public TurkeyHock( int amount )
            : base( amount, 0x9D3 )
        {
            Stackable = true;
            Weight = 2.0;
            Amount = amount;
            Name = "Turkey Hock";
            Hue = 0x457;
        }

        public TurkeyHock( Serial serial )
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

    public class BaconSlab : Food, ICarvable
    {
        [Constructable]
        public BaconSlab()
            : this( 1 )
        {
        }

        [Constructable]
        public BaconSlab( int amount )
            : base( amount, 0x976 )
        {
            Weight = 1.0;
            FillFactor = 5;
        }

        public BaconSlab( Serial serial )
            : base( serial )
        {
        }

        #region ICarvable Members
        public void Carve( Mobile from, Item item )
        {
            if( !Movable )
                return;

            base.ScissorHelper( from, new Bacon(), 5 );
            from.SendMessage( "You cut the slab into slices." );
        }
        #endregion

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

    public class CookedSteak : Food
    {
        [Constructable]
        public CookedSteak()
            : this( 1 )
        {
        }

        [Constructable]
        public CookedSteak( int amount )
            : base( amount, 0x097B )
        {
            Weight = 1.0;
            FillFactor = 5;
            Name = "Cooked Steak";
        }

        public CookedSteak( Serial serial )
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

    public class HamSlices : Food
    {
        [Constructable]
        public HamSlices()
            : this( 1 )
        {
        }

        [Constructable]
        public HamSlices( int amount )
            : base( amount, 0x1E1F )
        {
            Weight = 0.2;
            FillFactor = 1;
        }

        public HamSlices( Serial serial )
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

    /* WeddingCake
    public class WeddingCake : Food
    {
        [Constructable]
        public WeddingCake()
            : base( 0x3BCC )
        {
            Name = "Wedding Cake";
            Weight = 10.0;
            FillFactor = 10;
        }

        public WeddingCake( Serial serial )
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

    /* SliceOfWeddingCake
    public class SliceOfWeddingCake : Food
    {
        [Constructable]
        public SliceOfWeddingCake()
            : this( 1 )
        {
        }

        [Constructable]
        public SliceOfWeddingCake( int amount )
            : base( amount, 0x3BCB )
        {
            Name = "Slice of Wedding Cake";
            Weight = 1.0;
            FillFactor = 1;
        }

        public SliceOfWeddingCake( Serial serial )
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

    /* PennyCandy
    public class PennyCandy : Food
    {
        [Constructable]
        public PennyCandy()
            : this( 1 )
        {
        }

        [Constructable]
        public PennyCandy( int amount )
            : base( amount, 0x3BC7 )
        {
            Name = "Candy";
            Weight = 1.0;
            FillFactor = 1;
        }

        public PennyCandy( Serial serial )
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

    /* SliceOfCake
    [Flipable( 0x3BC5, 0x3BC4 )]
    public class SliceOfCake : Food
    {
        [Constructable]
        public SliceOfCake()
            : this( 1 )
        {
        }

        [Constructable]
        public SliceOfCake( int amount )
            : base( amount, 0x3BC5 )
        {
            Name = "Slice of Cake";
            Weight = 1.0;
            FillFactor = 1;
        }

        public SliceOfCake( Serial serial )
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

    [Flipable( 0x3BC0, 0x3BBF )]
    public class RoastHam : Food
    {
        [Constructable]
        public RoastHam()
            : base( 0x09C9 )
        {
            Name = "Roast Ham";
            Weight = 10.0;
            FillFactor = 10;
            Hue = 0x289; // brown
        }

        public RoastHam( Serial serial )
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

    /* BirthdayCake
    public class BirthdayCake : Food
    {
        [Constructable]
        public BirthdayCake()
            : base( 0x3BBD )
        {
            Name = "Birthday Cake";
            Weight = 10.0;
            FillFactor = 10;
        }

        public BirthdayCake( Serial serial )
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

    /* DarkSushiTray
    [Flipable( 10302, 10303 )]
    public class DarkSushiTray : Food
    {
        [Constructable]
        public DarkSushiTray()
            : base( 1, Utility.RandomList( 10302, 10303 ) )
        {
            FillFactor = 5;
        }

        public DarkSushiTray( Serial serial )
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

    /* LightSushiTray
    [Flipable( 10304, 10305 )]
    public class LightSushiTray : Food
    {
        [Constructable]
        public LightSushiTray()
            : base( 1, Utility.RandomList( 10304, 10305 ) )
        {
            FillFactor = 5;
        }

        public LightSushiTray( Serial serial )
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

    #region fish
    public class HalibutFishSteak : Food
    {
        [Constructable]
        public HalibutFishSteak()
            : this( 1 )
        {
        }

        [Constructable]
        public HalibutFishSteak( int amount )
            : base( amount, 0x97B )
        {
            FillFactor = 3;
            Name = "Halibut Fish Steak";
        }

        public HalibutFishSteak( Serial serial )
            : base( serial )
        {
        }

        public override double DefaultWeight
        {
            get { return 0.1; }
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

    public class FlukeFishSteak : Food
    {
        [Constructable]
        public FlukeFishSteak()
            : this( 1 )
        {
        }

        [Constructable]
        public FlukeFishSteak( int amount )
            : base( amount, 0x97B )
        {
            FillFactor = 3;
            Name = "Fluke Fish Steak";
        }

        public FlukeFishSteak( Serial serial )
            : base( serial )
        {
        }

        public override double DefaultWeight
        {
            get { return 0.1; }
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

    public class MahiFishSteak : Food
    {
        [Constructable]
        public MahiFishSteak()
            : this( 1 )
        {
        }

        [Constructable]
        public MahiFishSteak( int amount )
            : base( amount, 0x97B )
        {
            FillFactor = 3;
            Name = "Mahi Fish Steak";
        }

        public MahiFishSteak( Serial serial )
            : base( serial )
        {
        }

        public override double DefaultWeight
        {
            get { return 0.1; }
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

    public class SalmonFishSteak : Food
    {
        [Constructable]
        public SalmonFishSteak()
            : this( 1 )
        {
        }

        [Constructable]
        public SalmonFishSteak( int amount )
            : base( amount, 0x97B )
        {
            FillFactor = 3;
            Name = "Salmon Fish Steak";
        }

        public SalmonFishSteak( Serial serial )
            : base( serial )
        {
        }

        public override double DefaultWeight
        {
            get { return 0.1; }
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

    public class RedSnapperFishSteak : Food
    {
        [Constructable]
        public RedSnapperFishSteak()
            : this( 1 )
        {
        }

        [Constructable]
        public RedSnapperFishSteak( int amount )
            : base( amount, 0x97B )
        {
            FillFactor = 3;
            Name = "Red Snapper Fish Steak";
        }

        public RedSnapperFishSteak( Serial serial )
            : base( serial )
        {
        }

        public override double DefaultWeight
        {
            get { return 0.1; }
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

    public class ParrotFishSteak : Food
    {
        [Constructable]
        public ParrotFishSteak()
            : this( 1 )
        {
        }

        [Constructable]
        public ParrotFishSteak( int amount )
            : base( amount, 0x97B )
        {
            FillFactor = 3;
            Name = "Parrot Fish Fish Steak";
        }

        public ParrotFishSteak( Serial serial )
            : base( serial )
        {
        }

        public override double DefaultWeight
        {
            get { return 0.1; }
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

    public class TroutFishSteak : Food
    {
        [Constructable]
        public TroutFishSteak()
            : this( 1 )
        {
        }

        [Constructable]
        public TroutFishSteak( int amount )
            : base( amount, 0x97B )
        {
            FillFactor = 3;
            Name = "Trout Fish Steak";
        }

        public TroutFishSteak( Serial serial )
            : base( serial )
        {
        }

        public override double DefaultWeight
        {
            get { return 0.1; }
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

    public class CookedShrimp : Food
    {
        [Constructable]
        public CookedShrimp()
            : this( 1 )
        {
        }

        [Constructable]
        public CookedShrimp( int amount )
            : base( amount, 0x97B )
        {
            FillFactor = 3;
            Name = "Cooked Shrimp";
        }

        public CookedShrimp( Serial serial )
            : base( serial )
        {
        }

        public override double DefaultWeight
        {
            get { return 0.1; }
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
    #endregion
}