using System;

using Midgard;
using Midgard.Engines.SpellSystem;

using Server.Targeting;
using Server.Network;

namespace Server.Spells.Sixth
{
	public class EnergyBoltSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Energy Bolt", "Corp Por",
				230,
				9022,
				Reagent.BlackPearl,
				Reagent.Nightshade
			);

		public override SpellCircle Circle { get { return SpellCircle.Sixth; } }

		public EnergyBoltSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		public override bool DelayedDamage{ get{ return true; } }

		public void Target( Mobile m )
		{
			if ( !Caster.CanSee( m ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if ( CheckHSequence( m ) )
			{
				Mobile source = Caster;

				SpellHelper.Turn( Caster, m );

				SpellHelper.CheckReflect( (int)this.Circle, ref source, ref m );

				#region mod by Dies Irae
				if( HandleSelfMagicalAbsorption( m ) )
					return;
				#endregion

				double damage;

				if ( Core.AOS )
				{
					damage = GetNewAosDamage( 40, 1, 5, m );
				}
				else
				{
					//var dmg :=Randomdiceroll("5d8")+(cint(magery/4));
					damage = DiceRoll.Roll( new DiceRoll( 5, 8, (int) ( Caster.Skills[ SkillName.Magery ].Value / 4.0 ) ) );//GetBaseDamage(); // Utility.Random( 32, 8 ); // 24, 18 ); // mod by Dies Irae

					// Scale damage based on evalint and resist
					if ( Caster.Player && !m.Player )
						damage *= GetDamageScalar( m );

					if ( CheckResisted( m ) )
					{
						damage *= ( 1.0 - GetResistScalar( m ) );//damage *= 0.75;

						m.SendLocalizedMessage( 501783 ); // You feel yourself resisting magical energy.
					}
				}

				// Do the effects
				source.MovingParticles( m, 0x379F, 7, 0, false, true, 3043, 4043, 0x211 );
				source.PlaySound( 0x20A );

				// Deal the damage
				if( Core.AOS )
					SpellHelper.Damage( this, m, damage, 0, 0, 0, 0, 100 );
				else
					MidgardSpellHelper.Damage( this, m, damage, SpellType.Electric ); // mod by Dies Irae
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private EnergyBoltSpell m_Owner;

			public InternalTarget( EnergyBoltSpell owner ) : base( Core.ML ? 10 : 12, false, TargetFlags.Harmful )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if ( o is Mobile )
					m_Owner.Target( (Mobile)o );
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}