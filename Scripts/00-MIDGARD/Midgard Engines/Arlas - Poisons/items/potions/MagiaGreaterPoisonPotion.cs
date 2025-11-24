using System;
using Server;

namespace Server.Items
{
	public class MagiaGreaterPoisonPotion : BasePoisonPotion
	{
		public override Poison Poison{ get{ return Poison.MagiaGreater; } }

		public override double MinPoisoningSkill{ get{ return 60.0; } }
		public override double MaxPoisoningSkill{ get{ return 120.0; } }

		[Constructable]
		public MagiaGreaterPoisonPotion( int amount ) : base( 0xF04, PotionEffect.MagiaPoisonGreater, amount )
		{
			Hue = 2567;
		}
		
		[Constructable]
		public MagiaGreaterPoisonPotion() : this(1)
		{
		}

		public MagiaGreaterPoisonPotion( Serial serial ) : base( serial )
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