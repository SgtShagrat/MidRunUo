using System;
using Server.Items;
using Server.Network;

namespace Server.Items
{
	public abstract class BaseHides : Item, ICommodity
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
                return String.Format( Amount == 1 ? "{0} pile of {1} hides" : "{0} piles of {1} hides", Amount, CraftResources.GetName( m_Resource ).ToLower() );
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

		public BaseHides( CraftResource resource ) : this( resource, 1 )
		{
		}

		public BaseHides( CraftResource resource, int amount ) : base( 0x1079 )
		{
			Stackable = true;
			Weight = 5.0;
			Amount = amount;
			Hue = CraftResources.GetHue( resource );

			m_Resource = resource;
		}

		public BaseHides( Serial serial ) : base( serial )
		{
		}

		public override void AddNameProperty( ObjectPropertyList list )
		{
			if ( Amount > 1 )
				list.Add( 1050039, "{0}\t#{1}", Amount, 1024216 ); // ~1_NUMBER~ ~2_ITEMNAME~
			else
				list.Add( 1024216 ); // pile of hides
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
					return 1049687 + (int)(m_Resource - CraftResource.SpinedLeather);
                #region mod by Dies Irae
                else if( m_Resource >= CraftResource.HumanoidLeather && m_Resource <= CraftResource.AbyssLeather )
                    return 1066260 + (int)(m_Resource - CraftResource.HumanoidLeather);
				#endregion

				return 1047023;
			}
		}

        #region mod by Dies Irae : pre-aos stuff
        public override void OnSingleClick( Mobile from )
        {
            LabelTo( from, String.Format( Amount == 1 ? "{0} piece of {1} hides" : 
                                        "{0} pieces of {1} hides", Amount, 
                                        CraftResources.GetName( m_Resource ).ToLower() ) );
        } 
        #endregion
	}

	[FlipableAttribute( 0x1079, 0x1078 )]
	public class Hides : BaseHides, IScissorable
	{
		[Constructable]
		public Hides() : this( 1 )
		{
		}

		[Constructable]
		public Hides( int amount ) : base( CraftResource.RegularLeather, amount )
		{
		}

		public Hides( Serial serial ) : base( serial )
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

		

		public bool Scissor( Mobile from, Scissors scissors )
		{
			if ( Deleted || !from.CanSee( this ) ) return false;

			base.ScissorHelper( from, new Leather(), 1 );

			return true;
		}
	}

	[FlipableAttribute( 0x1079, 0x1078 )]
	public class SpinedHides : BaseHides, IScissorable
	{
		[Constructable]
		public SpinedHides() : this( 1 )
		{
		}

		[Constructable]
		public SpinedHides( int amount ) : base(Core.AOS ? CraftResource.SpinedLeather : CraftResource.OldReptileLeather, amount )
		{
		}

		public SpinedHides( Serial serial ) : base( serial )
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

		

		public bool Scissor( Mobile from, Scissors scissors )
		{
			if ( Deleted || !from.CanSee( this ) ) return false;

			base.ScissorHelper( from, new SpinedLeather(), 1 );

			return true;
		}
	}

	[FlipableAttribute( 0x1079, 0x1078 )]
	public class HornedHides : BaseHides, IScissorable
	{
		[Constructable]
		public HornedHides() : this( 1 )
		{
		}

		[Constructable]
		public HornedHides( int amount ) : base( Core.AOS ? CraftResource.HornedLeather : CraftResource.OldOphidianLeather, amount )
		{
		}

		public HornedHides( Serial serial ) : base( serial )
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

		

		public bool Scissor( Mobile from, Scissors scissors )
		{
			if ( Deleted || !from.CanSee( this ) ) return false;

			base.ScissorHelper( from, new HornedLeather(), 1 );

			return true;
		}
	}

	[FlipableAttribute( 0x1079, 0x1078 )]
	public class BarbedHides : BaseHides, IScissorable
	{
		[Constructable]
		public BarbedHides() : this( 1 )
		{
		}

		[Constructable]
		public BarbedHides( int amount ) : base( Core.AOS ? CraftResource.BarbedLeather : CraftResource.OldHumanoidLeather, amount )
		{
		}

		public BarbedHides( Serial serial ) : base( serial )
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

		

		public bool Scissor( Mobile from, Scissors scissors )
		{
			if ( Deleted || !from.CanSee( this ) ) return false;

			base.ScissorHelper( from, new BarbedLeather(), 1 );

			return true;
		}
    }

    #region modifica by Dies Irae
    [FlipableAttribute( 0x1079, 0x1078 )]
    public class FeyHides : BaseHides, IScissorable
    {
        [Constructable]
        public FeyHides()
            : this( 1 )
        {
        }

        [Constructable]
        public FeyHides( int amount )
            : base( CraftResource.FeyLeather, amount )
        {
        }

        public FeyHides( Serial serial )
            : base( serial )
        {
        }

        public bool Scissor( Mobile from, Scissors scissors )
        {
            if( Deleted || !from.CanSee( this ) )
                return false;

            base.ScissorHelper( from, new FeyLeather(), 1 );

            return true;
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

    [FlipableAttribute( 0x1079, 0x1078 )]
    public class HumanoidHides : BaseHides, IScissorable
    {
        [Constructable]
        public HumanoidHides()
            : this( 1 )
        {
        }

        [Constructable]
        public HumanoidHides( int amount )
            : base( CraftResource.HumanoidLeather, amount )
        {
        }

        public HumanoidHides( Serial serial )
            : base( serial )
        {
        }

        public bool Scissor( Mobile from, Scissors scissors )
        {
            if( Deleted || !from.CanSee( this ) )
                return false;

            base.ScissorHelper( from, new HumanoidLeather(), 1 );

            return true;
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

    [FlipableAttribute( 0x1079, 0x1078 )]
    public class UndeadHides : BaseHides, IScissorable
    {
        [Constructable]
        public UndeadHides()
            : this( 1 )
        {
        }

        [Constructable]
        public UndeadHides( int amount )
            : base( Core.AOS ? CraftResource.UndeadLeather : CraftResource.OldUndeadLeather, amount )
        {
        }

        public UndeadHides( Serial serial )
            : base( serial )
        {
        }

        public bool Scissor( Mobile from, Scissors scissors )
        {
            if( Deleted || !from.CanSee( this ) )
                return false;

            base.ScissorHelper( from, new UndeadLeather(), 1 );

            return true;
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

    [FlipableAttribute( 0x1079, 0x1078 )]
    public class WolfHides : BaseHides, IScissorable
    {
        [Constructable]
        public WolfHides()
            : this( 1 )
        {
        }

        [Constructable]
        public WolfHides( int amount )
            : base( Core.AOS ? CraftResource.WolfLeather : CraftResource.OldWolfLeather, amount )
        {
        }

        public WolfHides( Serial serial )
            : base( serial )
        {
        }

        public bool Scissor( Mobile from, Scissors scissors )
        {
            if( Deleted || !from.CanSee( this ) )
                return false;

            base.ScissorHelper( from, new WolfLeather(), 1 );

            return true;
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

    [FlipableAttribute( 0x1079, 0x1078 )]
    public class AracnidHides : BaseHides, IScissorable
    {
        [Constructable]
        public AracnidHides()
            : this( 1 )
        {
        }

        [Constructable]
        public AracnidHides( int amount )
            : base( Core.AOS ? CraftResource.AracnidLeather : CraftResource.OldArachnidLeather, amount )
        {
        }

        public AracnidHides( Serial serial )
            : base( serial )
        {
        }

        public bool Scissor( Mobile from, Scissors scissors )
        {
            if( Deleted || !from.CanSee( this ) )
                return false;

            base.ScissorHelper( from, new AracnidLeather(), 1 );

            return true;
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

    [FlipableAttribute( 0x1079, 0x1078 )]
    public class GreenDragonHides : BaseHides, IScissorable
    {
        [Constructable]
        public GreenDragonHides()
            : this( 1 )
        {
        }

        [Constructable]
        public GreenDragonHides( int amount )
            : base(  Core.AOS ? CraftResource.GreenDragonLeather : CraftResource.OldGreenDragonLeather, amount )
        {
        }

        public GreenDragonHides( Serial serial )
            : base( serial )
        {
        }

        public bool Scissor( Mobile from, Scissors scissors )
        {
            if( Deleted || !from.CanSee( this ) )
                return false;

            base.ScissorHelper( from, new GreenDragonLeather(), 1 );

            return true;
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

    [FlipableAttribute( 0x1079, 0x1078 )]
    public class BlackDragonHides : BaseHides, IScissorable
    {
        [Constructable]
        public BlackDragonHides()
            : this( 1 )
        {
        }

        [Constructable]
        public BlackDragonHides( int amount )
            : base( Core.AOS ? CraftResource.BlackDragonLeather : CraftResource.OldBlackDragonLeather, amount )
        {
        }

        public BlackDragonHides( Serial serial )
            : base( serial )
        {
        }

        public bool Scissor( Mobile from, Scissors scissors )
        {
            if( Deleted || !from.CanSee( this ) )
                return false;

            base.ScissorHelper( from, new BlackDragonLeather(), 1 );

            return true;
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

    [FlipableAttribute( 0x1079, 0x1078 )]
    public class BlueDragonHides : BaseHides, IScissorable
    {
        [Constructable]
        public BlueDragonHides()
            : this( 1 )
        {
        }

        [Constructable]
        public BlueDragonHides( int amount )
            : base( Core.AOS ? CraftResource.BlueDragonLeather : CraftResource.OldBlueDragonLeather, amount )
        {
        }

        public BlueDragonHides( Serial serial )
            : base( serial )
        {
        }

        public bool Scissor( Mobile from, Scissors scissors )
        {
            if( Deleted || !from.CanSee( this ) )
                return false;

            base.ScissorHelper( from, new BlueDragonLeather(), 1 );

            return true;
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

    [FlipableAttribute( 0x1079, 0x1078 )]
    public class RedDragonHides : BaseHides, IScissorable
    {
        [Constructable]
        public RedDragonHides()
            : this( 1 )
        {
        }

        [Constructable]
        public RedDragonHides( int amount )
            : base( Core.AOS ? CraftResource.RedDragonLeather : CraftResource.OldRedDragonLeather, amount )
        {
        }

        public RedDragonHides( Serial serial )
            : base( serial )
        {
        }

        public bool Scissor( Mobile from, Scissors scissors )
        {
            if( Deleted || !from.CanSee( this ) )
                return false;

            base.ScissorHelper( from, new RedDragonLeather(), 1 );

            return true;
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

    [FlipableAttribute( 0x1079, 0x1078 )]
    public class AbyssHides : BaseHides, IScissorable
    {
        [Constructable]
        public AbyssHides()
            : this( 1 )
        {
        }

        [Constructable]
        public AbyssHides( int amount )
            : base( Core.AOS ? CraftResource.AbyssLeather : CraftResource.OldDemonLeather, amount )
        {
        }

        public AbyssHides( Serial serial )
            : base( serial )
        {
        }

        public bool Scissor( Mobile from, Scissors scissors )
        {
            if( Deleted || !from.CanSee( this ) )
                return false;

            base.ScissorHelper( from, new AbyssLeather(), 1 );

            return true;
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
    [FlipableAttribute( 0x1079, 0x1078 )]
    public class ArcticHides : BaseHides, IScissorable
    {
        [Constructable]
        public ArcticHides()
            : this( 1 )
        {
        }

        [Constructable]
        public ArcticHides( int amount )
            : base( CraftResource.OldArcticLeather, amount )
        {
        }

        public ArcticHides( Serial serial )
            : base( serial )
        {
        }

        public bool Scissor( Mobile from, Scissors scissors )
        {
            if( Deleted || !from.CanSee( this ) )
                return false;

            base.ScissorHelper( from, new ArcticLeather(), 1 );

            return true;
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

    [FlipableAttribute( 0x1079, 0x1078 )]
    public class BearHides : BaseHides, IScissorable
    {
        [Constructable]
        public BearHides()
            : this( 1 )
        {
        }

        [Constructable]
        public BearHides( int amount )
            : base( CraftResource.OldBearLeather, amount )
        {
        }

        public BearHides( Serial serial )
            : base( serial )
        {
        }

        public bool Scissor( Mobile from, Scissors scissors )
        {
            if( Deleted || !from.CanSee( this ) )
                return false;

            base.ScissorHelper( from, new BearLeather(), 1 );

            return true;
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

    [FlipableAttribute( 0x1079, 0x1078 )]
    public class LavaHides : BaseHides, IScissorable
    {
        [Constructable]
        public LavaHides()
            : this( 1 )
        {
        }

        [Constructable]
        public LavaHides( int amount )
            : base( CraftResource.OldLavaLeather, amount )
        {
        }

        public LavaHides( Serial serial )
            : base( serial )
        {
        }

        public bool Scissor( Mobile from, Scissors scissors )
        {
            if( Deleted || !from.CanSee( this ) )
                return false;

            base.ScissorHelper( from, new LavaLeather(), 1 );

            return true;
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

    [FlipableAttribute( 0x1079, 0x1078 )]
    public class OrcishHides : BaseHides, IScissorable
    {
        [Constructable]
        public OrcishHides()
            : this( 1 )
        {
        }

        [Constructable]
        public OrcishHides( int amount )
            : base( CraftResource.OldOrcishLeather, amount )
        {
        }

        public OrcishHides( Serial serial )
            : base( serial )
        {
        }

        public bool Scissor( Mobile from, Scissors scissors )
        {
            if( Deleted || !from.CanSee( this ) )
                return false;

            base.ScissorHelper( from, new OrcishLeather(), 1 );

            return true;
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