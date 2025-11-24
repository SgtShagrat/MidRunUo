using System;
using Server;

namespace Server.Items
{
	public class LentezzaLesserPoisonPotion : BasePoisonPotion
	{
		public override Poison Poison{ get{ return Poison.LentezzaLesser; } }

		public override double MinPoisoningSkill{ get{ return 0.0; } }
		public override double MaxPoisoningSkill{ get{ return 60.0; } }

		[Constructable]
		public LentezzaLesserPoisonPotion( int amount ) : base( 0xF04, PotionEffect.LentezzaPoisonLesser, amount )
		{
			Hue = 1673;
		}
		
		[Constructable]
		public LentezzaLesserPoisonPotion() : this(1)
		{
		}
		
		public LentezzaLesserPoisonPotion( Serial serial ) : base( serial )
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