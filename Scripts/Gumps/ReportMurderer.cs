// #define DebugReportMurderer

using System;
using System.Collections.Generic;
using System.IO;

using Midgard.Engines.Classes;
using Midgard.Engines.MurderInfo;
using Midgard.Engines.MidgardTownSystem;
using Midgard.Engines.BountySystem;
using Server.Misc;
using Server.Mobiles;
using Server.Network;

namespace Server.Gumps
{
	public class ReportMurdererGump : Gump
	{
		private int m_Idx;
		private List<Mobile> m_Killers;
		private Mobile m_Victum;

		public static void Initialize()
		{
			EventSink.PlayerDeath += new MobileDeathEventHandler( EventSink_PlayerDeath );
		}
 
		public static void EventSink_PlayerDeath( MobileDeathEventArgs e )
		{
			Mobile m = e.Mobile;

			List<Mobile> killers = new List<Mobile>();
			List<Mobile> toGive  = new List<Mobile>();

			foreach ( AggressorInfo ai in m.Aggressors )
			{
				if ( ai.Attacker.Player && ai.CanReportMurder && !ai.Reported && 
                    ( /*!TownHelper.AreOpposedCitizens( ai.Attacker, m )||*/  !Midgard.Engines.BountySystem.Core.Attackable( ai.Attacker, e.Mobile ) ) ) // mod by Dies Irae. I nemici non danno murder.
				{
					killers.Add( ai.Attacker );
					ai.Reported = true;
				}
				if ( ai.Attacker.Player && (DateTime.Now - ai.LastCombatTime) < TimeSpan.FromSeconds( 30.0 ) && !toGive.Contains( ai.Attacker ) )
					toGive.Add( ai.Attacker );
			}

			foreach ( AggressorInfo ai in m.Aggressed )
			{
				if ( ai.Defender.Player && (DateTime.Now - ai.LastCombatTime) < TimeSpan.FromSeconds( 30.0 ) && !toGive.Contains( ai.Defender ) )
					toGive.Add( ai.Defender );
			}

			foreach ( Mobile g in toGive )
			{
				int n = Notoriety.Compute( g, m );

				int theirKarma = m.Karma, ourKarma = g.Karma;
				bool innocent = ( n == Notoriety.Innocent );
				bool criminal = ( n == Notoriety.Criminal || n == Notoriety.Murderer );

				int fameAward = m.Fame / 200;
				int karmaAward = 0;

				if ( innocent )
					karmaAward = ( ourKarma > -2500 ? -850 : -110 - (m.Karma / 100) );
				else if ( criminal )
					karmaAward = 50;

				Titles.AwardFame( g, fameAward, false );
				Titles.AwardKarma( g, karmaAward, true );
				
				Items.XmlQuest.RegisterKill( m, g); // ARTEGORDONMOD modification to support XmlQuest Killtasks of players
			}

			if ( m is PlayerMobile && ((PlayerMobile)m).NpcGuild == NpcGuild.ThievesGuild )
				return;

		    #region mod by Dies Irae
		    if( m is PlayerMobile && ClassSystem.IsThief( m ) || m.Skills.Stealing.Value >= 30.0 )
		        return;
		    #endregion

			if ( killers.Count > 0 )
				new GumpTimer( m, killers ).Start();

			/*
			TODO:  	Check entire combatant system and see if the
				cobatant lists should be handled a different
				way, and  change it accordingly.  This is a
				small-scope patch to prevent an exploit.
			*/

			for ( int i = m.Aggressors.Count - 1; i >= 0; --i )
			{
				m.Aggressors.RemoveAt ( i );
			}
		
		}

		private class GumpTimer : Timer
		{
			private Mobile m_Victim;
			private List<Mobile> m_Killers;

			public GumpTimer( Mobile victim, List<Mobile> killers ) : base( TimeSpan.FromSeconds( 4.0 ) )
			{
				m_Victim = victim;
				m_Killers = killers;
			}

			protected override void OnTick()
			{
				m_Victim.SendGump( new ReportMurdererGump( m_Victim, m_Killers ) );
			}
		}

		public ReportMurdererGump( Mobile victum, List<Mobile> killers ) : this( victum, killers, 0 )
		{
		}

		private ReportMurdererGump( Mobile victum, List<Mobile> killers, int idx ) : base( 0, 0 )
		{
			m_Killers = killers;
			m_Victum = victum;
			m_Idx = idx;
			BuildGump();
		}

		private void BuildGump() 
		{
			AddBackground( 265, 205, 320, 290, 5054 );
			Closable = false;
			Resizable = false;
			
			AddPage( 0 );      			
			
			AddImageTiled( 225, 175, 50, 45, 0xCE );   //Top left corner
			AddImageTiled( 267, 175, 315, 44, 0xC9 );  //Top bar
			AddImageTiled( 582, 175, 43, 45, 0xCF );   //Top right corner
			AddImageTiled( 225, 219, 44, 270, 0xCA );  //Left side
			AddImageTiled( 582, 219, 44, 270, 0xCB );  //Right side
			AddImageTiled( 225, 489, 44, 43, 0xCC );   //Lower left corner
			AddImageTiled( 267, 489, 315, 43, 0xE9 );  //Lower Bar
			AddImageTiled( 582, 489, 43, 43, 0xCD );   //Lower right corner

			AddPage( 1 );
			
			AddHtml( 260, 234, 300, 140, ((Mobile)m_Killers[m_Idx]).Name, false, false ); // Player's Name
			AddHtmlLocalized( 260, 254, 300, 140, 1049066, false, false ); // Would you like to report...

			AddButton( 260, 300, 0xFA5, 0xFA7, 1, GumpButtonType.Reply, 0 );
			AddHtmlLocalized( 300, 300, 300, 50, 1046362, false, false ); // Yes
			      	
			AddButton( 360, 300, 0xFA5, 0xFA7, 2, GumpButtonType.Reply, 0 );
			AddHtmlLocalized( 400, 300, 300, 50, 1046363, false, false ); // No      
		}

		public override void OnResponse( NetState state, RelayInfo info )
		{
			Mobile from = state.Mobile;

			switch ( info.ButtonID )
			{
				case 1: 
				{            
					Mobile killer = m_Killers[m_Idx];
					if ( killer != null && !killer.Deleted )
					{
						killer.Kills++;
						killer.ShortTermMurders++;

						if ( killer is PlayerMobile )
							((PlayerMobile)killer).ResetKillTime();

						killer.SendLocalizedMessage( 1049067 );//You have been reported for murder!

						if ( killer.Kills == 5 )
							killer.SendLocalizedMessage( 502134 );//You are now known as a murderer!
						else if ( SkillHandlers.Stealing.SuspendOnMurder && killer.Kills == 1 && killer is PlayerMobile && ((PlayerMobile)killer).NpcGuild == NpcGuild.ThievesGuild )
							killer.SendLocalizedMessage( 501562 ); // You have been suspended by the Thieves Guild.
					
                        #region modifica by Dies Irae per il kill tra fazionati e cittadini e per il bounty system	
						if( from != null && killer is Midgard2PlayerMobile && from is Midgard2PlayerMobile )
						{
							// I concittadini che si danno murder per l'uccisione danno anke lo Statloss
							if( Midgard.Engines.MidgardTownSystem.Notoriety.AreCoCitizens( killer, from ) )
							{
                                TownSystem sys = TownSystem.Find( killer.Location, killer.Map );
                                if( sys != null )
                                    sys.RegisterCriminal( killer, CrimeType.ReportedAsMurderer );

								if( !from.Region.IsPartOf( "Arena di Serpent's Hold" ) && !from.Region.IsPartOf( "Arena di Britain" ) )
								{
									string msg = String.Format( "{0}, you were killed by {1} a co-citizen of yours.<br>Would you like to punish him for this murder?<br>", from.Name, killer.Name);
									from.SendGump( new WarningGump( 1060635, 30720, msg, 0xFFC000, 420, 280, 
									                   new WarningGumpCallback( ConfirmPunishmentCallBack ), new object[]{ killer, from } ) );
								}
							}

                            if( Midgard.Engines.BountySystem.Config.Enabled )
                            {
                                // from.SendGump( new AddBountyGump( from, killer ));
                                from.SendStringQuery( new AddBountyQuery( from, killer ) );
                            }
						}

                        try
                        {
                            using( StreamWriter op = new StreamWriter( "Logs/player-murders.log", true ) )
                            {
                                op.WriteLine(
                                    "Player type {0}, name {1}, serial {2}, has reported a murder to {3} (account {4}) in date and time {5}.",
                                    GetType().Name, string.IsNullOrEmpty( from.Name ) ? "null name" : from.Name, Serial,
                                    killer.Name, killer.Account.Username, DateTime.Now );
                                op.WriteLine();
                            }
                        }
                        catch
                        {
                        }
						#endregion
					}
					break; 
				}
				case 2: 
				{
					break; 
				}
			}

			m_Idx++;
			if ( m_Idx < m_Killers.Count )
				from.SendGump( new ReportMurdererGump( from, m_Killers, m_Idx ) );
		}
		
		#region modifica by Dies Irae per il kill tra fazionati e cittadini
      	private static void ConfirmPunishmentCallBack( Mobile From, bool okay, object state )
      	{
      		object[] states = (object[])state;
      		
      		Midgard2PlayerMobile attacker = (Midgard2PlayerMobile)states[0];
      		Midgard2PlayerMobile defender = (Midgard2PlayerMobile)states[1];
      		
      		if( okay )
      			MurderInfoHelper.HandleInfo( attacker, defender );
      		else
      			attacker.SendMessage( 37, "You have killed a citizen ally but your victim decided to not consider this murder a criminal act." );
      	}
		#endregion
	}
}
