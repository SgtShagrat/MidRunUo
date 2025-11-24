/***************************************************************************
 *							   StuningArrow.cs
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
	public class StuningArrow : BaseScoutArrow, ICommodity
	{
		#region ICommodity members
		string ICommodity.Description{get { return String.Format( Amount == 1 ? "{0} stuning arrow" : "{0} stuning arrows", Amount ); }}

		int ICommodity.DescriptionNumber{get { return 0; }}
		#endregion

		public override string DefaultName{get { return "stuning arrow"; }}

		[Constructable]
		public StuningArrow() : this( 1 )
		{
		}

		[Constructable]
		public StuningArrow( int amount )
		{
			Amount = amount;
			Hue = 1908;
		}
		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "frecc%iae% stordent%ei%" : "stunning arrow%s%", Amount, from.Language ) );
		}

		public override void OnHit( BaseRanged baseRanged, Mobile attacker, Mobile defender, double damageBonus )
		{
			//if( attacker.CanBeginAction( typeof( StuningArrow ) ) )
			//{
				if( CheckSkill() )
				{
					attacker.SendLocalizedMessage( 1004013 ); // You successfully stun your opponent!
					defender.SendLocalizedMessage( 1004014 ); // You have been stunned!

					//attacker.BeginAction( typeof( StuningArrow ) );
					DoStun( attacker, defender );
					//Timer.DelayCall( baseRanged.GetCoolingDelay( attacker, typeof( StuningArrow ) ), new TimerStateCallback( ReleaseStuningArrowLock ), attacker );
				}
				else
				{
					attacker.SendMessage( attacker.Language == "ITA" ? "Hai fallito il tentativo di stordire il nemico." : "You failed in your attempt to stun." );
					defender.SendMessage( defender.Language == "ITA" ? "Il nemico ti sta lanciando frecce stordenti!" : "Your opponent tried to stun you and failed." );
				}
			//}

			base.OnHit( baseRanged, attacker, defender, damageBonus );
		}

		private static void DoStun( Mobile attacker, Mobile defender )
		{
			defender.Freeze( TimeSpan.FromSeconds( 4.0 ) );
		}

		private static void ReleaseStuningArrowLock( object state )
		{
			( (Mobile)state ).EndAction( typeof( StuningArrow ) );
		}

		#region serialization
		public StuningArrow( Serial serial ) : base( serial )
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