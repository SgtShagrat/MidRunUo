using System;
using Server;

namespace Server.Items
{
	public class ParalisiDeadlyPoisonPotion : BasePoisonPotion
	{
		public override Poison Poison{ get{ return Poison.ParalisiDeadly; } }

		public override double MinPoisoningSkill{ get{ return 95.0; } }
		public override double MaxPoisoningSkill{ get{ return 140.0; } }

		[Constructable]
		public ParalisiDeadlyPoisonPotion( int amount ) : base( 0xF04, PotionEffect.ParalisiPoisonDeadly, amount )
		{
			Hue = 1672;
		}
		
		[Constructable]
		public ParalisiDeadlyPoisonPotion() : this(1)
		{
		}

		public ParalisiDeadlyPoisonPotion( Serial serial ) : base( serial )
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