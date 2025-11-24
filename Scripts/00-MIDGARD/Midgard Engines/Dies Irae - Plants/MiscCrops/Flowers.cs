/*
using Server;

namespace Midgard.Items
{

    public class BlackRose : Item
    {
        [Constructable]
        public BlackRose()
            : this( 1 )
        {
        }

        [Constructable]
        public BlackRose( int amount )
            : base( 0x234B )
        {
            Name = "Black Rose";
            Hue = 2393;
        }

        public BlackRose( Serial serial )
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

    public class Dandelion : Item
    {
        [Constructable]
        public Dandelion()
            : this( 1 )
        {
        }

        [Constructable]
        public Dandelion( int amount )
            : base( 0x234B )
        {
            Name = "Dandelion";
            Hue = 2979;
        }

        public Dandelion( Serial serial )
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

    public class IrishRose : Item
    {
        [Constructable]
        public IrishRose()
            : this( 1 )
        {
        }

        [Constructable]
        public IrishRose( int amount )
            : base( 0x234B )
        {
            Name = "Irish Rose";
            Hue = 2723;
        }

        public IrishRose( Serial serial )
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

    public class Pansy : Item
    {
        [Constructable]
        public Pansy()
            : this( 1 )
        {
        }

        [Constructable]
        public Pansy( int amount )
            : base( 0x234B )
        {
            Name = "Pansy";
            Hue = 2971;
        }

        public Pansy( Serial serial )
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

    public class PinkCarnation : Item
    {
        [Constructable]
        public PinkCarnation()
            : this( 1 )
        {
        }

        [Constructable]
        public PinkCarnation( int amount )
            : base( 0x234B )
        {
            Name = "Pink Carnation";
            Hue = 2999;
        }

        public PinkCarnation( Serial serial )
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

    public class Poppy : Item
    {
        [Constructable]
        public Poppy()
            : this( 1 )
        {
        }

        [Constructable]
        public Poppy( int amount )
            : base( 0x234B )
        {
            Name = "Poppy";
            Hue = 2973;
        }

        public Poppy( Serial serial )
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

    public class RedRose : Item
    {
        [Constructable]
        public RedRose()
            : this( 1 )
        {
        }

        [Constructable]
        public RedRose( int amount )
            : base( 0x234B )
        {
            Name = "Red Rose";
            Hue = 2495;
        }

        public RedRose( Serial serial )
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

    public class Snapdragon : Item
    {
        [Constructable]
        public Snapdragon()
            : this( 1 )
        {
        }

        [Constructable]
        public Snapdragon( int amount )
            : base( 0x234B )
        {
            Name = "Snapdragon";
            Hue = 2816;
        }

        public Snapdragon( Serial serial )
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

    public class SpiritRose : Item
    {
        [Constructable]
        public SpiritRose()
            : this( 1 )
        {
        }

        [Constructable]
        public SpiritRose( int amount )
            : base( 0x234B )
        {
            Name = "Spirit Rose";
            Hue = 1947;
        }

        public SpiritRose( Serial serial )
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

    public class WhiteRose : Item
    {
        [Constructable]
        public WhiteRose()
            : this( 1 )
        {
        }

        [Constructable]
        public WhiteRose( int amount )
            : base( 0x234B )
        {
            Name = "White Rose";
            Hue = 1953;
        }

        public WhiteRose( Serial serial )
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

    public class YellowRose : Item
    {
        [Constructable]
        public YellowRose()
            : this( 1 )
        {
        }

        [Constructable]
        public YellowRose( int amount )
            : base( 0x234B )
        {
            Name = "Yellow Rose";
            Hue = 2858;
        }

        public YellowRose( Serial serial )
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
*/