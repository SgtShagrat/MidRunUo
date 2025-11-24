using System;
using Server;

namespace Server.Items
{
	public class LesserPoisonPotion : BasePoisonPotion
	{
		public override Poison Poison{ get{ return Poison.Lesser; } }

		public override double MinPoisoningSkill{ get{ return 0.0; } }
		public override double MaxPoisoningSkill{ get{ return 60.0; } }

		#region Modifica by Dies Irae per le pozioni Stackable
		[Constructable]
		public LesserPoisonPotion( int amount ) : base( PotionEffect.PoisonLesser, amount )
		{
		}
		
		[Constructable]
		public LesserPoisonPotion() : this(1)
		{
		}
		#endregion
		
		public LesserPoisonPotion( Serial serial ) : base( serial )
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
