using System;
using Server;

namespace Server.Items
{
	public class LentezzaGreaterPoisonPotion : BasePoisonPotion
	{
		public override Poison Poison{ get{ return Poison.LentezzaGreater; } }

		public override double MinPoisoningSkill{ get{ return 60.0; } }
		public override double MaxPoisoningSkill{ get{ return 120.0; } }

		[Constructable]
		public LentezzaGreaterPoisonPotion( int amount ) : base( 0xF04, PotionEffect.LentezzaPoisonGreater, amount )
		{
			Hue = 1673;
		}
		
		[Constructable]
		public LentezzaGreaterPoisonPotion() : this(1)
		{
		}

		public LentezzaGreaterPoisonPotion( Serial serial ) : base( serial )
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