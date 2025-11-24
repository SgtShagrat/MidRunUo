using System;
using Server;

namespace Server.Items
{
	public class BloccoGreaterPoisonPotion : BasePoisonPotion
	{
		public override Poison Poison{ get{ return Poison.BloccoGreater; } }

		public override double MinPoisoningSkill{ get{ return 60.0; } }
		public override double MaxPoisoningSkill{ get{ return 120.0; } }

		[Constructable]
		public BloccoGreaterPoisonPotion( int amount ) : base( 0xF04, PotionEffect.BloccoPoisonGreater, amount )
		{
			Hue = 1677;
		}
		
		[Constructable]
		public BloccoGreaterPoisonPotion() : this(1)
		{
		}

		public BloccoGreaterPoisonPotion( Serial serial ) : base( serial )
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