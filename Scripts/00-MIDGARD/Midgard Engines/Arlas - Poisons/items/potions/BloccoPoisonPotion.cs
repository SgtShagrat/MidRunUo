using System;
using Server;

namespace Server.Items
{
	public class BloccoPoisonPotion : BasePoisonPotion
	{
		public override Poison Poison{ get{ return Poison.BloccoRegular; } }

		public override double MinPoisoningSkill{ get{ return 30.0; } }
		public override double MaxPoisoningSkill{ get{ return 70.0; } }

		[Constructable]
		public BloccoPoisonPotion( int amount ) : base( 0xF04, PotionEffect.BloccoPoison, amount )
		{
			Hue = 1677;
		}
		
		[Constructable]
		public BloccoPoisonPotion() : this(1)
		{
		}
		
		public BloccoPoisonPotion( Serial serial ) : base( serial )
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