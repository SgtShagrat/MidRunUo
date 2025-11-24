/*
Description: Creates playerwealth.html in your shard's root folder. This file contains a
	breakdown of all player's wealth on your shard by account. The report 'walks' player's
	backpacks and bank boxes as well as items in houses they own recording all gold and checks 
	for the account.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using Midgard.Engines.MyXmlRPC;

using Server;
using Server.Accounting;
using Server.Commands;
using Server.Items;
using Server.Multis;

using Core = Midgard.Engines.MyXmlRPC.Core;

namespace Midgard.Commands
{
    public class PlayerWealth
    {
        public static void Initialize()
        {
            CommandSystem.Register( "PlayerWealth", AccessLevel.Developer, new CommandEventHandler( PlayerWealth_OnCommand ) );

            // http://89.97.211.236/xmlrpc?user=XmlRpcUser&pass=securexmlpass&xcmd=playerWealth
            Core.Register( "playerWealth", new MyXmlEventHandler( PlayerWealthOnCommand ), null );
        }

        public static void PlayerWealthOnCommand( MyXmlEventArgs e )
        {
            if( Core.Debug )
            {
                Core.Pkg.LogInfoLine( "PlayerWealthOnCommand command called..." );
            }

            e.Exitcode = -1;

            try
            {
                Update();

                e.CustomResultTree.Add( new XElement( "notice", new XElement( "wealth",
                                                                           new XAttribute( "generated", DateTime.Now.ToString() ),
                                                                           new XAttribute( "accounts", m_Totalaccounts.ToString() ),
                                                                           new XAttribute( "chars", m_Totalchars.ToString() ),
                                                                           new XAttribute( "homes", m_Totalhomes.ToString() ),
                                                                           new XAttribute( "shardgrandtotal", m_Shardtotal.ToString() ),
                                                                           new XElement( "infos", from ai in m_AccountWealth
                                                                                                  select new XElement( "info",
                                                                                                                      new XAttribute( "user", ai.Acct.Username ),
                                                                                                                      new XAttribute( "gold", ai.Wealth.TotalGold ),
                                                                                                                      new XAttribute( "checks", ai.Wealth.TotalChecks ),
                                                                                                                      new XAttribute( "grandtotal", ai.GrandTotal ) ) ) ) ) );
                e.Exitcode = 0;
            }
            catch( Exception warning )
            {
                e.Warnings.Add( warning );
            }
        }

        private static List<AccountInfo> m_AccountWealth = new List<AccountInfo>();

        private static long m_Shardtotal = 0;
        private static long m_Totalnotreportedamt = 0;
        private static uint m_Totalhomes = 0;
        private static uint m_Totalnotreported = 0;
        private static uint m_Totalaccounts = 0;
        private static uint m_Totalchars = 0;

        private static void Update()
        {
            m_AccountWealth = new List<AccountInfo>();

            AccountInfo info = new AccountInfo();

            info.GrandTotal = 0;

            WealthInfo backpack = new WealthInfo();
            WealthInfo bank = new WealthInfo();
            WealthInfo home = new WealthInfo();

            foreach( Account a in Accounts.GetAccounts() )
            {
                if( a == null || a.Username == null )
                    continue;

                m_Totalaccounts++;
                info.Acct = a;
                info.Wealth.TotalGold = 0;
                info.Wealth.TotalChecks = 0;

                backpack.TotalGold = 0;
                backpack.TotalChecks = 0;
                bank.TotalGold = 0;
                bank.TotalChecks = 0;
                home.TotalGold = 0;
                home.TotalChecks = 0;

                for( int i = 0; i < a.Length; i++ ) // First record gold in player's bank/backpack
                {
                    Mobile cm = a[ i ];
                    if( cm == null )
                        continue;

                    m_Totalchars++;

                    if( cm.Backpack != null )
                        backpack = SearchContainer( cm.Backpack );

                    if( cm.BankBox != null )
                        bank = SearchContainer( cm.BankBox );

                    info.Wealth.TotalGold += backpack.TotalGold;
                    info.Wealth.TotalChecks += backpack.TotalChecks;
                    info.Wealth.TotalGold += bank.TotalGold;
                    info.Wealth.TotalChecks += bank.TotalChecks;
                }

                List<BaseHouse> allHouses = new List<BaseHouse>( 2 );
                for( int i = 0; i < a.Length; ++i ) // Now houses they own
                {
                    Mobile mob = a[ i ];

                    if( mob != null )
                        allHouses.AddRange( BaseHouse.GetHouses( mob ) );
                }

                foreach( BaseHouse house in allHouses )
                {
                    m_Totalhomes++;
                    foreach( IEntity entity in house.GetHouseEntities() )
                    {
                        if( entity is Item && !( (Item)entity ).Deleted )
                        {
                            Item item = (Item)entity;
                            if( item is Gold )
                                home.TotalGold += item.Amount;
                            else if( item is BankCheck )
                                home.TotalChecks += ( (BankCheck)item ).Worth;
                            else if( item is Container )
                                home = SearchContainer( (Container)item, home );
                        }
                    }
                }

                info.Wealth.TotalGold += home.TotalGold;
                info.Wealth.TotalChecks += home.TotalChecks;
                info.GrandTotal = info.Wealth.TotalGold + info.Wealth.TotalChecks;
                m_Shardtotal += info.GrandTotal;

                if( info.GrandTotal < 10000 )
                {
                    m_Totalnotreportedamt += info.GrandTotal;
                    m_Totalnotreported++;
                }
                else
                    m_AccountWealth.Add( info );
            }

            m_AccountWealth.Sort( new SortArray() );
        }

        private static void Report()
        {
            using( StreamWriter op = new StreamWriter( "playerwealth.html" ) )
            {
                op.WriteLine( "<html><body><strong>Player Wealth report generated on {0}</strong>", DateTime.Now );
                op.WriteLine( "<br/><strong>Total Accounts: {0}</strong>", m_Totalaccounts );
                op.WriteLine( "<br/><strong>Total Characters: {0}</strong>", m_Totalchars );
                op.WriteLine( "<br/><strong>Total Houses: {0}</strong>", m_Totalhomes );
                op.WriteLine( "<br/><strong>Total Gold/Checks: {0: ##,###,###,###}</strong>", m_Shardtotal );
                op.WriteLine( "<br/><br/>" );
                op.WriteLine( "<table width=\"500\"  border=\"2\" bordercolor=\"#FFFFFF\" bgcolor=\"#DEB887\"<td colspan=\"5\"><div align=\"center\"><font color=\"#8B0000\" size=\"+2\"><strong>Player Wealth</strong></font></div></td>" );
                op.WriteLine( "<tr bgcolor=\"#667C3F\"><font color=\"#FFFFFF\" size=\"+1\"><td align=\"center\">Account</td><td width=\"100\" align=\"right\">Gold</td><td width=\"100\" align=\"right\">Checks</td><td width=\"100\" align=\"right\">Total</td><td width=\"100\" align=\"right\">% of shard</td></tr></font>" );
                double percent;
                foreach( AccountInfo ai in m_AccountWealth )
                {
                    percent = ai.GrandTotal / (double)m_Shardtotal * 100.00f;
                    op.WriteLine( "<tr bgcolor=\"#{5}\"><td align=\"center\">{0}</td><td align=\"Right\">{1: ##,###,###,##0}</td><td align=\"Right\">{2: ##,###,###,##0}</td><td align=\"Right\">{3: ##,###,###,##0}</td><td align=\"Right\">{4: ##0.00}%</td></tr></div>", ai.Acct.Username, ai.Wealth.TotalGold, ai.Wealth.TotalChecks, ai.GrandTotal, percent, percent > 0.70 ? ( percent > 3.5 ? "DC143C" : ( percent > 1.0 ? "B22222" : "F08080" ) ) : ( percent < 0.25 ? ( percent < 0.01 ? "7FFFD4" : "90EE90" ) : "F0E68C" ) );
                }
                percent = m_Totalnotreportedamt / (double)m_Shardtotal * 100.00f;
                op.WriteLine( "<tr bgcolor=\"#D3D3D3\"><td align=\"center\" colspan=\"3\">{0} accounts < 10000 not reported.</td><td align=\"Right\">{1: ##,###,###,##0}</td><td align=\"Right\">{2: ##0.00}%</td></tr>", m_Totalnotreported, m_Totalnotreportedamt, percent );
                op.WriteLine( "</table></body></html>" );
            }
        }

        [Usage( "PlayerWealth" )]
        [Description( "Creates the file playerwealth.html, which is a report of all gold/checks for players on shard." )]
        public static void PlayerWealth_OnCommand( CommandEventArgs e )
        {
            Mobile m = e.Mobile;

            Update();
            Report();

            m.SendMessage( "Total accounts processed: {0}", m_Totalaccounts );
        }

        private static WealthInfo SearchContainer( Container pack, WealthInfo w )
        {
            WealthInfo w1;
            w1.TotalGold = 0;
            w1.TotalChecks = 0;
            w1 = SearchContainer( pack );

            w.TotalGold += w1.TotalGold;
            w.TotalChecks += w1.TotalChecks;
            return w;
        }

        private static WealthInfo SearchContainer( Container pack )
        {
            WealthInfo w;
            w.TotalGold = 0;
            w.TotalChecks = 0;
            if( pack == null )
                return w;

            List<Item> packlist = pack.Items;
            foreach( Item item in packlist )
            {
                if( item == null || item.Deleted )
                    continue;

                if( item is Container )
                    w = SearchContainer( (Container)item, w );
                else if( item is Gold )
                    w.TotalGold += item.Amount;
                else if( item is BankCheck )
                    w.TotalChecks += ( (BankCheck)item ).Worth;
            }
            return w;
        }

        private struct AccountInfo
        {
            public Account Acct;
            public WealthInfo Wealth;
            public long GrandTotal;
        }

        private struct WealthInfo
        {
            public long TotalGold;
            public long TotalChecks;
        }

        private sealed class SortArray : IComparer<AccountInfo>
        {
            public int Compare( AccountInfo x, AccountInfo y )
            {
                if( x.GrandTotal == y.GrandTotal )
                    return 0;

                return ( ( x.GrandTotal > y.GrandTotal ) ? -1 : 1 );
            }
        }
    }
}