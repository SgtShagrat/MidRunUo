using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class CarpenterBag : Bag
	{
		public override string DefaultName
		{
			get { return "a Carpenter Kit"; }
		}

		[Constructable]
		public CarpenterBag() : this( 1 )
		{
			Movable = true;
			Hue = 0x315;
		}

		[Constructable]
		public CarpenterBag( int amount )
		{
			DropItem( new MouldingPlane( 5 ) );

            if( !Core.AOS )
            {
                DropItem( new Log( amount ) );
                DropItem( new OakLog( amount ) );
                DropItem( new WalnutLog( amount ) );
                DropItem( new OhiiLog( amount ) );
                DropItem( new CedarLog( amount ) );
                DropItem( new WillowLog( amount ) );
                DropItem( new CypressLog( amount ) );
                DropItem( new YewLog( amount ) );
                DropItem( new AppleLog( amount ) );
                DropItem( new PearLog( amount ) );
                DropItem( new PeachLog( amount ) );
                DropItem( new BananaLog( amount ) );
                DropItem( new StonewoodLog( amount ) );
                DropItem( new SilverLog( amount ) );
                DropItem( new BloodLog( amount ) );
                DropItem( new SwampLog( amount ) );
                DropItem( new CrystalLog( amount ) );
                DropItem( new EnchantedLog( amount ) );
            }
		}
		
		public CarpenterBag( Serial serial ) : base( serial )
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