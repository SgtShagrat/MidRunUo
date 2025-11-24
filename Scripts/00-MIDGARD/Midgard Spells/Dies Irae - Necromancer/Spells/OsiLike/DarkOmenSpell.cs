/***************************************************************************
 *							   DarkOmenSpell.cs
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
	public class DarkOmenSpell : RPGNecromancerSpell
	{
		private static readonly SpellInfo m_Info = new SpellInfo(
			"Dark Omen", "Pas Tym An Sanct",
			203,
			9031,
			Reagent.BatWing,
			Reagent.NoxCrystal
			);

		private static readonly Hashtable m_Table = new Hashtable();

		public DarkOmenSpell( Mobile Caster, Item scroll ) : base( Caster, scroll, m_Info )
		{
		}

		private static readonly ExtendedSpellInfo m_ExtendedInfo = new ExtendedSpellInfo
			(
			typeof( DarkOmenSpell ),
			"Cures the target from poison. Levels add targets and small bless bonus.",
			"Cura il bersaglio dal veleno. Livelli ulteriori aggiungono bersagli ulteriori e piccoli bless.",
			0x500B
			);

		public override ExtendedSpellInfo ExtendedInfo{get { return m_ExtendedInfo; }}

		public override TimeSpan CastDelayBase{get { return TimeSpan.FromSeconds( 0.75 ); }}
		public override double RequiredSkill{get { return 20.0; }}
		public override int RequiredMana{get { return 10; }}
		public override double DelayOfReuse{get { return 5.0; }}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		public void Target( Mobile m )
		{
			if ( !Caster.CanSee( m ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else
			{
				SpellHelper.Turn( Caster, m );

				Poison p = m.Poison;

				if ( p != null )
				{
					int chanceToCure = 10000 + (int)( Caster.Skills[ SkillName.Necromancy ].Value * 75 ) - ( p.RealLevel + 1 ) * 1750;

					chanceToCure /= 100;

					if ( chanceToCure > Utility.Random( 100 ) )
					{
						if ( m.CurePoison( Caster ) )
						{
							if ( Caster != m )
								Caster.SendLocalizedMessage( 1010058 ); // You have cured the target of all poisons!

							m.SendLocalizedMessage( 1010059 ); // You have been cured of all poisons.
						}
					}
					else
					{
						Caster.SendLocalizedMessage( 1010060 ); // You have failed to cure your target!
					}
				}

				SpellHelper.AddStatBonus( Caster, m, StatType.Str, 1 + 2*GetPowerLevel(), TimeSpan.FromSeconds( Caster.Skills[SkillName.Necromancy].Value * 1.2 ) ); SpellHelper.DisableSkillCheck = true;
				SpellHelper.AddStatBonus( Caster, m, StatType.Dex, 1 + 2*GetPowerLevel(), TimeSpan.FromSeconds( Caster.Skills[SkillName.Necromancy].Value * 1.2 ) );
				SpellHelper.AddStatBonus( Caster, m, StatType.Int, 1 + 2*GetPowerLevel(), TimeSpan.FromSeconds( Caster.Skills[SkillName.Necromancy].Value * 1.2 ) ); SpellHelper.DisableSkillCheck = false;

				m.FixedParticles( 0x373A, 10, 15, 5012, EffectLayer.Waist );
				m.PlaySound( 0x1E0 );
			}

			FinishSequence();
		}

		public class InternalTarget : Target
		{
			private DarkOmenSpell m_Owner;

			public InternalTarget( DarkOmenSpell owner ) : base( Core.ML ? 10 : 12, false, TargetFlags.Beneficial )
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