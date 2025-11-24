/***************************************************************************
 *							   LobotomySpell.cs
 *
 *   begin				: 26 September, 2009
 *   author			   :	Dies Irae	
 *   email				: tocasia@alice.it
 *   copyright			: (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Collections;

using Server;
using Server.Spells;
using Server.Targeting;

namespace Midgard.Engines.SpellSystem
{
	public class LobotomySpell : RPGNecromancerSpell
	{
		private static readonly SpellInfo m_Info = new SpellInfo(
			"Lobotomy", "Wis An Ben",
			203,
			9031,
			Reagent.BatWing,
			Reagent.PigIron,
			Reagent.DaemonBlood
			);

		private static readonly Hashtable m_Table = new Hashtable();

		public LobotomySpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		private static readonly ExtendedSpellInfo m_ExtendedInfo = new ExtendedSpellInfo
			(
			typeof( LobotomySpell ),
			"Attempts to place a curse on the Target that increases the mana cost of any spells they cast.",
			"Con questo potente maleficio aumenti l'utilizzo del mana da parte del tuo nemico.",
			0x500D
			);

		public override ExtendedSpellInfo ExtendedInfo{get { return m_ExtendedInfo; }}

		public override TimeSpan CastDelayBase{get { return TimeSpan.FromSeconds( 1.5 ); }}
		public override double RequiredSkill{get { return 30.0; }}
		public override int RequiredMana{get { return 17; }}
		public override double DelayOfReuse{get { return 5.0; }}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		public void Target( Mobile m )
		{
			if( HasMindRotScalar( m ) )
			{
				Caster.SendLocalizedMessage( 1005559 ); // This spell is already in effect.
			}
			else if( CheckHSequence( m ) )
			{
				SpellHelper.Turn( Caster, m );

				/* Attempts to place a curse on the Target that increases the mana cost of any spells they cast,
				 * for a duration based off a comparison between the Caster's Spirit Speak skill and the Target's Resisting Spells skill.
				 * The effect lasts for ((Spirit Speak skill level - target's Resist Magic skill level) / 50 ) + 20 seconds.
				 */

				m.PlaySound( 0x1FB );
				m.PlaySound( 0x258 );
				m.FixedParticles( 0x373A, 1, 17, 9903, 15, 4, EffectLayer.Head );

				double duration = ( ( ( ( GetPowerLevel() * 20 ) - GetResistSkill( m ) ) / 5.0 ) + 20.0 ) * ( m.Player ? 1.0 : 2.0 );

				if( m.Player )
					duration = MidgardSpellHelper.ScaleByCustomRes( duration, m, CustomResType.General );

				if ( CheckResisted( m ) )
				{
					duration *= ( 1.0 - GetResistScalar( m ) );//0.75;

					m.SendLocalizedMessage( 501783 ); // You feel yourself resisting magical energy.
				}

				if( m.Player )
					SetMindRotScalar( Caster, m, ( 1 + GetPowerLevel() * 0.05 ), TimeSpan.FromSeconds( duration ) );
				else
					SetMindRotScalar( Caster, m, ( 1.5 + GetPowerLevel() * 0.05 ), TimeSpan.FromSeconds( duration ) );
			}

			FinishSequence();
		}

		public static void ClearMindRotScalar( Mobile m )
		{
			if( !m_Table.ContainsKey( m ) )
				return;

			BuffInfo.RemoveBuff( m, BuffIcon.Mindrot );
			InternalBucket tmpB = (InternalBucket)m_Table[ m ];
			InternalTimer tmpT = tmpB.MRExpireTimer;
			tmpT.Stop();
			m_Table.Remove( m );
			m.SendLocalizedMessage( 1060872 ); // Your mind feels normal again.
		}

		public static bool HasMindRotScalar( Mobile m )
		{
			return m_Table.ContainsKey( m );
		}

		public static bool GetMindRotScalar( Mobile m, ref double scalar )
		{
			if( !m_Table.ContainsKey( m ) )
				return false;

			InternalBucket bucket = (InternalBucket)m_Table[ m ];
			scalar = bucket.Scalar;
			return true;
		}

		public static void SetMindRotScalar( Mobile caster, Mobile target, double scalar, TimeSpan duration )
		{
			if( !m_Table.ContainsKey( target ) )
			{
				m_Table.Add( target, new InternalBucket( scalar, new InternalTimer( target, duration ) ) );
				InternalBucket bucket = (InternalBucket)m_Table[ target ];
				InternalTimer timer = bucket.MRExpireTimer;
				timer.Start();
				target.SendLocalizedMessage( 1074384 );
			}
		}

		private class InternalTarget : Target
		{
			private readonly LobotomySpell m_Owner;

			public InternalTarget( LobotomySpell owner ) : base( 13, false, TargetFlags.Harmful )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if( o is Mobile )
					m_Owner.Target( (Mobile)o );
				else
					from.SendLocalizedMessage( 1060508 ); // You can't curse that.
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}

		private class InternalTimer : Timer
		{
			private readonly DateTime m_End;
			private readonly Mobile m_Target;

			public InternalTimer( Mobile target, TimeSpan delay ) : base( TimeSpan.FromSeconds( 1.0 ), TimeSpan.FromSeconds( 1.0 ) )
			{
				m_Target = target;
				m_End = DateTime.Now + delay;
				Priority = TimerPriority.TwoFiftyMS;
			}

			protected override void OnTick()
			{
				if( m_Target.Deleted || !m_Target.Alive || DateTime.Now >= m_End )
				{
					ClearMindRotScalar( m_Target );
					Stop();
				}
			}
		}

		private class InternalBucket
		{
			public readonly InternalTimer MRExpireTimer;
			public readonly double Scalar;

			public InternalBucket( double theScalar, InternalTimer theTimer )
			{
				Scalar = theScalar;
				MRExpireTimer = theTimer;
			}
		}
	}
}