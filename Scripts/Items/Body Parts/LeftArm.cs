using System;
using Server;

namespace Server.Items
{
	public class LeftArm : Item, ICarvable
	{
		public void Carve( Mobile from, Item item )
		{
			base.ScissorHelper( from, new CarneUmana(), 1 );
		}

		[Constructable]
		public LeftArm() : base( 0x1DA1 )
		{
		}

		public LeftArm( Serial serial ) : base( serial )
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