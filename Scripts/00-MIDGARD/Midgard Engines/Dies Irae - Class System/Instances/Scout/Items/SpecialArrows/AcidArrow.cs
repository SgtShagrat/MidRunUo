/***************************************************************************
 *							   AcidArrow.cs
 *
 *   begin				: 07 October, 2009
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
	public class AcidArrow : BaseScoutArrow, ICommodity
	{
		#region ICommodity members
		string ICommodity.Description{get { return String.Format( Amount == 1 ? "{0} acid arrow" : "{0} acid arrows", Amount ); }}
		int ICommodity.DescriptionNumber{get { return 0; }}
		#endregion

		public override string DefaultName{get { return "acid arrow"; }}

		[Constructable]
		public AcidArrow() : this( 1 )
		{
		}

		[Constructable]
		public AcidArrow( int amount )
		{
			Amount = amount;
			Hue = 2130;
		}

		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "frecc%iae% acid%ae%" : "acid arrow%s%", Amount, from.Language ) );
		}

		public override void OnHit( BaseRanged baseRanged, Mobile attacker, Mobile defender, double damageBonus )
		{
			//if( attacker.CanBeginAction( typeof( AcidArrow ) ) )
			//{
				if( CheckSkill() )
				{
					attacker.SendMessage( attacker.Language == "ITA" ? "Hai colpito il tuo nemico con successo con una freccia acida." : "You successfully hit your enemy with an acid arrow." );
					defender.SendMessage( defender.Language == "ITA" ? "Sei stato colpito da una freccia acida." : "You have been hit by an acid arrow." );

					//attacker.BeginAction( typeof( AcidArrow ) );
					SpillAcid( defender, Utility.Dice( 1, 10, 10 ), "acid" );
					//Timer.DelayCall( baseRanged.GetCoolingDelay( attacker, typeof( AcidArrow ) ), new TimerStateCallback( ReleaseAcidArrowLock ), attacker );
				}
				else
				{
					attacker.SendMessage( attacker.Language == "ITA" ? "La tua freccia acida non è esplosa!" : "Your acid arrow did not explode!" );
					defender.SendMessage( defender.Language == "ITA" ? "Il tuo nemico ti sta lanciando frecce acide!" : "Your opponent tried to hit you with an acid ammo but failed!" );
				}
			//}

			base.OnHit( baseRanged, attacker, defender, damageBonus );
		}

		private static void ReleaseAcidArrowLock( object state )
		{
			( (Mobile)state ).EndAction( typeof( AcidArrow ) );
		}

		private static void SpillAcid( IEntity target, int amount, string name )
		{
			SpillAcid( TimeSpan.FromSeconds( 10.0 ), 5, 10, target, amount, amount, name );
		}

		private static void SpillAcid( TimeSpan duration, int minDamage, int maxDamage, IEntity target, int minAmount, int maxAmount, string name )
		{
			if( ( target != null && target.Map == null ) || target == null )
				return;

			int pools = Utility.RandomMinMax( minAmount, maxAmount );

			for( int i = 0; i < pools; ++i )
			{
				Point3D loc = target.Location;
				Map map = target.Map;

				PoolOfAcid acid = new PoolOfAcid( duration, minDamage, maxDamage );

				if( pools == 1 )
				{
					loc = target.Location;
				}
				else
				{
					bool validLocation = false;
					for( int j = 0; !validLocation && j < 10; ++j )
					{
						loc = new Point3D( loc.X + ( Utility.Random( 0, 3 ) - 2 ), loc.Y + ( Utility.Random( 0, 3 ) - 2 ), loc.Z );
						loc.Z = map.GetAverageZ( loc.X, loc.Y );
						validLocation = map.CanFit( loc, 16, false, false );
					}
				}

				acid.Name = name;
				acid.MoveToWorld( loc, map );
			}
		}

		#region serialization
		public AcidArrow( Serial serial ) : base( serial )
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