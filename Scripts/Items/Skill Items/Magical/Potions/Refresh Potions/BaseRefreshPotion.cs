using System;
using Server;

namespace Server.Items
{
	public abstract class BaseRefreshPotion : BasePotion
	{
		public abstract double Refresh{ get; }

		#region Modifica by Dies Irae per le pozioni Stackable
		public BaseRefreshPotion( PotionEffect effect, int amount ) : base( 0xF0B, effect, amount )
		{
		}
		#endregion
		
		public BaseRefreshPotion( Serial serial ) : base( serial )
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

		public override void Drink( Mobile from )
		{
			if ( from.Stam < from.StamMax )
			{
                #region mod by Dies Irae
                if( !Core.AOS )
                    LockBasePotionUse( from );
                #endregion

				from.Stam += Scale( from, (int)(Refresh * from.StamMax) );

				BasePotion.PlayDrinkEffect( from );

				this.Consume();
			}
			else
			{
				from.SendMessage( "You decide against drinking this potion, as you are already at full stamina." );
			}
		}
	}
}
