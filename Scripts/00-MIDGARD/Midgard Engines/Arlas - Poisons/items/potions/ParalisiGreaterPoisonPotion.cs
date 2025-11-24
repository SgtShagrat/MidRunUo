using System;
using Server;

namespace Server.Items
{
	public class ParalisiGreaterPoisonPotion : BasePoisonPotion
	{
		public override Poison Poison{ get{ return Poison.ParalisiGreater; } }

		public override double MinPoisoningSkill{ get{ return 60.0; } }
		public override double MaxPoisoningSkill{ get{ return 120.0; } }

		[Constructable]
		public ParalisiGreaterPoisonPotion( int amount ) : base( 0xF04, PotionEffect.ParalisiPoisonGreater, amount )
		{
			Hue = 1672;
		}
		
		[Constructable]
		public ParalisiGreaterPoisonPotion() : this(1)
		{
		}

		public ParalisiGreaterPoisonPotion( Serial serial ) : base( serial )
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