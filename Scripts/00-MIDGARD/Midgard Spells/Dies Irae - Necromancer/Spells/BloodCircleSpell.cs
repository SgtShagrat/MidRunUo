/***************************************************************************
 *								  BloodCircleSpell.cs
 *									-------------------
 *  begin					: Gennaio, 2008
 *  version					: 2.0 **VERSION FOR RUNUO 2.0**
 *  copyright				: Midgard Uo Shard - Matteo Visintin
 *  email					: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 * 			Il maleficio del circolo di sangue crea attorno al bersaglio
 * 			un insieme di pericolosissime pozze di sangue putrefascente
 * 			che danneggiano chiunque ci passi sopra.
 * 
 * 			La formula per il danno e' random da 1 a SpiritSpeak / 15;
 * 			Il raggio inferiore e' 3, quello maggiore e' 3 + SpiritSpeak / 40;
 * 			La durata delle pozze e' SpiritSpeak / 10;
 * 
 * 			Se il necromante vuole usare lo spell come difesa dai nemici,
 * 			scegiendo se stesso ome centro del cerchio di sangue, esso ha raggio
 * 			doppio.
 ***************************************************************************/

using System;
using System.Collections;

using Midgard.Engines.Classes;

using Server;
using Server.Multis;
using Server.Spells;
using Server.Targeting;

namespace Midgard.Engines.SpellSystem
{
	public class BloodCircleSpell : RPGNecromancerSpell
	{
		private static readonly SpellInfo m_Info = new SpellInfo(
			"Blood Circle", "In Grav Corp",
			-1,
			9002,
			false,
			Reagent.DaemonBlood,
			Reagent.NoxCrystal
			);

		private static readonly ExtendedSpellInfo m_ExtendedInfo = new ExtendedSpellInfo
			(
			typeof( BloodCircleSpell ),
			"This power creates a circle of blood pools. These pools damage each creature passing on them.",
			"Il maleficio del circolo di sangue crea attorno al bersaglio un insieme di pericolosissime pozze di sangue putrefascente che danneggiano chiunque ci passi sopra.",
			0x5001
			);

		public override ExtendedSpellInfo ExtendedInfo{get { return m_ExtendedInfo; }}
		public override int RequiredMana{ get { return GetPowerLevel()*20; }}
		public override TimeSpan CastDelayBase{get { return TimeSpan.FromSeconds( 1.5 ); }}
		public override double DelayOfReuse{get { return GetPowerLevel()*60; }}
		public override double RequiredSkill{get { return 75.0; }}
		public override bool BlocksMovement{get { return true; }}

		private static readonly int DamageDivisor = 15;
		private static readonly int RadiusDivisor = 40;
		private static readonly int DurationDivisor = 5;

		public BloodCircleSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			if( Caster.CanBeginAction( typeof( BloodCircleSpell ) ) )
				Caster.Target = new InternalTarget( this );
			else
				Caster.SendMessage( Caster.Language == "ITA" ? "Sei troppo debole per lanciare del nuovo sangue." : "You are too weaken to split again your blood." );
		}

		public void Target( IPoint3D p )
		{
			if( !Caster.CanSee( p ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if( MidgardSpellHelper.CheckBlockField( p, Caster ) && SpellHelper.CheckTown( p, Caster ) && CheckSequence() )
			{
				Caster.BeginAction( typeof( BloodCircleSpell ) );

				int level = GetPowerLevel() * 20;

				int minDamage = 18;//bypassato nell'item
				int maxDamage = 20;//minDamage + ( level / DamageDivisor );

				int minRadius = 0;
				int maxRadius = 2 + GetPowerLevel();

				if( p is Mobile && p == Caster )
				{
					minRadius = 1;
					maxRadius = 3 + GetPowerLevel();
				}

				Map map = Caster == null ? Map.Felucca : Caster.Map;
				if( map == null || map == Map.Internal )
					return;

				TimeSpan duration = TimeSpan.FromSeconds( level / (double)DurationDivisor );

				SpellHelper.Turn( Caster, p );
				SpellHelper.GetSurfaceTop( ref p );

				IEntity to;

				if( p is Mobile )
					to = (Mobile)p;
				else
					to = new Entity( Serial.Zero, new Point3D( p ), map );

				if( Caster != null )
				{
					Caster.MovingEffect( to, 0xECA, 10, 0, false, false, 0, 0 );
					ClassSystem.Necromancer.SendOverheadMessage( Caster, Caster.Language == "ITA"? "*sprigiona sangue*" : "*splits blood on the ground*" );

					Timer.DelayCall( TimeSpan.FromSeconds( 1.0 ), new TimerStateCallback( DoCircle_Callback ), new object[] { Caster, minRadius, maxRadius, p, map, minDamage, maxDamage, duration } );
					Timer.DelayCall( GetDelayOfReuse(), new TimerStateCallback( ReleaseBloodCirleLock ), Caster );
				}
			}

			FinishSequence();
		}

		private static void DoCircle_Callback( object state )
		{
			object[] states = (object[])state;

			Mobile caster = (Mobile)states[ 0 ];
			int minRadius = (int)states[ 1 ];
			int maxRadius = (int)states[ 2 ];
			IPoint3D p = (IPoint3D)states[ 3 ];
			Map map = (Map)states[ 4 ];
			int minDamage = (int)states[ 5 ];
			int maxDamage = (int)states[ 6 ];
			TimeSpan duration = (TimeSpan)states[ 7 ];

			if( caster == null )
				return;

			//IPoint3D current;
			IPoint3D next;
			PoolOfBlood pool;

			for( int radius = minRadius; radius <= maxRadius; radius++ )
			{
				//current = new Point3D( p.X + radius, p.Y, p.Z );

				for( int i = 0; i <= 360; i++ )
				{
					next = new Point3D( (int)Math.Round( Math.Cos( i ) * radius ) + p.X, (int)Math.Round( Math.Sin( i ) * radius ) + p.Y, p.Z );
					SpellHelper.GetSurfaceTop( ref next );

					if( !FindPoolOfBlood( map, next ) )
					{
						if( caster.InLOS( next ) )//BaseHouse.FindHouseAt( next, map, next.Z ) == null && 
						{
							pool = new PoolOfBlood( caster, duration, minDamage, maxDamage );
							pool.MoveToWorld( new Point3D(next), map );
						}
					}

					//current = next;
				}
			}
		}

		private static bool FindPoolOfBlood( Map map, IPoint3D p )
		{
			IPooledEnumerable eable = map.GetItemsInRange( new Point3D(p), 0 );

			foreach( Item item in eable )
			{
				if( item is PoolOfBlood )
					return true;
			}

			eable.Free();

			return false;
		}

		private static void ReleaseBloodCirleLock( object state )
		{
			( (Mobile)state ).EndAction( typeof( BloodCircleSpell ) );
			( (Mobile)state ).SendMessage( ((Mobile)state).Language == "ITA"? "Puoi evocare un nuovo circolo di sangue!" : "You can make a new blood circle, now!" );
		}

		private class InternalTarget : Target
		{
			private readonly BloodCircleSpell m_Owner;

			public InternalTarget( BloodCircleSpell owner ) : base( 10, true, TargetFlags.None )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				IPoint3D p = o as IPoint3D;

				if( p != null )
					m_Owner.Target( p );
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}