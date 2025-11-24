/***************************************************************************
 *                                      OpenBank.cs
 *                            		-------------------
 *  begin                	: Febbraio 2007
 *  version					: 1.0
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 * 
 * 	Info
 * 				Apre la banca a se stesso.
 *  
 ***************************************************************************/

using Server;
using Server.Commands;
using Server.Items;
using Server.Multis;

namespace Midgard.Commands
{
	public class OpenBank
	{
		#region registrazione
		public static void Initialize()
		{
			CommandSystem.Register( "OpenBank" , AccessLevel.GameMaster, new CommandEventHandler( OpenBank_OnCommand ) );
		}
		#endregion

		#region callback
		[Usage( "OpenBank" )]
		[Description( "Apre la banca a se stessi." )]
		public static void OpenBank_OnCommand( CommandEventArgs e )
		{
			Mobile m = e.Mobile;
			
			BaseHouse house = BaseHouse.FindHouseAt( m );

			if( house == null )
			{
				m.SendMessage( "Il comando va usato in casa tua." );
				return;
			}
									    
			BankBox box = ( m.Player ? m.BankBox : m.FindBankNoCreate() );
			
			if ( box != null )
			{
				box.Open();
				m.SendMessage( "Ho aperto la tua banca. Essa contiene {0} oggetti.", box.TotalItems );
			}
			else
			{
				m.SendMessage( "Non hai un Bankbox da aprire." );
			}
		}
		#endregion
	}
}

