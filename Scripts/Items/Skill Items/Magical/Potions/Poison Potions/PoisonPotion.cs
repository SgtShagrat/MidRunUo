using System;
using Server;

namespace Server.Items
{
	public class PoisonPotion : BasePoisonPotion
	{
		public override Poison Poison{ get{ return Poison.Regular; } }

		public override double MinPoisoningSkill{ get{ return 30.0; } }
		public override double MaxPoisoningSkill{ get{ return 70.0; } }

		#region Modifica by Dies Irae per le pozioni Stackable
		[Constructable]
		public PoisonPotion( int amount ) : base( PotionEffect.Poison, amount )
		{
		}
		
		[Constructable]
		public PoisonPotion() : this(1)
		{
		}
		#endregion
		
		public PoisonPotion( Serial serial ) : base( serial )
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
