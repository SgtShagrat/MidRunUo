using Server;
using Server.Items;

namespace Midgard.Items
{
	public class TamlaPotion : BasePaganPotion
	{
		public override int DelayUse
		{
			get { return 30; }
		}

		public override int Strength
		{
			get { return 8; }
		}

		public override double AlchemyRequiredToDrink
		{
			get { return 70.0; }
		}

		public override int CustomSound
		{
			get { return 0x0203; }
		}

		public override int CustomAnim
		{
			get { return 0x0022; }
		}

		public override int CustomEffects
		{
			get { return 0x376a; }
		}

		[Constructable]
		public TamlaPotion( int amount ) : base( PotionEffect.Tamla, amount )
		{
			Weight = 1.0;
			Hue = 2466;
		}

		[Constructable]
		public TamlaPotion() : this( 1 )
		{
		}

		public override bool CanDrink( Mobile from, bool message )
		{
			if( !base.CanDrink( from, message ) )
				return false;

			if( from.Hits >= from.HitsMax /*|| from.Poisoned */ || MortalStrike.IsWounded( from ) )
			{
				from.SendMessage( from.Language == "ITA" ? "Non puoi usare questa pozione adesso." : "You cannot use this potion now." );
				return false;
			}

			return true;
		}

		public override bool DoPaganEffect( Mobile from )
		{
			int toHeal;

			if( Utility.RandomDouble() < 0.30 )
			{
				from.SendMessage( from.Language == "ITA" ? "Caspita! Un pò della pozione è caduta per terra!" : "Ouch! Some potion has fell down from the bottle!" );
				toHeal = (int)( 0.5 * ( from.HitsMax - from.Hits ) );
			}
			else
			{
				toHeal = from.HitsMax - from.Hits;
			}

			LockBasePotionUse( from );

			from.Heal( toHeal );

			return true;
		}

		#region serial deserial
		public TamlaPotion( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
		#endregion
	}
}