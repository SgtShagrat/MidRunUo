using System;
using Server;

namespace Server.Items
{
	public class GreaterPoisonPotion : BasePoisonPotion
	{
		public override Poison Poison{ get{ return Poison.Greater; } }

		public override double MinPoisoningSkill{ get{ return 60.0; } }
		public override double MaxPoisoningSkill{ get{ return 120.0; } }

		#region Modifica by Dies Irae per le pozioni Stackable
		[Constructable]
		public GreaterPoisonPotion( int amount ) : base( PotionEffect.PoisonGreater, amount )
		{
		}
		
		[Constructable]
		public GreaterPoisonPotion() : this(1)
		{
		}
		#endregion

		public GreaterPoisonPotion( Serial serial ) : base( serial )
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
