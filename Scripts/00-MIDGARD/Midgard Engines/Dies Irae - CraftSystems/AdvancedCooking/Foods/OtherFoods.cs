using System;

namespace Server.Items
{
    public class Meatballs : Food
    {
        [Constructable]
        public Meatballs()
            : this( 1 )
        {
        }

        [Constructable]
        public Meatballs( int amount )
            : base( amount, 0xE74 )
        {
            Weight = 1.0;
            Stackable = true;
            Hue = 0x475;
            FillFactor = 4;
            Name = "Meatballs";
        }

        public Meatballs( Serial serial )
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

    public class Meatloaf : Food
    {
        [Constructable]
        public Meatloaf()
            : this( 1 )
        {
        }

        [Constructable]
        public Meatloaf( int amount )
            : base( amount, 0xE79 )
        {
            Weight = 1.0;
            Stackable = true;
            Hue = 0x475;
            FillFactor = 5;
            Name = "Meatloaf";
        }

        public Meatloaf( Serial serial )
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

    public class PotatoStrings : Food
    {
        [Constructable]
        public PotatoStrings()
            : this( 1 )
        {
        }

        [Constructable]
        public PotatoStrings( int amount )
            : base( amount, 0x1B8D )
        {
            Weight = 1.0;
            Stackable = true;
            Hue = 0x225;
            FillFactor = 3;
            Name = "Potato Strings";
        }

        public PotatoStrings( Serial serial )
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

    public class Pancakes : Food
    {
        [Constructable]
        public Pancakes()
            : this( 1 )
        {
        }

        [Constructable]
        public Pancakes( int amount )
            : base( amount, 0x1E1F )
        {
            Weight = 1.0;
            Stackable = true;
            Hue = 0x22A;
            FillFactor = 5;
            Name = "Pancakes";
        }

        public Pancakes( Serial serial )
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

    public class Waffles : Food
    {
        [Constructable]
        public Waffles()
            : this( 1 )
        {
        }

        [Constructable]
        public Waffles( int amount )
            : base( amount, 0x1E1F )
        {
            Weight = 1.0;
            Stackable = true;
            Hue = 0x1C7;
            FillFactor = 4;
            Name = "Waffles";
        }

        public Waffles( Serial serial )
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

    public class GrilledHam : Food
    {
        [Constructable]
        public GrilledHam()
            : this( 1 )
        {
        }

        [Constructable]
        public GrilledHam( int amount )
            : base( amount, 0x1E1F )
        {
            Weight = 1.0;
            Stackable = true;
            Hue = 0x33D;
            FillFactor = 4;
            Name = "Grilled Ham";
        }

        public GrilledHam( Serial serial )
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

    public class Hotwings : Food
    {
        [Constructable]
        public Hotwings()
            : this( 1 )
        {
        }

        [Constructable]
        public Hotwings( int amount )
            : base( amount, 0x1608 )
        {
            Weight = 1.0;
            Stackable = true;
            Hue = 0x21A;
            FillFactor = 3;
            Name = "Hotwings";
        }

        public Hotwings( Serial serial )
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

    public class Taco : Food
    {
        [Constructable]
        public Taco()
            : this( 1 )
        {
        }

        [Constructable]
        public Taco( int amount )
            : base( amount, 0x1370 )
        {
            Weight = 1.0;
            Stackable = true;
            Hue = 0x465;
            FillFactor = 3;
            Name = "Taco";
        }

        public Taco( Serial serial )
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

    public class CornOnCob : Food
    {
        [Constructable]
        public CornOnCob()
            : this( 1 )
        {
        }

        [Constructable]
        public CornOnCob( int amount )
            : base( amount, 0xC81 )
        {
            Weight = 1.0;
            Stackable = true;
            Hue = 0x0;
            FillFactor = 3;
            Name = "Cooked Corn on the Cob";
        }

        public CornOnCob( Serial serial )
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

    public class Hotdog : Food
    {
        [Constructable]
        public Hotdog()
            : this( 1 )
        {
        }

        [Constructable]
        public Hotdog( int amount )
            : base( amount, 0xC7F )
        {
            Weight = 1.0;
            Stackable = true;
            Hue = 0x457;
            FillFactor = 3;
            Name = "Hotdog";
        }

        public Hotdog( Serial serial )
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

    public class CookedSunflowerSeeds : Food
    {
        [Constructable]
        public CookedSunflowerSeeds()
            : this( 1 )
        {
        }

        [Constructable]
        public CookedSunflowerSeeds( int amount )
            : base( amount, 0xF27 )
        {
            Weight = 0.1;
            Stackable = true;
            Hue = 0x44F;
            FillFactor = 2;
            Name = "Sunflower Seeds";
        }

        public CookedSunflowerSeeds( Serial serial )
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

    public class GrapeFruit : Food
    {
        [Constructable]
        public GrapeFruit()
            : this( 1 )
        {
        }

        [Constructable]
        public GrapeFruit( int amount )
            : base( amount, 0x1728 )
        {
            Weight = 1.0;
            FillFactor = 1;
        }

        public GrapeFruit( Serial serial )
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

    [Flipable( 0xc79, 0xc7a )]
    public class ChocolateBar : Food
    {
        [Constructable]
        public ChocolateBar()
            : this( 1 )
        {
        }

        [Constructable]
        public ChocolateBar( int amount )
            : base( amount, 0xc79 )
        {
            Weight = 1.0;
            FillFactor = 1;
        }

        public ChocolateBar( Serial serial )
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

    public class Cherries : Food
    {
        [Constructable]
        public Cherries()
            : this( 1 )
        {
        }

        [Constructable]
        public Cherries( int amount )
            : base( amount, 0x9D1 )
        {
            Weight = 1.0;
            FillFactor = 2;
            Hue = 0x85;
            Name = "Cherries";
        }

        public Cherries( Serial serial )
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

    #region berries

    public class CranBerry : Item, ICommodity
    {
        string ICommodity.Description
        {
            get { return String.Format( Amount == 1 ? "{0} cranberry" : "{0} cranberries", Amount ); }
        }

        int ICommodity.DescriptionNumber
        {
            get { return 0; }
        }

        [Constructable]
        public CranBerry()
            : this( 1 )
        {
        }

        [Constructable]
        public CranBerry( int amount )
            : base( 0x9D1 )
        {
            Stackable = true;
            Weight = 0.1;
            Amount = amount;
        }

        public CranBerry( Serial serial )
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

    public class BlackBerry : Item, ICommodity
    {
        string ICommodity.Description
        {
            get { return String.Format( Amount == 1 ? "{0} blackberry" : "{0} blackberries", Amount ); }
        }

        int ICommodity.DescriptionNumber
        {
            get { return 0; }
        }

        [Constructable]
        public BlackBerry()
            : this( 1 )
        {
        }

        [Constructable]
        public BlackBerry( int amount )
            : base( 0x9D1 )
        {
            Stackable = true;
            Weight = 0.1;
            Amount = amount;
        }

        public BlackBerry( Serial serial )
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

    public class BlueBerry : Item, ICommodity
    {
        string ICommodity.Description
        {
            get { return String.Format( Amount == 1 ? "{0} blueberry" : "{0} blueberries", Amount ); }
        }

        int ICommodity.DescriptionNumber
        {
            get { return 0; }
        }

        [Constructable]
        public BlueBerry()
            : this( 1 )
        {
        }

        [Constructable]
        public BlueBerry( int amount )
            : base( 0x9D1 )
        {
            Stackable = true;
            Weight = 0.1;
            Amount = amount;
        }

        public BlueBerry( Serial serial )
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

    public class ElderBerry : Item, ICommodity
    {
        string ICommodity.Description
        {
            get { return String.Format( Amount == 1 ? "{0} elderberry" : "{0} elderberries", Amount ); }
        }

        int ICommodity.DescriptionNumber
        {
            get { return 0; }
        }

        [Constructable]
        public ElderBerry()
            : this( 1 )
        {
        }

        [Constructable]
        public ElderBerry( int amount )
            : base( 0x9D1 )
        {
            Stackable = true;
            Weight = 0.1;
            Amount = amount;
        }

        public ElderBerry( Serial serial )
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

    public class HuckleBerry : Item, ICommodity
    {
        string ICommodity.Description
        {
            get { return String.Format( Amount == 1 ? "{0} huckleberry" : "{0} huckleberries", Amount ); }
        }

        int ICommodity.DescriptionNumber
        {
            get { return 0; }
        }

        [Constructable]
        public HuckleBerry()
            : this( 1 )
        {
        }

        [Constructable]
        public HuckleBerry( int amount )
            : base( 0x9D1 )
        {
            Stackable = true;
            Weight = 0.1;
            Amount = amount;
        }

        public HuckleBerry( Serial serial )
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

    public class LoganBerry : Item, ICommodity
    {
        string ICommodity.Description
        {
            get { return String.Format( Amount == 1 ? "{0} loganberry" : "{0} loganberries", Amount ); }
        }

        int ICommodity.DescriptionNumber
        {
            get { return 0; }
        }

        [Constructable]
        public LoganBerry()
            : this( 1 )
        {
        }

        [Constructable]
        public LoganBerry( int amount )
            : base( 0x9D1 )
        {
            Stackable = true;
            Weight = 0.1;
            Amount = amount;
        }

        public LoganBerry( Serial serial )
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

    public class GooseBerry : Item, ICommodity
    {
        string ICommodity.Description
        {
            get { return String.Format( Amount == 1 ? "{0} gooseberry" : "{0} gooseberries", Amount ); }
        }

        int ICommodity.DescriptionNumber
        {
            get { return 0; }
        }

        [Constructable]
        public GooseBerry()
            : this( 1 )
        {
        }

        [Constructable]
        public GooseBerry( int amount )
            : base( 0x9D1 )
        {
            Stackable = true;
            Weight = 0.1;
            Amount = amount;
        }

        public GooseBerry( Serial serial )
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

    public class BlackCurant : Item, ICommodity
    {
        string ICommodity.Description
        {
            get { return String.Format( Amount == 1 ? "{0} black curant" : "{0} black curants", Amount ); }
        }

        int ICommodity.DescriptionNumber
        {
            get { return 0; }
        }

        [Constructable]
        public BlackCurant()
            : this( 1 )
        {
        }

        [Constructable]
        public BlackCurant( int amount )
            : base( 0x9D1 )
        {
            Stackable = true;
            Weight = 0.1;
            Amount = amount;
        }

        public BlackCurant( Serial serial )
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

    public class JuniperBerry : Item, ICommodity
    {
        string ICommodity.Description
        {
            get { return String.Format( Amount == 1 ? "{0} juniperberry" : "{0} juniperberries", Amount ); }
        }

        int ICommodity.DescriptionNumber
        {
            get { return 0; }
        }

        [Constructable]
        public JuniperBerry()
            : this( 1 )
        {
        }

        [Constructable]
        public JuniperBerry( int amount )
            : base( 0x9D1 )
        {
            Stackable = true;
            Weight = 0.1;
            Amount = amount;
        }

        public JuniperBerry( Serial serial )
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

    public class RasBerry : Item, ICommodity
    {
        string ICommodity.Description
        {
            get { return String.Format( Amount == 1 ? "{0} rasberry" : "{0} rasberries", Amount ); }
        }

        int ICommodity.DescriptionNumber
        {
            get { return 0; }
        }

        [Constructable]
        public RasBerry()
            : this( 1 )
        {
        }

        [Constructable]
        public RasBerry( int amount )
            : base( 0x9D1 )
        {
            Stackable = true;
            Weight = 0.1;
            Amount = amount;
        }

        public RasBerry( Serial serial )
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

    public class StrawBerry : Item, ICommodity
    {
        string ICommodity.Description
        {
            get { return String.Format( Amount == 1 ? "{0} strawberry" : "{0} strawberries", Amount ); }
        }

        int ICommodity.DescriptionNumber
        {
            get { return 0; }
        }

        [Constructable]
        public StrawBerry()
            : this( 1 )
        {
        }

        [Constructable]
        public StrawBerry( int amount )
            : base( 0x9D1 )
        {
            Stackable = true;
            Weight = 0.1;
            Amount = amount;
        }

        public StrawBerry( Serial serial )
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

    #endregion
}