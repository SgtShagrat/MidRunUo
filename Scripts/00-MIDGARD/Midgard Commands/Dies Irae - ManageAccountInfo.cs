/***************************************************************************
 *                                   ManageAccountInfo.cs
 *                            		----------------
 *  begin                	: Marzo, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;
using System.Collections;
using System.IO;

using Server;
using Server.Accounting;
using Server.Commands;
using Server.Gumps;

namespace Midgard.Commands
{
    public class ManageAccountInfo
    {
        public static void Initialize()
        {
            CommandSystem.Register( "ManageAccountInfo", AccessLevel.Administrator, new CommandEventHandler( ManageAccountInfo_OnCommand ) );
        }

        [Usage( "ManageAccountInfo" )]
        [Description( "Utility to manage account infoes" )]
        public static void ManageAccountInfo_OnCommand( CommandEventArgs e )
        {
            ArrayList accounts = new ArrayList( (ICollection)Accounts.GetAccounts() );
            accounts.Sort( AccountComparer.Instance );

            using ( StreamWriter op = new StreamWriter( "Logs/accounts.log" ) )
            {
                op.WriteLine( "# Account table generated on {0}", DateTime.Now );
                op.WriteLine();
                op.WriteLine();

                foreach( IAccount a in accounts )
                {
                    Account acc = a as Account;
                    if( a != null )
                    {
                        op.WriteLine( "UsedID {0} - Email {1} - LastLogin {2} - HasLAN {3}",
                                     acc.Username, acc.Email, acc.LastLogin.ToString(), ( acc.GetTag( "LAN" ) != null ).ToString() );
                    }
                }
            }
            
            e.Mobile.SendMessage( "Account table has been generated. See the file : <runuo root>/Logs/accounts.log" );
        }
        
        private class AccountComparer : IComparer
        {
            public static readonly IComparer Instance = new AccountComparer();

            public AccountComparer()
            {
            }

            public int Compare( object x, object y )
            {
                if ( x == null && y == null )
                    return 0;
                else if ( x == null )
                    return -1;
                else if ( y == null )
                    return 1;

                Account a = x as Account;
                Account b = y as Account;

                if ( a == null || b == null )
                    throw new ArgumentException();

                AccessLevel aLevel, bLevel;
                bool aOnline, bOnline;

                AdminGump.GetAccountInfo( a, out aLevel, out aOnline );
                AdminGump.GetAccountInfo( b, out bLevel, out bOnline );

                if ( aOnline && !bOnline )
                    return -1;
                else if ( bOnline && !aOnline )
                    return 1;
                else if ( aLevel > bLevel )
                    return -1;
                else if ( aLevel < bLevel )
                    return 1;
                else
                    return Insensitive.Compare( a.Username, b.Username );
            }
        }
    }
}