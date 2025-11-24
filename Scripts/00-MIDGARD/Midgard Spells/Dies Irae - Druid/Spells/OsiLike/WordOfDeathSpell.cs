/***************************************************************************
 *							   WordOfDeath.cs
 *
 *   begin				: 27 September, 2009
 *   author			   :	Dies Irae	
 *   email				: tocasia@alice.it
 *   copyright			: (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;

using Server;
using Server.Spells;
using Server.Targeting;

namespace Midgard.Engines.SpellSystem
{
	public class WordOfDeathSpell : DruidSpell
	{
		private static readonly SpellInfo m_Info = new SpellInfo(
			"Word of Death", "Tia Kes En Telwa",
			224,
			9011,
			Reagent.FertileDirt,
			Reagent.PetrifiedWood,
			Reagent.GraveDust
			);

		private static readonly ExtendedSpellInfo m_ExtendedInfo = new ExtendedSpellInfo
			(
				typeof( WordOfDeathSpell ),
				"Does massive damage to creatures low in health.",
				"Questo arteficio finisce qualsiasi nemico gravemente ferito.",
				0x59e5
			);

		public override ExtendedSpellInfo ExtendedInfo { get { return m_ExtendedInfo; } }

		public override SpellCircle Circle{get { return SpellCircle.Sixth; }}

		public WordOfDeathSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast(){Caster.Target = new InternalTarget( this );}

		public void Target( Mobile m )
		{
			if( !Caster.CanSee( m ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if( CheckHSequence( m ) )
			{
				Point3D loc = m.Location;
				loc.Z += 50;

				m.PlaySound( 0x211 );
				m.FixedParticles( 0x3779, 1, 30, 0x26EC, 0x3, 0x3, EffectLayer.Waist );

				Effects.SendMovingParticles( 
					new Entity( Serial.Zero, loc, m.Map ), 
					new Entity( Serial.Zero, m.Location, m.Map ), 0xF5F, 1, 0, true, false, 0x21, 0x3F, 0x251D, 0, 0, EffectLayer.Head, 0 );

				int powerLevel = GetPowerLevel();

				if( ( (double)m.Hits / (double)m.HitsMax ) < 0.05 + ( 0.05 * powerLevel ) )
				{
					int damage = 0;
					damage = (int)(( m.HitsMax * 0.1 ) + FocusLevel);//GetNewAosDamage( (int)Math.Max( Caster.Skills.Spellweaving.Value / 24, 1 ) + 4, powerLevel, 4, m );
					if (!m.Player)
						damage *=2;
					MidgardSpellHelper.Damage( m, Caster, damage, CustomResType.Mental );
				}
				else
					Caster.SendMessage( Caster.Language == "ITA" ? "Non è stato indebolito abbastanza!" : "It has enough life to resist you!" );
			}

			FinishSequence();
		}

		public class InternalTarget : Target
		{
			private readonly WordOfDeathSpell m_Owner;

			public InternalTarget( WordOfDeathSpell owner ) : base( 10, false, TargetFlags.Harmful )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile m, object o )
			{
				if( o is Mobile )
				{
					m_Owner.Target( (Mobile)o );
				}
			}

			protected override void OnTargetFinish( Mobile m )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}