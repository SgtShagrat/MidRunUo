using Server;

namespace Midgard.Engines.BrewCrafing
{
    public class EmptyAleBottle : Item
    {
        [Constructable]
        public EmptyAleBottle()
            : this( 1 )
        {
        }

        [Constructable]
        public EmptyAleBottle( int amount )
            : base( 0x99F )
        {
            Stackable = true;
            Weight = 1.0;
            Name = "Empty Ale Bottle";
            Amount = amount;
        }

        public EmptyAleBottle( Serial serial )
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

    public class EmptyJug : Item
    {
        [Constructable]
        public EmptyJug()
            : this( 1 )
        {
        }

        [Constructable]
        public EmptyJug( int amount )
            : base( 0x9C8 )
        {
            Stackable = true;
            Weight = 1.0;
            Name = "Empty Jug";
            Amount = amount;
        }

        public EmptyJug( Serial serial )
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

    public class EmptyMeadBottle : Item
    {
        [Constructable]
        public EmptyMeadBottle()
            : this( 1 )
        {
        }

        [Constructable]
        public EmptyMeadBottle( int amount )
            : base( 0x99B )
        {
            Stackable = true;
            Weight = 1.0;
            Name = "Empty Mead Bottle";
            Amount = amount;
        }

        public EmptyMeadBottle( Serial serial )
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

    public class EmptyWhiskeyBottle : Item
    {
        [Constructable]
        public EmptyWhiskeyBottle()
            : this( 1 )
        {
        }

        [Constructable]
        public EmptyWhiskeyBottle( int amount )
            : base( 0x99B )
        {
            Stackable = true;
            Weight = 1.0;
            Name = "Empty Whiskey Bottle";
            Amount = amount;
        }

        public EmptyWhiskeyBottle( Serial serial )
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

    public class EmptyWineBottle : Item
    {
        [Constructable]
        public EmptyWineBottle()
            : this( 1 )
        {
        }

        [Constructable]
        public EmptyWineBottle( int amount )
            : base( 0x9C7 )
        {
            Stackable = true;
            Weight = 1.0;
            Name = "Empty Wine Bottle";
            Amount = amount;
        }

        public EmptyWineBottle( Serial serial )
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
}