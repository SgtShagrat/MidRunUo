using System;
using Server;

namespace Server.Items
{
	public class MagiaDeadlyPoisonPotion : BasePoisonPotion
	{
		public override Poison Poison{ get{ return Poison.MagiaDeadly; } }

		public override double MinPoisoningSkill{ get{ return 95.0; } }
		public override double MaxPoisoningSkill{ get{ return 140.0; } }

		[Constructable]
		public MagiaDeadlyPoisonPotion( int amount ) : base( 0xF04, PotionEffect.MagiaPoisonDeadly, amount )
		{
			Hue = 2567;
		}
		
		[Constructable]
		public MagiaDeadlyPoisonPotion() : this(1)
		{
		}

		public MagiaDeadlyPoisonPotion( Serial serial ) : base( serial )
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