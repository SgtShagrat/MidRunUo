using System;
using Server;

namespace Server.Items
{
	public class DeadlyPoisonPotion : BasePoisonPotion
	{
		public override Poison Poison{ get{ return Poison.Deadly; } }

		public override double MinPoisoningSkill{ get{ return 95.0; } }
		public override double MaxPoisoningSkill{ get{ return 140.0; } }

		#region Modifica by Dies Irae per le pozioni Stackable
		[Constructable]
		public DeadlyPoisonPotion( int amount ) : base( PotionEffect.PoisonDeadly, amount )
		{
		}
		
		[Constructable]
		public DeadlyPoisonPotion() : this(1)
		{
		}
		#endregion

		public DeadlyPoisonPotion( Serial serial ) : base( serial )
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
