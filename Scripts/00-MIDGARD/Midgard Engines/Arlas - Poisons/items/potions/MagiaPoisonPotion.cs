using System;
using Server;

namespace Server.Items
{
	public class MagiaPoisonPotion : BasePoisonPotion
	{
		public override Poison Poison{ get{ return Poison.MagiaRegular; } }

		public override double MinPoisoningSkill{ get{ return 30.0; } }
		public override double MaxPoisoningSkill{ get{ return 70.0; } }

		[Constructable]
		public MagiaPoisonPotion( int amount ) : base( 0xF04, PotionEffect.MagiaPoison, amount )
		{
			Hue = 2567;
		}
		
		[Constructable]
		public MagiaPoisonPotion() : this(1)
		{
		}
		
		public MagiaPoisonPotion( Serial serial ) : base( serial )
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