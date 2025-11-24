using System;
using Server;

namespace Server.Items
{
	public class GreaterAgilityPotion : BaseAgilityPotion
	{
		public override int DexOffset{ get{ return 20; } }
	    public override TimeSpan Duration{ get{ return TimeSpan.FromMinutes( 5.0 ); } } // mod by Dies Irae

		#region Modifica by Dies Irae per le pozioni Stackable
		[Constructable]
		public GreaterAgilityPotion( int amount ) : base( PotionEffect.AgilityGreater, amount )
		{
		}
		
		[Constructable]
		public GreaterAgilityPotion() : this(1)
		{
		}
		#endregion
		
		public GreaterAgilityPotion( Serial serial ) : base( serial )
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
