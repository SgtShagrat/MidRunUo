using System;
using Server;

namespace Server.Items
{
	public class AgilityPotion : BaseAgilityPotion
	{
		public override int DexOffset{ get{ return 10; } }
	    public override TimeSpan Duration{ get{ return TimeSpan.FromMinutes( 3.0 ); } } // mod by Dies Irae

		#region Modifica by Dies Irae per le pozioni Stackable	
		[Constructable]
		public AgilityPotion( int amount ) : base( PotionEffect.Agility, amount )
		{
		}
		
		[Constructable]
		public AgilityPotion() : this(1)
		{
		}
		#endregion
		
		public AgilityPotion( Serial serial ) : base( serial )
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
