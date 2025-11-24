using System;
using Server;

namespace Server.Items
{
	public class StanchezzaLesserPoisonPotion : BasePoisonPotion
	{
		public override Poison Poison{ get{ return Poison.StanchezzaLesser; } }

		public override double MinPoisoningSkill{ get{ return 0.0; } }
		public override double MaxPoisoningSkill{ get{ return 60.0; } }

		[Constructable]
		public StanchezzaLesserPoisonPotion( int amount ) : base( 0xF04, PotionEffect.StanchezzaPoisonLesser, amount )
		{
			Hue = 1671;
		}
		
		[Constructable]
		public StanchezzaLesserPoisonPotion() : this(1)
		{
		}
		
		public StanchezzaLesserPoisonPotion( Serial serial ) : base( serial )
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