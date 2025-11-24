using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Server;
using Server.Accounting;
using Server.Commands;
using Server.Mobiles;

namespace Midgard.Engines.ThirdCrownPorting
{
    class PortingLogger
    {
        public static void RegisterCommands()
        {
            CommandSystem.Register( "PortingLog", AccessLevel.Administrator, new CommandEventHandler( CratePortingLog_OnCommand ) );
        }

        [Usage( "PortingLog" )]
        [Description( "Create a log file with all porting data inside" )]
        private static void CratePortingLog_OnCommand( CommandEventArgs e )
        {
            DoLog();
            e.Mobile.SendMessage( "Porting log has been generated. See the file : <runuo root>/Logs/porting.log" );
        }

        private static void DoLog()
        {
            List<Account> list = BuildList();
            list.Sort( new AccountSorter() );

            using( StreamWriter op = new StreamWriter( "Logs/porting.log" ) )
            {
                op.WriteLine( "# Porting log generated on {0}", DateTime.Now );
                op.WriteLine();
                op.WriteLine();

                foreach( Account a in list )
                {
                    LogAccount( op, a );
                }
            }
        }

        private static void LogAccount( TextWriter op, Account account )
        {
            List<Midgard2PlayerMobile> mobiles = GetChars( account );

            op.WriteLine( "Account: {0}", account.Username );
            op.WriteLine( "Mail: {0}", account.Email );
            op.WriteLine( "Created on: {0}", account.Created );
            op.WriteLine( "Ported tag: {0}", account.GetTag( "PortedAccount" ) );
            op.WriteLine( "Players: {0}", mobiles.Count );

            foreach( Midgard2PlayerMobile mobile in mobiles )
                LogMobile( op, mobile );

            op.WriteLine( "" );
        }

        private static void LogMobile( TextWriter op, Mobile player )
        {
            List<Skill> skills = new List<Skill>();
            for( int i = 0; i < player.Skills.Length; i++ )
            {
                if( player.Skills[ i ].Base > 0 )
                    skills.Add( player.Skills[ i ] );
            }

            op.WriteLine( "\tPlayer: {0}", player.Name );
            op.WriteLine( "\tCreated on: {0}", player.CreationTime );
            op.WriteLine( "\tStats: str {0} - dex {1} - int {2}", player.RawStr, player.RawDex, player.RawInt );
            op.WriteLine( "\tSkills:" );

            foreach( Skill skill in skills )
                op.WriteLine( "\t\t{0} - base {1:F2} - cap {2:F2}", skill.Name, skill.Base, skill.Cap );
        }

        private static List<Account> BuildList()
        {
            List<Account> list = new List<Account>();

            /* old c# style...
            foreach( IAccount account in Accounts.GetAccounts() )
            {
                Account a = account as Account;
                if( a != null && a.GetTag( "PortedAccount" ) != null )
                    list.Add( a );
            }
            */

            IEnumerable<Account> query = from account in Accounts.GetAccounts()
                                         where ( account != null && account is Account && ( (Account)account ).GetTag( "PortedAccount" ) != null )
                                         select (Account)account;
            list.AddRange( query );

            return list;
        }

        private static List<Midgard2PlayerMobile> GetChars( IAccount account )
        {
            List<Midgard2PlayerMobile> list = new List<Midgard2PlayerMobile>();

            for( int i = 0; i < account.Length; i++ )
            {
                Midgard2PlayerMobile m = account[ i ] as Midgard2PlayerMobile;
                if( m != null && !m.Deleted )
                    list.Add( m );
            }

            return list;
        }

        private class AccountSorter : IComparer<Account>
        {
            public int Compare( Account x, Account y )
            {
                return Insensitive.Compare( x.Username, y.Username );
            }
        }
    }
}
