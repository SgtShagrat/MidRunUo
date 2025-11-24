using System;

using Midgard;
using Midgard.Engines.SpellSystem;

using Server.Targeting;
using Server.Network;
using System.Collections.Generic;

namespace Server.Spells.Sixth
{
	public class ExplosionSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Explosion", "Vas Ort Flam",
				230,
				9041,
				Reagent.Bloodmoss,
				Reagent.MandrakeRoot
			);

		public override SpellCircle Circle { get { return SpellCircle.Sixth; } }

		public ExplosionSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override bool DelayedDamageStacking { get { return !Core.AOS; } }

		public override void OnCast()
		{
			#region mod by Dies Irae [Strange Effects]
			if( HandleChangeTarget( Target ) )
				return;
			#endregion

			#region mod by Dies Irae
			if( OldSchoolRules )
			{
				Caster.Target = new OldInternalTarget( this );
				return;
			}
			#endregion

			Caster.Target = new InternalTarget( this );
		}

		public override bool DelayedDamage { get { return OldSchoolRules; } }

		public void Target( Mobile m )
		{
			if ( !Caster.CanSee( m ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if ( Caster.CanBeHarmful( m ) && CheckSequence() )
			{
				Mobile attacker = Caster, defender = m;

				SpellHelper.Turn( Caster, m );

				SpellHelper.CheckReflect( (int) this.Circle, Caster, ref m );

				#region mod by Dies Irae
				if( HandleSelfMagicalAbsorption( m ) )
					return;
				#endregion

				InternalTimer t = new InternalTimer( this, attacker, defender, m );
				t.Start();

				attacker.HarmfulCheck( defender ); // mod by Dies Irae
			}

			FinishSequence();
		}

		private class InternalTimer : Timer
		{
			private MagerySpell m_Spell;
			private Mobile m_Target;
			private Mobile m_Attacker, m_Defender;

			public InternalTimer( MagerySpell spell, Mobile attacker, Mobile defender, Mobile target )
				: base( TimeSpan.FromSeconds( Core.AOS ? 3.0 : 2.5 ) )
			{
				m_Spell = spell;
				m_Attacker = attacker;
				m_Defender = defender;
				m_Target = target;

				if ( m_Spell != null )
					m_Spell.StartDelayedDamageContext( attacker, this );

				Priority = TimerPriority.FiftyMS;
			}

			protected override void OnTick()
			{
				if ( m_Attacker.HarmfulCheck( m_Defender ) )
				{
					double damage;

					if ( Core.AOS )
					{
						damage = m_Spell.GetNewAosDamage( 40, 1, 5, m_Defender );
					}
					else
					{//Randomdiceroll("5d8")+(cint(magery/5));
						damage = DiceRoll.Roll( new DiceRoll( 5, 8, (int) ( m_Attacker.Skills[ SkillName.Magery ].Value / 5.0 ) ) );//m_Spell.GetBaseDamage(); // Utility.Random( 32, 8 ); // 23, 22 ); // mod by Dies irae

						if ( m_Spell.CheckResisted( m_Target ) )
						{
							damage *= 0.75;

							m_Target.SendLocalizedMessage( 501783 ); // You feel yourself resisting magical energy.
						}

						if ( m_Attacker.Player && !m_Target.Player )
							damage *= m_Spell.GetDamageScalar( m_Target );
					}

					m_Target.FixedParticles( 0x36BD, 20, 10, 5044, EffectLayer.Head );
					m_Target.PlaySound( 0x307 );

					if( Core.AOS )
						SpellHelper.Damage( m_Spell, m_Target, damage, 0, 100, 0, 0, 0 );
					else
						MidgardSpellHelper.Damage( m_Spell, m_Target, damage, SpellType.Fire ); // mod by Dies Irae

					if ( m_Spell != null )
						m_Spell.RemoveDelayedDamageContext( m_Attacker );
				}
			}
		}

		private class InternalTarget : Target
		{
			private ExplosionSpell m_Owner;

			public InternalTarget( ExplosionSpell owner )
				: base( Core.ML ? 10 : 12, false, TargetFlags.Harmful )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if ( o is Mobile )
					m_Owner.Target( (Mobile) o );
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}

		#region mod by Dies Irae
		public void Target( IPoint3D p )
		{
			if( !Caster.CanSee( p ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if( SpellHelper.CheckTown( p, Caster ) && CheckSequence() )
			{
				SpellHelper.Turn( Caster, p );

				if( p is Item )
					p = ( (Item)p ).GetWorldLocation();

				List<Mobile> targets = new List<Mobile>();

				Map map = Caster.Map;

				if( map != null )
				{
					IPooledEnumerable eable = map.GetMobilesInRange( new Point3D( p ), 3 );

					foreach( Mobile m in eable )
					{
						if( m == null || Caster == null )//|| m == Caster )
							continue;

						if( !Caster.InLOS( m ) )//!SpellHelper.ValidIndirectTarget( Caster, m ) || !Caster.CanBeHarmful( m, false ) ||
							continue;

						targets.Add( m );
					}

					eable.Free();
				}

				if( targets.Count > 0 )
				{
					double damage = DiceRoll.Roll( "5d8" ) + (int)( Caster.Skills[ CastSkill ].Value / 10.0 );

					foreach( Mobile m in targets )
					{
						double toDeal = damage;
						toDeal *= GetDamageScalar( m );

						if( CheckResisted( m ) )
						{
							toDeal *= ( 1.0 - GetResistScalar( m ) );//0.5;
							m.SendLocalizedMessage( 501783 ); // You feel yourself resisting magical energy.
						}

						if ( Caster.CanAreaHarmful( m ) )
							Caster.DoHarmful( m );

						MidgardSpellHelper.Damage( this, m, toDeal, SpellType.Fire );

						Effects.PlaySound( m.Location, map, 0x0208 );
						Effects.SendLocationEffect( m.Location, map, 0x36B0, 10 );
					}
				}
			}

			FinishSequence();
		}

		public class OldInternalTarget : Target
		{
			private readonly ExplosionSpell m_Owner;

			public OldInternalTarget( ExplosionSpell owner ) : base( 12, true, TargetFlags.None )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				IPoint3D p = o as IPoint3D;

				if( p != null )
					m_Owner.Target( p );
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
		#endregion
	}
}