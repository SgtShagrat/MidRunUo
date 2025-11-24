using System;

namespace Server.Items
{
	public class GreaterStrengthPotion : BaseStrengthPotion
	{
		public override int StrOffset{ get{ return 20; } }
	    public override TimeSpan Duration{ get{ return TimeSpan.FromMinutes( 5.0 ); } } // mod by Dies Irae

		#region Modifica by Dies Irae per le pozioni Stackable
		[Constructable]
		public GreaterStrengthPotion( int amount ) : base( PotionEffect.StrengthGreater, amount )
		{
		}
		
		[Constructable]
		public GreaterStrengthPotion() : this(1)
		{
		}
		#endregion

		public GreaterStrengthPotion( Serial serial ) : base( serial )
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
