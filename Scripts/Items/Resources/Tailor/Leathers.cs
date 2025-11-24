using System;
using Server.Items;
using Server.Network;

namespace Server.Items
{
	public abstract class BaseLeather : Item, ICommodity
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
				return String.Format( Amount == 1 ? "{0} piece of {1} leather" : "{0} pieces of {1} leather", Amount, CraftResources.GetName( m_Resource ).ToLower() );	
			}
		}

		int ICommodity.DescriptionNumber { get { return LabelNumber; } }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version

			writer.Write( (int) m_Resource );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 1:
				{
					m_Resource = (CraftResource)reader.ReadInt();
					break;
				}
				case 0:
				{
					OreInfo info = new OreInfo( reader.ReadInt(), reader.ReadInt(), reader.ReadString() );

					m_Resource = CraftResources.GetFromOreInfo( info );
					break;
				}
			}
		}

		public BaseLeather( CraftResource resource ) : this( resource, 1 )
		{
		}

		public BaseLeather( CraftResource resource, int amount ) : base( 0x1081 )
		{
			Stackable = true;
			Weight = 1.0;
			Amount = amount;
			Hue = CraftResources.GetHue( resource );

			m_Resource = resource;
		}

		public BaseLeather( Serial serial ) : base( serial )
		{
		}

		public override void AddNameProperty( ObjectPropertyList list )
		{
			if ( Amount > 1 )
				list.Add( 1050039, "{0}\t#{1}", Amount, 1024199 ); // ~1_NUMBER~ ~2_ITEMNAME~
			else
				list.Add( 1024199 ); // cut leather
		}

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

		public override int LabelNumber
		{
			get
			{
				if ( m_Resource >= CraftResource.SpinedLeather && m_Resource <= CraftResource.BarbedLeather )
					return 1049684 + (int)(m_Resource - CraftResource.SpinedLeather);
                #region mod by Dies Irae
                else if( m_Resource >= CraftResource.HumanoidLeather && m_Resource <= CraftResource.AbyssLeather )
                    return 1066220 + (int)( m_Resource - CraftResource.HumanoidLeather );
                #endregion

                return 1047022;
			}
		}

        #region mod by Dies Irae : pre-aos stuff
        public override void OnSingleClick( Mobile from )
        {
            LabelTo( from, String.Format( Amount == 1 ? "{0} piece of {1} leather" : 
                                        "{0} pieces of {1} leather", Amount, 
                                        CraftResources.GetName( m_Resource ).ToLower() ) );
        } 
        #endregion
	}

	[FlipableAttribute( 0x1081, 0x1082 )]
    public class Leather : BaseLeather, Midgard.Items.IFurLeather
	{
		[Constructable]
		public Leather() : this( 1 )
		{
		}

		[Constructable]
		public Leather( int amount ) : base( CraftResource.RegularLeather, amount )
		{
		}

		public Leather( Serial serial ) : base( serial )
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

	[FlipableAttribute( 0x1081, 0x1082 )]
	public class SpinedLeather : BaseLeather
	{
		[Constructable]
		public SpinedLeather() : this( 1 )
		{
		}

		[Constructable]
		public SpinedLeather( int amount ) : base( Core.AOS ? CraftResource.SpinedLeather : CraftResource.OldReptileLeather, amount )
		{
		}

		public SpinedLeather( Serial serial ) : base( serial )
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

	[FlipableAttribute( 0x1081, 0x1082 )]
	public class HornedLeather : BaseLeather
	{
		[Constructable]
		public HornedLeather() : this( 1 )
		{
		}

		[Constructable]
		public HornedLeather( int amount ) : base( Core.AOS ? CraftResource.HornedLeather : CraftResource.OldOphidianLeather, amount )
		{
		}

		public HornedLeather( Serial serial ) : base( serial )
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

	[FlipableAttribute( 0x1081, 0x1082 )]
	public class BarbedLeather : BaseLeather
	{
		[Constructable]
		public BarbedLeather() : this( 1 )
		{
		}

		[Constructable]
		public BarbedLeather( int amount ) : base( Core.AOS ? CraftResource.BarbedLeather : CraftResource.OldHumanoidLeather, amount )
		{
		}

		public BarbedLeather( Serial serial ) : base( serial )
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

    #region modifica by Dies Irae
    [FlipableAttribute( 0x1081, 0x1082 )]
    public class FeyLeather : BaseLeather
    {
        [Constructable]
        public FeyLeather()
            : this( 1 )
        {
        }

        [Constructable]
        public FeyLeather( int amount )
            : base( CraftResource.FeyLeather, amount )
        {
        }

        public FeyLeather( Serial serial )
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

    [FlipableAttribute( 0x1081, 0x1082 )]
    public class HumanoidLeather : BaseLeather
    {
        [Constructable]
        public HumanoidLeather()
            : this( 1 )
        {
        }

        [Constructable]
        public HumanoidLeather( int amount )
            : base( CraftResource.HumanoidLeather, amount )
        {
        }

        public HumanoidLeather( Serial serial )
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

    [FlipableAttribute( 0x1081, 0x1082 )]
    public class UndeadLeather : BaseLeather
    {
        [Constructable]
        public UndeadLeather()
            : this( 1 )
        {
        }

        [Constructable]
        public UndeadLeather( int amount )
            : base( Core.AOS ? CraftResource.UndeadLeather : CraftResource.OldUndeadLeather, amount )
        {
        }

        public UndeadLeather( Serial serial )
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

    [FlipableAttribute( 0x1081, 0x1082 )]
    public class WolfLeather : BaseLeather, Midgard.Items.IFurLeather
    {
        [Constructable]
        public WolfLeather()
            : this( 1 )
        {
        }

        [Constructable]
        public WolfLeather( int amount )
            : base( Core.AOS ? CraftResource.WolfLeather : CraftResource.OldWolfLeather, amount )
        {
        }

        public WolfLeather( Serial serial )
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

    [FlipableAttribute( 0x1081, 0x1082 )]
    public class AracnidLeather : BaseLeather, Midgard.Items.IFurLeather
    {
        [Constructable]
        public AracnidLeather()
            : this( 1 )
        {
        }

        [Constructable]
        public AracnidLeather( int amount )
            : base( Core.AOS ? CraftResource.AracnidLeather : CraftResource.OldArachnidLeather, amount )
        {
        }

        public AracnidLeather( Serial serial )
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

    [FlipableAttribute( 0x1081, 0x1082 )]
    public class GreenDragonLeather : BaseLeather
    {
        [Constructable]
        public GreenDragonLeather()
            : this( 1 )
        {
        }

        [Constructable]
        public GreenDragonLeather( int amount )
            : base( Core.AOS ? CraftResource.GreenDragonLeather : CraftResource.OldGreenDragonLeather, amount )
        {
        }

        public GreenDragonLeather( Serial serial )
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

    [FlipableAttribute( 0x1081, 0x1082 )]
    public class BlackDragonLeather : BaseLeather
    {
        [Constructable]
        public BlackDragonLeather()
            : this( 1 )
        {
        }

        [Constructable]
        public BlackDragonLeather( int amount )
            : base( Core.AOS ? CraftResource.BlackDragonLeather : CraftResource.OldBlackDragonLeather, amount )
        {
        }

        public BlackDragonLeather( Serial serial )
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

    [FlipableAttribute( 0x1081, 0x1082 )]
    public class BlueDragonLeather : BaseLeather
    {
        [Constructable]
        public BlueDragonLeather()
            : this( 1 )
        {
        }

        [Constructable]
        public BlueDragonLeather( int amount )
            : base( Core.AOS ? CraftResource.BlueDragonLeather : CraftResource.OldBlueDragonLeather, amount )
        {
        }

        public BlueDragonLeather( Serial serial )
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

    [FlipableAttribute( 0x1081, 0x1082 )]
    public class RedDragonLeather : BaseLeather
    {
        [Constructable]
        public RedDragonLeather()
            : this( 1 )
        {
        }

        [Constructable]
        public RedDragonLeather( int amount )
            : base( Core.AOS ? CraftResource.RedDragonLeather : CraftResource.OldRedDragonLeather, amount )
        {
        }

        public RedDragonLeather( Serial serial )
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

    [FlipableAttribute( 0x1081, 0x1082 )]
    public class AbyssLeather : BaseLeather
    {
        [Constructable]
        public AbyssLeather()
            : this( 1 )
        {
        }

        [Constructable]
        public AbyssLeather( int amount )
            : base( Core.AOS ? CraftResource.AbyssLeather : CraftResource.OldDemonLeather, amount )
        {
        }

        public AbyssLeather( Serial serial )
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

    #region mod by Dies Irae : pre-aos stuff
    [FlipableAttribute( 0x1081, 0x1082 )]
    public class ArcticLeather : BaseLeather
    {
        [Constructable]
        public ArcticLeather()
            : this( 1 )
        {
        }

        [Constructable]
        public ArcticLeather( int amount )
            : base( CraftResource.OldArcticLeather, amount )
        {
        }

        public ArcticLeather( Serial serial )
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

    [FlipableAttribute( 0x1081, 0x1082 )]
    public class BearLeather : BaseLeather, Midgard.Items.IFurLeather
    {
        [Constructable]
        public BearLeather()
            : this( 1 )
        {
        }

        [Constructable]
        public BearLeather( int amount )
            : base( CraftResource.OldBearLeather, amount )
        {
        }

        public BearLeather( Serial serial )
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

    [FlipableAttribute( 0x1081, 0x1082 )]
    public class LavaLeather : BaseLeather
    {
        [Constructable]
        public LavaLeather()
            : this( 1 )
        {
        }

        [Constructable]
        public LavaLeather( int amount )
            : base( CraftResource.OldLavaLeather, amount )
        {
        }

        public LavaLeather( Serial serial )
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

    [FlipableAttribute( 0x1081, 0x1082 )]
    public class OrcishLeather : BaseLeather
    {
        [Constructable]
        public OrcishLeather()
            : this( 1 )
        {
        }

        [Constructable]
        public OrcishLeather( int amount )
            : base( CraftResource.OldOrcishLeather, amount )
        {
        }

        public OrcishLeather( Serial serial )
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