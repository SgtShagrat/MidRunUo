/***************************************************************************
 *							   Thunderstorm.cs
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
using Server.Spells;
using Server.Targeting;

namespace Midgard.Engines.SpellSystem
{
	public class ThunderstormSpell : DruidSpell
	{
		private static readonly SpellInfo m_Info = new SpellInfo(
			"Thunderstorm", "Kes Sec Tia",
			224,
			9011,
			Reagent.Kindling,
			Reagent.DestroyingAngel,
			Reagent.SpringWater
			);

		private static readonly ExtendedSpellInfo m_ExtendedInfo = new ExtendedSpellInfo
			(
				typeof( ThunderstormSpell ),
				"Thunderstorm",
				"Blasts a devasting rumor. Each creature in range is stunned.",
				"Un fulmine scatenato dallo scontro fra l'elemento aereo e quello terrestre colpisce tutti i nemici nell'area intorno al Druido.",
				"Il druido casta su un punto uno sciame di fulmini. Il raggio è (2+FL). Il danno è 6d8+6 diviso tra i bersagli. Il danno e' elettrico." +
				"In più provoca un anticast che dura (skill/20 + PL).",
				0x59dc
			);

		public override ExtendedSpellInfo ExtendedInfo{get { return m_ExtendedInfo; }}

		public override SpellCircle Circle{get { return SpellCircle.Fourth; }}

		public ThunderstormSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		private static readonly DiceRoll DamageDice = new DiceRoll( "6d8+6" );
		private const int MinDamageRange = 2;

		public void Target( IPoint3D p )
		{
			if( !Caster.CanSee( p ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if( !Caster.InRange( p, 12 ) )
			{
				Caster.SendLocalizedMessage( 1060178 ); // You are too far away to perform that action!
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
					int range = MinDamageRange + FocusLevel;

					IPooledEnumerable eable = map.GetMobilesInRange( new Point3D( p ), range );

					foreach( Mobile m in eable )
					{
						if( m == Caster )
							continue;

						if( SpellHelper.ValidIndirectTarget( Caster, m, true, false, false ) && Caster.CanBeHarmful( m, false ) )
						{
							if( !Caster.InLOS( m ) )
								continue;

							targets.Add( m );
						}
					}

					eable.Free();
				}

				TimeSpan duration = TimeSpan.FromSeconds( Caster.Skills[ CastSkill ].Value / 20 + FocusLevel );
 
				if( targets.Count > 0 )
				{
					int damage = DamageDice.Roll() + ( 2 * FocusLevel );
					
					foreach( Mobile m in targets )
					{
						double toDeal = damage;

						if( CheckResisted( m ) )
						{
							toDeal *= 0.5;
							m.SendLocalizedMessage( 501783 ); // You feel yourself resisting magical energy.
						}
						else
							m_Table[ m ] = Timer.DelayCall( duration, DoExpire, m );

						Caster.DoHarmful( m );

						m.BoltEffect( 0 );

						MidgardSpellHelper.Damage( this, m, toDeal, SpellType.Electric );

					}
				}
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private readonly ThunderstormSpell m_Owner;

			public InternalTarget( ThunderstormSpell owner ) : base( 12, false, TargetFlags.None )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if( o != null && o is Mobile && o == m_Owner.Caster )
				{
					m_Owner.Caster.SendMessage( (m_Owner.Caster.Language == "ITA" ? "Non puoi farlo su te stesso." : "Thou cannot target yourself.") );
					return;
				}

				if( o is Mobile )
					m_Owner.Target( new Point3D( (IPoint3D)o ) );
				else if( m_Owner.Caster != null )
					m_Owner.Caster.SendMessage( (m_Owner.Caster.Language == "ITA" ? "Devi selezionare un essere vivente." : "Thou must target a valid living target.") );
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}

		private static readonly Dictionary<Mobile, Timer> m_Table = new Dictionary<Mobile, Timer>();

		public static bool IsUnderEffect( Mobile m )
		{
			return m_Table.ContainsKey( m );
		}

		private static void DoExpire( Mobile m )
		{
			Timer t;

			if( m_Table.TryGetValue( m, out t ) )
			{
				t.Stop();
				m_Table.Remove( m );
			}
		}
	}
}
