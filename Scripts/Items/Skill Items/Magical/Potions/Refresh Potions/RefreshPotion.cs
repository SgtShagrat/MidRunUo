using System;
using Server;

namespace Server.Items
{
	public class RefreshPotion : BaseRefreshPotion
	{
		public override double Refresh{ get{ return 0.25; } }

		#region Modifica by Dies Irae per le pozioni Stackable
		[Constructable]
		public RefreshPotion( int amount ) : base( PotionEffect.Refresh, amount )
		{
		}
		
		[Constructable]
		public RefreshPotion() : this(1)
		{
		}
		#endregion

		public RefreshPotion( Serial serial ) : base( serial )
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
