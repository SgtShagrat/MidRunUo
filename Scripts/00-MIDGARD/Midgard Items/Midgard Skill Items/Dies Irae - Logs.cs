//#define PrePassaggioMondain

/***************************************************************************
 *                                        Logs.cs
 *                            		------------------
 *  begin                	: July, 2006
 *  version					: 1.0
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;

namespace Server.Items
{
	public abstract class BaseLog : Item, ICommodity, IAxe
	{
		private CraftResource m_Resource;

		[CommandProperty( AccessLevel.GameMaster )]
		public CraftResource Resource
		{
			get{ return m_Resource; }
			set{ m_Resource = value; InvalidateProperties(); }
		}

		public override double DefaultWeight
		{
			get { return 2.0; }
		}
		
		string ICommodity.Description
		{
			get
			{
				return String.Format( Amount == 1 ? "{0} {1} log" : "{0} {1} logs", Amount, CraftResources.GetName( m_Resource ).ToLower() );
			}
		}

        int ICommodity.DescriptionNumber { get { return 0; } }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version

			writer.Write( (int) m_Resource );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

#if PrePassaggioMondain
#else
			int version = reader.ReadInt();

			switch ( version )
			{
				case 1:
				{
					m_Resource = (CraftResource) reader.ReadInt();
					break;
				}
			}
#endif
		}

		public virtual bool TryCreateBoards( Mobile from, double skill, Item item )
		{
			if ( Deleted || !from.CanSee( this ) ) 
				return false;
			else if ( from.Skills.Carpentry.Value < skill && from.Skills.Lumberjacking.Value < skill )
			{
				from.SendLocalizedMessage( 1072652 ); // You cannot work this strange and unusual wood.
				return false;
			}

			base.ScissorHelper( from, item, 1, false );

			return true;
		}

		public virtual bool Axe( Mobile from, BaseWeapon axe )
		{
            double skill = 10.0;
            CraftResourceInfo info = CraftResources.GetInfo( Resource );
            if( info != null )
                skill = info.AttributeInfo.OldAxeRequiredSkill;

            if ( !TryCreateBoards( from , skill, GetBoard() ) )
                return false;
			
			return true;
		}

	    public abstract BaseBoards GetBoard(); // mod by Dies Irae

		public BaseLog( CraftResource resource ) : this( resource, 1 )
		{
		}

		public BaseLog( CraftResource resource, int amount ) : base( 0x1BE0 )
		{
			Stackable = true;
			Amount = amount;
			Hue = CraftResources.GetHue( resource );

			m_Resource = resource;
		}

		public BaseLog( Serial serial ) : base( serial )
		{
		}

		public override void AddNameProperty( ObjectPropertyList list )
		{
			if ( Amount > 1 )
				list.Add( 1050039, "{0}\t#{1}", Amount, 1027134 ); // ~1_NUMBER~ ~2_ITEMNAME~
			else
				list.Add( 1027133 ); // log
		}

		/*
		public override int LabelNumber
		{
			get
			{
				if ( m_Resource >= CraftResource.Oak && m_Resource <= CraftResource.Frostwood )
					return 1075052 + (int)(m_Resource - CraftResource.Oak);

				return 1015101;
			}
		}
		*/
		
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

        #region mod by Dies Irae : pre-aos stuff
        public override void OnSingleClick( Mobile from )
        {
            if( Resource != CraftResource.RegularWood )
                LabelTo( from, ( Amount == 1 ? "{0} {1} log" : "{0} {1} logs" ), Amount, CraftResources.GetName( m_Resource ).ToLower() );
            else
                LabelTo( from, ( Amount == 1 ? "{0} log" : "{0} logs" ), Amount );
        }
        #endregion
    }
	
	#region normal Log
	[FlipableAttribute(0x1BD7, 0x1BDA)]
	public class Log : BaseLog
	{
		#region costruttori
		[Constructable]
		public Log() : this( 1 )
		{
		}

		[Constructable]
		public Log( int amount ) : base( CraftResource.RegularWood, amount )
		{
		}

		public Log( Serial serial ) : base( serial )
		{
		}
		#endregion

		#region mod by Dies Irae
		public override BaseBoards GetBoard()
		{
			return new Board();
		}
		#endregion

		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "tronc%ohi%" : "log%s%", Amount, from.Language ) );
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

        /*
		public override bool Axe( Mobile from, BaseAxe axe )
		{
			if ( !TryCreateBoards( from , 0.0, new Board() ) )
				return false;

			return true;
		}
        */
	}
	
	[FlipableAttribute(0x1BD7, 0x1BDA)]
	public class OakLog : BaseLog
	{
		#region costruttori
		[Constructable]
		public OakLog() : this( 1 )
		{
		}

		[Constructable]
		public OakLog( int amount ) : base( CraftResource.Oak, amount )
		{
		}

		public OakLog( Serial serial ) : base( serial )
		{
		}
		#endregion
		
		#region mod by Dies Irae
		public override BaseBoards GetBoard()
		{
			return new OakBoard();
		}
		#endregion

		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "tronc%ohi% di quercia" : "oak log%s%", Amount, from.Language ) );
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
	
	[FlipableAttribute(0x1BD7, 0x1BDA)]
	public class WalnutLog : BaseLog
	{
		#region costruttori
		[Constructable]
		public WalnutLog() : this( 1 )
		{
		}

		[Constructable]
		public WalnutLog( int amount ) : base( CraftResource.Walnut, amount )
		{
		}

		public WalnutLog( Serial serial ) : base( serial )
		{
		}
		#endregion
		
		#region mod by Dies Irae
		public override BaseBoards GetBoard()
		{
			return new WalnutBoard();
		}
		#endregion

		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "tronc%ohi% di noce" : "walnut log%s%", Amount, from.Language ) );
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
	
	[FlipableAttribute(0x1BD7, 0x1BDA)]
	public class OhiiLog : BaseLog
	{
		#region costruttori
		[Constructable]
		public OhiiLog() : this( 1 )
		{
		}

		[Constructable]
		public OhiiLog( int amount ) : base( CraftResource.Ohii, amount )
		{
		}

		public OhiiLog( Serial serial ) : base( serial )
		{
		}
		#endregion
		
		#region mod by Dies Irae
		public override BaseBoards GetBoard()
		{
			return new OhiiBoard();
		}
		#endregion

		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "tronc%ohi% di ohii" : "ohii log%s%", Amount, from.Language ) );
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
	
	[FlipableAttribute(0x1BD7, 0x1BDA)]
	public class CedarLog : BaseLog
	{
		#region costruttori
		[Constructable]
		public CedarLog() : this( 1 )
		{
		}

		[Constructable]
		public CedarLog( int amount ) : base( CraftResource.Cedar, amount )
		{
		}

		public CedarLog( Serial serial ) : base( serial )
		{
		}
		#endregion
		
		#region mod by Dies Irae
		public override BaseBoards GetBoard()
		{
			return new CedarBoard();
		}
		#endregion

		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "tronc%ohi% di cedro" : "cedar log%s%", Amount, from.Language ) );
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
	
	[FlipableAttribute(0x1BD7, 0x1BDA)]
	public class WillowLog : BaseLog
	{
		#region costruttori
		[Constructable]
		public WillowLog() : this( 1 )
		{
		}

		[Constructable]
		public WillowLog( int amount ) : base( CraftResource.Willow, amount )
		{
		}

		public WillowLog( Serial serial ) : base( serial )
		{
		}
		#endregion
		
		#region mod by Dies Irae
		public override BaseBoards GetBoard()
		{
			return new WillowBoard();
		}
		#endregion

		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "tronc%ohi% di salice" : "willow log%s%", Amount, from.Language ) );
		}

		#region serial-deserial
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
			
			this.Resource = CraftResource.Willow;
		}
		#endregion
	}
	
	[FlipableAttribute(0x1BD7, 0x1BDA)]
	public class CypressLog : BaseLog
	{
		#region costruttori
		[Constructable]
		public CypressLog() : this( 1 )
		{
		}

		[Constructable]
		public CypressLog( int amount ) : base( CraftResource.Cypress, amount )
		{
		}

		public CypressLog( Serial serial ) : base( serial )
		{
		}
		#endregion
		
		#region mod by Dies Irae
		public override BaseBoards GetBoard()
		{
			return new CypressBoard();
		}
		#endregion

		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "tronc%ohi% di cipresso" : "cypress log%s%", Amount, from.Language ) );
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
	
	[FlipableAttribute(0x1BD7, 0x1BDA)]
	public class YewLog : BaseLog
	{
		#region costruttori
		[Constructable]
		public YewLog() : this( 1 )
		{
		}

		[Constructable]
		public YewLog( int amount ) : base( CraftResource.Yew, amount )
		{
		}

		public YewLog( Serial serial ) : base( serial )
		{
		}
		#endregion
		
		#region mod by Dies Irae
		public override BaseBoards GetBoard()
		{
			return new YewBoard();
		}
		#endregion

		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "tronc%ohi% di yew" : "yew log%s%", Amount, from.Language ) );
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
	
	[FlipableAttribute(0x1BD7, 0x1BDA)]
	public class AppleLog : BaseLog
	{
		#region costruttori
		[Constructable]
		public AppleLog() : this( 1 )
		{
		}

		[Constructable]
		public AppleLog( int amount ) : base( CraftResource.Apple, amount )
		{
		}

		public AppleLog( Serial serial ) : base( serial )
		{
		}
		#endregion
		
		#region mod by Dies Irae
		public override BaseBoards GetBoard()
		{
			return new AppleBoard();
		}
		#endregion

		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "tronc%ohi% di melo" : "apple log%s%", Amount, from.Language ) );
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
	
	[FlipableAttribute(0x1BD7, 0x1BDA)]
	public class PearLog : BaseLog
	{
		#region costruttori
		[Constructable]
		public PearLog() : this( 1 )
		{
		}

		[Constructable]
		public PearLog( int amount ) : base( CraftResource.Pear, amount )
		{
		}

		public PearLog( Serial serial ) : base( serial )
		{
		}
		#endregion
		
		#region mod by Dies Irae
		public override BaseBoards GetBoard()
		{
			return new PearBoard();
		}
		#endregion

		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "tronc%ohi% di pero" : "pear log%s%", Amount, from.Language ) );
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
	
	[FlipableAttribute(0x1BD7, 0x1BDA)]
	public class PeachLog : BaseLog
	{
		#region costruttori
		[Constructable]
		public PeachLog() : this( 1 )
		{
		}

		[Constructable]
		public PeachLog( int amount ) : base( CraftResource.Peach, amount )
		{
		}

		public PeachLog( Serial serial ) : base( serial )
		{
		}
		#endregion
		
		#region mod by Dies Irae
		public override BaseBoards GetBoard()
		{
			return new PeachBoard();
		}
		#endregion

		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "tronc%ohi% di pesco" : "peach log%s%", Amount, from.Language ) );
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
	
	[FlipableAttribute(0x1BD7, 0x1BDA)]
	public class BananaLog : BaseLog
	{
		#region costruttori
		[Constructable]
		public BananaLog() : this( 1 )
		{
		}

		[Constructable]
		public BananaLog( int amount ) : base( CraftResource.Banana, amount )
		{
		}

		public BananaLog( Serial serial ) : base( serial )
		{
		}
		#endregion
		
		#region mod by Dies Irae
		public override BaseBoards GetBoard()
		{
			return new BananaBoard();
		}
		#endregion

		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "tronc%ohi% di banano" : "banana log%s%", Amount, from.Language ) );
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
	
	#region special Log
	[FlipableAttribute(0x1BD7, 0x1BDA)]
	public class StonewoodLog : BaseLog
	{
		#region costruttori
		[Constructable]
		public StonewoodLog() : this( 1 )
		{
		}

		[Constructable]
		public StonewoodLog( int amount ) : base( CraftResource.Stonewood, amount )
		{
		}

		public StonewoodLog( Serial serial ) : base( serial )
		{
		}
		#endregion
		
		#region mod by Dies Irae
		public override BaseBoards GetBoard()
		{
			return new StonewoodBoard();
		}
		#endregion

		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "tronc%ohi% di stonewood" : "stonewood log%s%", Amount, from.Language ) );
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
	
	[FlipableAttribute(0x1BD7, 0x1BDA)]
	public class SilverLog : BaseLog
	{
		#region costruttori
		[Constructable]
		public SilverLog() : this( 1 )
		{
		}

		[Constructable]
		public SilverLog( int amount ) : base( CraftResource.Silver, amount )
		{
		}

		public SilverLog( Serial serial ) : base( serial )
		{
		}
		#endregion
		
		#region mod by Dies Irae
		public override BaseBoards GetBoard()
		{
			return new SilverBoard();
		}
		#endregion

		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "tronc%ohi% di silver" : "silver log%s%", Amount, from.Language ) );
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
	
	[FlipableAttribute(0x1BD7, 0x1BDA)]
	public class BloodLog : BaseLog
	{
		#region costruttori
		[Constructable]
		public BloodLog() : this( 1 )
		{
		}

		[Constructable]
		public BloodLog( int amount ) : base( CraftResource.Blood, amount )
		{
		}

		public BloodLog( Serial serial ) : base( serial )
		{
		}
		#endregion
		
		#region mod by Dies Irae
		public override BaseBoards GetBoard()
		{
			return new BloodBoard();
		}
		#endregion

		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "tronc%ohi% di blood" : "blood log%s%", Amount, from.Language ) );
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
	
	[FlipableAttribute(0x1BD7, 0x1BDA)]
	public class SwampLog : BaseLog
	{
		#region costruttori
		[Constructable]
		public SwampLog() : this( 1 )
		{
		}

		[Constructable]
		public SwampLog( int amount ) : base( CraftResource.Swamp, amount )
		{
		}

		public SwampLog( Serial serial ) : base( serial )
		{
		}
		#endregion
		
		#region mod by Dies Irae
		public override BaseBoards GetBoard()
		{
			return new SwampBoard();
		}
		#endregion

		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "tronc%ohi% di swamp" : "swamp log%s%", Amount, from.Language ) );
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
	
	[FlipableAttribute(0x1BD7, 0x1BDA)]
	public class CrystalLog : BaseLog
	{
		#region costruttori
		[Constructable]
		public CrystalLog() : this( 1 )
		{
		}

		[Constructable]
		public CrystalLog( int amount ) : base( CraftResource.Crystal, amount )
		{
		}

		public CrystalLog( Serial serial ) : base( serial )
		{
		}
		#endregion
		
		#region mod by Dies Irae
		public override BaseBoards GetBoard()
		{
			return new CrystalBoard();
		}
		#endregion

		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "tronc%ohi% di crystal" : "crystal log%s%", Amount, from.Language ) );
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
	
	[FlipableAttribute(0x1BD7, 0x1BDA)]
	public class ElvenLog : BaseLog
	{
		#region costruttori
		[Constructable]
		public ElvenLog() : this( 1 )
		{
		}

		[Constructable]
		public ElvenLog( int amount ) : base( CraftResource.Elven, amount )
		{
		}

		public ElvenLog( Serial serial ) : base( serial )
		{
		}
		#endregion
		
		#region mod by Dies Irae
		public override BaseBoards GetBoard()
		{
			return new ElvenBoard();
		}
		#endregion

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
	
	[FlipableAttribute(0x1BD7, 0x1BDA)]
	public class ElderLog : BaseLog
	{
		#region costruttori
		[Constructable]
		public ElderLog() : this( 1 )
		{
		}

		[Constructable]
		public ElderLog( int amount ) : base( CraftResource.Elder, amount )
		{
		}

		public ElderLog( Serial serial ) : base( serial )
		{
		}
		#endregion
		
        #region mod by Dies Irae
        public override BaseBoards GetBoard()
        {
            return new ElderBoard();
        }
        #endregion

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
	
	[FlipableAttribute(0x1BD7, 0x1BDA)]
	public class EnchantedLog : BaseLog
	{
		#region costruttori
		[Constructable]
		public EnchantedLog() : this( 1 )
		{
		}

		[Constructable]
		public EnchantedLog( int amount ) : base( CraftResource.Enchanted, amount )
		{
		}

		public EnchantedLog( Serial serial ) : base( serial )
		{
		}
		#endregion
		
		#region mod by Dies Irae
		public override BaseBoards GetBoard()
		{
			return new EnchantedBoard();
		}
		#endregion

		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "tronc%ohi% di enchanted" : "enchanted log%s%", Amount, from.Language ) );
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
	
	[FlipableAttribute(0x1BD7, 0x1BDA)]
	public class DeathLog : BaseLog
	{
		#region costruttori
		[Constructable]
		public DeathLog() : this( 1 )
		{
		}

		[Constructable]
		public DeathLog( int amount ) : base( CraftResource.Death, amount )
		{
		}

		public DeathLog( Serial serial ) : base( serial )
		{
		}
		#endregion
		
		#region mod by Dies Irae
		public override BaseBoards GetBoard()
		{
			return new DeathBoard();
		}
		#endregion

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
}
