using System;

namespace Server.Items
{
	public class BagOfPaganReagents : Bag
	{
		#region metodi
		public override int LabelNumber { get { return 1064036; } } // bag of pagan reagents
		#endregion

		[Constructable]
		public BagOfPaganReagents() : this( 50 )
		{
		}

		[Constructable]
		public BagOfPaganReagents( int amount )
		{
			DropItem( new BatWing ( amount ) );
			DropItem( new VolcaninAsh ( amount ) );
			DropItem( new SerpentScale ( amount ) );
			DropItem( new BlackMoor ( amount ) );
			DropItem( new DragonBlood ( amount ) );
			DropItem( new WyrmHeart ( amount ) );
			DropItem( new DeadWood ( amount ) );
			DropItem( new DaemonBone ( amount ) );
			DropItem( new Pumices ( amount ) );
			DropItem( new Obsidian ( amount ) );
			DropItem( new ExecutionerCap ( amount ) );
			DropItem( new EyeOfNewt ( amount ) );
			DropItem( new BloodSpawn ( amount ) );	
		}

		public BagOfPaganReagents( Serial serial ) : base( serial )
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
}
