using System;
using Server;

namespace Server.Items
{
	public class BloccoLesserPoisonPotion : BasePoisonPotion
	{
		public override Poison Poison{ get{ return Poison.BloccoLesser; } }

		public override double MinPoisoningSkill{ get{ return 0.0; } }
		public override double MaxPoisoningSkill{ get{ return 60.0; } }

		[Constructable]
		public BloccoLesserPoisonPotion( int amount ) : base( 0xF04, PotionEffect.BloccoPoisonLesser, amount )
		{
			Hue = 1677;
		}
		
		[Constructable]
		public BloccoLesserPoisonPotion() : this(1)
		{
		}
		
		public BloccoLesserPoisonPotion( Serial serial ) : base( serial )
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