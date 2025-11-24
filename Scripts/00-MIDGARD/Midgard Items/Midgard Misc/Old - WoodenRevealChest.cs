using System;

namespace Server.Items
{
	[FlipableAttribute( 0xe43, 0xe42 )] 
	public class WoodenRevealChest : LockableContainer 
	{ 
		public override int DefaultGumpID{ get{ return 0x49; } }
		public override int DefaultDropSound{ get{ return 0x42; } }

		public override Rectangle2D Bounds
		{
			get{ return new Rectangle2D( 20, 105, 150, 180 ); }
		}

		[Constructable] 
		public WoodenRevealChest() : base( 0xE43 ) 
		{ 
		} 

		public WoodenRevealChest( Serial serial ) : base( serial ) 
		{ 
		} 
		
		public override void OnItemLifted( Mobile from, Item item )
		{
			if ( from.Hidden )
			{
				from.RevealingAction();
				from.SendMessage( "You make too much noise!" );
			}
			
			base.OnItemLifted( from, item );
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
}
