using System;

namespace Server.Items
{
	public class AlchemicalBag : Bag
	{
		#region metodi
		public override int LabelNumber { get { return 1064033; } } // alchemical bag
		#endregion

		#region costruttori
		[Constructable]
		public AlchemicalBag() : this( 10 )
		{
			Movable = true;
			Hue = 2444;
		}

		[Constructable]
		public AlchemicalBag( int amount )
		{
			DropItem( new GreaterHealPotion( amount ) );
			DropItem( new GreaterAgilityPotion( amount ) );
			DropItem( new GreaterCurePotion( amount ) );
			DropItem( new GreaterExplosionPotion( amount ) );
			DropItem( new DeadlyPoisonPotion( amount ) );
			DropItem( new TotalRefreshPotion( amount ) );
			DropItem( new GreaterStrengthPotion( amount ) );   
			DropItem( new NightSightPotion( amount ) ); 
		}

		public AlchemicalBag( Serial serial ) : base( serial )
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
	}
}
