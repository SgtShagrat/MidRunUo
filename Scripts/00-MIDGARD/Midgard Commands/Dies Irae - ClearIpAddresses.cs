/***************************************************************************
 *                                  ClearAccountIps.cs
 *                            		------------------
 *  begin                	: September, 2007
 *  version					: 2.0 **VERSION FOR RUNUO 2.0**
 *  copyright            	: Matteo Visintin
 *  email                	: tocasia@alice.it
 *  msn						: Matteo_Visintin@hotmail.com
 ***************************************************************************/

/***************************************************************************
 *  Info:
 *          Comando per rimuovere gli ip che hanno effettuato l'accesso
 *          su un account.
 * 
 ***************************************************************************/

using System;
using System.Net;
using Server;
using Server.Accounting;
using Server.Gumps;
using Server.Commands;

namespace Midgard.Commands
{
    public class ClearAccountIps
    {
        public static void Initialize()
        {
            CommandSystem.Register( "ClearAccountIps", AccessLevel.Developer, new CommandEventHandler( ClearAccountIps_OnCommand ) );
        }

        [Usage( "ClearAccountIps <account>" )]
        [Description( "Remove all account ips from target account" )]
        public static void ClearAccountIps_OnCommand( CommandEventArgs e )
        {
            Mobile from = e.Mobile;
            if( from == null || from.Deleted )
                return;

            if( e.Length == 1 )
            {
                string accString = e.GetString( 0 );

                if( !string.IsNullOrEmpty( accString ) )
                {
                    Account acct = Accounts.GetAccount( accString ) as Account;

                    if( acct != null )
                    {
                        string msg = string.Format( "You are going to remove all ip adress that accessed" +
                                                    "accunt <em><basefont color=red>{0}</basefont></em>.<br>" +
                                                    "Are you sure you want to proceed?", accString );
                        from.SendGump( new WarningGump( 1060635, 30720, msg, 0xFFC000, 420, 280, new WarningGumpCallback( ConfirmRemoveCallBack ), new object[] { acct }, true ) );
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
                from.SendMessage( "CommandUse: [ClearAccountIps <account>" );
            }
        }

        private static void ConfirmRemoveCallBack( Mobile from, bool okay, object state )
        {
            object[] states = (object[])state;

            Account acct = (Account)states[ 0 ];

            if( okay )
            {
                from.SendMessage( "You have decided to proceede." );

                try
                {
                    acct.LoginIPs = new IPAddress[ 0 ];
                    from.SendMessage( "You have successfully removed ips from account >>{0}<<.", acct.Username );
                }
                catch( Exception e )
                {
                    Console.WriteLine( e.ToString() );
                }
            }
        }
    }
}