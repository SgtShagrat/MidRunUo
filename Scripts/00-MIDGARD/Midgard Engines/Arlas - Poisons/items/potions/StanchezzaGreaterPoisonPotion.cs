using System;
using Server;

namespace Server.Items
{
	public class StanchezzaGreaterPoisonPotion : BasePoisonPotion
	{
		public override Poison Poison{ get{ return Poison.StanchezzaGreater; } }

		public override double MinPoisoningSkill{ get{ return 60.0; } }
		public override double MaxPoisoningSkill{ get{ return 120.0; } }

		[Constructable]
		public StanchezzaGreaterPoisonPotion( int amount ) : base( 0xF04, PotionEffect.StanchezzaPoisonGreater, amount )
		{
			Hue = 1671;
		}
		
		[Constructable]
		public StanchezzaGreaterPoisonPotion() : this(1)
		{
		}

		public StanchezzaGreaterPoisonPotion( Serial serial ) : base( serial )
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