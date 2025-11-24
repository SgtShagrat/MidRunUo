/***************************************************************************
 *                                  Ice.cs
 *                            		-------------------
 *  begin                	: Dicembre, 2007
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info
 * 
 ***************************************************************************/

using System;
using System.Collections;
using System.Xml;

using Server.Mobiles;
using Server.Network;
using Server.Spells;
using Server.Spells.Chivalry;
using Server.Spells.Fourth;
using Server.Spells.Ninjitsu;
using Server.Spells.Seventh;
using Server.Spells.Sixth;
using Server.Spells.Spellweaving;

namespace Server.Regions
{
	public class Ice : DungeonRegion
	{
		public virtual TimeSpan DamageInterval{ get{ return TimeSpan.FromSeconds( 5.0 ); } }

		private Hashtable m_Table;
		
		public Hashtable Table
		{
			get{ return m_Table; }
		}

		public static void Initialize() 
		{ 
			EventSink.Login += new LoginEventHandler( IceDungeon_OnLogin );
		}

		private static void IceDungeon_OnLogin( LoginEventArgs e ) 
		{
			Mobile m = e.Mobile;

			if( m.Region.IsPartOf( typeof( Ice ) ) && m.AccessLevel < AccessLevel.GameMaster )
			{
				m.Send( SpeedControl.WalkSpeed );

				Dismount( m );
			}
		}

		public Ice( XmlElement xml, Map map, Region parent ) : base( xml, map, parent )
		{			
		}

		public override void OnEnter( Mobile m )
		{
			if ( m.Player && m.Alive && m.AccessLevel == AccessLevel.Player )
			{
				StartTimer( m );
			}

			if( m.NetState != null && !TransformationSpellHelper.UnderTransformation( m, typeof( AnimalForm ) ) && m.AccessLevel < AccessLevel.GameMaster )
			{
				m.SendMessage( "You have to walk slowly on ice..." );
				m.Send( SpeedControl.WalkSpeed );
			}

			Dismount( m );
		}

		public override void OnExit( Mobile m )
		{
			base.OnExit( m );

			if ( m.NetState != null && !TransformationSpellHelper.UnderTransformation( m, typeof( AnimalForm ) ) && !TransformationSpellHelper.UnderTransformation( m, typeof( ReaperFormSpell ) ) )
			{
				m.SendMessage( "Well done. Now you can run if you would..." );
				m.Send( SpeedControl.Disable );
			}

			StopTimer( m );
		}

		public override void OnLocationChanged( Mobile m, Point3D oldLocation )
		{
			base.OnLocationChanged( m, oldLocation );
			
//			StopTimer( m );
//
//			if( m.Player && m.Alive && m.AccessLevel == AccessLevel.Player )
//				StartTimer( m );

			Dismount( m );
		}

		public override bool OnBeginSpellCast( Mobile m, ISpell s )
		{
			if ( ( s is GateTravelSpell || s is RecallSpell || s is MarkSpell || s is SacredJourneySpell ) && m.AccessLevel == AccessLevel.Player )
			{
				m.SendLocalizedMessage( 501802 ); // Thy spell doth not appear to work...
				
				return false;
			}
			
			return base.OnBeginSpellCast( m, s );
		}

		public void StartTimer( Mobile m )
		{
			if( m_Table == null )
				m_Table = new Hashtable();
				
			m_Table[ m ] = Timer.DelayCall( TimeSpan.Zero, DamageInterval, new TimerStateCallback( Damage ), m );
		}
		
		public void StopTimer( Mobile m )
		{
			if( m_Table == null )
				m_Table = new Hashtable();
				
			if( m_Table[ m ] != null )
			{
				Timer timer = (Timer)m_Table[ m ];
				timer.Stop();
			}			
		}
		
		public void Damage( object state )
		{
			if( state is Mobile && ((Mobile)state).ColdResistance < 60 )
			{
				Damage( (Mobile)state );
			}
		}
		
		public void Damage( Mobile m )
		{
			if( m.Player && !m.Alive )
			{
				StopTimer( m );
			}
			
			if( m.NetState != null )
			{
				m.SendMessage( "Your body is getting frozen!" );
				m.RevealingAction( true );
				m.FixedParticles( 0x376A, 1, 31, 9961, 1154, 0, EffectLayer.Waist );
				AOS.Damage( m, Utility.Random( 2, 3 ), 0, 0, 100, 0, 0 );
			}
		}

		private static void Dismount( Mobile m )
		{
			if( m.NetState != null && m.Mount != null )
			{
				IMount mount = m.Mount;

				if( mount != null )
				{
					m.SendMessage( "The roof is too much low and you must dismount." );
					mount.Rider = null;
				}
			}			
		}
	}
}
