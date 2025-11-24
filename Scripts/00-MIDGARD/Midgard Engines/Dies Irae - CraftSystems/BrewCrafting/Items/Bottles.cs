using Server;

namespace Midgard.Engines.BrewCrafing
{
    public class BottleOfAle : BaseCraftAle
    {
        public override Item EmptyItem
        {
            get { return new EmptyAleBottle(); }
        }

        [Constructable]
        public BottleOfAle()
            : base( 0x99F )
        {
            Weight = 0.2;
            FillFactor = 3;
        }

        public BottleOfAle( Serial serial )
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

    /* BottleOfMead
    public class BottleOfMead : BaseCraftMead
    {
        public override Item EmptyItem
        {
            get { return new EmptyMeadBottle(); }
        }

        [Constructable]
        public BottleOfMead()
            : base( 0x99F )
        {
            Weight = 0.2;
            FillFactor = 3;
        }

        public BottleOfMead( Serial serial )
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
    */

    public class BottleOfWine : BaseCraftWine
    {
        public override Item EmptyItem
        {
            get { return new EmptyWineBottle(); }
        }

        [Constructable]
        public BottleOfWine()
            : base( 0x9C7 )
        {
            Weight = 0.2;
            FillFactor = 4;
        }

        public BottleOfWine( Serial serial )
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

    public class BottleOfWhiskey : BaseCraftWhiskey
    {
        public override Item EmptyItem
        {
            get { return new EmptyWhiskeyBottle(); }
        }

        [Constructable]
        public BottleOfWhiskey()
            : base( 0x99F )
        {
            Weight = 0.2;
            FillFactor = 3;
        }

        public BottleOfWhiskey( Serial serial )
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

    /* JugOfCider
    public class JugOfCider : BaseCraftCider
    {
        public override Item EmptyItem
        {
            get { return new EmptyJug(); }
        }

        [Constructable]
        public JugOfCider()
            : base( 0x9C8 )
        {
            Name = "Jug of Cider";
            Weight = 3.0;
            FillFactor = 3;
        }

        public JugOfCider( Serial serial )
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
     */
}