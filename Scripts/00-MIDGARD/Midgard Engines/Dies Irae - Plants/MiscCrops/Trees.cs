using Server;
using Server.Items;
using Server.Targeting;
using Midgard.Engines.AdvancedCooking;

namespace Midgard.Items
{
    #region fruits

    public class Almond : Food
    {
        [Constructable]
        public Almond()
            : this( 1 )
        {
        }

        [Constructable]
        public Almond( int amount )
            : base( amount, 0x1AA2 )
        {
            Weight = 0.1;
            FillFactor = 1;
            Hue = 0x482;
            Name = "Almond";
        }

        public Almond( Serial serial )
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

    public class Apricot : Food
    {
        [Constructable]
        public Apricot()
            : this( 1 )
        {
        }

        [Constructable]
        public Apricot( int amount )
            : base( amount, 0x9D2 )
        {
            FillFactor = 2;
            Hue = 0x31;
            Name = "Apricot";
        }

        public Apricot( Serial serial )
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

    /* Avocado
    public class Avocado : Food
    {
        [Constructable]
        public Avocado()
            : this( 1 )
        {
        }

        [Constructable]
        public Avocado( int amount )
            : base( amount, 0xC80 )
        {
            Weight = 0.5;
            FillFactor = 1;
            Stackable = true;
            Hue = 0x483;
            Name = "Avocado";
        }

        public Avocado( Serial serial )
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

    public class Cherry : Food
    {
        [Constructable]
        public Cherry()
            : this( 1 )
        {
        }

        [Constructable]
        public Cherry( int amount )
            : base( amount, 0x9D1 )
        {
            FillFactor = 2;
            Hue = 0x85;
            Name = "Cherry";
        }

        public Cherry( Serial serial )
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

    /* Cranberry
    public class Cranberry : Food
    {
        [Constructable]
        public Cranberry()
            : this( 1 )
        {
        }

        [Constructable]
        public Cranberry( int amount )
            : base( amount, 0x9D1 )
        {
            FillFactor = 1;
            Hue = 0xE8;
            Name = "Cranberry";
        }

        public Cranberry( Serial serial )
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

    /*
    public class Grapefruit : Food
    {
        [Constructable]
        public Grapefruit()
            : this( 1 )
        {
        }

        [Constructable]
        public Grapefruit( int amount )
            : base( amount, 0x9D0 )
        {
            FillFactor = 2;
            Hue = 0x15E;
            Name = "Grapefruit";
        }

        public Grapefruit( Serial serial )
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

    /* Kiwi
    public class Kiwi : Food
    {
        [Constructable]
        public Kiwi()
            : this( 1 )
        {
        }

        [Constructable]
        public Kiwi( int amount )
            : base( amount, 0xF8B )
        {
            FillFactor = 1;
            Hue = 0x458;
            Name = "Kiwi";
        }

        public Kiwi( Serial serial )
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

    /*
    public class Mango : Food
    {
        [Constructable]
        public Mango()
            : this( 1 )
        {
        }

        [Constructable]
        public Mango( int amount )
            : base( amount, 0x9D0 )
        {
            FillFactor = 2;
            Hue = 0x489;
            Name = "Mango";
        }

        public Mango( Serial serial )
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

    public class Orange : Food
    {
        [Constructable]
        public Orange()
            : this( 1 )
        {
        }

        [Constructable]
        public Orange( int amount )
            : base( amount, 0x9D0 )
        {
            Name = "orange";
            Hue = 48;
            FillFactor = 1;
        }

        public Orange( Serial serial )
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

    public class Pineapple : Food
    {
        [Constructable]
        public Pineapple()
            : this( 1 )
        {
        }

        [Constructable]
        public Pineapple( int amount )
            : base( amount, 0xFC4 )
        {
            FillFactor = 2;
            Hue = 0x46E;
            Name = "Pineapple";
        }

        public Pineapple( Serial serial )
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

    /* Pistacio
    public class Pistacio : Food
    {
        [Constructable]
        public Pistacio()
            : this( 1 )
        {
        }

        [Constructable]
        public Pistacio( int amount )
            : base( amount, 0x1AA2 )
        {
            Weight = 0.1;
            FillFactor = 1;
            Hue = 0x47E;
            Name = "Pistacio";
        }

        public Pistacio( Serial serial )
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

    /* Plum
    public class Plum : Food
    {
        [Constructable]
        public Plum()
            : this( 1 )
        {
        }

        [Constructable]
        public Plum( int amount )
            : base( amount, 0x9D2 )
        {
            FillFactor = 2;
            Hue = 0x264;
            Name = "Plum";
        }

        public Plum( Serial serial )
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

    /*
    public class Pomegranate : Food
    {
        [Constructable]
        public Pomegranate()
            : this( 1 )
        {
        }

        [Constructable]
        public Pomegranate( int amount )
            : base( amount, 0x9D0 )
        {
            FillFactor = 2;
            Hue = 0x215;
            Name = "Pomegranate";
        }

        public Pomegranate( Serial serial )
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

    /*
    // *** mela acida *** //
    public class SourApple : Item, ICommodity
    {
        string ICommodity.Description
        {
            get { return String.Format( Amount == 1 ? "{0} sour apple" : "{0} sour apples", Amount ); }
        }

        int ICommodity.DescriptionNumber
        {
            get { return 0; }
        }

        [Constructable]
        public SourApple()
            : this( 1 )
        {
        }

        [Constructable]
        public SourApple( int amount )
            : base( 0x1039 )
        {
            Name = "Sour Apple";
            Stackable = true;
            Weight = 6.0;
            Amount = amount;
        }

        public SourApple( Serial serial )
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

    public class CocoaNut : Item
    {
        [Constructable]
        public CocoaNut()
            : this( 1 )
        {
        }

        [Constructable]
        public CocoaNut( int amount )
            : base( 0x1726 )
        {
            Name = "Cocoa Nut";
            Hue = 0x422;
            Stackable = true;
            Amount = amount;
        }

        public void Carve( Mobile from, Item item )
        {
            if( !Movable )
                return;
            base.ScissorHelper( from, new CocoaBean(), 1 );
            from.SendMessage( "You cut away the husk to get the bean." );
        }

        public CocoaNut( Serial serial )
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

    public class Prune : Food
    {
        [Constructable]
        public Prune()
            : this( 1 )
        {
        }

        [Constructable]
        public Prune( int amount )
            : base( amount, 0xF2B )
        {
            Weight = 1.0;
            FillFactor = 1;
            Hue = 0x205;
            Name = "Prune";
            Stackable = true;
        }

        public Prune( Serial serial )
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
    #endregion

    #region beans
    public class CocoaBean : Food
    {
        [Constructable]
        public CocoaBean()
            : this( 1 )
        {
        }

        [Constructable]
        public CocoaBean( int amount )
            : base( amount, 0x172A )
        {
            Weight = 0.5;
            FillFactor = 1;
            Hue = 0x47A;
            Name = "Cocoa Bean";
        }

        public CocoaBean( Serial serial )
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
            private CocoaBean m_Item;

            public InternalTarget( CocoaBean item )
                : base( 1, false, TargetFlags.None )
            {
                m_Item = item;
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                if( m_Item.Deleted )
                    return;

                else if( FoodHelper.IsFlourMill( targeted ) )
                {
                    if( m_Item.Amount >= 30 )
                    {
                        m_Item.Consume( 30 );
                        from.SendMessage( "You made a bag of cocoa." );
                        from.AddToBackpack( new BagOfCocoa() );
                    }
                    else
                        from.SendMessage( "You don't have enough cocoa beans." );
                }
            }
        }
    }

    public class CoffeeBean : Item
    {
        [Constructable]
        public CoffeeBean()
            : this( 1 )
        {
        }

        [Constructable]
        public CoffeeBean( int amount )
            : base( 0xC64 )
        {
            Amount = amount;
            Weight = 0.1;
            Hue = 0x46A;
            Stackable = true;
            Name = "Coffee Bean";
        }

        public CoffeeBean( Serial serial )
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
            private CoffeeBean m_Item;

            public InternalTarget( CoffeeBean item )
                : base( 1, false, TargetFlags.None )
            {
                m_Item = item;
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                if( m_Item.Deleted )
                    return;

                else if( FoodHelper.IsFlourMill( targeted ) )
                {
                    if( m_Item.Amount >= 30 )
                    {
                        m_Item.Consume( 30 );
                        from.SendMessage( "You made a bag of coffee." );
                        from.AddToBackpack( new BagOfCoffee() );
                    }
                    else
                        from.SendMessage( "You don't have enough coffee beans." );
                }
            }
        }
    }
    #endregion
}