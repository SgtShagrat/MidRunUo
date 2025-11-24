using System;

namespace Server.Items
{
	public abstract class BaseStrengthPotion : BasePotion
	{
		public abstract int StrOffset{ get; }
		public abstract TimeSpan Duration{ get; }

		#region Modifica by Dies Irae per le pozioni Stackable
		public BaseStrengthPotion( PotionEffect effect, int amount ) : base( 0xF09, effect, amount )
		{
		}
		#endregion

		public BaseStrengthPotion( Serial serial ) : base( serial )
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

		public bool DoStrength( Mobile from )
		{
			// TODO: Verify scaled; is it offset, duration, or both?
			if ( Spells.SpellHelper.AddStatOffset( from, StatType.Str, Scale( from, StrOffset ), Duration ) )
			{
				from.FixedEffect( 0x375A, 10, 15 );
				from.PlaySound( 0x1E7 );
				return true;
			}

			from.SendLocalizedMessage( 502173 ); // You are already under a similar effect.
			return false;
		}

		public override void Drink( Mobile from )
		{
			if ( DoStrength( from ) )
			{
                #region mod by Dies Irae
                if( !Core.AOS )
                    LockBasePotionUse( from );
                #endregion

				BasePotion.PlayDrinkEffect( from );

				this.Consume();
			}
		}
	}
}
