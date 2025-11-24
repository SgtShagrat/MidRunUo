//#define PrePassaggioMondain

/***************************************************************************
 *                                       Boards.cs
 *                            		------------------
 *  begin                	: July, 2006
 *  version					: 1.0
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 * 
 *  
 ***************************************************************************/
 
using System;

namespace Server.Items
{
	public class BaseBoards : Item, ICommodity
	{
		private CraftResource m_Resource;

		[CommandProperty( AccessLevel.Developer )]
		public CraftResource Resource
		{
			get{ return m_Resource; }
			set
			{
				if ( m_Resource != value )
				{
					m_Resource = value;
					Hue = CraftResources.GetHue( m_Resource );
					InvalidateProperties();
				}
			}
		}

		public override double DefaultWeight
		{
			get { return 0.1; }
		}
		
		string ICommodity.Description
		{
			get
			{
				return String.Format( Amount == 1 ? "{0} {1} board" : "{0} {1} boards", Amount, CraftResources.GetName( m_Resource ).ToLower() );
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
			if( m_Resource != CraftResource.RegularWood && Hue != CraftResources.GetHue( m_Resource ) )
				Hue = CraftResources.GetHue( m_Resource );
		}

		public BaseBoards( CraftResource resource ) : this( resource, 1 )
		{
		}

		public BaseBoards( CraftResource resource, int amount ) : base( 0x1BD7 )
		{
			Stackable = true;
			Amount = amount;
			Hue = CraftResources.GetHue( resource );

			m_Resource = resource;
		}

		public BaseBoards( Serial serial ) : base( serial )
		{
		}

		/*
		TODO: 
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
		
		public override void AddNameProperty( ObjectPropertyList list )
		{
			if ( Amount > 1 )
				list.Add( 1050039, "{0}\t#{1}", Amount, 1027128 ); // ~1_NUMBER~ ~2_ITEMNAME~
			else
				list.Add( 1027127 ); // boards
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

        #region mod by Dies Irae : pre-aos stuff
        public override void OnSingleClick( Mobile from )
        {
            if( Resource != CraftResource.RegularWood )
                LabelTo( from, ( Amount == 1 ? "{0} {1} board" : "{0} {1} boards" ), Amount, CraftResources.GetName( m_Resource ).ToLower() );
            else
                LabelTo( from, ( Amount == 1 ? "{0} board" : "{0} boards" ), Amount );
        }
        #endregion
	}
	
	#region normal Boards
	[FlipableAttribute(0x1BD7, 0x1BDA)]
	public class Board : BaseBoards
	{
		#region costruttori
		[Constructable]
		public Board() : this( 1 )
		{
		}

		[Constructable]
		public Board( int amount ) : base( CraftResource.RegularWood, amount )
		{
		}

		public Board( Serial serial ) : base( serial )
		{
		}
		#endregion


		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "tavol%ae%" : "board%s%", Amount, from.Language ) );
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
	public class OakBoard : BaseBoards
	{
		#region costruttori
		[Constructable]
		public OakBoard() : this( 1 )
		{
		}

		[Constructable]
		public OakBoard( int amount ) : base( CraftResource.Oak, amount )
		{
		}

		public OakBoard( Serial serial ) : base( serial )
		{
		}
		#endregion

		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "tavol%ae% di quercia" : "oak board%s%", Amount, from.Language ) );
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
	public class WalnutBoard : BaseBoards
	{
		#region costruttori
		[Constructable]
		public WalnutBoard() : this( 1 )
		{
		}

		[Constructable]
		public WalnutBoard( int amount ) : base( CraftResource.Walnut, amount )
		{
		}

		public WalnutBoard( Serial serial ) : base( serial )
		{
		}
		#endregion

		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "tavol%ae% di noce" : "walnut board%s%", Amount, from.Language ) );
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
	public class OhiiBoard : BaseBoards
	{
		#region costruttori
		[Constructable]
		public OhiiBoard() : this( 1 )
		{
		}

		[Constructable]
		public OhiiBoard( int amount ) : base( CraftResource.Ohii, amount )
		{
		}

		public OhiiBoard( Serial serial ) : base( serial )
		{
		}
		#endregion

		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "tavol%ae% di ohii" : "ohii board%s%", Amount, from.Language ) );
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
	public class CedarBoard : BaseBoards
	{
		#region costruttori
		[Constructable]
		public CedarBoard() : this( 1 )
		{
		}

		[Constructable]
		public CedarBoard( int amount ) : base( CraftResource.Cedar, amount )
		{
		}

		public CedarBoard( Serial serial ) : base( serial )
		{
		}
		#endregion

		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "tavol%ae% di cedro" : "cedar board%s%", Amount, from.Language ) );
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
	public class WillowBoard : BaseBoards
	{
		#region costruttori
		[Constructable]
		public WillowBoard() : this( 1 )
		{
		}

		[Constructable]
		public WillowBoard( int amount ) : base( CraftResource.Willow, amount )
		{
		}

		public WillowBoard( Serial serial ) : base( serial )
		{
		}
		#endregion


		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "tavol%ae% di salice" : "willow board%s%", Amount, from.Language ) );
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
	public class CypressBoard : BaseBoards
	{
		#region costruttori
		[Constructable]
		public CypressBoard() : this( 1 )
		{
		}

		[Constructable]
		public CypressBoard( int amount ) : base( CraftResource.Cypress, amount )
		{
		}

		public CypressBoard( Serial serial ) : base( serial )
		{
		}
		#endregion


		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "tavol%ae% di cipresso" : "cypress board%s%", Amount, from.Language ) );
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

			if( Resource != CraftResource.Cypress )			
				this.Resource = CraftResource.Cypress;
		}
		#endregion
	}
	
	[FlipableAttribute(0x1BD7, 0x1BDA)]
	public class YewBoard : BaseBoards
	{
		#region costruttori
		[Constructable]
		public YewBoard() : this( 1 )
		{
		}

		[Constructable]
		public YewBoard( int amount ) : base( CraftResource.Yew, amount )
		{
		}

		public YewBoard( Serial serial ) : base( serial )
		{
		}
		#endregion


		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "tavol%ae% di yew" : "yew board%s%", Amount, from.Language ) );
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
	public class AppleBoard : BaseBoards
	{
		#region costruttori
		[Constructable]
		public AppleBoard() : this( 1 )
		{
		}

		[Constructable]
		public AppleBoard( int amount ) : base( CraftResource.Apple, amount )
		{
		}

		public AppleBoard( Serial serial ) : base( serial )
		{
		}
		#endregion


		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "tavol%ae% di melo" : "apple board%s%", Amount, from.Language ) );
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
	public class PearBoard : BaseBoards
	{
		#region costruttori
		[Constructable]
		public PearBoard() : this( 1 )
		{
		}

		[Constructable]
		public PearBoard( int amount ) : base( CraftResource.Pear, amount )
		{
		}

		public PearBoard( Serial serial ) : base( serial )
		{
		}
		#endregion


		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "tavol%ae% di pero" : "pear board%s%", Amount, from.Language ) );
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
	public class PeachBoard : BaseBoards
	{
		#region costruttori
		[Constructable]
		public PeachBoard() : this( 1 )
		{
		}

		[Constructable]
		public PeachBoard( int amount ) : base( CraftResource.Peach, amount )
		{
		}

		public PeachBoard( Serial serial ) : base( serial )
		{
		}
		#endregion


		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "tavol%ae% di pesco" : "peach board%s%", Amount, from.Language ) );
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
	public class BananaBoard : BaseBoards
	{
		#region costruttori
		[Constructable]
		public BananaBoard() : this( 1 )
		{
		}

		[Constructable]
		public BananaBoard( int amount ) : base( CraftResource.Banana, amount )
		{
		}

		public BananaBoard( Serial serial ) : base( serial )
		{
		}
		#endregion


		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "tavol%ae% di banano" : "banana board%s%", Amount, from.Language ) );
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
	
	#region special Boards
	[FlipableAttribute(0x1BD7, 0x1BDA)]
	public class StonewoodBoard : BaseBoards
	{
		#region costruttori
		[Constructable]
		public StonewoodBoard() : this( 1 )
		{
		}

		[Constructable]
		public StonewoodBoard( int amount ) : base( CraftResource.Stonewood, amount )
		{
		}

		public StonewoodBoard( Serial serial ) : base( serial )
		{
		}
		#endregion


		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "tavol%ae% di stonewood" : "stonewood board%s%", Amount, from.Language ) );
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
	public class SilverBoard : BaseBoards
	{
		#region costruttori
		[Constructable]
		public SilverBoard() : this( 1 )
		{
		}

		[Constructable]
		public SilverBoard( int amount ) : base( CraftResource.Silver, amount )
		{
		}

		public SilverBoard( Serial serial ) : base( serial )
		{
		}
		#endregion

		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "tavol%ae% di silver" : "silver board%s%", Amount, from.Language ) );
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
	public class BloodBoard : BaseBoards
	{
		#region costruttori
		[Constructable]
		public BloodBoard() : this( 1 )
		{
		}

		[Constructable]
		public BloodBoard( int amount ) : base( CraftResource.Blood, amount )
		{
		}

		public BloodBoard( Serial serial ) : base( serial )
		{
		}
		#endregion


		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "tavol%ae% di blood" : "blood board%s%", Amount, from.Language ) );
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
	public class SwampBoard : BaseBoards
	{
		#region costruttori
		[Constructable]
		public SwampBoard() : this( 1 )
		{
		}

		[Constructable]
		public SwampBoard( int amount ) : base( CraftResource.Swamp, amount )
		{
		}

		public SwampBoard( Serial serial ) : base( serial )
		{
		}
		#endregion


		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "tavol%ae% di swamp" : "swamp board%s%", Amount, from.Language ) );
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
	public class CrystalBoard : BaseBoards
	{
		#region costruttori
		[Constructable]
		public CrystalBoard() : this( 1 )
		{
		}

		[Constructable]
		public CrystalBoard( int amount ) : base( CraftResource.Crystal, amount )
		{
		}

		public CrystalBoard( Serial serial ) : base( serial )
		{
		}
		#endregion


		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "tavol%ae% di crystal" : "crystal board%s%", Amount, from.Language ) );
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
	public class ElvenBoard : BaseBoards
	{
		#region costruttori
		[Constructable]
		public ElvenBoard() : this( 1 )
		{
		}

		[Constructable]
		public ElvenBoard( int amount ) : base( CraftResource.Elven, amount )
		{
		}

		public ElvenBoard( Serial serial ) : base( serial )
		{
		}
		#endregion


		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "tavol%ae% di elven" : "elven board%s%", Amount, from.Language ) );
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
	public class ElderBoard : BaseBoards
	{
		#region costruttori
		[Constructable]
		public ElderBoard() : this( 1 )
		{
		}

		[Constructable]
		public ElderBoard( int amount ) : base( CraftResource.Elder, amount )
		{
		}

		public ElderBoard( Serial serial ) : base( serial )
		{
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
	public class EnchantedBoard : BaseBoards
	{
		#region costruttori
		[Constructable]
		public EnchantedBoard() : this( 1 )
		{
		}

		[Constructable]
		public EnchantedBoard( int amount ) : base( CraftResource.Enchanted, amount )
		{
		}

		public EnchantedBoard( Serial serial ) : base( serial )
		{
		}
		#endregion

		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "tavol%ae% di enchanted" : "enchanted board%s%", Amount, from.Language ) );
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
	public class DeathBoard : BaseBoards
	{
		#region costruttori
		[Constructable]
		public DeathBoard() : this( 1 )
		{
		}

		[Constructable]
		public DeathBoard( int amount ) : base( CraftResource.Death, amount )
		{
		}

		public DeathBoard( Serial serial ) : base( serial )
		{
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
