using System;
using Server;

namespace Server.Items
{
	public class BloccoDeadlyPoisonPotion : BasePoisonPotion
	{
		public override Poison Poison{ get{ return Poison.BloccoDeadly; } }

		public override double MinPoisoningSkill{ get{ return 95.0; } }
		public override double MaxPoisoningSkill{ get{ return 140.0; } }

		[Constructable]
		public BloccoDeadlyPoisonPotion( int amount ) : base( 0xF04, PotionEffect.BloccoPoisonDeadly, amount )
		{
			Hue = 1677;
		}
		
		[Constructable]
		public BloccoDeadlyPoisonPotion() : this(1)
		{
		}

		public BloccoDeadlyPoisonPotion( Serial serial ) : base( serial )
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