using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class TailorBag : Bag
	{
		public override string DefaultName
		{
			get { return "a Tailoring Kit"; }
		}

		[Constructable]
		public TailorBag() : this( 1 )
		{
			Movable = true;
			Hue = 0x315;
		}

		[Constructable]
		public TailorBag( int amount )
		{
			DropItem( new SewingKit( 5 ) );
			DropItem( new Scissors() );
			DropItem( new Hides( 500 ) );
			DropItem( new BoltOfCloth( 20 ) );
			DropItem( new DyeTub() );
			DropItem( new DyeTub() );
			DropItem( new BlackDyeTub() );
			DropItem( new Dyes() );

            if( !Core.AOS )
            {
                DropItem( new Leather( amount ) );
                DropItem( new WolfLeather( amount ) );
                DropItem( new BearLeather( amount ) );
                DropItem( new AracnidLeather( amount ) );
                DropItem( new SpinedLeather( amount ) );
                DropItem( new OrcishLeather( amount ) );
                DropItem( new BarbedLeather( amount ) );
                DropItem( new UndeadLeather( amount ) );
                DropItem( new HornedLeather( amount ) );
                DropItem( new LavaLeather( amount ) );
                DropItem( new ArcticLeather( amount ) );
                DropItem( new GreenDragonLeather( amount ) );
                DropItem( new BlueDragonLeather( amount ) );
                DropItem( new BlackDragonLeather( amount ) );
                DropItem( new RedDragonLeather( amount ) );
                DropItem( new AbyssLeather( amount ) );
            }
		}
		
		public TailorBag( Serial serial ) : base( serial )
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