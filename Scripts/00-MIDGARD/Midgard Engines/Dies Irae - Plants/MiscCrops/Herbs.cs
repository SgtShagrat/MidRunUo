using Server;
using Server.Items;
using System;

namespace Midgard.Items
{
    public class TanGinger : Food
    {
        [Constructable]
        public TanGinger()
            : this( 1 )
        {
        }

        [Constructable]
        public TanGinger( int amount )
            : base( amount, 0xF85 )
        {
            FillFactor = 1;
            Hue = 0x413;
            Name = "Tan Ginger";
        }

        public TanGinger( Serial serial )
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

    public class TanMushroom : Food
    {
        [Constructable]
        public TanMushroom()
            : this( 1 )
        {
        }

        [Constructable]
        public TanMushroom( int amount )
            : base( amount, 0xD19 )
        {
            FillFactor = 4;
        }

        public TanMushroom( Serial serial )
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

    public class RedMushroom : Food
    {
        [Constructable]
        public RedMushroom()
            : this( 1 )
        {
        }

        [Constructable]
        public RedMushroom( int amount )
            : base( amount, 0xD16 )
        {
            FillFactor = 4;
        }

        public RedMushroom( Serial serial )
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

    public class Mint : Item, ICommodity
    {
        string ICommodity.Description
        {
            get { return String.Format( Amount == 1 ? "{0} peppermint" : "{0} peppermint", Amount ); }
        }

        int ICommodity.DescriptionNumber
        {
            get { return 0; }
        }

        [Constructable]
        public Mint()
            : this( 1 )
        {
        }

        [Constructable]
        public Mint( int amount )
            : base( 0x26B8 )
        {
            Name = "PepperMint";
            Stackable = true;
            Weight = 0.1;
            Amount = amount;
        }

        public Mint( Serial serial )
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