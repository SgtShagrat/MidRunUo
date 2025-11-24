/***************************************************************************
 *								  TimeOfDeathSpell.cs
 *									-------------------
 *  begin					: Gennaio, 2008
 *  version					: 2.0 **VERSION FOR RUNUO 2.0**
 *  copyright				: Midgard Uo Shard - Matteo Visintin
 *  email					: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 * 			Paralizza la creatura bersaglio per:
 * 			( ( Spirit Speak / 10 ) - ( Magic Resist / 10 ) ) + 2 secondi
 * 			evocando un circolo arcano nel luogo della vittima.
 * 			Se il bersaglio è un paladino egli viene danneggiato di 1d10+6 danni.
 * 			
 ***************************************************************************/

using System;
using System.Collections.Generic;

using Midgard.Engines.Classes;

using Server;
using Server.Items;
using Server.Spells;
using Server.Targeting;

namespace Midgard.Engines.SpellSystem
{
	public class TimeOfDeathSpell : RPGNecromancerSpell
	{
		private static readonly SpellInfo m_Info = new SpellInfo(
			"Time of Death", "An Ex Corp Tym",
			-1,
			9002,
			false,
			Reagent.DaemonBlood,
			Reagent.GraveDust
			);

		private static readonly ExtendedSpellInfo m_ExtendedInfo = new ExtendedSpellInfo
			(
			typeof( TimeOfDeathSpell ),
			"This curse paralize the foe in an evil circle of power.",
			"Paralizza la creatura bersaglio in un circolo malvagio.",
			0x5007
			);

		public override SpellCircle Circle{get { return SpellCircle.First; }}

		public override ExtendedSpellInfo ExtendedInfo{get { return m_ExtendedInfo; }}
		public override int RequiredMana{get { return 15; }}
		public override TimeSpan CastDelayBase{get { return TimeSpan.FromSeconds( 1.0 ); }}
		public override double DelayOfReuse{get { return 10.0; }}

		public override double RequiredSkill{get { return 65.0; }}
		public override bool BlocksMovement{get { return true; }}

		public TimeOfDeathSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			if( Caster.CanBeginAction( typeof( TimeOfDeathSpell ) ) )
			{
				Caster.Target = new InternalTarget( this );
				Caster.SendMessage( Caster.Language == "ITA" ? "Scegli la creatura da maledire..." : "Choose the creature you wish to curse..." );
			}
		}

		public void Target( Mobile m )
		{
			if( !Caster.CanSee( m ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if( m.Frozen || m.Paralyzed || ( m.Spell != null && m.Spell.IsCasting ) )
			{
				Caster.SendLocalizedMessage( 1061923 ); // The target is already frozen.
			}
			else if( CheckHSequence( m ) )
			{
				Caster.BeginAction( typeof( TimeOfDeathSpell ) );

				SpellHelper.Turn( Caster, m );
				SpellHelper.CheckReflect( 5, Caster, ref m );

				if( HandleSelfMagicalAbsorption( m ) )
					return;

				double duration = (int)( ( ( GetPowerLevel() * 30 ) / 10.0 ) - ( GetResistSkill( m ) / 10 ) ) + 2;

				if( !m.Player )
					duration *= 3;

				if( duration < 2.0 )
					duration = 2.0;

				if( m.Map != Map.Internal )
					BuildArcaneCircle( m.Location, m.Map );

				if( ClassSystem.IsPaladine( m ) )
				{
					m.Damage( Utility.Dice( 1, 10, 6 ) );
					m.FixedEffect( 0x37C4, 1, 12, 1109, 3 );
					m.SendMessage( m.Language == "ITA" ? "Senti la vita scorrere via!" : "You feel your life drained away!" );
				}

				if( m.Player )
					duration = MidgardSpellHelper.ScaleByCustomRes( duration, m, CustomResType.General );

				TimeSpan durationSpan = TimeSpan.FromSeconds( duration );

				m.Paralyze( durationSpan );
				m.SendMessage( "You were cursed by {0} and now suffer his anger!", Caster.Name );

				Timer.DelayCall( durationSpan, new TimerCallback( RemoveArcaneCircle ) );
				Timer.DelayCall( GetDelayOfReuse(), new TimerStateCallback( ReleaseTimeOfDeathLock ), Caster );
			}

			FinishSequence();
		}

		private static void ReleaseTimeOfDeathLock( object state )
		{
			( (Mobile)state ).EndAction( typeof( TimeOfDeathSpell ) );
			( (Mobile)state ).SendMessage( ((Mobile)state).Language == "ITA" ? "Puoi riutilizzare la maledizione mortale." : "You can use again death curse." );
		}

		public class InternalTarget : Target
		{
			private readonly TimeOfDeathSpell m_Owner;

			public InternalTarget( TimeOfDeathSpell owner )
				: base( 13, false, TargetFlags.Harmful )
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

		private List<Static> m_Circle = new List<Static>();

		private static readonly int[] m_Ids = new int[]
		{
			0x3080, -1, 0,
			0x3082, 0, -1,
			0x3081, 1, -1,
			0x307D, -1, 1,
			0x307F, 0, 0,
			0x307E, 1, 0,
			0x307C, 0, 1,
			0x307B, 1, 1,
			0x3083, 1, 1,
		};

		private void BuildArcaneCircle( IPoint3D location, Map map )
		{
			if( map == null || map == Map.Internal )
				return;

			if( m_Circle == null )
				m_Circle = new List<Static>();

			for( int i = 0; i < 8; i++ )
			{
				Static s = new Static( m_Ids[ i * 3 ] );
				s.MoveToWorld( new Point3D( location.X + m_Ids[ ( i * 3 ) + 1 ], location.Y + m_Ids[ ( i * 3 ) + 2 ], location.Z ), map );
				m_Circle.Add( s );
			}
		}

		private void RemoveArcaneCircle()
		{
			if( m_Circle == null )
				return;

			for( int i = 0; i < m_Circle.Count; i++ )
			{
				if( !m_Circle[ i ].Deleted )
					m_Circle[ i ].Delete();
			}
		}
	}
}