/***************************************************************************
 *							   DismountingArrow.cs
 *
 *   begin				: 08 October, 2009
 *   author			   :	Dies Irae	
 *   email				: tocasia@alice.it
 *   copyright			: (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;

using Midgard.Engines.Classes;

using Server;
using Server.Items;
using Server.Mobiles;

namespace Midgard.Items
{
	public class DismountingArrow : BaseScoutArrow, ICommodity
	{
		#region ICommodity members
		string ICommodity.Description{get { return String.Format( Amount == 1 ? "{0} dismounting arrow" : "{0} dismounting arrows", Amount ); }}
		int ICommodity.DescriptionNumber{get { return 0; }}
		#endregion

		public override string DefaultName{get { return "dismounting arrow"; }}

		[Constructable]
		public DismountingArrow() : this( 1 )
		{
		}

		[Constructable]
		public DismountingArrow( int amount )
		{
			Amount = amount;
			Hue = 1784;
		}

		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "frecc%iae% dismontant%ei%" : "dismounting arrow%s%", Amount, from.Language ) );
		}

		public override bool OnFired( BaseRanged bow, Mobile attacker, Mobile defender )
		{
			if( !( ClassSystem.IsScout( attacker ) ) )
				return false;

			IMount mount = defender.Mount;

			if( mount == null )
			{
				attacker.SendLocalizedMessage( 1060848 ); // This attack only works on mounted targets
				//return false;
			}

			return base.OnFired( bow, attacker, defender );
		}

		public override void OnHit( BaseRanged baseRanged, Mobile attacker, Mobile defender, double damageBonus )
		{
			//if( attacker.CanBeginAction( typeof( DismountingArrow ) ) )
			//{
				if( CheckSkill() )
				{
					attacker.SendMessage( attacker.Language == "ITA" ? "Hai colpito il tuo nemico con successo con una freccia dismontante." : "You successfully hit your enemy with a dismounting arrow." );
					defender.SendMessage( defender.Language == "ITA" ? "Sei stato colpito da una freccia dismontante." : "You have been hit by a dismounting arrow." );

					//attacker.BeginAction( typeof( DismountingArrow ) );
					Dismount( attacker, defender );
					//Timer.DelayCall( baseRanged.GetCoolingDelay( attacker, typeof( DismountingArrow ) ), new TimerStateCallback( ReleaseDismountingArrowLock ), attacker );
				}
				else
				{
					attacker.SendMessage( attacker.Language == "ITA" ? "Il colpo dismontante ha fallito." : "You failed in your attempt to dismount." );
					defender.SendMessage( defender.Language == "ITA" ? "Il tuo nemico cerca di farti cadere dalla cavalcatura!" : "Your opponent tried to dismount you and failed." );
				}
			//}

			base.OnHit( baseRanged, attacker, defender, damageBonus );
		}

		private static void Dismount( Mobile attacker, Mobile defender )
		{
			IMount mount = defender.Mount;

			if( mount == null )
				return;

			attacker.SendLocalizedMessage( 1060082 ); // The force of your attack has dislodged them from their mount!

			if( attacker.Mounted )
				defender.SendLocalizedMessage( 1062315 ); // You fall off your mount!
			else
				defender.SendLocalizedMessage( 1060083 ); // You fall off of your mount and take damage!

			defender.PlaySound( 0x140 );
			defender.FixedParticles( 0x3728, 10, 15, 9955, EffectLayer.Waist );

			mount.Rider = null;

			BaseMount.SetMountPrevention( attacker, BlockMountType.DismountRecovery, TimeSpan.FromSeconds( 3.0 ) );

			if( !attacker.Mounted )
				AOS.Damage( defender, attacker, Utility.Dice( 1, 20, 5 ), 100, 0, 0, 0, 0 );
		}

		private static void ReleaseDismountingArrowLock( object state )
		{
			( (Mobile)state ).EndAction( typeof( DismountingArrow ) );
		}

		#region serialization
		public DismountingArrow( Serial serial ) : base( serial )
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