/***************************************************************************
 *                                  CheckLogout.cs
 *                            		--------------
 *  begin                	: Aprile, 2008
 *  version					: 1.0
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 * 
 * 	Info
 *  Il comando dice a chi lo usa quanto tempo dura il logout in quella
 *  regione.
 *  
 ***************************************************************************/

using Server;
using Server.Commands;

namespace Midgard.Commands
{
	public class CheckLogOut
	{
		public static void Initialize()
		{
			CommandSystem.Register( "VerificaLogout" , AccessLevel.Player, new CommandEventHandler( CheckLogOut_OnCommand ) );
		}

		[Usage( "VerificaLogout" )]
		[Description( "Informa dell'eventuale tempo di logout nella regione corrente." )]
		public static void CheckLogOut_OnCommand( CommandEventArgs e )
		{
            e.Mobile.SendMessage( "Il tempo di logout in questa regione e' di {0} minuti.",
                e.Mobile.Region.GetLogoutDelay( e.Mobile ).TotalMinutes.ToString( "F0" ) );
		}
	}
}