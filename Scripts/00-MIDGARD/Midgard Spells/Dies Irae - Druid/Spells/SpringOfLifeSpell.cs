/***************************************************************************
 *							   SpringOfLifeSpell.cs
 *
 *   begin				: 27 September, 2009
 *   author			   :	Dies Irae	
 *   email				: tocasia@alice.it
 *   copyright			: (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;

using Server;
using Server.Mobiles;
using Server.Spells;
using Server.Engines.PartySystem;

namespace Midgard.Engines.SpellSystem
{
	public class SpringOfLifeSpell : DruidSpell
	{
		private static readonly SpellInfo m_Info = new SpellInfo(
			"Spring Of Life", "En Sepa Aete",
			204,
			9061,
			true,
			Reagent.SpringWater,
			Reagent.Ginseng
			);

		private static readonly ExtendedSpellInfo m_ExtendedInfo = new ExtendedSpellInfo
			(
			typeof( SpringOfLifeSpell ),
			"Creates a magical spring that heals the Druid and their party.",
			"Cura il druido e tutto il suo party."+
			"Raggio (5+FL+PL), HP (DS*0.4 + [1...(FL+PL)*2]).",
			0x4ff
			);

		public override ExtendedSpellInfo ExtendedInfo{get { return m_ExtendedInfo; }}

		public override SpellCircle Circle{get { return SpellCircle.Fourth; }}

		public override bool BlocksMovement{get { return true; }}

		public SpringOfLifeSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			if( CheckSequence() )
			{
				if( Caster.BeginAction( typeof( SpringOfLifeSpell ) ) )
				{
					List<Mobile> targets = new List<Mobile>();
					Map map = Caster.Map;
					int level = GetFocusLevel(Caster) + GetPowerLevel();
					int range = 5 + level;
					int toHeal = ( (int)( Caster.Skills[ DamageSkill ].Value * 0.4 ) ) + Utility.Random( 1, level * 2 );
					Party party = Party.Get( Caster );

					if( map != null )
					{
						IPooledEnumerable eable = map.GetMobilesInRange( Caster.Location, range );

						foreach( Mobile m in eable )
						{//no NatureFury
							/*il lanciatore deve SEMPR essere investito dall'incantesimo*/
							//if( Caster.AllFollowers != null && Caster.AllFollowers.Contains( m ) )
							//	targets.Add( m );
							if (m != Caster)
							{
								if (m is NatureFury || m.Criminal)
									continue;

								int noto = Notoriety.Compute( Caster, m );
								if( noto == Notoriety.Enemy || noto == Notoriety.Murderer || noto == Notoriety.Criminal )
									continue;
							}
							Party partym = Party.Get( m );

							if( m is BaseCreature && ( (BaseCreature)m ).GetMaster() == Caster )
								targets.Add( m );
							else if( Caster == m || Caster.CanBeBeneficial( m, false ) || party == partym )
								targets.Add( m );
						}

						eable.Free();
					}

					Effects.PlaySound( Caster.Location, Caster.Map, 0x11 );

					if( targets.Count > 0 )
					{
						foreach( Mobile m in targets )
						{
							Caster.DoBeneficial( m );
							m.FixedParticles( 0x375A, 9, 20, 5027, EffectLayer.Head );
							m.PlaySound( 0xAF );

							SpellHelper.Heal( toHeal, m, Caster );
						}

						new InternalTimer( targets, Caster, toHeal, range ).Start();
					}
					#region mod by Magius(CHE)
					if( targets.Count == 1 ) /*non ci sono altri bersagli oltre te da curare*/
						Caster.SendMessage( (Caster.Language == "ITA" ? "Non ci sono altri alleati in giro da curare." : "The Spring of Life matches no other suitable allies to heal except you.") );
					#endregion
				}
			}

			FinishSequence();
		}

		private class InternalTimer : Timer
		{
			private readonly List<Mobile> m_Targets = new List<Mobile>();
			private readonly Mobile m_Caster;
			private int m_ToHeal;
			private readonly DateTime m_Expiration;
			private readonly int m_Range;

			public InternalTimer( List<Mobile> targets, Mobile caster, int toHeal, int range )
				: base( TimeSpan.FromSeconds( 3.0 ), TimeSpan.FromSeconds( 3.0 ) )
			{
				m_Targets = targets;
				m_Caster = caster;
				m_ToHeal = toHeal;
				m_Range = range;
				m_Expiration = DateTime.Now + TimeSpan.FromSeconds( 15.0 );
			}

			protected override void OnTick()
			{
				m_ToHeal = m_ToHeal / 2;

				if( m_ToHeal < 5 || DateTime.Now > m_Expiration )
				{
					foreach( Mobile target in m_Targets )
					{
						target.SendMessage( (target.Language == "ITA" ? "L'effetto benefico è finito." : "The Spring of Life effect ends.") );
						m_Caster.EndAction( typeof( SpringOfLifeSpell ) );
					}

					Stop();
				}
				else
				{
					foreach( Mobile target in m_Targets )
					{
						if( target == null || m_Caster == null )
							continue;

						if( target.InLOS( m_Caster ) )
						{
							if( target.GetDistanceToSqrt( m_Caster ) < m_Range )
							{
								m_Caster.DoBeneficial( target );
								target.FixedParticles( 0x375A, 9, 20, 5027, EffectLayer.Head );
								SpellHelper.Heal( m_ToHeal, target, m_Caster );
							}
							else
								target.SendMessage( (target.Language == "ITA" ? "Sei troppo lontano dal Druido!" : "Thou are too far away from the Druid!") );
						}
						else
							target.SendMessage( (target.Language == "ITA" ? "Non riesci a mantenere il contatto visivo con il Druido!" : "Thou cannot see the Druid!") );
					}
				}
			}
		}
	}
}