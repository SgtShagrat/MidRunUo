using System;
using Server;
using Server.Items;

namespace Midgard.Engines.BrewCrafing
{
    /*** luppolo ***/
    /*
    public abstract class BaseHops : Item, ICommodity
    {
        private BrewVariety m_Variety;

        [CommandProperty( AccessLevel.GameMaster )]
        public BrewVariety Variety
        {
            get { return m_Variety; }
            set
            {
                m_Variety = value;
                InvalidateProperties();
            }
        }

        string ICommodity.Description
        {
            get
            {
                return String.Format( Amount == 1 ? "{0} {1} hops" : "{0} {1} hops", Amount,
                                     BrewingResources.GetName( m_Variety ).ToLower() );
            }
        }

        int ICommodity.DescriptionNumber
        {
            get { return 0; }
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version

            writer.Write( (int)m_Variety );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            switch( version )
            {
                case 0:
                    {
                        m_Variety = (BrewVariety)reader.ReadInt();
                        break;
                    }
            }
        }

        public BaseHops( BrewVariety variety )
            : this( variety, 1 )
        {
        }

        public BaseHops( BrewVariety variety, int amount )
            : base( 0x1AA2 )
        {
            Stackable = true;
            Weight = 0.1;
            Amount = amount;
            Hue = BrewingResources.GetHue( variety );

            m_Variety = variety;
        }

        public BaseHops( Serial serial )
            : base( serial )
        {
        }

        public override void AddNameProperty( ObjectPropertyList list )
        {
            if( Amount > 1 )
                list.Add( 1050039, "{0}\t{1}", Amount, "Bunches of " + BrewingResources.GetName( m_Variety ) + " Cones" );
            // ~1_NUMBER~ ~2_ITEMNAME~
            else
                list.Add( "Bunch of " + BrewingResources.GetName( m_Variety ) + " Cones" );
        }
    }

    public class BitterHops : BaseHops
    {
        [Constructable]
        public BitterHops()
            : this( 1 )
        {
        }

        [Constructable]
        public BitterHops( int amount )
            : base( BrewVariety.BitterHops, amount )
        {
            Name = "Bitter Hops";
        }

        public BitterHops( Serial serial )
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

    public class SnowHops : BaseHops
    {
        [Constructable]
        public SnowHops()
            : this( 1 )
        {
        }

        [Constructable]
        public SnowHops( int amount )
            : base( BrewVariety.SnowHops, amount )
        {
        }

        public SnowHops( Serial serial )
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

    public class ElvenHops : BaseHops
    {
        [Constructable]
        public ElvenHops()
            : this( 1 )
        {
        }

        [Constructable]
        public ElvenHops( int amount )
            : base( BrewVariety.ElvenHops, amount )
        {
        }

        public ElvenHops( Serial serial )
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

    public class SweetHops : BaseHops
    {
        [Constructable]
        public SweetHops()
            : this( 1 )
        {
        }

        [Constructable]
        public SweetHops( int amount )
            : base( BrewVariety.SweetHops, amount )
        {
        }

        public SweetHops( Serial serial )
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