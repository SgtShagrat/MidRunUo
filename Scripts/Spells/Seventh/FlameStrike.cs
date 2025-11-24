using System;

using Midgard;
using Midgard.Engines.SpellSystem;

using Server.Mobiles;
using Server.Targeting;
using Server.Network;

namespace Server.Spells.Seventh
{
	public class FlameStrikeSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Flame Strike", "Kal Vas Flam",
				245,
				9042,
				Reagent.SpidersSilk,
				Reagent.SulfurousAsh
			);

		public override SpellCircle Circle { get { return SpellCircle.Seventh; } }

		public override double TimeoutDelay { get { return 10.0; } } // mod by Dies Irae

		public FlameStrikeSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			#region mod by Dies Irae [Strange Effects]
			if( HandleChangeTarget( Target ) )
				return;
			#endregion

			Caster.Target = new InternalTarget( this );
		}

		public override bool DelayedDamage{ get{ return true; } }

		private static readonly TimeSpan CoolingDelay = TimeSpan.FromSeconds( 3.0 ); // mod by Dies Irae

		public void Target( Mobile m )
		{
			if ( !Caster.CanSee( m ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if ( CheckHSequence( m ) )
			{
				SpellHelper.Turn( Caster, m );

				SpellHelper.CheckReflect( (int)this.Circle, Caster, ref m );

				#region mod by Dies Irae
				if( HandleSelfMagicalAbsorption( m ) )
					return;
				#endregion

				double damage;

				if ( Core.AOS )
				{
					damage = GetNewAosDamage( 48, 1, 5, m );
				}
				else
				{
					damage = GetBaseDamage(); // Utility.Random( 27, 22 );

					if( OldSchoolRules )
						damage = DiceRoll.Roll( "20d3+30" );

					double baseDamage = damage;
					
					if ( Caster.Player && !m.Player )
						damage *= GetDamageScalar( m );

					if( Caster.PlayerDebug )
						Caster.SendMessage( "Debug: base {0} - second {1}", baseDamage, damage );

					bool recentlyFlamed = ( ( m is Midgard2PlayerMobile ) && ( (Midgard2PlayerMobile)m ).LastFlameStrike > DateTime.Now - CoolingDelay );

					if( recentlyFlamed )
					{
						damage *= 0.30;

						m.SendMessage( m.Language == "ITA"? "Il tuo corpo attutisce il colpo magico." : "Your body attuned the magical energy." );
					}
					else if( CheckResisted( m ) ) // mod by Dies Irae
					{
						// damage *= 0.6;

						damage *= ( 1.0 - GetResistScalar( m ) );//GetResistScalar( m );

						m.SendLocalizedMessage( 501783 ); // You feel yourself resisting magical energy.
					}

					if( Caster.PlayerDebug )
						Caster.SendMessage( "Debug: base {0} - final {1}; Scalar {2}", baseDamage, damage, GetResistScalar( m ) );
				}

				m.FixedParticles( 0x3709, 10, 30, 5052, EffectLayer.LeftFoot );
				m.PlaySound( 0x208 );

				if( Core.AOS )
					SpellHelper.Damage( this, m, damage, 0, 100, 0, 0, 0 );
				else
					MidgardSpellHelper.Damage( this, m, damage, SpellType.Fire ); // mod by Dies Irae

				#region mod by Dies Irae
				if( ( m is Midgard2PlayerMobile ) )
					( (Midgard2PlayerMobile)m ).LastFlameStrike = DateTime.Now;
				#endregion
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private FlameStrikeSpell m_Owner;

			public InternalTarget( FlameStrikeSpell owner ) : base( Core.ML ? 10 : 12, false, TargetFlags.Harmful )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if ( o is Mobile )
				{
					m_Owner.Target( (Mobile)o );
				}
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}