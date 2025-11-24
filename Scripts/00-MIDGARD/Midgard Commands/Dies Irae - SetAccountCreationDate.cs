/***************************************************************************
 *                                  SetAccountAge.cs
 *                            		-------------------
 *  begin                	: Ottobre, 2007
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 * 			Set age (in days) of target account
 * 
 ***************************************************************************/

using System;
using Server;
using Server.Accounting;
using Server.Gumps;
using Server.Commands;

namespace Midgard.Commands
{
	public class SetAccountAge
	{
		#region registrazione
		public static void Initialize()
		{
			CommandSystem.Register( "SetAccountAge" , AccessLevel.Developer, new CommandEventHandler( SetAccountAge_OnCommand ) );
		}
		#endregion

		#region callback
		[Usage( "SetAccountAge <account> <accountAge>" )]
		[Description( "Set age (in days) of target account" )]
		public static void SetAccountAge_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			if( from == null || from.Deleted )
				return;

			if( e.Length == 2 )
			{
	  			try
	  			{
					string accString = e.GetString( 0 );
					TimeSpan age = e.GetTimeSpan( 1 );

					from.SendMessage( "You have choosen an age of {0} days", age.TotalDays );

					if( !string.IsNullOrEmpty( accString ) )
					{
						Account acct = Accounts.GetAccount( accString ) as Account;

						if( acct != null )
						{
							if( age != TimeSpan.Zero )
							{
								string msg = string.Format( "You are going to set age time for account to <em><basefont color=red>{0}</basefont></em> " +
								                            "to <em><basefont color=red>{1}</basefont></em> days.<br>" +
							                            	"Are you sure you want to proceed?", accString, age.TotalDays );
								from.SendGump( new WarningGump( 1060635, 30720, msg, 0xFFC000, 420, 280, new WarningGumpCallback( ConfirmSetAgeCallBack ), new object[]{ acct, age }, true ) );
							}
							else
							{
								from.SendMessage( "Invalid timespan for account age." );
							}
						}
						else
						{
							from.SendMessage( "Target account does not exist." );
						}
					}
					else
					{
						from.SendMessage( "Invalid userName (null or empty)." );
					}
	  			}
	  			catch( Exception ex )
	  			{
	  				Console.WriteLine( ex.ToString() );
	  			}
			}
			else
			{
				from.SendMessage( "CommandUse: [SetAccountAge <account> <accountAge>" );
			}
		}

  		private static void ConfirmSetAgeCallBack( Mobile from, bool okay, object state )
	  	{
	  		object[] states = (object[])state;
	  		Account acct = (Account)states[0];
	  		TimeSpan age = (TimeSpan)states[1];

	  		if( okay )
	  		{
	  			from.SendMessage( "You have decided to proceede." );

	  			try
	  			{
	  				DateTime creationTime = DateTime.Now - age;

	  				acct.Created = creationTime;
	  				from.SendMessage( "You have successfully set creation time to \"{0}\" for account \"{1}\".", creationTime.ToShortDateString(), acct.Username );
	  			}
	  			catch( Exception ex )
	  			{
	  				Console.WriteLine( ex.ToString() );
	  			}
	  		}
	  	}
		#endregion
	}
}
