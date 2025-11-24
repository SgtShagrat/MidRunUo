/***************************************************************************
 *                                    WarriorMageryScroll.cs
 *                            		-------------------
 *  begin                	: March, 2007
 *  version					: 1.0
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 * 
 * 	Info:
 *  		Scrolla che aumenta i thitepoints se usata.
 * 			in 4 versioni:
 * 					Versione Base con costruttore che richiede i punti.
 * 					Versioni da 1000, 5000, 10000 punti.
 ***************************************************************************/	

namespace Server.Items
{
	public class WarriorMageryScroll : Item
	{
		#region campi
		private int m_Points;
		#endregion
		
		#region proprietà
		public override int LabelNumber{ get{ return 1064338; } } // Scroll of Warrior Magery
		public int Points { get { return m_Points; } }
		#endregion
		
		#region costruttori
		[Constructable]
		public WarriorMageryScroll( int points ) : base( 0x1F23 )
		{
			Weight = 2.0;
			Hue = 2024;
			m_Points = points;
		}
		
		public WarriorMageryScroll( Serial serial ) : base( serial ) 
		{
		}
		#endregion
		
		#region metodi
		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties( list );

			if ( m_Points > 0 )
				list.Add( 1060658, "Points\t{0}", m_Points ); // ~1_val~: ~2_val~
		}
		
		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) ) // Make sure its in their pack
			{
				 from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}
			else
			{
				from.TithingPoints += m_Points;
				from.SendLocalizedMessage( 1064339 ); // Your Warrior Magery power has Increased.
				Delete();
			}			
		}
		#endregion
		
		#region serial-deserial
		public override void Serialize( GenericWriter writer ) 
		{ 
			base.Serialize( writer ); 
			writer.Write( (int) 0 ); // version 
			writer.Write( (int) m_Points );
		} 
		
		public override void Deserialize( GenericReader reader ) 
		{ 
			base.Deserialize( reader ); 
			int version = reader.ReadInt(); 
			int m_Points = reader.ReadInt();
		} 
		#endregion
	}
	
	public class WarriorMageryScroll1000 : WarriorMageryScroll
	{
		#region costruttori
		[Constructable]
		public WarriorMageryScroll1000() : base( 1000 )
		{
		}
		
		public WarriorMageryScroll1000( Serial serial ) : base( serial ) 
		{
		}
		#endregion
		
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
		} 
		#endregion		
	}
	
	public class WarriorMageryScroll5000 : WarriorMageryScroll
	{
		#region costruttori
		[Constructable]
		public WarriorMageryScroll5000() : base( 5000 )
		{
		}
		
		public WarriorMageryScroll5000( Serial serial ) : base( serial ) 
		{
		}
		#endregion
		
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
		} 
		#endregion				
	}
	
	public class WarriorMageryScroll10000 : WarriorMageryScroll
	{
		#region costruttori
		[Constructable]
		public WarriorMageryScroll10000() : base( 10000 )
		{
		}
		
		public WarriorMageryScroll10000( Serial serial ) : base( serial ) 
		{
		}
		#endregion
		
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
		} 
		#endregion				
	}
}
	
