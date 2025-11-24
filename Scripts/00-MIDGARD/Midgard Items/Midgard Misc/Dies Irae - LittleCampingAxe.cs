using System;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Items
{	
	public class LittleCampingAxe : Item
	{
		#region costruttori
		[Constructable]
		public LittleCampingAxe() : base( 3907 )
		{
			Name= "a little portable axe";
			Weight = 2.0;
		}

		public LittleCampingAxe( Serial serial ) : base( serial )
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
		
		#region metodi
		public override void OnDoubleClick( Mobile from )
		{
			from.SendMessage( "What should I use this axe on?" );
			from.Target = new InternalTarget( this );
		}
		#endregion
		
		private class InternalTarget : Target
		{
			#region campi
			private LittleCampingAxe m_Item;
			#endregion
			
			#region costruttori
			public InternalTarget( LittleCampingAxe item ) : base( 1, false, TargetFlags.None )
			{
				m_Item = item;
			}
			#endregion
			
			#region metodi
			protected override void OnTarget( Mobile from, object targeted )
			{
				Item item = targeted as Item;
				PlayerMobile pm = from as PlayerMobile;
				
				int itemID = 0;
				
				if( targeted is StaticTarget )
					itemID = ((StaticTarget)targeted).ItemID;

				if( item.Deleted ) 
					return;
						
				if( IsTreeTile( itemID ) )
				{
					pm.AddToBackpack( new Kindling( 1 ) );
					pm.SendMessage( "Now you have something to burn." );
				}
				else  if( item.GetType().IsSubclassOf( typeof(BaseLog) ) )
				{
					item.Delete();
					pm.AddToBackpack( new Kindling( item.Amount * 5 ) );	
					pm.SendMessage( "You use the log to produce something to burn." );
				}
				else
				{
					from.SendMessage( "This axe can not be used on that to produce anything." );
				}
			}
			
			public static bool IsTreeTile( int itemid )
			{
				for( int i = 0; i < m_TreeTiles.Length; i++ )
				{
					if( itemid == m_TreeTiles[i] )
						return true;
				}
				return false;
			}
			
			#region m_TreeTiles
			private static int[] m_TreeTiles = new int[]
			{
				0x4CCA, 0x4CCB, 0x4CCC, 0x4CCD, 0x4CD0, 0x4CD3, 0x4CD6, 0x4CD8,
				0x4CDA, 0x4CDD, 0x4CE0, 0x4CE3, 0x4CE6, 0x4CF8, 0x4CFB, 0x4CFE,
				0x4D01, 0x4D41, 0x4D42, 0x4D43, 0x4D44, 0x4D57, 0x4D58, 0x4D59,
				0x4D5A, 0x4D5B, 0x4D6E, 0x4D6F, 0x4D70, 0x4D71, 0x4D72, 0x4D84,
				0x4D85, 0x4D86, 0x52B5, 0x52B6, 0x52B7, 0x52B8, 0x52B9, 0x52BA,
				0x52BB, 0x52BC, 0x52BD,
	
				0x4D98, 0x4DA0, 0x4D9C, 0x4DA4, 0x4DA8, 0x4D94, 0x4C9E, 0x4CAA,
				0x4CA8,
	
				0x4CCE, 0x4CCF, 0x4CD1, 0x4CD2, 0x4CD4, 0x4CD5, 0x4CD7, 0x4CD9,
				0x4CDB, 0x4CDC, 0x4CDE, 0x4CDF, 0x4CE1, 0x4CE2, 0x4CE4, 0x4CE5,
				0x4CE7, 0x4CE8, 0x4CF9, 0x4CFA, 0x4CFC, 0x4CFD, 0x4CFF, 0x4D00,
				0x4D02, 0x4D03, 0x4D45, 0x4D46, 0x4D47, 0x4D48, 0x4D49, 0x4D4A,
				0x4D4B, 0x4D4C, 0x4D4D, 0x4D4E, 0x4D4F, 0x4D50, 0x4D51, 0x4D52,
				0x4D53, 0x4D5C, 0x4D5D, 0x4D5E, 0x4D5F, 0x4D60, 0x4D61, 0x4D62,
				0x4D63, 0x4D64, 0x4D65, 0x4D66, 0x4D67, 0x4D68, 0x4D69, 0x4D73,
				0x4D74, 0x4D75, 0x4D76, 0x4D77, 0x4D78, 0x4D79, 0x4D7A, 0x4D7B,
				0x4D7C, 0x4D7D, 0x4D7E, 0x4D7F, 0x4D87, 0x4D88, 0x4D89, 0x4D8A,
				0x4D8B, 0x4D8C, 0x4D8D, 0x4D8E, 0x4D8F, 0x4D90, 0x4D95, 0x4D96,
				0x4D97, 0x4D99, 0x4D9A, 0x4D9B, 0x4D9D, 0x4D9E, 0x4D9F, 0x4DA1,
				0x4DA2, 0x4DA3, 0x4DA5, 0x4DA6, 0x4DA7, 0x4DA9, 0x4DAA, 0x4DAB,
				0x52BE, 0x52BF, 0x52C0, 0x52C1, 0x52C2, 0x52C3, 0x52C4, 0x52C5,
				0x52C6, 0x52C7
			};
			#endregion
			#endregion
		}
	}
}
