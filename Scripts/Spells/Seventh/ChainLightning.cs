using System;
using System.Collections.Generic;

using Midgard;
using Midgard.Engines.SpellSystem;

using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Spells.Seventh
{
	public class ChainLightningSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Chain Lightning", "Vas Ort Grav",
				209,
				9022,
				false,
				Reagent.BlackPearl,
				Reagent.Bloodmoss,
				Reagent.MandrakeRoot,
				Reagent.SulfurousAsh
			);

		public override SpellCircle Circle { get { return SpellCircle.Seventh; } }

		public ChainLightningSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		public override bool DelayedDamage{ get{ return true; } }

		public void Target( IPoint3D p )
		{
			if ( !Caster.CanSee( p ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if ( SpellHelper.CheckTown( p, Caster ) && CheckSequence() )
			{
				SpellHelper.Turn( Caster, p );

				if ( p is Item )
					p = ((Item)p).GetWorldLocation();

				List<Mobile> targets = new List<Mobile>();

				Map map = Caster.Map;

				bool playerVsPlayer = false;
				//range := cint(magery/15);
				int range = (int) ( Caster.Skills[ SkillName.Magery ].Value / 15.0);

				if ( map != null )
				{
					IPooledEnumerable eable = map.GetMobilesInRange( new Point3D( p ), range );

					foreach ( Mobile m in eable )
					{
						if ( Core.AOS && m == Caster )
							continue;

						//if ( SpellHelper.ValidIndirectTarget( Caster, m ) && Caster.CanBeHarmful( m, false ) )
						//{
						    if ( !Caster.InLOS( m ) ) // mod by Dies Irae
								continue;

							targets.Add( m );

							if ( m.Player )
								playerVsPlayer = true;
						//}
					}

					eable.Free();
				}

				double damage;
				//dmg :=Randomdiceroll("7d6+15")-cint((distance(caster,mobile)-1)*2);
				//al caster dmg :=Randomdiceroll("3d5+5");

				if ( Core.AOS )
					damage = GetNewAosDamage( 51, 1, 5, playerVsPlayer );
				else
					damage = DiceRoll.Roll( new DiceRoll( 7, 6, 15 ));//GetBaseDamage(); // Utility.Random( 27, 22 ); // mod by Dies Irae

				if ( targets.Count > 0 )
				{
					//if ( Core.AOS && targets.Count > 2 )
					//	damage = (damage * 2) / targets.Count;
					//else if ( !Core.AOS )
					//	damage /= targets.Count;

					for ( int i = 0; i < targets.Count; ++i )
					{
						Mobile m = targets[i];

						double toDeal = ((m == Caster)? DiceRoll.Roll( new DiceRoll( 3, 5, 5 )) : damage - (Caster.GetDistanceToSqrt(m)*2));

						if ( !Core.AOS && CheckResisted( m ) )
						{
							toDeal *= ( 1.0 - GetResistScalar( m ) );//0.5;

							m.SendLocalizedMessage( 501783 ); // You feel yourself resisting magical energy.
						}

						if ( Caster.CanAreaHarmful( m ) )
							Caster.DoHarmful( m );

						if( Core.AOS )
							SpellHelper.Damage( this, m, toDeal, 0, 0, 0, 0, 100 );
						else
							MidgardSpellHelper.Damage( this, m, toDeal, SpellType.Electric ); // mod by Dies Irae

						m.BoltEffect( 0 );
					}
				}
				else
				{
					Caster.PlaySound ( 0x29 );
				}
			}

			FinishSequence();
		}

		public class InternalTarget : Target
		{
			private ChainLightningSpell m_Owner;

			public InternalTarget( ChainLightningSpell owner ) : base( Core.ML ? 10 : 12, true, TargetFlags.None )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				IPoint3D p = o as IPoint3D;

				if ( p != null )
					m_Owner.Target( p );
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}