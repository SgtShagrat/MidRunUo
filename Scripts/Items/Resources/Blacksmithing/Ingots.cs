using System;
using Server.Items;
using Server.Network;

namespace Server.Items
{
    public abstract class BaseIngot : Item, ICommodity
    {
        #region mod by Dies Irae
        public static readonly bool POLWeight = true;

        public override double DefaultWeight
        {
            get { return POLWeight ? 0.5 : 0.1; }
        }
        #endregion

        private CraftResource m_Resource;

        [CommandProperty( AccessLevel.GameMaster )]
        public CraftResource Resource
        {
            get { return m_Resource; }
            set { m_Resource = value; InvalidateProperties(); }
        }

        string ICommodity.Description
        {
            get
            {
                return String.Format( Amount == 1 ? "{0} {1} ingot" : "{0} {1} ingots", Amount, CraftResources.GetName( m_Resource ).ToLower() );
            }
        }

        int ICommodity.DescriptionNumber { get { return LabelNumber; } }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)1 ); // version

            writer.Write( (int)m_Resource );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            switch( version )
            {
                case 1:
                    {
                        m_Resource = (CraftResource)reader.ReadInt();
                        break;
                    }
                case 0:
                    {
                        OreInfo info;

                        switch( reader.ReadInt() )
                        {
                            case 0:
                                info = OreInfo.Iron;
                                break;
                            case 1:
                                info = OreInfo.DullCopper;
                                break;
                            case 2:
                                info = OreInfo.ShadowIron;
                                break;
                            case 3:
                                info = OreInfo.Copper;
                                break;
                            case 4:
                                info = OreInfo.Bronze;
                                break;
                            case 5:
                                info = OreInfo.Gold;
                                break;
                            case 6:
                                info = OreInfo.Agapite;
                                break;
                            case 7:
                                info = OreInfo.Verite;
                                break;
                            case 8:
                                info = OreInfo.Valorite;
                                break;

                            default:
                                info = null;
                                break;
                        }

                        m_Resource = CraftResources.GetFromOreInfo( info );
                        break;
                    }
            }

            #region mod by Dies Irae
            if( Hue != CraftResources.GetHue( m_Resource ) )
                Hue = CraftResources.GetHue( m_Resource );
            #endregion
        }

        public BaseIngot( CraftResource resource )
            : this( resource, 1 )
        {
        }

        public BaseIngot( CraftResource resource, int amount )
            : base( 0x1BF2 )
        {
            Stackable = true;
            Amount = amount;
            Hue = CraftResources.GetHue( resource );

            m_Resource = resource;
        }

        public BaseIngot( Serial serial )
            : base( serial )
        {
        }

        public override void AddNameProperty( ObjectPropertyList list )
        {
            if( Amount > 1 )
                list.Add( 1050039, "{0}\t#{1}", Amount, 1027154 ); // ~1_NUMBER~ ~2_ITEMNAME~
            else
                list.Add( 1027154 ); // ingots
        }

        public override void GetProperties( ObjectPropertyList list )
        {
            base.GetProperties( list );

            if( !CraftResources.IsStandard( m_Resource ) )
            {
                int num = CraftResources.GetLocalizationNumber( m_Resource );

                if( num > 0 )
                    list.Add( num );
                else
                    list.Add( CraftResources.GetName( m_Resource ) );
            }
        }

        public override int LabelNumber
        {
            get
            {
                if( m_Resource >= CraftResource.DullCopper && m_Resource <= CraftResource.Valorite )
                    return 1042684 + (int)( m_Resource - CraftResource.DullCopper );

                return 1042692;
            }
        }

        #region mod by Dies Irae : pre-aos stuff
        public override void OnSingleClick( Mobile from )
        {
            string format = ( Amount == 1 ? "{0} {1} ingot" : "{0} {1} ingots" );

            LabelTo( from, format, Amount, CraftResources.GetName( m_Resource ).ToLower() );
        }
        #endregion
    }

    [FlipableAttribute( 0x1BF2, 0x1BEF )]
    public class IronIngot : BaseIngot
    {
        [Constructable]
        public IronIngot()
            : this( 1 )
        {
        }

        [Constructable]
        public IronIngot( int amount )
            : base( CraftResource.Iron, amount )
        {
        }

        public IronIngot( Serial serial )
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

    [FlipableAttribute( 0x1BF2, 0x1BEF )]
    public class DullCopperIngot : BaseIngot
    {
        [Constructable]
        public DullCopperIngot()
            : this( 1 )
        {
        }

        [Constructable]
        public DullCopperIngot( int amount )
            : base( Core.AOS ? CraftResource.DullCopper : CraftResource.OldDullCopper, amount )
        {
        }

        public DullCopperIngot( Serial serial )
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

    [FlipableAttribute( 0x1BF2, 0x1BEF )]
    public class ShadowIronIngot : BaseIngot
    {
        [Constructable]
        public ShadowIronIngot()
            : this( 1 )
        {
        }

        [Constructable]
        public ShadowIronIngot( int amount )
            : base( Core.AOS ? CraftResource.ShadowIron : CraftResource.OldShadowIron, amount )
        {
        }

        public ShadowIronIngot( Serial serial )
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

    [FlipableAttribute( 0x1BF2, 0x1BEF )]
    public class CopperIngot : BaseIngot
    {
        [Constructable]
        public CopperIngot()
            : this( 1 )
        {
        }

        [Constructable]
        public CopperIngot( int amount )
            : base( Core.AOS ? CraftResource.Copper : CraftResource.OldCopper, amount )
        {
        }

        public CopperIngot( Serial serial )
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

    [FlipableAttribute( 0x1BF2, 0x1BEF )]
    public class BronzeIngot : BaseIngot
    {
        [Constructable]
        public BronzeIngot()
            : this( 1 )
        {
        }

        [Constructable]
        public BronzeIngot( int amount )
            : base( Core.AOS ? CraftResource.Bronze : CraftResource.OldBronze, amount )
        {
        }

        public BronzeIngot( Serial serial )
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

    [FlipableAttribute( 0x1BF2, 0x1BEF )]
    public class GoldIngot : BaseIngot
    {
        [Constructable]
        public GoldIngot()
            : this( 1 )
        {
        }

        [Constructable]
        public GoldIngot( int amount )
            : base( Core.AOS ? CraftResource.Gold : CraftResource.OldGold, amount )
        {
        }

        public GoldIngot( Serial serial )
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

    [FlipableAttribute( 0x1BF2, 0x1BEF )]
    public class AgapiteIngot : BaseIngot
    {
        [Constructable]
        public AgapiteIngot()
            : this( 1 )
        {
        }

        [Constructable]
        public AgapiteIngot( int amount )
            : base( Core.AOS ? CraftResource.Agapite : CraftResource.OldAgapite, amount )
        {
        }

        public AgapiteIngot( Serial serial )
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

    [FlipableAttribute( 0x1BF2, 0x1BEF )]
    public class VeriteIngot : BaseIngot
    {
        [Constructable]
        public VeriteIngot()
            : this( 1 )
        {
        }

        [Constructable]
        public VeriteIngot( int amount )
            : base( Core.AOS ? CraftResource.Verite : CraftResource.OldVerite, amount )
        {
        }

        public VeriteIngot( Serial serial )
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

    [FlipableAttribute( 0x1BF2, 0x1BEF )]
    public class ValoriteIngot : BaseIngot
    {
        [Constructable]
        public ValoriteIngot()
            : this( 1 )
        {
        }

        [Constructable]
        public ValoriteIngot( int amount )
            : base( Core.AOS ? CraftResource.Valorite : CraftResource.OldValorite, amount )
        {
        }

        public ValoriteIngot( Serial serial )
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

    #region mod by Dies Irae
    [FlipableAttribute( 0x1BF2, 0x1BEF )]
    public class PlatinumIngot : BaseIngot
    {
        [Constructable]
        public PlatinumIngot()
            : this( 1 )
        {
        }

        [Constructable]
        public PlatinumIngot( int amount )
            : base( Core.AOS ? CraftResource.Platinum : CraftResource.OldPlatinum, amount )
        {
        }

        public PlatinumIngot( Serial serial )
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

    [FlipableAttribute( 0x1BF2, 0x1BEF )]
    public class TitaniumIngot : BaseIngot
    {
        [Constructable]
        public TitaniumIngot()
            : this( 1 )
        {
        }

        [Constructable]
        public TitaniumIngot( int amount )
            : base( Core.AOS ? CraftResource.Titanium : CraftResource.OldTitanium, amount )
        {
        }

        public TitaniumIngot( Serial serial )
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

    [FlipableAttribute( 0x1BF2, 0x1BEF )]
    public class ObsidianIngot : BaseIngot
    {
        [Constructable]
        public ObsidianIngot()
            : this( 1 )
        {
        }

        [Constructable]
        public ObsidianIngot( int amount )
            : base( Core.AOS ? CraftResource.Obsidian : CraftResource.OldObsidian, amount )
        {
        }

        public ObsidianIngot( Serial serial )
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

    [FlipableAttribute( 0x1BF2, 0x1BEF )]
    public class DarkRubyIngot : BaseIngot
    {
        [Constructable]
        public DarkRubyIngot()
            : this( 1 )
        {
        }

        [Constructable]
        public DarkRubyIngot( int amount )
            : base( Core.AOS ? CraftResource.DarkRuby : CraftResource.OldDarkRuby, amount )
        {
        }

        public DarkRubyIngot( Serial serial )
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

    [FlipableAttribute( 0x1BF2, 0x1BEF )]
    public class EbonSapphireIngot : BaseIngot
    {
        [Constructable]
        public EbonSapphireIngot()
            : this( 1 )
        {
        }

        [Constructable]
        public EbonSapphireIngot( int amount )
            : base( Core.AOS ? CraftResource.EbonSapphire : CraftResource.OldEbonSapphire, amount )
        {
        }

        public EbonSapphireIngot( Serial serial )
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

    [FlipableAttribute( 0x1BF2, 0x1BEF )]
    public class RadiantDiamondIngot : BaseIngot
    {
        [Constructable]
        public RadiantDiamondIngot()
            : this( 1 )
        {
        }

        [Constructable]
        public RadiantDiamondIngot( int amount )
            : base( Core.AOS ? CraftResource.RadiantDiamond : CraftResource.OldRadiantDiamond, amount )
        {
        }

        public RadiantDiamondIngot( Serial serial )
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

    [FlipableAttribute( 0x1BF2, 0x1BEF )]
    public class EldarIngot : BaseIngot
    {
        public EldarIngot()
            : this( 1 )
        {
        }

        public EldarIngot( int amount )
            : base( CraftResource.Eldar, amount )
        {
        }

        public EldarIngot( Serial serial )
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

    [FlipableAttribute( 0x1BF2, 0x1BEF )]
    public class CrystalineIngot : BaseIngot
    {
        public CrystalineIngot()
            : this( 1 )
        {
        }

        public CrystalineIngot( int amount )
            : base( CraftResource.Crystaline, amount )
        {
        }

        public CrystalineIngot( Serial serial )
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

    [FlipableAttribute( 0x1BF2, 0x1BEF )]
    public class VulcanIngot : BaseIngot
    {
        public VulcanIngot()
            : this( 1 )
        {
        }

        public VulcanIngot( int amount )
            : base( CraftResource.Vulcan, amount )
        {
        }

        public VulcanIngot( Serial serial )
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

    [FlipableAttribute( 0x1BF2, 0x1BEF )]
    public class AquaIngot : BaseIngot
    {
        public AquaIngot()
            : this( 1 )
        {
        }

        public AquaIngot( int amount )
            : base( CraftResource.Aqua, amount )
        {
        }

        public AquaIngot( Serial serial )
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

    #region mod by Dies Irae : pre-Aos stuff
    [FlipableAttribute( 0x1BF2, 0x1BEF )]
    public class GraphiteIngot : BaseIngot
    {
        [Constructable]
        public GraphiteIngot()
            : this( 1 )
        {
        }

        [Constructable]
        public GraphiteIngot( int amount )
            : base( CraftResource.OldGraphite, amount )
        {
        }

        #region serialization
        public GraphiteIngot( Serial serial )
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
        #endregion
    }

    [FlipableAttribute( 0x1BF2, 0x1BEF )]
    public class PyriteIngot : BaseIngot
    {
        [Constructable]
        public PyriteIngot()
            : this( 1 )
        {
        }

        [Constructable]
        public PyriteIngot( int amount )
            : base( CraftResource.OldPyrite, amount )
        {
        }

        #region serialization
        public PyriteIngot( Serial serial )
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
        #endregion
    }

    [FlipableAttribute( 0x1BF2, 0x1BEF )]
    public class AzuriteIngot : BaseIngot
    {
        [Constructable]
        public AzuriteIngot()
            : this( 1 )
        {
        }

        [Constructable]
        public AzuriteIngot( int amount )
            : base( CraftResource.OldAzurite, amount )
        {
        }

        #region serialization
        public AzuriteIngot( Serial serial )
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
        #endregion
    }

    [FlipableAttribute( 0x1BF2, 0x1BEF )]
    public class VanadiumIngot : BaseIngot
    {
        [Constructable]
        public VanadiumIngot()
            : this( 1 )
        {
        }

        [Constructable]
        public VanadiumIngot( int amount )
            : base( CraftResource.OldVanadium, amount )
        {
        }

        #region serialization
        public VanadiumIngot( Serial serial )
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
        #endregion
    }

    [FlipableAttribute( 0x1BF2, 0x1BEF )]
    public class SilverIngot : BaseIngot
    {
        [Constructable]
        public SilverIngot()
            : this( 1 )
        {
        }

        [Constructable]
        public SilverIngot( int amount )
            : base( CraftResource.OldSilver, amount )
        {
        }

        #region serialization
        public SilverIngot( Serial serial )
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
        #endregion
    }

    [FlipableAttribute( 0x1BF2, 0x1BEF )]
    public class AmethystIngot : BaseIngot
    {
        [Constructable]
        public AmethystIngot()
            : this( 1 )
        {
        }

        [Constructable]
        public AmethystIngot( int amount )
            : base( CraftResource.OldAmethyst, amount )
        {
        }

        #region serialization
        public AmethystIngot( Serial serial )
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
        #endregion
    }

    [FlipableAttribute( 0x1BF2, 0x1BEF )]
    public class XenianIngot : BaseIngot
    {
        [Constructable]
        public XenianIngot()
            : this( 1 )
        {
        }

        [Constructable]
        public XenianIngot( int amount )
            : base( CraftResource.OldXenian, amount )
        {
        }

        #region serialization
        public XenianIngot( Serial serial )
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
        #endregion
    }

    [FlipableAttribute( 0x1BF2, 0x1BEF )]
    public class RubidianIngot : BaseIngot
    {
        [Constructable]
        public RubidianIngot()
            : this( 1 )
        {
        }

        [Constructable]
        public RubidianIngot( int amount )
            : base( CraftResource.OldRubidian, amount )
        {
        }

        #region serialization
        public RubidianIngot( Serial serial )
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
        #endregion
    }
    #endregion
}