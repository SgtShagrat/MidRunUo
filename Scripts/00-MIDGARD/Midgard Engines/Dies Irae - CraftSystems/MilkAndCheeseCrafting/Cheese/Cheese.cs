using Server;

namespace Midgard.Engines.CheeseCrafting
{
    public class CowRawCheeseForm : Server.Items.Food, ICarvable
    {
        public void Carve( Mobile from, Item item )
        {
            if( !Movable )
                return;

            if( Amount > 1 )
            {
                from.SendMessage( "You can only cut up one wheel at a time." );
                return;
            }

            base.ScissorHelper( from, new CowRawCheeseWedge(), 1 );

            from.AddToBackpack( new CowRawCheeseWedgeSmall() );

            from.SendMessage( "You cut a wedge out of the wheel." );
        }

        [Constructable]
        public CowRawCheeseForm()
            : this( 1 )
        {
        }

        [Constructable]
        public CowRawCheeseForm( int amount )
            : base( amount, 0x97E )
        {
            Weight = 0.4;
            FillFactor = 12;
            Name = "raw cow cheese";
            Hue = 0x481;
        }

        public CowRawCheeseForm( Serial serial )
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

    public class CowRawCheeseWedge : Server.Items.Food, ICarvable
    {
        public void Carve( Mobile from, Item item )
        {
            if( !Movable )
                return;

            base.ScissorHelper( from, new CowRawCheeseWedgeSmall(), 3 );
            from.SendMessage( "You cut the wheel into 3 wedges." );
        }

        [Constructable]
        public CowRawCheeseWedge()
            : this( 1 )
        {
        }

        [Constructable]
        public CowRawCheeseWedge( int amount )
            : base( amount, 0x97D )
        {
            Weight = 0.3;
            FillFactor = 9;
            Name = "raw cow cheese wedge";
            Hue = 0x481;
        }

        public CowRawCheeseWedge( Serial serial )
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

    public class CowRawCheeseWedgeSmall : Server.Items.Food
    {
        [Constructable]
        public CowRawCheeseWedgeSmall()
            : this( 1 )
        {
        }

        [Constructable]
        public CowRawCheeseWedgeSmall( int amount )
            : base( amount, 0x97C )
        {
            Weight = 0.1;
            FillFactor = 3;
            Name = "small raw cow cheese wedge";
            Hue = 0x481;
        }

        public CowRawCheeseWedgeSmall( Serial serial )
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

    public class SheepRawCheeseForm : Server.Items.Food, ICarvable
    {
        public void Carve( Mobile from, Item item )
        {
            if( !Movable )
                return;

            if( Amount > 1 )
            {
                from.SendMessage( "You can only cut up one wheel at a time." );
                return;
            }

            base.ScissorHelper( from, new SheepRawCheeseWedge(), 1 );

            from.AddToBackpack( new SheepRawCheeseWedgeSmall() );

            from.SendMessage( "You cut a wedge out of the wheel." );
        }

        [Constructable]
        public SheepRawCheeseForm()
            : this( 1 )
        {
        }

        [Constructable]
        public SheepRawCheeseForm( int amount )
            : base( amount, 0x97E )
        {
            Weight = 0.4;
            FillFactor = 12;
            Name = "raw sheep cheese";
            Hue = 0x481;
        }

        public SheepRawCheeseForm( Serial serial )
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

    public class SheepRawCheeseWedge : Server.Items.Food, ICarvable
    {
        public void Carve( Mobile from, Item item )
        {
            if( !Movable )
                return;

            base.ScissorHelper( from, new SheepRawCheeseWedgeSmall(), 3 );
            from.SendMessage( "You cut the wheel into 3 wedges." );
        }

        [Constructable]
        public SheepRawCheeseWedge()
            : this( 1 )
        {
        }

        [Constructable]
        public SheepRawCheeseWedge( int amount )
            : base( amount, 0x97D )
        {
            Weight = 0.3;
            FillFactor = 9;
            Name = "raw sheep cheese wedge";
            Hue = 0x481;
        }

        public SheepRawCheeseWedge( Serial serial )
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

    public class SheepRawCheeseWedgeSmall : Server.Items.Food
    {
        [Constructable]
        public SheepRawCheeseWedgeSmall()
            : this( 1 )
        {
        }

        [Constructable]
        public SheepRawCheeseWedgeSmall( int amount )
            : base( amount, 0x97C )
        {
            Weight = 0.1;
            FillFactor = 3;
            Name = "small raw sheep cheese wedge";
            Hue = 0x481;
        }

        public SheepRawCheeseWedgeSmall( Serial serial )
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

    public class GoatRawCheeseForm : Server.Items.Food, ICarvable
    {
        public void Carve( Mobile from, Item item )
        {
            if( !Movable )
                return;

            if( Amount > 1 )
            {
                from.SendMessage( "You can only cut up one wheel at a time." );
                return;
            }

            base.ScissorHelper( from, new GoatRawCheeseWedge(), 1 );

            from.AddToBackpack( new GoatRawCheeseWedgeSmall() );

            from.SendMessage( "You cut a wedge out of the wheel." );
        }

        [Constructable]
        public GoatRawCheeseForm()
            : this( 1 )
        {
        }

        [Constructable]
        public GoatRawCheeseForm( int amount )
            : base( amount, 0x97E )
        {
            Weight = 0.4;
            FillFactor = 12;
            Name = "raw goat cheese";
            Hue = 0x481;
        }

        public GoatRawCheeseForm( Serial serial )
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

    public class GoatRawCheeseWedge : Server.Items.Food, ICarvable
    {
        public void Carve( Mobile from, Item item )
        {
            if( !Movable )
                return;

            base.ScissorHelper( from, new GoatRawCheeseWedgeSmall(), 3 );
            from.SendMessage( "You cut the wheel into 3 wedges." );
        }

        [Constructable]
        public GoatRawCheeseWedge()
            : this( 1 )
        {
        }

        [Constructable]
        public GoatRawCheeseWedge( int amount )
            : base( amount, 0x97D )
        {
            Weight = 0.3;
            FillFactor = 9;
            Name = "raw goat cheese wedge";
            Hue = 0x481;
        }

        public GoatRawCheeseWedge( Serial serial )
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

    public class GoatRawCheeseWedgeSmall : Server.Items.Food
    {
        [Constructable]
        public GoatRawCheeseWedgeSmall()
            : this( 1 )
        {
        }

        [Constructable]
        public GoatRawCheeseWedgeSmall( int amount )
            : base( amount, 0x97C )
        {
            Weight = 0.1;
            FillFactor = 3;
            Name = "small raw goat cheese wedge";
            Hue = 0x481;
        }

        public GoatRawCheeseWedgeSmall( Serial serial )
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