/***************************************************************************
 *							   ChokingSpell.cs
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
	public class ChokingSpell : RPGNecromancerSpell
	{
		private static readonly SpellInfo m_Info = new SpellInfo(
			"Choking", "In Bal Nox",
			209,
			9031,
			Reagent.DaemonBlood,
			Reagent.NoxCrystal
			);

		private static readonly Hashtable m_Table = new Hashtable();

		public ChokingSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		private static readonly ExtendedSpellInfo m_ExtendedInfo = new ExtendedSpellInfo
			(
			typeof( ChokingSpell ),
			"Temporarily chokes off the air supply of the target with poisonous fumes. The target is inflicted with poison damage over time.",
			"Con questo maleficio rimuovi l'aria attorno al bersaglio tramite una nube di veleno. Il bersaglio subisce DoT.",
			0x5010
			);

		public override ExtendedSpellInfo ExtendedInfo{get { return m_ExtendedInfo; }}

		public override TimeSpan CastDelayBase{get { return TimeSpan.FromSeconds( 2.0 ); }}
		public override double RequiredSkill{get { return 65.0; }}
		public override int RequiredMana{get { return 29; }}
		public override double DelayOfReuse{get { return 5.0; }}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		public void Target( Mobile m )
		{
			if( CheckHSequence( m ) )
			{
				SpellHelper.Turn( Caster, m );

				SpellHelper.CheckReflect( 3, Caster, ref m );

				if( HandleSelfMagicalAbsorption( m ) )
					return;

				/* Temporarily chokes off the air suply of the target with poisonous fumes.
				 * The target is inflicted with poison damage over time.
				 * The amount of damage dealt each "hit" is based off of the caster's Spirit Speak skill and the Target's current Stamina.
				 * The less Stamina the target has, the more damage is done by Strangle.
				 * Duration of the effect is Spirit Speak skill level / 10 rounds, with a minimum number of 4 rounds.
				 * The first round of damage is dealt after 5 seconds, and every next round after that comes 1 second sooner than the one before, until there is only 1 second between rounds.
				 * The base damage of the effect lies between (Spirit Speak skill level / 10) - 2 and (Spirit Speak skill level / 10) + 1.
				 * Base damage is multiplied by the following formula: (3 - (target's current Stamina / target's maximum Stamina) * 2).
				 * Example:
				 * For a target at full Stamina the damage multiplier is 1,
				 * for a target at 50% Stamina the damage multiplier is 2 and
				 * for a target at 20% Stamina the damage multiplier is 2.6
				 */

				m.PlaySound( 0x22F );
				m.FixedParticles( 0x36CB, 1, 9, 9911, 67, 5, EffectLayer.Head );
				m.FixedParticles( 0x374A, 1, 17, 9502, 1108, 4, (EffectLayer)255 );

				if( !m_Table.Contains( m ) )
				{
					int damage = 10;
					if ( CheckResisted( m ) )
					{
						damage = 5;

						m.SendLocalizedMessage( 501783 ); // You feel yourself resisting magical energy.
					}

					Timer t = new InternalTimer( m, Caster, damage );
					t.Start();

					m_Table[ m ] = t;
				}
			}

			FinishSequence();
		}

		public static bool RemoveCurse( Mobile m )
		{
			Timer t = (Timer)m_Table[ m ];

			if( t == null )
				return false;

			t.Stop();
			m.SendLocalizedMessage( 1061687 ); // You can breath normally again.

			m_Table.Remove( m );
			return true;
		}

		private class InternalTarget : Target
		{
			private readonly ChokingSpell m_Owner;

			public InternalTarget( ChokingSpell owner ) : base( 13, false, TargetFlags.Harmful )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if( o is Mobile )
					m_Owner.Target( (Mobile)o );
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}

		private class InternalTimer : Timer
		{
			private int m_Count;
			private readonly Mobile m_From;
			private int m_HitDelay;
			//private readonly double m_MaxBaseDamage;

			private readonly int m_MaxCount;
			private readonly double m_MinBaseDamage;
			private DateTime m_NextHit;
			private readonly Mobile m_Target;

			public InternalTimer( Mobile target, Mobile from, int damage ) : base( TimeSpan.FromSeconds( 0.1 ), TimeSpan.FromSeconds( 0.1 ) )
			{
				Priority = TimerPriority.FiftyMS;

				m_Target = target;
				m_From = from;

				double level = RPGSpellsSystem.GetPowerLevel( m_From, typeof( ChokingSpell ) );

				m_MinBaseDamage = damage;//level - 2;
				//m_MaxBaseDamage = level + 1;

				m_HitDelay = 5;
				m_NextHit = DateTime.Now + TimeSpan.FromSeconds( m_HitDelay );

				m_Count = (int)level + 1;

				if( m_Count < 2 )
					m_Count = 2;

				m_MaxCount = m_Count;
			}

			protected override void OnTick()
			{
				if( !m_Target.Alive )
				{
					m_Table.Remove( m_Target );
					Stop();
				}

				if( !m_Target.Alive || DateTime.Now < m_NextHit )
					return;

				--m_Count;

				if( m_HitDelay > 1 )
				{
					if( m_MaxCount < 5 )
					{
						--m_HitDelay;
					}
					else
					{
						int delay = (int)( Math.Ceiling( ( 1.0 + ( 5 * m_Count ) ) / m_MaxCount ) );

						m_HitDelay = delay <= 5 ? delay : 5;
					}
				}

				if( m_Count == 0 )
				{
					m_Target.SendLocalizedMessage( 1061687 ); // You can breath normally again.
					m_Table.Remove( m_Target );
					Stop();
				}
				else
				{
					m_NextHit = DateTime.Now + TimeSpan.FromSeconds( m_HitDelay );

					double damage = m_MinBaseDamage;// + ( Utility.RandomDouble() * ( m_MaxBaseDamage - m_MinBaseDamage ) );

					damage *= ( 3 - ( ( (double)m_Target.Stam / m_Target.StamMax ) * 2 ) );

					if( damage < 1 )
						damage = 1;

					if( !m_Target.Player )
						damage *= 1.75;

					MidgardSpellHelper.Damage( m_Target, m_From, (int)damage, CustomResType.Venom );
				}
			}
		}
	}
}