// Pesci per il nuovo engine di pesca e bulk orders
// by snow
/*
using System;
using Server;
using Server.Network;
using Server.Targeting;

namespace Server.Items
{
	public class AdvFish : Item, ICarvable
	{
		public int m_weight=0;
		public virtual void Carve( Mobile from, Item item )
		{
			base.ScissorHelper( from, new RawFishSteak(), (int)(1) );
		}
		
		[Constructable]
		public AdvFish( int weight,int ItemID ) : base( ItemID )
		{
			Name="Piper Fish";
			Stackable = false;
			Weight = 1.0;
			Amount = 1;
			m_weight=weight;
		}

		public AdvFish( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
			writer.Write( (int) m_weight ); // weight
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
			m_weight = reader.ReadInt();
		}


		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );			
			
			list.Add( 1060658, "{0}\t{1}gr.", string.Format( "weight"), m_weight ); // ~1_val~: ~2_val~
		}
	} // AdvFish BaseClass

	public class PiperFish : AdvFish
	{
		// ID 1
		public override void Carve( Mobile from, Item item )
		{
			base.ScissorHelper( from, new RawFishSteak(), (int)(1) );
		}
		
		[Constructable]
		public PiperFish( int weight ) : base( weight,0x3F1D )
		{
			Name="Piper Fish";
			Stackable = false;
			Weight = 1.0;
			Amount = 1;			
		}

		public PiperFish( Serial serial ) : base( serial )
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
	} // PiperFish MARE

	public class SalmonFish : AdvFish
	{
		// ID 2
		public override void  Carve( Mobile from, Item item )
		{
			base.ScissorHelper( from, new RawFishSteak(), (int)(((int)(m_weight/100))>0 ? (m_weight/100) : 1));
		}
		
		[Constructable]
		public SalmonFish( int weight ) : base( weight, 0x3F15 )
		{
			Name="Salmon Fish";
			Stackable = false;
			Weight = 1.0;
			Amount = 1;			
		}

		public SalmonFish( Serial serial ) : base( serial )
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
	} // SalmonFish MARE
	
	public class GranchioFish : AdvFish
	{		
		// ID 3
		public override void Carve( Mobile from, Item item )
		{
			base.ScissorHelper( from, new RawFishSteak(), (int)(((int)(m_weight/500))>0 ? (m_weight/500) : 1));
		}
		
		[Constructable]
		public GranchioFish( int weight ) : base( weight, 0x3F23 )
		{
			Name="Crab";
			Stackable = false;
			Weight = 1.0;
			Amount = 1;
		}

		public GranchioFish( Serial serial ) : base( serial )
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
	}// GRANCHIO MARE
	
	public class MarlinFish : AdvFish
	{
		// ID 4
		public override void Carve( Mobile from, Item item )
		{			
			//33% di trovare nel Marlin la Sword of the abyss
			if (Utility.RandomDouble() <= 0.33)			
			{
				//Console.WriteLine("Crea SPada");				
				Item newItem=(Item)new SwordOfTheAbyss();				
				if ( !(this.Parent is Container) || !((Container)this.Parent).TryDropItem( from, newItem, false ) )
					newItem.MoveToWorld( Location, this.Map );
			}
			base.ScissorHelper( from, new RawFishSteak(), (int)(((int)(m_weight/100))>0 ? (m_weight/100) : 1));
		}
		
		[Constructable]
		public MarlinFish( int weight ) : base( weight, 0x3F25 )
		{
			Name="Blue Marlin Fish";
			Stackable = false;
			Weight = 1.0;
			Amount = 1;
		}

		public MarlinFish( Serial serial ) : base( serial )
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
	} // blue marlin MARE DEED ID 4
	
	public class MantaFish : AdvFish
	{
		// ID 5
		public override void Carve( Mobile from, Item item )
		{
			base.ScissorHelper( from, new RawFishSteak(), (int)(((int)(m_weight/300))>0 ? (m_weight/300) : 1));
		}
		
		[Constructable]
		public MantaFish( int weight ) : base( weight, 0x3F1E )
		{
			Name="Manta Fish";
			Stackable = false;
			Weight = 1.0;
			Amount = 1;
		}

		public MantaFish( Serial serial ) : base( serial )
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
	}// manta MARE id 5

	public class CarpFish : AdvFish
	{
		// ID 6
		public override void Carve( Mobile from, Item item )
		{
			base.ScissorHelper( from, new RawFishSteak(), (int)(((int)(m_weight/700))>0 ? (m_weight/700) : 1));
		}
		
		[Constructable]
		public CarpFish( int weight ) : base( weight, 0x3F13 )
		{
			Name="Carp Fish";
			Stackable = false;
			Weight = 1.0;
			Amount = 1;
		}

		public CarpFish( Serial serial ) : base( serial )
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
	}// carp LAGO ID 6

	public class CatFish : AdvFish
	{
		// ID 7
		public override void Carve( Mobile from, Item item )
		{
			base.ScissorHelper( from, new RawFishSteak(), (int)(((int)(m_weight/600))>0 ? (m_weight/600) : 1));
		}
		
		[Constructable]
		public CatFish( int weight ) : base( weight, 0x3F12 )
		{
			Name="Cat Fish";
			Stackable = false;
			Weight = 1.0;
			Amount = 1;
		}

		public CatFish( Serial serial ) : base( serial )
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
	}// cat fish LAGO id 7

	public class SmallMouthFish : AdvFish
	{
		// id 8
		public override void Carve( Mobile from, Item item )
		{
			base.ScissorHelper( from, new RawFishSteak(), (int)(((int)(m_weight/400))>0 ? (m_weight/400) : 1));
		}
		
		[Constructable]
		public SmallMouthFish( int weight ) : base( weight, 0x3F14 )
		{
			Name="Smallmouth Fish";
			Stackable = false;
			Weight = 1.0;
			Amount = 1;
		}

		public SmallMouthFish( Serial serial ) : base( serial )
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
	}// SmallMouthFish LAGO id 8

	public class TropicalFish : AdvFish
	{
		// ID 9
		public override void Carve( Mobile from, Item item )
		{
			base.ScissorHelper( from, new RawFishSteak(), (int)(((int)(m_weight/900))>0 ? (m_weight/900) : 1));
		}
		
		[Constructable]
		public TropicalFish( int weight ) : base( weight, Utility.Random( 0x3F20, 3 ) )
		{
			Name="Tropical Fish";
			Stackable = false;
			Weight = 1.0;
			Amount = 1;
		}

		public TropicalFish( Serial serial ) : base( serial )
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
	}// TropicalFish MARE ID 9

	public class TenchFish : AdvFish
	{
		// ID 10
		public override void Carve( Mobile from, Item item )
		{
			base.ScissorHelper( from, new RawFishSteak(), (int)(((int)(m_weight/550))>0 ? (m_weight/550) : 1));
		}
		
		[Constructable]
		public TenchFish( int weight ) : base( weight, 0x3F0A )
		{
			// tinka
			Name="Tench Fish";
			Stackable = false;
			Weight = 1.0;
			Amount = 1;
		}

		public TenchFish( Serial serial ) : base( serial )
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
	}// TenchFish LAGO ID 10
	
	public class BleakFish : AdvFish
	{
		// ID 11
		public override void Carve( Mobile from, Item item )
		{
			base.ScissorHelper( from, new RawFishSteak(), (int)(((int)(m_weight/900))>0 ? (m_weight/900) : 1));
		}
		
		[Constructable]
		public BleakFish( int weight ) : base( weight, 0x3F18 )
		{
			// alborella
			Name="Bleak Fish";
			Stackable = false;
			Weight = 1.0;
			Amount = 1;
		}

		public BleakFish( Serial serial ) : base( serial )
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
	}// BleakFish LAGO ID 11
			
	public class ChubFish : AdvFish
	{
		// ID 12
		public override void Carve( Mobile from, Item item )
		{
			base.ScissorHelper( from, new RawFishSteak(), 1 );
		}
		
		[Constructable]
		public ChubFish( int weight ) : base( weight, 0x3F0C )
		{
			// Cavedano
			Name="Chub Fish";
			Stackable = false;
			Weight = 1.0;
			Amount = 1;
		}

		public ChubFish( Serial serial ) : base( serial )
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
	}// ChubFish LAGO ID 12
}
*/