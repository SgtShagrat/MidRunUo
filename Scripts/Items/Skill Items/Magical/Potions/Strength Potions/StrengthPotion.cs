using System;
using Server;

namespace Server.Items
{
	public class StrengthPotion : BaseStrengthPotion
	{
		public override int StrOffset{ get{ return 10; } }
	    public override TimeSpan Duration{ get{ return TimeSpan.FromMinutes( 3.0 ); } } // mod by Dies Irae

		#region Modifica by Dies Irae per le pozioni Stackable
		[Constructable]
		public StrengthPotion( int amount ) : base( PotionEffect.Strength, amount )
		{
		}
		
		[Constructable]
		public StrengthPotion() : this(1)
		{
		}
		#endregion

		public StrengthPotion( Serial serial ) : base( serial )
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
