/***************************************************************************
 *                               PendingAccounts.cs
 *
 *   begin                : 11 October, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;

using Midgard.Misc;

using Server;
using Server.Accounting;

namespace Midgard.Engines.RemoteAccounting
{
    public class PendingAccounts
    {
        private static string m_PendingAccountsFile = Path.Combine( Core.BaseDirectory, "Saves/Accounts/pendingAccounts.xml" );
        private static List<PendingAccount> m_List = new List<PendingAccount>();
        private static XDocument m_Document;

        public static List<PendingAccount> List { get { return m_List; } }

        internal static void Load()
        {
            int count = 0;
            int justActivated = 0;

            Config.Pkg.LogInfoLine( "Loading pending accounts:" );

            if( File.Exists( m_PendingAccountsFile ) )
            {
                try
                {
                    using( var stream = new FileStream( m_PendingAccountsFile, FileMode.Open, FileAccess.Read ) )
                    {
                        var reader = XmlReader.Create( stream );
                        m_Document = XDocument.Load( reader );
                    }

                    lock( m_List )
                        foreach( var elem in m_Document.Element( "Pendings" ).Elements( "Account" ) )
                        {
                            var account = new PendingAccount( elem );
                            if( account.Activated )
                                justActivated++;
                            else
                                m_List.Add( account );
                        }
                }
                catch( Exception ex )
                {
                    Config.Pkg.LogError( ex );
                    Config.Pkg.LogWarningLine( "Il file di salvataggio verrà ricreato con le informazioni presenti." );
                    Config.Pkg.LogWarning( " Backup del file di salvataggio..." );
                    try
                    {
                        File.Move( m_PendingAccountsFile, m_PendingAccountsFile + "_ERROR" + DateTime.Now.ToString( "yyyyMMddHHmmss" ) + ".bkp" );
                        Config.Pkg.LogWarningLine( "done!" );
                    }
                    catch( Exception mex )
                    {
                        Config.Pkg.LogErrorLine( mex.GetType().Name + ". Inpossibile effettuare backup." );
                    }
                    m_Document = null;
                }
            }
            if( m_Document == null )
                m_Document = new XDocument( new XDeclaration( "1.0", "utf-8", "yes" ), new XElement( "Pendings" ) );

            Config.Pkg.LogInfoLine( "Loading pending accounts: Loaded {0} accounts.", count );

            if( justActivated > 0 )
            {
                Config.Pkg.LogInfoLine( "Just activated accounts: {0}/{1}", justActivated, count );
                Config.Pkg.LogInfoLine( " Activated accounts has been removed from file." );
            }
        }

        public static bool Register( PendingAccount account )
        {
            lock( m_List )
                if( !m_List.Contains( account ) && account.IsValid )
                {
                    m_Document.Element( "Pendings" ).Add( account.SourceXElement );
                    m_List.Add( account );
                    return true;
                }
            return false;
        }

        public static void Store()
        {
            if( !Directory.Exists( Path.GetDirectoryName( m_PendingAccountsFile ) ) )
                Directory.CreateDirectory( Path.GetDirectoryName( m_PendingAccountsFile ) );

            m_Document.Save( m_PendingAccountsFile );
        }

        public static string Activate( string user )
        {
            PendingAccount pending = GetPendingAccountByUser( user );
            if( pending == null )
            {
                Config.Pkg.LogErrorLine( "Error: pending account null for user {0}", user );
                return null;
            }

            Account a = null;
            if( pending.IsValid )
                a = new Account( pending.User, "...", pending.Mail );
            else
            {
                Config.Pkg.LogErrorLine( "Error: pending account invalid!" );
                return null;
            }
            if( a != null )
            {
                var newpassword = Password.GetNewPassword( 8 );
                a.SetPassword( newpassword, true );

                pending.Activated = true; //cause DESTROY :-)

                Config.Pkg.LogInfoLine( "Account activated: {0}", a.Username );
                m_List.Remove( pending );

                Store();

                return newpassword;
            }

            Config.Pkg.LogErrorLine( "Error: account to activate failed on creation." );

            return null;
        }

        internal static void AbortRequest( PendingAccount pend )
        {
            lock( m_List )
            {
                pend.Destroy();
                m_List.Remove( pend );
            }
        }

        public static PendingAccount GetPendingAccountByUser( string user )
        {
            lock( m_List )
                foreach( PendingAccount account in m_List )
                {
                    if( Insensitive.Equals( account.User, user ) )
                        return account;
                }

            return null;
        }

        public static bool IsPending( string user )
        {
            if( m_List == null )
                return false;

            lock( m_List )
                foreach( PendingAccount account in m_List )
                {
                    if( Insensitive.Equals( account.User, user ) )
                        return true;
                }

            return false;
        }
    }
}
