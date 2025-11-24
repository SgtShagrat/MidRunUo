using System;
using Server;

namespace Server.Items
{
	public class ParalisiPoisonPotion : BasePoisonPotion
	{
		public override Poison Poison{ get{ return Poison.ParalisiRegular; } }

		public override double MinPoisoningSkill{ get{ return 30.0; } }
		public override double MaxPoisoningSkill{ get{ return 70.0; } }

		[Constructable]
		public ParalisiPoisonPotion( int amount ) : base( 0xF04, PotionEffect.ParalisiPoison, amount )
		{
			Hue = 1672;
		}
		
		[Constructable]
		public ParalisiPoisonPotion() : this(1)
		{
		}
		
		public ParalisiPoisonPotion( Serial serial ) : base( serial )
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