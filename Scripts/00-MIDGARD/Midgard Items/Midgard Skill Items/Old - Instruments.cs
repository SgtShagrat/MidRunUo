using System;

namespace Server.Items
{
	[FlipableAttribute( 3737, 3738 )]
	public class DecoratedHarp : BaseInstrument
	{
		public override int LabelNumber{ get{ return 1065125; } } // Decorated Standing Harp
		
		[Constructable]
		public DecoratedHarp() : base( 3737, 0x43, 0x44 )
		{
			Weight = 35.0;
		}

		public DecoratedHarp( Serial serial ) : base( serial )
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
	
	public class DecoratedLapHarp : BaseInstrument
	{
		public override int LabelNumber{ get{ return 1065127; } } // Decorated Lap Harp
		[Constructable]
		public DecoratedLapHarp() : base( 3736, 0x45, 0x46 )
		{
			Weight = 10.0;
		}
		
		public DecoratedLapHarp( Serial serial ) : base( serial )
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
