using System;
using Server;
using Server.Items;
using Midgard.Engines.BrewCrafing;

namespace Midgard.Engines.WineCrafting
{
    public abstract class BaseWineGrapes : Item, ICommodity
    {
        private BrewVariety m_Variety;

        [CommandProperty( AccessLevel.GameMaster )]
        public BrewVariety Variety
        {
            get { return m_Variety; }
            set { m_Variety = value; InvalidateProperties(); }
        }

        string ICommodity.Description
        {
            get
            {
                return String.Format( Amount == 1 ? "{0} {1} grape" : "{0} {1} grapes", Amount, BrewingResources.GetName( m_Variety ).ToLower() );
            }
        }

        int ICommodity.DescriptionNumber { get { return 0; } }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)1 ); // version

            writer.Write( (int)m_Variety );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            switch( version )
            {
                case 1:
                    {
                        m_Variety = (BrewVariety)reader.ReadInt();
                        break;
                    }
            }
        }

        public BaseWineGrapes( BrewVariety variety )
            : this( variety, 1 )
        {
        }

        public BaseWineGrapes( BrewVariety variety, int amount )
            : base( 0x9D1 )
        {
            Stackable = true;
            Weight = 0.1;
            Amount = amount;
            Hue = BrewingResources.GetHue( variety );

            m_Variety = variety;
        }

        public BaseWineGrapes( Serial serial )
            : base( serial )
        {
        }

        public override void AddNameProperty( ObjectPropertyList list )
        {
            if( Amount > 1 )
                list.Add( 1050039, "{0}\t{1}", Amount, "Bunches of " + BrewingResources.GetName( m_Variety ) + " Grapes" ); // ~1_NUMBER~ ~2_ITEMNAME~
            else
                list.Add( "Bunch of " + BrewingResources.GetName( m_Variety ) + " Grapes" );
        }

        public override void OnSingleClick( Mobile from )
        {
            //base.OnSingleClick( from );

            if( this.Amount > 1 )
                this.LabelTo( from, "{0} Grape Bunches : {1}", BrewingResources.GetName( m_Variety ), Amount );
            else
                this.LabelTo( from, "{0} Grape Bunch", BrewingResources.GetName( m_Variety ) );
        }
    }

    #region grapes
    public class CabernetSauvignonGrapes : BaseWineGrapes
    {
        [Constructable]
        public CabernetSauvignonGrapes()
            : this( 1 )
        {
        }

        [Constructable]
        public CabernetSauvignonGrapes( int amount )
            : base( BrewVariety.CabernetSauvignon, amount )
        {
            Name = "Cabernet Sauvignon Grapes";
        }

        public CabernetSauvignonGrapes( Serial serial )
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

    public class ChardonnayGrapes : BaseWineGrapes
    {
        [Constructable]
        public ChardonnayGrapes()
            : this( 1 )
        {
        }

        [Constructable]
        public ChardonnayGrapes( int amount )
            : base( BrewVariety.Chardonnay, amount )
        {
        }

        public ChardonnayGrapes( Serial serial )
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

    public class CheninBlancGrapes : BaseWineGrapes
    {
        [Constructable]
        public CheninBlancGrapes()
            : this( 1 )
        {
        }

        [Constructable]
        public CheninBlancGrapes( int amount )
            : base( BrewVariety.CheninBlanc, amount )
        {
        }

        public CheninBlancGrapes( Serial serial )
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

    public class MerlotGrapes : BaseWineGrapes
    {
        [Constructable]
        public MerlotGrapes()
            : this( 1 )
        {
        }

        [Constructable]
        public MerlotGrapes( int amount )
            : base( BrewVariety.Merlot, amount )
        {
        }

        public MerlotGrapes( Serial serial )
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

    public class PinotNoirGrapes : BaseWineGrapes
    {
        [Constructable]
        public PinotNoirGrapes()
            : this( 1 )
        {
        }

        [Constructable]
        public PinotNoirGrapes( int amount )
            : base( BrewVariety.PinotNoir, amount )
        {
        }

        public PinotNoirGrapes( Serial serial )
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

    public class RieslingGrapes : BaseWineGrapes
    {
        [Constructable]
        public RieslingGrapes()
            : this( 1 )
        {
        }

        [Constructable]
        public RieslingGrapes( int amount )
            : base( BrewVariety.Riesling, amount )
        {
        }

        public RieslingGrapes( Serial serial )
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

    public class SangioveseGrapes : BaseWineGrapes
    {
        [Constructable]
        public SangioveseGrapes()
            : this( 1 )
        {
        }

        [Constructable]
        public SangioveseGrapes( int amount )
            : base( BrewVariety.Sangiovese, amount )
        {
        }

        public SangioveseGrapes( Serial serial )
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

    public class SauvignonBlancGrapes : BaseWineGrapes
    {
        [Constructable]
        public SauvignonBlancGrapes()
            : this( 1 )
        {
        }

        [Constructable]
        public SauvignonBlancGrapes( int amount )
            : base( BrewVariety.SauvignonBlanc, amount )
        {
        }

        public SauvignonBlancGrapes( Serial serial )
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

    public class ShirazGrapes : BaseWineGrapes
    {
        [Constructable]
        public ShirazGrapes()
            : this( 1 )
        {
        }

        [Constructable]
        public ShirazGrapes( int amount )
            : base( BrewVariety.Shiraz, amount )
        {
        }

        public ShirazGrapes( Serial serial )
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

    public class ViognierGrapes : BaseWineGrapes
    {
        [Constructable]
        public ViognierGrapes()
            : this( 1 )
        {
        }

        [Constructable]
        public ViognierGrapes( int amount )
            : base( BrewVariety.Viognier, amount )
        {
        }

        public ViognierGrapes( Serial serial )
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

    public class ZinfandelGrapes : BaseWineGrapes
    {
        [Constructable]
        public ZinfandelGrapes()
            : this( 1 )
        {
        }

        [Constructable]
        public ZinfandelGrapes( int amount )
            : base( BrewVariety.Zinfandel, amount )
        {
        }

        public ZinfandelGrapes( Serial serial )
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
    #endregion
}