using Server.Mobiles;

namespace Server.Items
{
	public class EtherealPolarBear : EtherealMount
	{
		[Constructable] 
		public EtherealPolarBear() : base( 0x20E1, 0x3F0F )                            
		{ 
			Name = "Ethereal Polar Bear Statuette";
		} 

		public EtherealPolarBear( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class EtherealSkeletalMount : EtherealMount
	{
		[Constructable] 
		public EtherealSkeletalMount() : base( 0x2617, 0x3EBB )                            
		{ 
			Name = "Ethereal Skeletal Mount Statuette";
		} 

		public EtherealSkeletalMount( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}
