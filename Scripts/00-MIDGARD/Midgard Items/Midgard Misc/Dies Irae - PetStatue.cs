/***************************************************************************
 *                                 		PetStatue.cs
 *                            		-------------------
 *  begin                	: Gennaio, 2006
 *  version		: 1.0
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 * 
 * 	Info:
 * 			Le Pet statue sono frame statici delle animazioni.
 * 			Usabili per prove colori o per altro.
 *  
 ***************************************************************************/
 
using System;
using Server;

namespace Server.Items
{
	public class PetStatue : Item
	{
		#region enums
		public enum PetStatueType
		{
			DrakeGray,
			DrakeRed,
			AncientWyrmBlue,
			AncientWyrmRed,
			SpawmDragonNotArmoured,
			SpawmDragonArmoured,
			Beetle,
			FactionHorse,
			Ostard,
			OstardFrenzied,
			Hiryu,
			GreatWyrmGray,
			GreatWyrmRed,
			Kirin,
			Unicorn,
			Nightmare,
			HorseGrey,
			HorseTan,	
			SkeletalSteed,
			SavageRidgeback,
			SavageRidgebackRare,
			Llama,
			LlamaPacked,
			Dragon,
		}
		#endregion
		
		#region campi
		private PetStatueType m_Type;
		#endregion
		
		#region proprietà
		[CommandProperty( AccessLevel.GameMaster )]
		public PetStatueType Type
		{
			get{ return m_Type; }
			set
			{
				m_Type = value;
				ItemID = PetStatueInfo.GetInfo( m_Type ).ItemID;
				InvalidateProperties();
			}
		}
		
		public override int LabelNumber
		{
			get{ return PetStatueInfo.GetInfo( m_Type ).LabelNumber; }
		}
		#endregion
		
		#region costruttori
		[Constructable]
		public PetStatue( PetStatueType type ) : this( type, 0 )
		{
		}
		
		[Constructable]
		public PetStatue() : this( PetStatueType.Dragon, 0 )
		{
		}
		
		[Constructable]
		public PetStatue( PetStatueType type, int hue ) : base( PetStatueInfo.GetInfo( type ).ItemID )
		{
			m_Type = type;
			
			Hue = hue;
			
			if( PetStatueInfo.GetInfo( type ).Name.Length > 0 )
			{
				Name = PetStatueInfo.GetInfo( type ).Name + " Statue";
			}
		}

		public PetStatue( Serial serial ) : base( serial )
		{
		}
		#endregion
		
		#region serial deserial
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
		#endregion
		
		public class PetStatueInfo
		{
			#region campi
			private int m_LabelNumber;
			private int m_ItemID;
			private string m_Name;
			#endregion
			
			#region proprietà
			public int LabelNumber{ get{ return m_LabelNumber; } }
			public int ItemID{ get{ return m_ItemID; } }
			public string Name{ get{ return m_Name; } }
			#endregion
			
			#region costruttori
			public PetStatueInfo( int labelNumber, int itemID ) : this ( labelNumber, itemID, string.Empty )
			{
			}
			
			public PetStatueInfo( int itemID, string name ) : this ( 0, itemID, name )
			{
			}
			
			public PetStatueInfo( int labelNumber, int itemID, string name )
			{
				m_LabelNumber = labelNumber;
				m_ItemID = itemID;
				m_Name = name;
			}
			#endregion
			
			#region metodi
			public static PetStatueInfo GetInfo( PetStatueType type )
			{
				int v = (int)type;
	
				if ( v < 0 || v >= m_Table.Length )
					v = 0;
	
				return m_Table[v];
			}
			#endregion
			
			#region Lista di statue
			private static PetStatueInfo[] m_Table = new PetStatueInfo[]
			{	
				new PetStatueInfo( 9894, 	"DrakeGray" ),
				new PetStatueInfo( 9895, 	"DrakeRed" ),
				new PetStatueInfo( 9896, 	"AncientWyrmBlue" ),
				new PetStatueInfo( 9897, 	"AncientWyrmRed" ),
				new PetStatueInfo( 9898, 	"SpawmDragonNotArmoured" ),
				new PetStatueInfo( 9899, 	"SpawmDragonArmoured" ),
				new PetStatueInfo( 9973, 	"Beetle" ),
				new PetStatueInfo( 9975, 	"FactionHorse" ),
				new PetStatueInfo( 9997, 	"Ostard" ),
				new PetStatueInfo( 9998, 	"OstardFrenzied" ),
				new PetStatueInfo( 9999, 	"Hiryu" ),
				new PetStatueInfo( 10160, 	"GreatWyrmGray" ),
				new PetStatueInfo( 10161, 	"GreatWyrmRed" ),
				new PetStatueInfo( 10162, 	"Kirin" ),
				new PetStatueInfo( 10163, 	"Unicorn" ),
				new PetStatueInfo( 10164, 	"Nightmare" ),
				new PetStatueInfo( 10165, 	"HorseGrey" ),
				new PetStatueInfo( 10166, 	"HorseTan" ),	
				new PetStatueInfo( 10167, 	"SkeletalSteed" ),
				new PetStatueInfo( 10168, 	"SavageRidgeback" ),
				new PetStatueInfo( 10169, 	"SavageRidgebackRare" ),
				new PetStatueInfo( 10170, 	"Llama" ),
				new PetStatueInfo( 10171, 	"LlamaPacked" ),
				new PetStatueInfo( 8504, 	"Dragon" ),
			};
			#endregion
		}
	}
}
