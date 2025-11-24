/***************************************************************************
 *                                  WipeNullAccountMobiles.cs
 *                            		-------------------------
 *  begin                	: Agosto, 2007
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 * 			Rimuove tutti i player giocanti ma senza account.
 * 
 ***************************************************************************/
 
using System;
using System.Collections.Generic;
using System.IO;

using Server;
using Server.Commands;
using Server.Gumps;

namespace Midgard.Commands
{
	public class WipeNullAccountMobiles
	{
		#region registrazione
		public static void Initialize()
		{
			CommandSystem.Register( "WipeNullAccountMobiles" , AccessLevel.Developer, new CommandEventHandler( WipeNullAccountMobiles_OnCommand ) );
		}
		#endregion

		#region callback
		[Usage( "WipeNullAccountMobiles" )]
		[Description( "Wipe all players with no account" )]
		public static void WipeNullAccountMobiles_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			if( from == null  )
				return;

			List<Mobile> mobs = new List<Mobile>( World.Mobiles.Values );
			List<Mobile> mobsToWipe = new List<Mobile>();

			// Counter dei pg da eliminare
			int count = 0;

			for(int i = 0; i < mobs.Count; i++ )
			{
				Mobile m = mobs[i];
				if( m.Player && m.Account == null )
				{
					mobsToWipe.Add( m );
					count++;
				}
			}

			if( count > 0 )
			{
				from.SendGump( new WarningGump( 1060635, 30720, string.Format( "You are going to delete {0} player mobile with no account.<br>" +
				               "This process is irreversible. Are you sure do you want to proceed?", count ),
				               0xFFC000, 420, 280, new WarningGumpCallback( ConfirmDeleteCallBack ), new object[]{ mobsToWipe }, true ) );
			}
			else
			{
				from.SendMessage( "There are no players to wipe." );
			}
		}

      	private static void ConfirmDeleteCallBack( Mobile from, bool okay, object state )
      	{
      		object[] pars = state as object[];
      		
      		List<Mobile> mobsToWipe = pars[0] as List<Mobile>;

      		if( okay )
      		{
				using ( StreamWriter tw = new StreamWriter( "Logs/LogWipeNullAccountPlayers.log", true ) )
				{
					tw.WriteLine( "Inizio Wipe dei pg." );

					int delCounter = 0;

					for( int i = 0; i < mobsToWipe.Count; i++ )
					{
						Mobile m = mobsToWipe[i];
						try
						{
							m.Delete();
							tw.WriteLine( "Wiped pg {0}, Serial {1}, in date {2}.", m.Name, m.Serial.ToString(), DateTime.Now.ToShortTimeString() );
							delCounter++;
						}
						catch( Exception e )
						{
							Console.WriteLine( e.ToString() );
						}
					}

					from.SendGump( new NoticeGump( 1060635, 30720, string.Format( "Proccess completed. Wiped {0} mobiles on {1} deletable.", delCounter, mobsToWipe.Count),
					          0xFFC000, 420, 280, new NoticeGumpCallback( CloseNoticeGumpCallBack ), null ) );
				}
      		}
      	}

		private static void CloseNoticeGumpCallBack( Mobile from, object state )
		{
		}
		#endregion
	}
}
