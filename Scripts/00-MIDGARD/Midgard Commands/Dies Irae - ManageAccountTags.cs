/***************************************************************************
 *                                  RemoveAccountTag.cs
 *                            		-------------------
 *  begin                	: September, 2007
 *  version					: 2.0 **VERSION FOR RUNUO 2.0**
 *  copyright            	: Matteo Visintin
 *  email                	: tocasia@alice.it
 *  msn						: Matteo_Visintin@hotmail.com
 ***************************************************************************/

/***************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 2 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 * 
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;

using Server.Accounting;
using Server.Gumps;

namespace Server.Commands
{
	public class ManageAccountTags
	{
		#region registration
		public static void Initialize()
		{
			CommandSystem.Register( "RemoveAccountTag" , AccessLevel.Developer, new CommandEventHandler( RemoveAccountTag_OnCommand ) );
			CommandSystem.Register( "ResetAllOfOneTypeTags" , AccessLevel.Developer, new CommandEventHandler( ResetAllOfOneTypeTags_OnCommand ) );
			CommandSystem.Register( "ListAllTags" , AccessLevel.Developer, new CommandEventHandler( ListAllTags_OnCommand ) );
		}
		#endregion

		#region callback
		[Usage( "RemoveAccountTag <account> <tag>" )]
		[Description( "Remove a tag from target account" )]
		public static void RemoveAccountTag_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			if( from == null || from.Deleted )
				return;

			if( e.Length == 2 )
			{
				string accString = e.GetString( 0 );
				string tagString = e.GetString( 1 );

				if( !string.IsNullOrEmpty( accString ) )
				{
					Account acct = Accounts.GetAccount( accString ) as Account;

					if( acct != null )
					{
						if( !string.IsNullOrEmpty( tagString ) )
						{
							if( !string.IsNullOrEmpty(acct.GetTag( tagString )) )
							{
								string msg = string.Format( "You are going to remove tag <em><basefont color=red>{0}</basefont></em> " +
								                            "from accunt <em><basefont color=red>{1}</basefont></em>.<br>" +
							                            	"Are you sure you want to proceed?", tagString, accString );
								from.SendGump( new WarningGump( 1060635, 30720, msg, 0xFFC000, 420, 280, new WarningGumpCallback( ConfirmRemoveCallBack ), new object[]{ acct, tagString }, true ) );
							}
							else
							{
								from.SendMessage( "Target account exist but does not have that tag." );
							}
						}
						else
						{
							from.SendMessage( "Invalid tag (null or empty)." );
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
			else
			{
				from.SendMessage( "CommandUse: [RemoveAccountTag <account> <tag>" );
			}
		}

		[Usage( "ResetAllOfOneTypeTags <tag>" )]
		[Description( "Remove a tag from all account on server" )]
		public static void ResetAllOfOneTypeTags_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			if( from == null || from.Deleted )
				return;

			if( e.Length == 1 )
			{
				string tagString = e.GetString( 0 );

				if( !string.IsNullOrEmpty( tagString ) )
				{
					string msg = string.Format( "You are going to remove tag <em><basefont color=red>{0}</basefont></em> " +
								                "from all accounts on this shard.<br>" +
							                    "Are you sure you want to proceed?", tagString );
					from.SendGump( new WarningGump( 1060635, 30720, msg, 0xFFC000, 420, 280, new WarningGumpCallback( ConfirmResetCallBack ), new object[]{ tagString }, true ) );
				}
				else
				{
					from.SendMessage( "Invalid tag (null or empty)." );
				}
			}
			else
			{
				from.SendMessage( "CommandUse: ResetAllOfOneTypeTags <tag>" );
			}
		}

		[Usage( "ListAllTags" )]
		[Description( "List all accounts tags to file" )]
		public static void ListAllTags_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			if( from == null || from.Deleted )
				return;

			if( e.Length < 2 )
			{
				string tagString = string.Empty;
				if( e.Length == 1 )
					tagString = e.GetString( 0 );

				
      			try
      			{
					using( StreamWriter op = new StreamWriter( "Logs/MidgardAccountsTags.log" ) )
					{
						op.WriteLine( "List generated on {0} in time {1}.", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString() );
						op.WriteLine( "Total accounts processed {0}", Accounts.GetAccounts().Count );
						op.WriteLine( "" );

						foreach( Account a in Accounts.GetAccounts() )
						{
							List<AccountTag> tags = a.Tags;

							if( tags.Count > 0 )
							{
								op.WriteLine( "{0}", a.Username );
								for( int i = 0; i < tags.Count; i++ )
								{
									if( !String.IsNullOrEmpty( tagString ) && Utility.InsensitiveCompare( tags[i].Name, tagString ) != 0 )
										continue;

									op.WriteLine( "\t{0} - {1}", tags[i].Name, tags[i].Value );
								}
							}
						}
					}
      			}
      			catch( System.Exception ex )
      			{
      				Console.WriteLine( ex.ToString() );
      			}
			}
			else
			{
				from.SendMessage( "CommandUse: ListAllTags" );
			}
		}

      	private static void ConfirmRemoveCallBack( Mobile from, bool okay, object state )
      	{
      		object[] states = (object[])state;

      		Account acct = (Account)states[0];
      		string tag = (string)states[1];

      		if( okay )
      		{
      			from.SendMessage( "You have decided to proceede." );

      			try
      			{
      				acct.RemoveTag( tag );
      				from.SendMessage( "You have successfully removed tag >>{0}<< from account >>{1}<<.", tag, acct.Username );
      			}
      			catch( System.Exception e )
      			{
      				Console.WriteLine( e.ToString() );
      			}
      		}
      	}

		private static void ConfirmResetCallBack( Mobile from, bool okay, object state )
		{
      		object[] states = (object[])state;

      		string tag = (string)states[0];

      		if( okay )
      		{
      			from.SendMessage( "You have decided to proceede." );

      			try
      			{
					foreach( Account a in Accounts.GetAccounts() )
					{
						if( !string.IsNullOrEmpty( a.GetTag( tag )) )
							a.RemoveTag( tag );
					}

      				from.SendMessage( "You have successfully removed tag \"{0}\" from all accounts.", tag );
      			}
      			catch( System.Exception e )
      			{
      				Console.WriteLine( e.ToString() );
      			}
      		}
		}
		#endregion
	}
}
