/***************************************************************************
 *							   BloodConjunctionSpell.cs
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
using Server.Mobiles;
using Server.Spells;
using Server.Targeting;

namespace Midgard.Engines.SpellSystem
{
	public class BloodConjunctionSpell : RPGNecromancerSpell
	{
		private static readonly SpellInfo m_Info = new SpellInfo(
			"Blood Conjunction", "In Jux Mani Xen",
			203,
			9031,
			Reagent.DaemonBlood
			);

		private static readonly Hashtable m_OathTable = new Hashtable();
		private static readonly Hashtable m_Table = new Hashtable();

		public BloodConjunctionSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		private static readonly ExtendedSpellInfo m_ExtendedInfo = new ExtendedSpellInfo
			(
				typeof( BloodConjunctionSpell ),
				"Temporarily creates a dark pact between the caster and the target. Any damage dealt by the target to the caster is increased, but the target receives the same amount of damage.",
				"Questo maleficio unisce il caster e il bersaglio. Ogni danno inferto dal bersaglio al caster è aumentato, ma il bersaglio riceve lo stesso danno.",
				0x500A
			);

		public override ExtendedSpellInfo ExtendedInfo { get { return m_ExtendedInfo; } }

		public override TimeSpan CastDelayBase{get { return TimeSpan.FromSeconds( 1.5 ); }}
		public override double RequiredSkill{get { return 20.0; }}
		public override int RequiredMana{get { return 13; }}
		public override double DelayOfReuse{get { return 5.0; }}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		public void Target( Mobile m )
		{
			if( Caster == m || !( m is PlayerMobile || m is BaseCreature ) ) // only PlayerMobile and BaseCreature implement blood oath checking
			{
				Caster.SendLocalizedMessage( 1060508 ); // You can't curse that.
			}
			else if( m_OathTable.Contains( Caster ) )
			{
				Caster.SendLocalizedMessage( 1061607 ); // You are already bonded in a Blood Oath.
			}
			else if( m_OathTable.Contains( m ) )
			{
				if( m.Player )
					Caster.SendLocalizedMessage( 1061608 ); // That player is already bonded in a Blood Oath.
				else
					Caster.SendLocalizedMessage( 1061609 ); // That creature is already bonded in a Blood Oath.
			}
			else if( CheckHSequence( m ) )
			{
				SpellHelper.Turn( Caster, m );

				/* Temporarily creates a dark pact between the caster and the target.
				 * Any damage dealt by the target to the caster is increased, but the target receives the same amount of damage.
				 * The effect lasts for ((Spirit Speak skill level - target's Resist Magic skill level) / 80 ) + 8 seconds.
				 * 
				 * NOTE: The above algorithm must be fixed point, it should be:
				 * ((ss-rm)/8)+8
				 */

				ExpireTimer timer = (ExpireTimer)m_Table[ m ];
				if( timer != null )
					timer.DoExpire();

				m_OathTable[ Caster ] = Caster;
				m_OathTable[ m ] = Caster;

				Caster.PlaySound( 0x175 );

				Caster.FixedParticles( 0x375A, 1, 17, 9919, 33, 7, EffectLayer.Waist );
				Caster.FixedParticles( 0x3728, 1, 13, 9502, 33, 7, (EffectLayer)255 );

				m.FixedParticles( 0x375A, 1, 17, 9919, 33, 7, EffectLayer.Waist );
				m.FixedParticles( 0x3728, 1, 13, 9502, 33, 7, (EffectLayer)255 );

				double duration = ( (Caster.Skills[SkillName.SpiritSpeak].Value - m.Skills[SkillName.MagicResist].Value ) / 8 ) + 10 + GetPowerLevel();

				if( m.Player )
					duration = MidgardSpellHelper.ScaleByCustomRes( duration, m, CustomResType.General );

				if ( CheckResisted( m ) )
				{
					duration *= ( 1.0 - GetResistScalar( m ) );//0.75;

					m.SendLocalizedMessage( 501783 ); // You feel yourself resisting magical energy.
				}

				timer = new ExpireTimer( Caster, m, TimeSpan.FromSeconds( duration ) );
				timer.Start();

				BuffInfo.AddBuff( Caster, new BuffInfo( BuffIcon.BloodOathCaster, 1075659, TimeSpan.FromSeconds( duration ), Caster, m.Name ) );
				BuffInfo.AddBuff( m, new BuffInfo( BuffIcon.BloodOathCurse, 1075661, TimeSpan.FromSeconds( duration ), m, Caster.Name ) );

				m_Table[ m ] = timer;
			}

			FinishSequence();
		}

		public static bool RemoveCurse( Mobile m )
		{
			ExpireTimer t = (ExpireTimer)m_Table[ m ];

			if( t == null )
				return false;

			t.DoExpire();
			return true;
		}

		public static Mobile GetBloodOath( Mobile m )
		{
			if( m == null )
				return null;

			Mobile oath = (Mobile)m_OathTable[ m ];

			if( oath == m )
				oath = null;

			return oath;
		}

		private class ExpireTimer : Timer
		{
			private readonly Mobile m_Caster;
			private readonly DateTime m_End;
			private readonly Mobile m_Target;

			public ExpireTimer( Mobile caster, Mobile target, TimeSpan delay )
				: base( TimeSpan.FromSeconds( 1.0 ), TimeSpan.FromSeconds( 1.0 ) )
			{
				m_Caster = caster;
				m_Target = target;
				m_End = DateTime.Now + delay;

				Priority = TimerPriority.TwoFiftyMS;
			}

			protected override void OnTick()
			{
				if( m_Caster.Deleted || m_Target.Deleted || !m_Caster.Alive || !m_Target.Alive || DateTime.Now >= m_End )
				{
					DoExpire();
				}
			}

			public void DoExpire()
			{
				if( m_OathTable.Contains( m_Caster ) )
				{
					m_Caster.SendLocalizedMessage( 1061620 ); // Your Blood Oath has been broken.
					m_OathTable.Remove( m_Caster );
				}

				if( m_OathTable.Contains( m_Target ) )
				{
					m_Target.SendLocalizedMessage( 1061620 ); // Your Blood Oath has been broken.
					m_OathTable.Remove( m_Target );
				}

				Stop();

				BuffInfo.RemoveBuff( m_Caster, BuffIcon.BloodOathCaster );
				BuffInfo.RemoveBuff( m_Target, BuffIcon.BloodOathCurse );

				m_Table.Remove( m_Caster );
			}
		}

		private class InternalTarget : Target
		{
			private readonly BloodConjunctionSpell m_Owner;

			public InternalTarget( BloodConjunctionSpell owner )
				: base( 13, false, TargetFlags.Harmful )
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
	}
}