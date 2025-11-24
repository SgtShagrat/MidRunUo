using System;
using Server;
using Server.Items;

namespace Midgard.Engines.BrewCrafing
{
    /*
    public abstract class BaseBrewMix : Item, ICommodity
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
                return String.Format( Amount == 1 ? "{0} {1} brewmix" : "{0} {1} brewmix", Amount,
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

        public BaseBrewMix( BrewVariety variety )
            : this( variety, 1 )
        {
        }

        public BaseBrewMix( BrewVariety variety, int amount )
            : base( 0x1AA2 )
        {
            Stackable = true;
            Weight = 0.1;
            Amount = amount;
            Hue = BrewingResources.GetHue( variety );

            m_Variety = variety;
        }

        public BaseBrewMix( Serial serial )
            : base( serial )
        {
        }

        public override void AddNameProperty( ObjectPropertyList list )
        {
            if( Amount > 1 )
                list.Add( 1050039, "{0}\t{1}", Amount, "Bag of " + BrewingResources.GetName( m_Variety ) + " BrewingMix" );
            // ~1_NUMBER~ ~2_ITEMNAME~
            else
                list.Add( "Bag of " + BrewingResources.GetName( m_Variety ) + " BrewingMix" );
        }
    }
     */

    /*** lievito per vini ***/
    public class CommonYeast : Item, ICommodity
    {
        string ICommodity.Description
        {
            get
            {
                return String.Format( Amount == 1 ? "{0} dose of yeast" : "{0} doses of yeast", Amount );
            }
        }

        int ICommodity.DescriptionNumber
        {
            get { return 0; }
        }

        [Constructable]
        public CommonYeast()
            : this( 1 )
        {
        }

        [Constructable]
        public CommonYeast( int amount )
            : base( 0x1AA2 )
        {
            Name = "Yeast";
            Stackable = true;
            Weight = 0.1;
            Amount = amount;
            Hue = 2020;
        }

        #region serialization

        public CommonYeast( Serial serial )
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

        #endregion
    }

    /*** lievito Cereviasiae ***/
    public class CereviasiaeYeast : Item, ICommodity
    {
        string ICommodity.Description
        {
            get
            {
                return String.Format( Amount == 1 ? "{0} dose of cereviasiae yeast" : "{0} doses of cereviasiae yeast", Amount );
            }
        }

        int ICommodity.DescriptionNumber
        {
            get { return 0; }
        }

        [Constructable]
        public CereviasiaeYeast()
            : this( 1 )
        {
        }

        [Constructable]
        public CereviasiaeYeast( int amount )
            : base( 0x1AA2 )
        {
            Name = "Cereviasiae Yeast";
            Stackable = true;
            Weight = 0.1;
            Amount = amount;
            Hue = 2024;
        }

        #region serialization

        public CereviasiaeYeast( Serial serial )
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

        #endregion
    }

    /*** lievito Carlsbergensis ***/
    public class CarlsbergensisYeast : Item, ICommodity
    {
        string ICommodity.Description
        {
            get
            {
                return String.Format( Amount == 1 ? "{0} dose of carlsbergensis yeast" : "{0} doses of carlsbergensis yeast", Amount );
            }
        }

        int ICommodity.DescriptionNumber
        {
            get { return 0; }
        }

        [Constructable]
        public CarlsbergensisYeast()
            : this( 1 )
        {
        }

        [Constructable]
        public CarlsbergensisYeast( int amount )
            : base( 0x1AA2 )
        {
            Name = "Carlsbergensis Yeast";
            Stackable = true;
            Weight = 0.1;
            Amount = amount;
            Hue = 2023;
        }

        #region serialization

        public CarlsbergensisYeast( Serial serial )
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

        #endregion
    }

    /*** zucchero ***/
    public class Sugar : Item, ICommodity
    {
        string ICommodity.Description
        {
            get
            {
                return String.Format( Amount == 1 ? "{0} dose of brewer's sugar" : "{0} doses of brewer's sugar", Amount );
            }
        }

        int ICommodity.DescriptionNumber
        {
            get { return 0; }
        }

        [Constructable]
        public Sugar()
            : this( 1 )
        {
        }

        [Constructable]
        public Sugar( int amount )
            : base( 0x1006 )
        {
            Name = "Sugar";
            Stackable = true;
            Weight = 0.1;
            Amount = amount;
            Hue = 1154;
        }

        #region serialization

        public Sugar( Serial serial )
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

        #endregion
    }

    /*** orzo ***/
    public class Barley : Item, ICommodity
    {
        string ICommodity.Description
        {
            get
            {
                return String.Format( Amount == 1 ? "{0} dose of brewer's barley" : "{0} doses of brewer's barley", Amount );
            }
        }

        int ICommodity.DescriptionNumber
        {
            get { return 0; }
        }

        [Constructable]
        public Barley()
            : this( 1 )
        {
        }

        [Constructable]
        public Barley( int amount )
            : base( 0x1AA2 )
        {
            Name = "Barley";
            Stackable = true;
            Weight = 0.1;
            Amount = amount;
            Hue = 2025;
        }

        #region serialization

        public Barley( Serial serial )
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

        #endregion
    }

    /*** malto ***/
    public class Malt : Item, ICommodity
    {
        string ICommodity.Description
        {
            get
            {
                return String.Format( Amount == 1 ? "{0} dose of brewer's malt" : "{0} doses of brewer's malt", Amount );
            }
        }

        int ICommodity.DescriptionNumber
        {
            get { return 0; }
        }

        [Constructable]
        public Malt()
            : this( 1 )
        {
        }

        [Constructable]
        public Malt( int amount )
            : base( 0x1AA2 )
        {
            Name = "Malt";
            Stackable = true;
            Weight = 0.1;
            Amount = amount;
            Hue = 2025;
        }

        #region serialization

        public Malt( Serial serial )
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

        #endregion
    }

    /*** luppolo ***/
    public class Hop : Item, ICommodity
    {
        string ICommodity.Description
        {
            get
            {
                return String.Format( Amount == 1 ? "{0} dose of brewer's hop" : "{0} doses of brewer's hop", Amount );
            }
        }

        int ICommodity.DescriptionNumber
        {
            get { return 0; }
        }

        [Constructable]
        public Hop()
            : this( 1 )
        {
        }

        [Constructable]
        public Hop( int amount )
            : base( 0x1AA2 )
        {
            Name = "Hop";
            Stackable = true;
            Weight = 0.1;
            Amount = amount;
            Hue = 2028;
        }

        #region serialization

        public Hop( Serial serial )
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

        #endregion
    }

    /*** luppolo ***/
    public class Ray : Item, ICommodity
    {
        string ICommodity.Description
        {
            get
            {
                return String.Format( Amount == 1 ? "{0} dose of brewer's ray" : "{0} doses of brewer's ray", Amount );
            }
        }

        int ICommodity.DescriptionNumber
        {
            get { return 0; }
        }

        [Constructable]
        public Ray()
            : this( 1 )
        {
        }

        [Constructable]
        public Ray( int amount )
            : base( 0x1AA2 )
        {
            Name = "Ray";
            Stackable = true;
            Weight = 0.1;
            Amount = amount;
            Hue = 2029;
        }

        #region serialization

        public Ray( Serial serial )
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

        #endregion
    }
}