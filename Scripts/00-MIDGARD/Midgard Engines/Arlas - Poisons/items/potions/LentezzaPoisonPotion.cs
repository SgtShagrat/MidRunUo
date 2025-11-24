using System;
using Server;

namespace Server.Items
{
	public class LentezzaPoisonPotion : BasePoisonPotion
	{
		public override Poison Poison{ get{ return Poison.LentezzaRegular; } }

		public override double MinPoisoningSkill{ get{ return 30.0; } }
		public override double MaxPoisoningSkill{ get{ return 70.0; } }

		[Constructable]
		public LentezzaPoisonPotion( int amount ) : base( 0xF04, PotionEffect.LentezzaPoison, amount )
		{
			Hue = 1673;
		}
		
		[Constructable]
		public LentezzaPoisonPotion() : this(1)
		{
		}
		
		public LentezzaPoisonPotion( Serial serial ) : base( serial )
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