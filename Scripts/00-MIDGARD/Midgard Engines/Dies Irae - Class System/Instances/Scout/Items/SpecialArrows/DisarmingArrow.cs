/***************************************************************************
 *							   DisarmingArrow.cs
 *
 *   begin				: 08 October, 2009
 *   author			   :	Dies Irae	
 *   email				: tocasia@alice.it
 *   copyright			: (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;

using Server;
using Server.Items;

namespace Midgard.Items
{
	public class DisarmingArrow : BaseScoutArrow, ICommodity
	{
		#region ICommodity members
		string ICommodity.Description{get { return String.Format( Amount == 1 ? "{0} disarming arrow" : "{0} disarmin arrows", Amount ); }}
		int ICommodity.DescriptionNumber{get { return 0; }}
		#endregion

		public override string DefaultName{get { return "disarming arrow"; }}

		[Constructable]
		public DisarmingArrow() : this( 1 )
		{
		}

		[Constructable]
		public DisarmingArrow( int amount )
		{
			Amount = amount;
			Hue = 2199;
		}
		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "frecc%iae% disarmant%ei%" : "disarming arrow%s%", Amount, from.Language ) );
		}

		public override void OnHit( BaseRanged baseRanged, Mobile attacker, Mobile defender, double damageBonus )
		{
			//if( attacker.CanBeginAction( typeof( DisarmingArrow ) ) )
			//{
				if( CheckSkill() )
				{
					attacker.SendMessage( attacker.Language == "ITA" ? "Hai colpito il nemico con successo con una freccia disarmante." : "You successfully hit your enemy with a disarming arrow." );
					defender.SendMessage( attacker.Language == "ITA" ? "Sei stato colpito da una freccia disarmante!" : "You have been hit by a disarming arrow." );

					//attacker.BeginAction( typeof( DisarmingArrow ) );
					Disarm( attacker, defender );
					//Timer.DelayCall( baseRanged.GetCoolingDelay( attacker, typeof( DisarmingArrow ) ), new TimerStateCallback( ReleaseDisarmingArrowLock ), attacker );
				}
				else
				{
					attacker.SendLocalizedMessage( 1004004 ); // You failed in your attempt to disarm.
					defender.SendLocalizedMessage( 1004005 ); // Your opponent tried to disarm you but failed.
				}
			//}

			base.OnHit( baseRanged, attacker, defender, damageBonus );
		}

		private static void Disarm( Mobile attacker, Mobile defender )
		{
			if( defender.Player || defender.Body.IsHuman )
			{
				Item toDisarm = defender.FindItemOnLayer( Layer.OneHanded );

				if( toDisarm == null || !toDisarm.Movable )
					toDisarm = defender.FindItemOnLayer( Layer.TwoHanded );

				Container pack = defender.Backpack;

				if( pack == null || toDisarm == null || !toDisarm.Movable )
				{
					attacker.SendLocalizedMessage( 1004001 ); // You cannot disarm your opponent.
				}
				else
				{
					attacker.SendLocalizedMessage( 1004006 ); // You successfully disarm your opponent!
					defender.SendLocalizedMessage( 1004007 ); // You have been disarmed!

					pack.DropItem( toDisarm );
				}
			}
		}

		private static void ReleaseDisarmingArrowLock( object state )
		{
			( (Mobile)state ).EndAction( typeof( DisarmingArrow ) );
		}

		#region serialization
		public DisarmingArrow( Serial serial )
			: base( serial )
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