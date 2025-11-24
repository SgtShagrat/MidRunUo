/***************************************************************************
 *							   PoisonSpikeSpell.cs
 *
 *   begin				: 26 September, 2009
 *   author			   :	Dies Irae	
 *   email				: tocasia@alice.it
 *   copyright			: (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;

using Server;
using Server.Items;
using Server.Spells;
using Server.Targeting;

namespace Midgard.Engines.SpellSystem
{
	public class PoisonSpikeSpell : RPGNecromancerSpell
	{
		private static readonly SpellInfo m_Info = new SpellInfo(
			"Poison Spike", "In Vas Nox",
			203,
			9031,
			Reagent.NoxCrystal
			);

		public PoisonSpikeSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		private static readonly ExtendedSpellInfo m_ExtendedInfo = new ExtendedSpellInfo
			(
			typeof( PoisonSpikeSpell ),
			"Creates a blast of poisonous energy centered on the target.",
			"Crea un'esplosione velenosa attorno al bersaglio.",
			0x500F
			);

		public override ExtendedSpellInfo ExtendedInfo{get { return m_ExtendedInfo; }}

		public override TimeSpan CastDelayBase{get { return TimeSpan.FromSeconds( 1.75 ); }}
		public override double RequiredSkill{get { return 50.0; }}
		public override int RequiredMana{get { return 17; }}
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

				/* Creates a blast of poisonous energy centered on the target.
				* The main target is inflicted with a large amount of Poison damage, and all valid targets in a radius of 2 tiles around the main target are inflicted with a lesser effect.
				* One tile from main target receives 50% damage, two tiles from target receives 33% damage.
				*/

				Effects.SendLocationParticles( EffectItem.Create( m.Location, m.Map, EffectItem.DefaultDuration ), 0x36B0, 1, 14, 63, 7, 9915, 0 );
				Effects.PlaySound( m.Location, m.Map, 0x229 );

				Map map = m.Map;

				if( map != null )
				{
					List<Mobile> targets = new List<Mobile>();

					foreach( Mobile targ in m.GetMobilesInRange( GetPowerLevel() ) )
					{
						if( ( Caster == targ || m == targ || SpellHelper.ValidIndirectTarget( Caster, targ ) ) && Caster.CanBeHarmful( targ, false ) )
						{
							if( !Caster.InLOS( m ) )
								continue;

							targets.Add( targ );
						}
					}

					for( int i = 0; i < targets.Count; ++i )
					{
						//int rawLevel = ( GetPowerLevel() + 1 ) * 200;
						int spelllevel = GetPowerLevel();
						int rangeOffset = Caster.Skills[ CastSkill ].Fixed / 600;
						int level = (spelllevel > 3 ? 3 : spelllevel);//4 lethal

						if( !Caster.InRange( targets[ i ], 2 + rangeOffset ) )
						{
							level--;
						}

						if ( !targets[i].Player )
							level++;
						//Magia Minore	19 //0 + 19
						//Stanchezza Minore // 0 + 24

						if ( spelllevel > 3 )
						{
							level += ( (Utility.Random(2))*(19 + (Utility.Random(spelllevel-3))*5) );
						}

						level = MidgardSpellHelper.ScaleByCustomRes( level, targets[ i ], CustomResType.Venom );

						targets[ i ].ApplyPoison( Caster, Poison.GetPoison( level ) );

						Caster.DoHarmful( targets[ i ] );
					}
				}
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private readonly PoisonSpikeSpell m_Owner;

			public InternalTarget( PoisonSpikeSpell owner ) : base( 13, false, TargetFlags.Harmful )
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