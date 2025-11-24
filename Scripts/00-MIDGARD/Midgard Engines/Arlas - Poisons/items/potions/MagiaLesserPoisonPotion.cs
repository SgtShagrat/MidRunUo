using System;
using Server;

namespace Server.Items
{
	public class MagiaLesserPoisonPotion : BasePoisonPotion
	{
		public override Poison Poison{ get{ return Poison.MagiaLesser; } }

		public override double MinPoisoningSkill{ get{ return 0.0; } }
		public override double MaxPoisoningSkill{ get{ return 60.0; } }

		[Constructable]
		public MagiaLesserPoisonPotion( int amount ) : base( 0xF04, PotionEffect.MagiaPoisonLesser, amount )
		{
			Hue = 2567;
		}
		
		[Constructable]
		public MagiaLesserPoisonPotion() : this(1)
		{
		}
		
		public MagiaLesserPoisonPotion( Serial serial ) : base( serial )
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