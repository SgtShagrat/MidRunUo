/***************************************************************************
 *							   PainStrikeSpell.cs
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
using Server.Misc;
using Server.Spells;
using Server.Targeting;

namespace Midgard.Engines.SpellSystem
{
	public class PainStrikeSpell : RPGNecromancerSpell
	{
		private static readonly SpellInfo m_Info = new SpellInfo(
			"Pain Strike", "In Sar",
			203,
			9031,
			Reagent.GraveDust,
			Reagent.PigIron
			);

		//private static readonly Hashtable m_Table = new Hashtable();

		public PainStrikeSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		private static readonly ExtendedSpellInfo m_ExtendedInfo = new ExtendedSpellInfo
			(
				typeof( PainStrikeSpell ),
				"Gives direct damage to Target base on level.",
				"Con questo potente maleficio infliggi dolore istantaneo alle tue vittime",
				0x500E
			);

		public override ExtendedSpellInfo ExtendedInfo { get { return m_ExtendedInfo; } }

		public override TimeSpan CastDelayBase{get { return TimeSpan.FromSeconds( 2.5 ); }}
		public override double RequiredSkill{get { return 20.0; }}
		public override int RequiredMana{get { return 5; }}
		public override double DelayOfReuse{get { return 5.0; }}
		public override bool DelayedDamage{get { return false; }}

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

				/* Temporarily causes intense physical pain to the target, dealing direct damage.
							 * After 10 seconds the spell wears off, and if the target is still alive, 
							 * some of the Hit Points lost through Pain Spike are restored.
							 */

				m.FixedParticles( 0x37C4, 1, 8, 9916, 39, 3, EffectLayer.Head );
				m.FixedParticles( 0x37C4, 1, 8, 9502, 39, 4, EffectLayer.Head );
				m.PlaySound( 0x210 );

				double damage = ( m.Player ? 6 : 12 )*GetPowerLevel() + Utility.Random(11); //( ( ( GetPowerLevel() * 20 ) - GetResistSkill( m ) ) / 10 ) + ( m.Player ? 18 : 30 );
				//double damage = DiceRoll.Roll( new DiceRoll( (int) ( Caster.Skills[ SkillName.Magery ].Value / 10.0 ), 5, 0 ) );
				//var dmg :=Randomdiceroll("5d8")+(cint(magery/4));
				//double damage = DiceRoll.Roll( new DiceRoll( 5, GetPowerLevel()+3, (int) ( Caster.Skills[ SkillName.Necromancy ].Value / 4.0 ) ) );

				if ( CheckResisted( m ) )
				{
					damage *= ( 1.0 - GetResistScalar( m ) );//0.75;

					m.SendLocalizedMessage( 501783 ); // You feel yourself resisting magical energy.
				}

				if( damage < 1 )
					damage = 1;

				m.Damage( (int)damage, Caster );

				SpellHelper.DoLeech( (int)damage, Caster, m );

				//WeightOverloading.DFA = DFAlgorithm.Standard;
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private readonly PainStrikeSpell m_Owner;

			public InternalTarget( PainStrikeSpell owner ) : base( 13, false, TargetFlags.Harmful )
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
	}
}