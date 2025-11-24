/***************************************************************************
 *                                  GenPlayerLogs.cs
 *                            		-------------------
 *  begin                	: Settembre, 2006
 *  version					: 1.0
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 * 
 * 	Info
 *  Lista i PG nn staff esistenti su Mid 2.
 *  
 ***************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

using Server;
using Server.Accounting;
using Server.Commands;
using Server.Mobiles;

namespace Midgard.Commands
{
    public class GenPlayerLogs
    {
        #region registrazione
        public static void Initialize()
        {
            CommandSystem.Register( "GenPlayerLogs", AccessLevel.Developer, new CommandEventHandler( GenPlayerLogs_OnCommand ) );
        }
        #endregion

        #region callback
        [Usage( "GenPlayerLogs" )]
        [Description( "Generates debug logs for player instances" )]
        public static void GenPlayerLogs_OnCommand( CommandEventArgs e )
        {
            string NomeFile = "MidgardPGsInAccounts.xml";
            XmlTextWriter xmlTw = new XmlTextWriter( NomeFile, null );
            xmlTw.Formatting = Formatting.Indented;
            xmlTw.WriteStartElement( "MidgardPGsInAccounts" );
            foreach( Account a in Accounts.GetAccounts() )
            {
                if( a.AccessLevel == AccessLevel.Player )
                {
                    xmlTw.WriteStartElement( "account" );
                    xmlTw.WriteAttributeString( "ID", a.ToString() );
                    for( int i = 0; i < a.Length; i++ )
                    {
                        Mobile m = a[ i ];
                        if( m != null )
                        {
                            xmlTw.WriteStartElement( "pg" );
                            xmlTw.WriteElementString( "name", a[ i ].Name );
                            xmlTw.WriteEndElement();
                        }
                    }
                    xmlTw.WriteEndElement();
                }
            }
            xmlTw.WriteEndElement();
            xmlTw.Flush();
            xmlTw.Close();

            ListaPg_OnCommand( e );
            ListaAnzianita_OnCommand();
        }

        private static void ListaPg_OnCommand( CommandEventArgs e )
        {
            ArrayList mobileArray;
            string NomeFile = "MidgardPGs.xml";
            XmlTextWriter xmlTw = new XmlTextWriter( NomeFile, null );
            xmlTw.Formatting = Formatting.Indented;

            try
            {
                mobileArray = new ArrayList( World.Mobiles.Values );
            }
            catch
            {
                e.Mobile.SendMessage( "Errore di lettura della tabella dei Mobiles." );
                return;
            }

            xmlTw.WriteStartElement( "MidgardPGs" );
            foreach( Mobile m in mobileArray )
            {
                if( m == null || m.Deleted )
                    continue;

                Mobile pm = m as PlayerMobile;
                if( pm == null || pm.Deleted )
                    continue;
                xmlTw.WriteElementString( "pg", pm.Name );
                //				XmlTw.WriteAttributeString( "CreationTime", pm.CreationTime.ToShortDateString() );
            }
            xmlTw.WriteEndElement();
            xmlTw.Flush();
            xmlTw.Close();
        }

        private static void ListaAnzianita_OnCommand()
        {
            List<Account> accountList = new List<Account>();
            foreach( Account account in Accounts.GetAccounts() )
                accountList.Add( account );

            accountList.Sort( AccountComparer.Instance );

            using( TextWriter tw = File.CreateText( "Logs/anzianita.log" ) )
            {
                foreach( Account account in accountList )
                {
                    tw.WriteLine( "{0}\t{1}\t{2}", account.Username, ( DateTime.Now - account.Created ).TotalHours,
                                 ( account.TotalGameTime ).TotalHours );
                }
            }
        }

        private class AccountComparer : IComparer<Account>
        {
            public static readonly IComparer<Account> Instance = new AccountComparer();

            public int Compare( Account x, Account y )
            {
                if( x == null && y == null )
                    return 0;
                else if( x == null )
                    return -1;
                else if( y == null )
                    return 1;

                return Insensitive.Compare( x.Username, y.Username );
            }
        }
        #endregion
    }
}

