/***************************************************************************
 *								  BonesThrowSpell.cs
 *									------------------
 *  begin					: Gennaio, 2008
 *  version					: 2.0 **VERSION FOR RUNUO 2.0**
 *  copyright				: Midgard Uo Shard - Matteo Visintin
 *  email					: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 * 			Questo maleficio provoca un danno dovuto a delle ossa lanciate
 * 			dal necromante addosso alla sua vittima.
 * 
 * 			La formula per il danno e' ( SpiritSpeak / 10 ) + 8 punti danno.
 * 
 * 			Se il bersaglio è un paladino il danno e' amplificato di un 
 * 			fattore 1.2.
 * 
 ***************************************************************************/

using System;

using Server;
using Server.Spells;
using Server.Targeting;

namespace Midgard.Engines.SpellSystem
{
	public class BonesThrowSpell : RPGNecromancerSpell
	{
		private static readonly SpellInfo m_Info = new SpellInfo(
			"Bones Throw", "In Ort Sar",
			-1,
			9002,
			true,
			Reagent.PigIron
			);

		private static readonly ExtendedSpellInfo m_ExtendedInfo = new ExtendedSpellInfo
			(
			typeof( BonesThrowSpell ),
			"This curse damage the Necromancer enemy with a pile of bones.",
			"Questo maleficio provoca un danno dovuto a delle ossa lanciate dal necromante addosso alla sua vittima.",
			0x5003
			);

		public override ExtendedSpellInfo ExtendedInfo{get { return m_ExtendedInfo; }}

		public override int RequiredMana{get { return 5; }}
		public override TimeSpan CastDelayBase{get { return TimeSpan.FromSeconds( 0.5 ); }}
		public override double DelayOfReuse{get { return 1.0; }}
		public override double RequiredSkill{get { return 55.0; }}
		public override bool BlocksMovement{get { return false; }}
		public override bool DelayedDamageStacking{get { return !Core.AOS; }}

		private static readonly int PaladinMultiplierPercent = 120;

		public BonesThrowSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
			Caster.SendMessage( (Caster.Language == "ITA" ? "Scegli la tua vittima!" : "Choose your victim!") );
		}

		public override bool DelayedDamage
		{
			get { return true; }
		}

		public void Target( Mobile mobile )
		{
			if( !Caster.CanSee( mobile ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if( CheckHSequence( mobile ) )
			{
				Mobile source = Caster;

				SpellHelper.Turn( source, mobile );
				SpellHelper.CheckReflect( 3, ref source, ref mobile );

				double damage = DiceRoll.Roll( "1d5" ) + ( GetPowerLevel() * 2 );

				if ( CheckResisted( mobile ) )
				{
					damage *= ( 1.0 - GetResistScalar( mobile ) );//0.75;

					mobile.SendLocalizedMessage( 501783 ); // You feel yourself resisting magical energy.
				}

				// Paladins suffer more damage from this spell
				if( IsSuperVulnerable( mobile ) )
					damage = AOS.Scale( (int)damage, PaladinMultiplierPercent );

				if( mobile.Player )
					damage = MidgardSpellHelper.ScaleByCustomRes( damage, mobile, CustomResType.Venom );

				Caster.MovingEffect( mobile, 0x3B6F, 10, 0, false, false, 0, 0 );

				SpellHelper.Damage( this, mobile, damage, 0, 0, 0, 100, 0 );
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private readonly BonesThrowSpell m_Owner;

			public InternalTarget( BonesThrowSpell owner ) : base( 13, false, TargetFlags.Harmful )
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