// #define DebugGuardedRegion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Midgard.Engines.OrderChaosWars;
using Server;
using Server.Commands;
using Server.Mobiles;
using Server.Spells;

#if DebugGuardedRegion
using System.Diagnostics;
#endif

using Midgard.Engines.MidgardTownSystem;
using System.Diagnostics;
using System.IO;
using Midgard.Regions;

namespace Server.Regions
{
	public class GuardedRegion : BaseRegion
	{
		private static object[] m_GuardParams = new object[2];
		private Type m_GuardType;
		private bool m_Disabled;

		public bool Disabled{ get{ return m_Disabled; } set{ m_Disabled = value; } }

		public virtual bool IsDisabled()
		{
			return m_Disabled;
		}

		public static void Initialize()
		{
			CommandSystem.Register( "CheckGuarded", AccessLevel.GameMaster, new CommandEventHandler( CheckGuarded_OnCommand ) );
			CommandSystem.Register( "SetGuarded", AccessLevel.Seer, new CommandEventHandler( SetGuarded_OnCommand ) );
			CommandSystem.Register( "ToggleGuarded", AccessLevel.Seer, new CommandEventHandler( ToggleGuarded_OnCommand ) );

			NaglundRegion.StartBreathTimer();
		}

		[Usage( "CheckGuarded" )]
		[Description( "Returns a value indicating if the current region is guarded or not." )]
		private static void CheckGuarded_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			GuardedRegion reg = (GuardedRegion) from.Region.GetRegion( typeof( GuardedRegion ) );

			if ( reg == null )
				from.SendMessage( "You are not in a guardable region." );
			else if ( reg.Disabled )
				from.SendMessage( "The guards in this region have been disabled." );
			else
				from.SendMessage( "This region is actively guarded." );
		}

		[Usage( "SetGuarded <true|false>" )]
		[Description( "Enables or disables guards for the current region." )]
		private static void SetGuarded_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;

			if ( e.Length == 1 )
			{
				GuardedRegion reg = (GuardedRegion) from.Region.GetRegion( typeof( GuardedRegion ) );

				if ( reg == null )
				{
					from.SendMessage( "You are not in a guardable region." );
				}
				else
				{
					reg.Disabled = !e.GetBoolean( 0 );

					if ( reg.Disabled )
						from.SendMessage( "The guards in this region have been disabled." );
					else
						from.SendMessage( "The guards in this region have been enabled." );
				}
			}
			else
			{
				from.SendMessage( "Format: SetGuarded <true|false>" );
			}
		}

		[Usage( "ToggleGuarded" )]
		[Description( "Toggles the state of guards for the current region." )]
		private static void ToggleGuarded_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			GuardedRegion reg = (GuardedRegion) from.Region.GetRegion( typeof( GuardedRegion ) );

			if ( reg == null )
			{
				from.SendMessage( "You are not in a guardable region." );
			}
			else
			{
				reg.Disabled = !reg.Disabled;

				if ( reg.Disabled )
					from.SendMessage( "The guards in this region have been disabled." );
				else
					from.SendMessage( "The guards in this region have been enabled." );
			}
		}

		public static GuardedRegion Disable( GuardedRegion reg )
		{
			reg.Disabled = true;
			return reg;
		}

		#region modifica by Dies Irae per  non rendere le città di default accessibili ai rossi
//		public virtual bool AllowReds{ get{ return Core.AOS; } }
		private bool m_AllowReds;
		public virtual bool AllowReds{ get{ return m_AllowReds;} set{ m_AllowReds = value; } }

		private Virtue m_WarVirtue;
		public virtual Virtue WarVirtue{ get{ return m_WarVirtue;} set{ m_WarVirtue = value; } }
		#endregion

		public virtual bool CheckVendorAccess( BaseVendor vendor, Mobile from )
		{
			if ( from.AccessLevel >= AccessLevel.GameMaster || IsDisabled() )
				return true;

            #region modifica by Dies Irae per rendere i vendor accessibili in citta' che permettono i rossi
            if( !BaseVendor.YoungDeal( vendor, from ) )
                return false;
            if( !BaseVendor.CitizenDeal( vendor, from, false ) )
                return false;

            return !from.Criminal && ( AllowReds || Notoriety.Compute( from, from ) != Notoriety.Murderer ); // return ( from.Kills < 5 );
            #endregion
        }

		public virtual Type DefaultGuardType
		{
			get
			{
                #region mod by Dies Irae
                if( TownSystem.Find( this ) != null )
                    return typeof( TownGuard );
                #endregion
                
				if ( this.Map == Map.Ilshenar || this.Map == Map.Malas )
					return typeof( ArcherGuard );
				else
					return typeof( WarriorGuard );
			}
		}

		public GuardedRegion( string name, Map map, int priority, params Rectangle3D[] area ) : base( name, map, priority, area )
		{
			m_GuardType = DefaultGuardType;
		}

		public GuardedRegion( string name, Map map, int priority, params Rectangle2D[] area )
			: base( name, map, priority, area )
		{
			m_GuardType = DefaultGuardType;
		}
		
		public GuardedRegion( XmlElement xml, Map map, Region parent ) : base( xml, map, parent )
		{
			XmlElement el = xml["guards"];

			if ( ReadType( el, "type", ref m_GuardType, false ) )
			{
				if ( !typeof( Mobile ).IsAssignableFrom( m_GuardType ) )
				{
					Console.WriteLine( "Invalid guard type for region '{0}'", this );
					m_GuardType = DefaultGuardType;
				}
			}
			else
			{
				m_GuardType = DefaultGuardType;
			}

			#region modifica by Dies Irae per il default di guardie disabilitate e per leggere l'eventuale AllowReds di una GuardedRegion
            bool disabled = false;
            if( ReadBoolean( el, "disabled", ref disabled, false ) )
                this.Disabled = disabled;

#if DebugGuardedRegion
			if( !Disabled )
				Console.WriteLine( "Message: Guards are enabled for region {0} (Map: {1}).", Name, Map.Name );
#endif

			el = xml["allowreds"];

			bool allowed = false;
			if ( ReadBoolean( el, "allowed", ref allowed, false ) )
				AllowReds = allowed;
			#endregion
		}

		public override bool OnBeginSpellCast( Mobile m, ISpell s )
		{
			if ( !IsDisabled() && !s.OnCastInTown( this ) )
			{
				m.SendLocalizedMessage( 500946 ); // You cannot cast this in town!
				return false;
			}

			return base.OnBeginSpellCast( m, s );
		}

		public override bool AllowHousing( Mobile from, Point3D p )
		{
			return false;
		}

        public override void MakeGuard( Mobile focus )
        {
            MakeGuard( focus, false );
        }

        public static int GuardsPerTarget = 1;

        public override void MakeGuard( Mobile focus, bool isBackup )
        {
            if( focus == null || focus.Deleted )
                return;

            int amount = GuardsPerTarget;

            foreach( Mobile m in focus.GetMobilesInRange( 15 ) )
            {
                if( m is BaseGuard )
                {
                    BaseGuard g = (BaseGuard)m;

                    if( g.Focus == null ) // idling
                    {
                        g.Focus = focus;

                        --amount;
                    }
                    else if( g.Focus == focus )
                    {
                        --amount;
                    }
                }
            }

            while( amount-- > 0 && focus != null)
            {
                m_GuardParams[ 0 ] = focus;

                TownSystem t = TownSystem.Find( this );

                if( t == null && this is HouseRegion )
                {
                    Midgard.Engines.MidgardTownSystem.Config.Pkg.LogWarningLine( "Warning: Unable to find TownSystem. Trying to find by houseRegion." );
                    t = TownSystem.Find( (HouseRegion)this );
                }

                if( t == null )
                {
                    Midgard.Engines.MidgardTownSystem.Config.Pkg.LogWarningLine( "Warning: Unable to find TownSystem. Trying to find by location." );
                    t = TownSystem.Find( focus.Location, focus.Map );
                }

                if( t != null )
                    m_GuardParams[ 1 ] = t.Definition.Town;
                else
                    Midgard.Engines.MidgardTownSystem.Config.Pkg.LogWarningLine( "Warning: MakeGuards spawned without townsystem at location {0}.", focus.Location );

                try
                {
                    BaseGuard guard;

                    if( t == null )
                    {
                        m_GuardParams[ 1 ] = null;
                        guard = (BaseGuard)Activator.CreateInstance( m_GuardType, m_GuardParams );
                    }
                    else
                    {
                        guard = new TownGuard( focus, t.Definition.Town );
                    }

                    if( isBackup )
                    {
                        guard.IsBackupGuard = true;
                        guard.Say( "I'm here, in the help of my militia!" );
                    }
                }
                catch( Exception ex )
                {
                    Console.WriteLine( ex.ToString() );
                }
            }
        }

		public override void OnEnter( Mobile m )
		{
            base.DisplayRegionalInfo( m ); // mod by Dies Irae

			if ( IsDisabled() )
				return;

			// if ( !AllowReds && m.Kills >= 5 ) // mod by Dies Irae
				CheckGuardCandidate( m );

            #region modifica by Dies Irae per leggere l'eventuale AllowReds di una GuardedRegion
            if( AllowReds && Notoriety.Compute( m, m ) == Notoriety.Murderer )
                m.SendLocalizedMessage( 1064675, string.Empty, 37 );
            #endregion
		}

		public override void OnExit( Mobile m )
		{
			if ( IsDisabled() )
				return;
		}

		public override void OnSpeech( SpeechEventArgs args )
		{
			base.OnSpeech( args );

			if ( IsDisabled() )
				return;

			#region modifica by Dies Irae per non poter chiamare le guardie se non cittadino
			if ( args.Mobile.Alive && args.HasKeyword( 0x0007 ) && CheckGuardsAvailability( args.Mobile ) ) // *guards*
				CallGuards( args.Mobile.Location );
			#endregion	
		}

		#region modifica by Dies Irae per non poter chiamare le guardie se non cittadino
        private bool CheckGuardsAvailability( Mobile caller )
        {
            if( caller == null || caller.Deleted )
                return false;

            //if( TownHelper.IsInHisOwnCity( caller ) || TownHelper.IsInAlliedCity( caller ) )
            //    return true;

            //caller.SendMessage( "Only citizen of this town or allied ones can call the town guards." );

            return true;
        }
		#endregion

		public override void OnAggressed( Mobile aggressor, Mobile aggressed, bool criminal )
		{
			base.OnAggressed( aggressor, aggressed, criminal );

			if ( !IsDisabled() && aggressor != aggressed && criminal )
				CheckGuardCandidate( aggressor );
		}

		public override void OnGotBeneficialAction( Mobile helper, Mobile helped )
		{
			base.OnGotBeneficialAction( helper, helped );

			if ( IsDisabled() )
				return;

			int noto = Notoriety.Compute( helper, helped );

			if ( helper != helped && (noto == Notoriety.Criminal || noto == Notoriety.Murderer) )
				CheckGuardCandidate( helper );
		}

		public override void OnCriminalAction( Mobile m, bool message )
		{
			base.OnCriminalAction( m, message );

			if ( !IsDisabled() )
				CheckGuardCandidate( m );
		}

		private Dictionary<Mobile, GuardTimer> m_GuardCandidates = new Dictionary<Mobile, GuardTimer>();

		public void CheckGuardCandidate( Mobile m )
		{
#if DebugGuardedRegion
            //Console.WriteLine( "CheckGuardCandidate for {0}", m.GetType().Name );
#endif
			if ( IsDisabled() )
				return;

			if ( IsGuardCandidate( m ) )
			{
#if DebugGuardedRegion
                Console.WriteLine( "CheckGuardCandidate: {0} IsGuardCandidate", m.GetType().Name );
#endif
				GuardTimer timer = null;
				m_GuardCandidates.TryGetValue( m, out timer );

				if ( timer == null )
				{
					timer = new GuardTimer( m, m_GuardCandidates );
					timer.Start();

					m_GuardCandidates[m] = timer;
					m.SendLocalizedMessage( 502275 ); // Guards can now be called on you!

					Map map = m.Map;

					if ( map != null )
					{
						Mobile fakeCall = null;
						double prio = 0.0;

						foreach ( Mobile v in m.GetMobilesInRange( 15 ) )
						{
                            if( !v.Player && v != m && !IsGuardCandidate( v ) && ( ( v is BaseCreature ) ? ( (BaseCreature)v ).IsHumanInTown() : ( v.Body.IsHuman && v.Region.IsPartOf( this ) ) ) /* && v.InLOS( m.Location ) */ )
							{
								double dist = m.GetDistanceToSqrt( v );

								if ( fakeCall == null || dist < prio )
								{
									fakeCall = v;
									prio = dist;
								}
							}
						}
#if DebugGuardedRegion
                        Console.WriteLine( "( fakeCall != null ): {0}", ( fakeCall != null ) );
#endif
						if ( fakeCall != null )
						{
                            if( !( fakeCall is BaseGuard ) ) // mod by Dies Irae
							    fakeCall.Say( Utility.RandomList( 1007037, 501603, 1013037, 1013038, 1013039, 1013041, 1013042, 1013043, 1013052 ) );
							MakeGuard( m );
							timer.Stop();
							m_GuardCandidates.Remove( m );
							// m.SendLocalizedMessage( 502276 ); // Guards can no longer be called on you.
						}
					}
				}
				else
				{
					timer.Stop();
					timer.Start();
				}
			}
		}

		public void CallGuards( Point3D p )
		{
			if ( IsDisabled() )
				return;

			IPooledEnumerable eable = Map.GetMobilesInRange( p, 10 );

            try
            {
                foreach( Mobile m in eable )
                {
#if DebugGuardedRegion
                    Console.WriteLine( !string.IsNullOrEmpty( m.Name ) ? m.Name : "nullName" );
                    Console.WriteLine( IsGuardCandidate( m ) );
                    Console.WriteLine( m.Region.IsPartOf( this ) );
                    Console.WriteLine( m_GuardCandidates.ContainsKey( m ) );
                    Console.WriteLine("");

                    if( IsGuardCandidate( m ) && 
                        ( 
                            ( !AllowReds && Notoriety.Compute( m, m ) == Notoriety.Murderer && m.Region.IsPartOf( this ) ) 
                            || m_GuardCandidates.ContainsKey( m ) 
                        ) 
                    )
#endif
                    if( IsGuardCandidate( m ) || m_GuardCandidates.ContainsKey( m ) )
                    {
                        GuardTimer timer = null;
                        m_GuardCandidates.TryGetValue( m, out timer );

                        if( timer != null )
                        {
                            timer.Stop();
                            m_GuardCandidates.Remove( m );
                        }

                        MakeGuard( m );
                        // m.SendLocalizedMessage( 502276 ); // Guards can no longer be called on you.
                        break;
                    }
                }
            }
            catch( Exception ex )
            {
                try
                {
                    using( StreamWriter op = new StreamWriter( "Logs/guards-errors.log", true ) )
                    {
                        op.WriteLine( "Time: {0}", DateTime.Now );
                        op.WriteLine( "Exception:" );
                        op.WriteLine( ex.ToString() );
                        op.WriteLine();

                        StackTrace st = new StackTrace( true );
                        string stackIndent = String.Empty;

                        for( int i = 0; i < st.FrameCount; i++ )
                        {
                            StackFrame sf = st.GetFrame( i );
                            Console.WriteLine( stackIndent + " Method: {0}", sf.GetMethod().Name );
                            stackIndent += "  ";
                        }
                    }
                }
                catch( Exception e )
                {
                    Console.WriteLine( e.ToString() );
                }
            }

			eable.Free();
		}

		public bool IsGuardCandidate( Mobile m )
		{
			if ( m is BaseGuard || m is BaseVendor || !m.Alive || m.AccessLevel > AccessLevel.Player || m.Blessed || IsDisabled() )
				return false;

			#region modifica by Dies Irae
			if( TownSystem.IsInAnyTownArena( m ) )
				return false;

			if( !m.Region.IsPartOf( this ) )
				return false;

			if( m is BaseCreature )
			{
				BaseCreature bc = m as BaseCreature;
				Mobile master = bc.GetMaster();

				if( (master as PlayerMobile) != null && !master.Criminal && !m.Criminal )
					return false;

				if( (master as PlayerMobile) != null && AllowReds && Notoriety.Compute( master, master ) == Notoriety.Murderer ) 
					return false;

				if( master == null && m.Combatant != null )
					return true;

				if( bc.GuardImmune )
					return false;
			}
			#endregion

			bool isGuardCandidate = m.Criminal || ( !AllowReds && Notoriety.Compute( m, m ) == Notoriety.Murderer );

			return isGuardCandidate;
		}

		private class GuardTimer : Timer
		{
			private Mobile m_Mobile;
			private Dictionary<Mobile, GuardTimer> m_Table;

			public GuardTimer( Mobile m, Dictionary<Mobile, GuardTimer> table ) : base( TimeSpan.FromSeconds( 1.0 ), TimeSpan.FromSeconds( 1.0 ) ) // : base( TimeSpan.FromSeconds( 15.0 ) ) // mod by Dies Irae
			{
				Priority = TimerPriority.TwoFiftyMS;

				m_Mobile = m;
				m_Table = table;
			}

			protected override void OnTick()
			{
				#region mod by Dies Irae
				if( m_Mobile == null || m_Mobile.Criminal )
					return;
				#endregion

				if ( m_Table.ContainsKey( m_Mobile ) )
				{
					m_Table.Remove( m_Mobile );
					m_Mobile.SendLocalizedMessage( 502276 ); // Guards can no longer be called on you.
				}
			}
		}
	}
}
