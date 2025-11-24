using System;
using System.Text;
using Server.Network;

namespace Server.Items
{
	public abstract class BaseGranite : Item, ICommodity
	{
		private CraftResource m_Resource;

		[CommandProperty( AccessLevel.GameMaster )]
		public CraftResource Resource
		{
			get{ return m_Resource; }
			set{ m_Resource = value; InvalidateProperties(); }
		}
		
		string ICommodity.Description
		{
			get
			{
				return String.Format( "{0} {1} high quality granite", Amount, CraftResources.GetName( m_Resource ).ToLower() );
			}
		}

        int ICommodity.DescriptionNumber { get { return 0; } }

        #region mod by Dies Irae
        public override void OnSingleClick( Mobile from )
        {
            string format = ( Amount == 1 ? "{0} {1} high quality granite" : "{0} {1} high quality granites" );

            LabelTo( from, format, Amount, CraftResources.GetName( m_Resource ).ToLower() );
        }
        #endregion

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			writer.Write( (int) m_Resource );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
				{
					m_Resource = (CraftResource)reader.ReadInt();
					break;
				}
			}
		}

		public BaseGranite( CraftResource resource ) : base( 0x1779 )
		{
			Weight = 10.0;
			Hue = CraftResources.GetHue( resource );
            Stackable = true;

			m_Resource = resource;
		}

		public BaseGranite( Serial serial ) : base( serial )
		{
		}

		public override int LabelNumber{ get{ return 1044607; } } // high quality granite

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			if ( !CraftResources.IsStandard( m_Resource ) )
			{
				int num = CraftResources.GetLocalizationNumber( m_Resource );

				if ( num > 0 )
					list.Add( num );
				else
					list.Add( CraftResources.GetName( m_Resource ) );
			}
        }

        public static Item GetGraniteFor( CraftResource craftResource )
        {
            switch( craftResource )
            {
                case CraftResource.OldDullCopper: return new Granite();
                case CraftResource.OldShadowIron: return new ShadowIronGranite();
                case CraftResource.OldCopper: return new CopperGranite();
                case CraftResource.OldBronze: return new BronzeGranite();
                case CraftResource.OldGold: return new GoldGranite();
                case CraftResource.OldAgapite: return new AgapiteGranite();
                case CraftResource.OldVerite: return new VeriteGranite();
                case CraftResource.OldValorite: return new ValoriteGranite();
                case CraftResource.OldGraphite: return new GraphiteGranite();
                case CraftResource.OldPyrite: return new PyriteGranite();
                case CraftResource.OldAzurite: return new AzuriteGranite();
                case CraftResource.OldVanadium: return new VanadiumGranite();
                case CraftResource.OldSilver: return new SilverGranite();
                case CraftResource.OldPlatinum: return new PlatinumGranite();
                case CraftResource.OldAmethyst: return new AmethystGranite();
                case CraftResource.OldTitanium: return new TitaniumGranite();
                case CraftResource.OldXenian: return new XenianGranite();
                case CraftResource.OldRubidian: return new RubidianGranite();
                case CraftResource.OldObsidian: return new ObsidianGranite();
                case CraftResource.OldEbonSapphire: return new EbonSapphireGranite();
                case CraftResource.OldDarkRuby: return new DarkRubyGranite();
                case CraftResource.OldRadiantDiamond: return new RadiantDiamondGranite();
                default: return new Granite();
            }
        }
    }

	public class Granite : BaseGranite
	{
		[Constructable]
		public Granite() : base( CraftResource.Iron )
		{
		}

		public Granite( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class DullCopperGranite : BaseGranite
	{
		[Constructable]
		public DullCopperGranite() : base( Core.AOS ? CraftResource.DullCopper : CraftResource.OldDullCopper )
		{
		}

		public DullCopperGranite( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class ShadowIronGranite : BaseGranite
	{
		[Constructable]
		public ShadowIronGranite() : base( CraftResource.ShadowIron )
		{
		}

		public ShadowIronGranite( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class CopperGranite : BaseGranite
	{
		[Constructable]
        public CopperGranite()
            : base( Core.AOS ? CraftResource.Copper : CraftResource.OldCopper )
		{
		}

		public CopperGranite( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class BronzeGranite : BaseGranite
	{
		[Constructable]
        public BronzeGranite()
            : base( Core.AOS ? CraftResource.Bronze : CraftResource.OldBronze )
		{
		}

		public BronzeGranite( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class GoldGranite : BaseGranite
	{
		[Constructable]
        public GoldGranite()
            : base( Core.AOS ? CraftResource.Gold : CraftResource.OldGold )
		{
		}

		public GoldGranite( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class AgapiteGranite : BaseGranite
	{
		[Constructable]
        public AgapiteGranite()
            : base( Core.AOS ? CraftResource.Agapite : CraftResource.OldAgapite )
		{
		}

		public AgapiteGranite( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class VeriteGranite : BaseGranite
	{
		[Constructable]
		public VeriteGranite() : base( Core.AOS ? CraftResource.Verite : CraftResource.OldVerite )
		{
		}

		public VeriteGranite( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class ValoriteGranite : BaseGranite
	{
		[Constructable]
		public ValoriteGranite() : base( Core.AOS ? CraftResource.Valorite : CraftResource.OldValorite )
		{
		}

		public ValoriteGranite( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
    }

    #region mod by Dies Irae
    public class PlatinumGranite : BaseGranite
	{
		[Constructable]
        public PlatinumGranite()
            : base( Core.AOS ? CraftResource.Platinum : CraftResource.OldPlatinum )
		{
		}

		public PlatinumGranite( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class TitaniumGranite : BaseGranite
	{
		[Constructable]
        public TitaniumGranite()
            : base( Core.AOS ? CraftResource.Titanium : CraftResource.OldTitanium )
		{
		}

		public TitaniumGranite( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class ObsidianGranite : BaseGranite
	{
		[Constructable]
        public ObsidianGranite()
            : base( Core.AOS ? CraftResource.Obsidian : CraftResource.OldObsidian )
		{
		}

		public ObsidianGranite( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class DarkRubyGranite : BaseGranite
	{
		[Constructable]
        public DarkRubyGranite()
            : base( Core.AOS ? CraftResource.DarkRuby : CraftResource.OldDarkRuby )
		{
		}

		public DarkRubyGranite( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class EbonSapphireGranite : BaseGranite
	{
		[Constructable]
        public EbonSapphireGranite()
            : base( Core.AOS ? CraftResource.EbonSapphire : CraftResource.OldEbonSapphire )
		{
		}

		public EbonSapphireGranite( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class RadiantDiamondGranite : BaseGranite
	{
		[Constructable]
        public RadiantDiamondGranite()
            : base( Core.AOS ? CraftResource.RadiantDiamond : CraftResource.OldRadiantDiamond )
		{
		}

		public RadiantDiamondGranite( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class EldarGranite : BaseGranite
	{
		[Constructable]
		public EldarGranite() : base( CraftResource.Eldar )
		{
		}

		public EldarGranite( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class CrystalineGranite : BaseGranite
	{
		[Constructable]
		public CrystalineGranite() : base( CraftResource.Crystaline )
		{
		}

		public CrystalineGranite( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class VulcanGranite : BaseGranite
	{
		[Constructable]
		public VulcanGranite() : base( CraftResource.Vulcan )
		{
		}

		public VulcanGranite( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class AquaGranite : BaseGranite
	{
		[Constructable]
		public AquaGranite() : base( CraftResource.Aqua )
		{
		}

		public AquaGranite( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
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
    public class GraphiteGranite : BaseGranite
    {
        [Constructable]
        public GraphiteGranite()
            : base( CraftResource.OldGraphite )
        {
        }

        #region serialization
        public GraphiteGranite( Serial serial )
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
    public class PyriteGranite : BaseGranite
    {
        [Constructable]
        public PyriteGranite()
            : base( CraftResource.OldPyrite )
        {
        }

        #region serialization
        public PyriteGranite( Serial serial )
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
    public class AzuriteGranite : BaseGranite
    {
        [Constructable]
        public AzuriteGranite()
            : base( CraftResource.OldAzurite )
        {
        }

        #region serialization
        public AzuriteGranite( Serial serial )
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
    public class VanadiumGranite : BaseGranite
    {
        [Constructable]
        public VanadiumGranite()
            : base( CraftResource.OldVanadium )
        {
        }

        #region serialization
        public VanadiumGranite( Serial serial )
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
    public class SilverGranite : BaseGranite
    {
        [Constructable]
        public SilverGranite()
            : base( CraftResource.OldSilver )
        {
        }

        #region serialization
        public SilverGranite( Serial serial )
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
    public class AmethystGranite : BaseGranite
    {
        [Constructable]
        public AmethystGranite()
            : base( CraftResource.OldAmethyst )
        {
        }

        #region serialization
        public AmethystGranite( Serial serial )
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
    public class XenianGranite : BaseGranite
    {
        [Constructable]
        public XenianGranite()
            : base( CraftResource.OldXenian )
        {
        }

        #region serialization
        public XenianGranite( Serial serial )
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
    public class RubidianGranite : BaseGranite
    {
        [Constructable]
        public RubidianGranite()
            : base( CraftResource.OldRubidian )
        {
        }

        #region serialization
        public RubidianGranite( Serial serial )
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
