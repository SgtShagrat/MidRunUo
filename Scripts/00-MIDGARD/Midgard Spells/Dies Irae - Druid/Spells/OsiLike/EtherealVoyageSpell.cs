/***************************************************************************
 *							   EtherealVoyage.cs
 *
 *   begin				: 27 September, 2009
 *   author			   :	Dies Irae	
 *   email				: tocasia@alice.it
 *   copyright			: (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;

using Server;
using Server.Spells;

namespace Midgard.Engines.SpellSystem
{
	public class EtherealVoyageSpell : DruidForm
	{
		private static readonly SpellInfo m_Info = new SpellInfo(
			"Ethereal Voyage", "Helma Aete",
			224,
			9011,
			Reagent.DestroyingAngel,
			Reagent.BlackPearl
			);

		private static readonly ExtendedSpellInfo m_ExtendedInfo = new ExtendedSpellInfo
			(
				typeof( EtherealVoyageSpell ),
				"Prevents monsters from being able to see the caster for a short time.",
				"Il Druido abbandona le sue spoglie per viaggiare in forma eterea senza essere visto dalle creature circostanti per un certo lasso di tempo."+
				"Durata (SK + 12 + FL*20).",
				0x59e4
			);

		public override ExtendedSpellInfo ExtendedInfo{get { return m_ExtendedInfo; }}

		public override SpellCircle Circle{get { return SpellCircle.Fifth; }}
		public override int Body{get { return 0x302; }}
		public override int Hue{get { return 0x48F; }}

		public EtherealVoyageSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public static void Initialize()
		{
			EventSink.AggressiveAction += new AggressiveActionEventHandler( delegate( AggressiveActionEventArgs e )
									{
										if( TransformationSpellHelper.UnderTransformation( e.Aggressor, typeof( EtherealVoyageSpell ) ) )
										{
											TransformationSpellHelper.RemoveContext( e.Aggressor, true );
										}
									} );
		}

		public override bool CheckCast()
		{
			if( TransformationSpellHelper.UnderTransformation( Caster, typeof( EtherealVoyageSpell ) ) )
			{
				Caster.SendLocalizedMessage( 501775 ); // This spell is already in effect.
			}
			else if( !Caster.CanBeginAction( typeof( EtherealVoyageSpell ) ) )
			{
				Caster.SendLocalizedMessage( 1075124 ); // You must wait before casting that spell again.
			}
			else if( Caster.Combatant != null )
			{
				Caster.SendLocalizedMessage( 1072586 ); // You cannot cast Ethereal Voyage while you are in combat.
			}
			else
			{
				return base.CheckCast();
			}

			return false;
		}

		public override void DoEffect( Mobile m )
		{
			m.PlaySound( 0x5C8 );
			m.SendLocalizedMessage( 1074770 ); // You are now under the effects of Ethereal Voyage.

			double skill = Caster.Skills.Spellweaving.Value;

			TimeSpan duration = TimeSpan.FromSeconds( 12 + (int)( skill ) + ( FocusLevel * 20 ) );

			Timer.DelayCall( duration, new TimerStateCallback<Mobile>( RemoveEffect ), Caster );

			Caster.BeginAction( typeof( EtherealVoyageSpell ) ); //Cannot cast this spell for another 5 minutes(300sec) after effect removed.
		}

		public override void RemoveEffect( Mobile m )
		{
			m.SendLocalizedMessage( 1074771 ); // You are no longer under the effects of Ethereal Voyage.

			TransformationSpellHelper.RemoveContext( m, true );

			Timer.DelayCall( TimeSpan.FromMinutes( 5 ), delegate
			{
				m.SendMessage( m.Language == "ITA" ? "Puoi usare di nuovo il tuo viaggio etereo." : "You can use again your ethereal voyage." );
				m.EndAction( typeof( EtherealVoyageSpell ) );
			} );
		}
	}
}