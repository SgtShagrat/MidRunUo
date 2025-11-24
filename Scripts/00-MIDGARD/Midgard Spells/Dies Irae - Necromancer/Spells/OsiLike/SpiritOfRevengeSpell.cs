/***************************************************************************
 *							   SpiritOfRevengeSpell.cs
 * 
 *   begin				: 27 September, 2009
 *   author			   :	Dies Irae	
 *   email				: tocasia@alice.it
 *   copyright			: (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;

using Server;
using Server.Mobiles;
using Server.Spells;
using Server.Targeting;

namespace Midgard.Engines.SpellSystem
{
	public class SpiritOfRevengeSpell : RPGNecromancerSpell
	{
		private static readonly SpellInfo m_Info = new SpellInfo(
			"Spirit Of Revenge", "Kal Xen Bal Beh",
			203,
			9031,
			Reagent.BatWing,
			Reagent.GraveDust,
			Reagent.PigIron
			);

		public SpiritOfRevengeSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		private static readonly ExtendedSpellInfo m_ExtendedInfo = new ExtendedSpellInfo
			(
			typeof( SpiritOfRevengeSpell ),
			"Summons a Revenant which haunts the target until either the target or the Revenant is dead.",
			"Evoca un Revenant che segue il bersaglio fino alla morte.",
			0x5011
			);

		public override TimeSpan CastDelayBase{get { return TimeSpan.FromSeconds( 2.0 ); }}
		public override ExtendedSpellInfo ExtendedInfo{get { return m_ExtendedInfo; }}
		public override double RequiredSkill{get { return 80.0; }}
		public override int RequiredMana{get { return 41; }}
		public override double DelayOfReuse{get { return 5.0; }}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		public override bool CheckCast()
		{
			if( !base.CheckCast() )
				return false;

			if( ( Caster.Followers + 3 ) > Caster.FollowersMax )
			{
				Caster.SendLocalizedMessage( 1049645 ); // You have too many followers to summon that creature.
				return false;
			}

			return true;
		}

		public void Target( Mobile m )
		{
			if( Caster == m )
			{
				Caster.SendLocalizedMessage( 1061832 ); // You cannot exact vengeance on yourself.
			}
			else if( SpellHelper.CheckTown( m.Location, Caster ) && CheckHSequence( m ) )
			{
				SpellHelper.Turn( Caster, m );

				/* Summons a Revenant which haunts the target until either the target or the Revenant is dead.
				 * Revenants have the ability to track down their targets wherever they may travel.
				 * A Revenant's strength is determined by the Necromancy and Spirit Speak skills of the Caster.
				 * The effect lasts for ((Spirit Speak skill level * 80) / 120) + 10 seconds.
				 */

				double duration = 10 + ( ( ( GetPowerLevel() * 2 ) * 80 ) / 120 );

				if( m.Player )
					duration = MidgardSpellHelper.ScaleByCustomRes( duration, m, CustomResType.General );

				Revenant rev = new Revenant( Caster, m, TimeSpan.FromDays( duration ) );

				if( BaseCreature.Summon( rev, false, Caster, m.Location, 0x81, TimeSpan.FromSeconds( duration + 2.0 ) ) )
					rev.FixedParticles( 0x373A, 1, 15, 9909, EffectLayer.Waist );
			}

			FinishSequence();
		}

		#region Nested type: InternalTarget
		private class InternalTarget : Target
		{
			private readonly SpiritOfRevengeSpell m_Owner;

			public InternalTarget( SpiritOfRevengeSpell owner ) : base( 13, false, TargetFlags.Harmful )
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
		#endregion
	}
}