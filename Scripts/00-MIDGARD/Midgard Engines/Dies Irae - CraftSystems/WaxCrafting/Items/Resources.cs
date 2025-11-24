using Server;

namespace Midgard.Items
{
    public class BlankCandle : Item
    {
        public override int LabelNumber { get { return 1065257; } } // Blank Candle

        [Constructable]
        public BlankCandle()
            : base( 0x1433 )
        {
            Weight = 0.5;
            Hue = 1150;
        }

        public BlankCandle( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }

    public class CandleFitSkull : Item
    {
        public override int LabelNumber { get { return 1065258; } } // Candle Fit Skull

        [Constructable]
        public CandleFitSkull()
            : base( 0x1AE3 )
        {
            Weight = 0.5;
        }

        public CandleFitSkull( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }

    public class CandleWick : Item
    {
        public override int LabelNumber { get { return 1065259; } } // Candle Wick

        [Constructable]
        public CandleWick()
            : this( 1 )
        {
        }

        [Constructable]
        public CandleWick( int amount )
            : base( 0x979 )
        {
            Stackable = true;
            Weight = 0.5;
            Amount = amount;
            Hue = 1052;
        }

        public CandleWick( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }

    public class EssenceOfLove : Item
    {
        public override int LabelNumber { get { return 1065260; } } // Essence Of Love

        [Constructable]
        public EssenceOfLove()
            : base( 0x1C18 )
        {
            Weight = 0.5;
            Hue = 1157;
        }

        public EssenceOfLove( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }

    public class Slumgum : Item
    {
        public override int LabelNumber { get { return 1065261; } } // Slumgum

        [Constructable]
        public Slumgum()
            : this( 1 )
        {
        }

        [Constructable]
        public Slumgum( int amount )
            : base( 5927 )
        {
            Weight = 1.0;
            Stackable = true;
            Amount = amount;
            Hue = 1126;
        }

        public Slumgum( Serial serial )
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

    public class RawBeeswax : Item
    {
        public override int LabelNumber { get { return 1065262; } } // Raw Beeswax

        [Constructable]
        public RawBeeswax()
            : this( 1 )
        {
        }

        [Constructable]
        public RawBeeswax( int amount )
            : base( 0x1422 )
        {
            Weight = 1.0;
            Stackable = true;
            Amount = amount;
            Hue = 1126;
        }

        public RawBeeswax( Serial serial )
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