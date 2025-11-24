/***************************************************************************
 *                               WebCommands.cs
 *
 *   begin                : 21 October, 2009
 *   author               :	Dies Irae - Magiusche	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

using Midgard.Engines.MyXmlRPC;
using Midgard.Misc;

using Server;
using Server.Accounting;
using Server.Misc;

using Core = Midgard.Engines.MyXmlRPC.Core;
using Utility = Midgard.Engines.MyMidgard.Utility;

namespace Midgard.Engines.RemoteAccounting
{
    public class WebCommands
    {
        #region AccountComandType enum
        public enum AccountComandType
        {
            Invalid = 0,
            Info,
            Request,
            Activate,
            Ban,
            Delete,
            ChangePassword,
            ChangeEmail
        }
        #endregion

        #region ErrorResultTypes enum
        public enum ErrorResultTypes
        {
            InvalidAdminCommand = -50,
            InvalidAccount = -100,
            InvalidAccountAlreadyExists = -101,
            InvalidAccountAlreadyPending = -102,
            InvalidAccountNotExists = -103,
            InvalidMailNull = -120,
            InvalidMailNotWellFormatted = -121,
            InvalidMailAlreadyExists = -122,
            InvalidPassword = -130,
        }
        #endregion

        public static void RegisterCommands()
        {
            // template
            // MyXmlRPC.Core.Register( "command", new MyXmlEventHandler( Callback ) );

            Core.Register( "remoteAdmin", new MyXmlEventHandler( RemoteAccounting ), null );
            Core.Register( "requestAccount", new MyXmlEventHandler( RequestAccount ), null );
        }

        public static void RequestAccount( MyXmlEventArgs e )
        {
            try
            {
                e.Exitcode = -1;

                if( Config.Debug )
                    Config.Pkg.LogInfoLine( "RequestAccount command called..." );

                var user = Utility.SafeGetKey( e.Args, "account" );
                var email = Utility.SafeGetKey( e.Args, "email" );

                if( string.IsNullOrEmpty( user ) )
                {
                    e.Exitcode = (int)ErrorResultTypes.InvalidAccount;
                    throw new ArgumentNullException( "account" );
                }
                if( string.IsNullOrEmpty( email ) )
                {
                    e.Exitcode = (int)ErrorResultTypes.InvalidMailNull;
                    throw new ArgumentNullException( "email" );
                }
                if( !Email.IsValid( email ) )
                {
                    e.Exitcode = (int)ErrorResultTypes.InvalidMailNotWellFormatted;
                    throw new ArgumentException( "email" );
                }
                if( Accounts.GetAccount( user ) != null )
                {
                    e.Exitcode = (int)ErrorResultTypes.InvalidAccountAlreadyExists;
                    throw new ArgumentException( "Account already exists.", "account" );
                }
                if( PendingAccounts.IsPending( user ) )
                {
                    e.Exitcode = (int)ErrorResultTypes.InvalidAccountAlreadyPending;
                    throw new ArgumentException( "Account is already in pending status.", "account" );
                }

                PendingAccounts.Register( new PendingAccount( user, email ) );
                e.Exitcode = 0;
                e.ReturnMessage.AppendLine( String.Format( "{0} : Account requested.", user ) );
            }
            catch( Exception warning )
            {
                e.Warnings.Add( warning );
            }
        }

        public static void RemoteAccounting( MyXmlEventArgs e )
        {
            try
            {
                e.Exitcode = -1;

                if( Config.Debug )
                    Config.Pkg.LogInfoLine( "RemoteAccounting command called..." );

                var commandType = Utility.SafeGetKey( e.Args, "acccmd", null );

                if( string.IsNullOrEmpty( commandType ) )
                {
                    e.Exitcode = (int)ErrorResultTypes.InvalidAdminCommand;
                    throw new ArgumentNullException( "acccmd" );
                }

                var handle = typeof( WebCommands ).GetMethod( "Handle" + commandType,
                                                              BindingFlags.Static | BindingFlags.Public |
                                                              BindingFlags.NonPublic );

                if( handle == null )
                {
                    e.Exitcode = (int)ErrorResultTypes.InvalidAdminCommand;
                    throw new ArgumentException( "Invalid command \"" + commandType + "\".", "acccmd" );
                }

                // All Handle...methods can use Throw to raise an exception.
                // Exit code will be automatically setted to -1 if errors occours.                
                handle.Invoke( null, new object[] { e } );

                e.Exitcode = 0;
            }
            catch( Exception warning )
            {
                e.Warnings.Add( warning );
            }
        }

        private static List<Mobile> GetMobiles( IAccount a )
        {
            var mobs = new List<Mobile>();

            for( var i = 0; i < a.Length; ++i )
            {
                var m = a[ i ];
                if( m != null && !m.Deleted )
                    mobs.Add( m );
            }

            return mobs;
        }

        private static Account VerifyAccount( MyXmlEventArgs e )
        {
            var user = Utility.SafeGetKey( e.Args, "account" );
            if( string.IsNullOrEmpty( user ) )
            {
                e.Exitcode = (int)ErrorResultTypes.InvalidAccount;
                ;
                throw new ArgumentNullException( "account" );
            }

            var account = Accounts.GetAccount( user );
            var a = account as Account;

            if( account == null || a == null )
            {
                e.Exitcode = (int)ErrorResultTypes.InvalidAccountNotExists;
                throw new ArgumentException( string.Format( "Account \"{0}\" does not exists.", user ), "account" );
            }
            return a;
        }

        #region Handlers...
        private static void HandleChangeEmail( MyXmlEventArgs e )
        {
            var a = VerifyAccount( e );

            var newEmail = Utility.SafeGetKey( e.Args, "email" );

            if( string.IsNullOrEmpty( newEmail ) )
            {
                e.Exitcode = (int)ErrorResultTypes.InvalidMailNull;
                throw new ArgumentNullException( "email" );
            }
            if( !Email.IsValid( newEmail ) )
            {
                e.Exitcode = (int)ErrorResultTypes.InvalidMailNotWellFormatted;
                throw new ArgumentException( "email" );
            }

            e.ReturnMessage.AppendLine( string.Format( "Account \"{0}\" has now a new Mail: \"{1}\"", a.Username, newEmail ) );
        }

        private static void HandleChangePassword( MyXmlEventArgs e )
        {
            var a = VerifyAccount( e );

            var newPassword = Password.GetNewPassword( 8 );
            a.SetPassword( newPassword, true );
            e.ReturnMessage.AppendLine( string.Format( "Account \"{0}\" has now a new Password: \"{1}\"", a.Username, newPassword ) );
            e.Returns.Add( "password", newPassword );
        }

        private static void HandleDelete( MyXmlEventArgs e )
        {
            var user = Utility.SafeGetKey( e.Args, "account" );
            var pend = PendingAccounts.GetPendingAccountByUser( user );
            if( pend != null && !pend.Activated )
            {
                PendingAccounts.AbortRequest( pend );
                e.ReturnMessage.AppendLine( string.Format( "Account \"{0}\" successfully removed from pending.", user ) );
                return;
            }
            var a = VerifyAccount( e );

            a.Delete();
            e.ReturnMessage.AppendLine( string.Format( "Account \"{0}\" successfully deleted.", a.Username ) );
        }

        private static void HandleBan( MyXmlEventArgs e )
        {
            var a = VerifyAccount( e );

            a.SetUnspecifiedBan( null );
            a.Banned = true;
            e.ReturnMessage.AppendLine( string.Format( "Account \"{0}\" successfully banned.", a.Username ) );
        }

        private static void HandleActivate( MyXmlEventArgs e )
        {
            var user = Utility.SafeGetKey( e.Args, "account" );
            if( string.IsNullOrEmpty( user ) )
            {
                e.Exitcode = (int)ErrorResultTypes.InvalidAccount;
                throw new ArgumentNullException( "account" );
            }

            var a = PendingAccounts.GetPendingAccountByUser( user );
            if( a == null )
            {
                e.Exitcode = (int)ErrorResultTypes.InvalidAccountNotExists;
                throw new ArgumentException( "account" );
            }

            if( Accounts.GetAccount( user ) != null )
            {
                e.Exitcode = (int)ErrorResultTypes.InvalidAccountAlreadyExists;
                throw new ArgumentException( "Account already exists.", "account" );
            }

            var pass = PendingAccounts.Activate( user );
            if( pass != null )
                e.Returns.Add( "password", pass );
            e.ReturnMessage.AppendLine( string.Format( "Account \"{0}\" successfully activated.", user ) );
        }

        private static void HandleInfo( MyXmlEventArgs e )
        {
            var user = Utility.SafeGetKey( e.Args, "account" );
            var pend = PendingAccounts.GetPendingAccountByUser( user );
            if( pend != null )
            {
                var tosendelem = pend.GetElementToSend();
                tosendelem.Name = "Pending";
                e.CustomResultTree.Add( new XElement( "Notice", tosendelem ) );
                return;
            }

            var a = VerifyAccount( e );

            e.CustomResultTree.Add( new XElement( "Notice", GetXElementAccount( a, true ) ) );
        }

        private static void HandleGetPendings( MyXmlEventArgs e )
        {
            var list = PendingAccounts.List;

            e.CustomResultTree.Add( new XElement( "Notice",
                                                  new XElement( "PendingAccounts", from a in list
                                                                                   select
                                                                                       a.GetElementToSend() ) ) );
        }

        private static void HandleSearchAccounts( MyXmlEventArgs e )
        {
            var ret = new List<Account>();
            foreach( var avv in Accounts.GetAccounts() )
            {
                if( avv.Username.IndexOf( e.Args[ "qry" ], StringComparison.InvariantCultureIgnoreCase ) > -1 )
                    ret.Add( (Account)avv );
            }
            e.CustomResultTree.Add( new XElement( "Notice", new XElement( "Accounts", from a in ret
                                                                                      select GetXElementAccount( a, false ) ) ) );
        }

        private static XElement GetXElementAccount( Account a, bool extendedinfo )
        {
            var ret = new XElement( "Account", new XAttribute( "User", a.Username ), new XAttribute( "Mail", a.Email ) );
            ret.Add( new XAttribute( "Flags", a.Flags.ToString() ) );
            ret.Add( new XAttribute( "LastLogin", a.LastLogin.ToString() ) );
            ret.Add( new XAttribute( "TotalGameTime", a.TotalGameTime.ToString() ) );

            var mobs = GetMobiles( a );
            ret.Add( new XElement( "Mobiles", from m in mobs
                                              select new XElement( "Character", new XAttribute( "Name", m.Name ), new XAttribute( "Serial", m.Serial ) ) ) );
            if( extendedinfo )
            {
                ret.Add( new XAttribute( "Created", a.Created.ToString() ) );
                ret.Add( new XAttribute( "AccessLevel", a.AccessLevel.ToString() ) );
                ret.Add( new XElement( "Comments", from c in a.Comments
                                                   select
                                                       new XElement( "Comment", new XAttribute( "AddedBy", c.AddedBy ),
                                                                     new XAttribute( "LastModified", c.LastModified.ToString() ),
                                                                     new XElement( "Content", c.Content ) ) ) );
                ret.Add( new XElement( "Tags", from t in a.Tags
                                               select new XElement( "Tag", new XAttribute( "Name", t.Name ), new XAttribute( "Value", t.Value ) ) ) );

                ret.Add( new XElement( "IPRestrictions", from i in a.IPRestrictions
                                                         select new XElement( "Ip", new XAttribute( "Value", i ) ) ) );
                ret.Add( new XElement( "LoginIPs", from l in a.LoginIPs
                                                   select new XElement( "Ip", new XAttribute( "Value", l ) ) ) );
            }
            return ret;
        }
        #endregion
    }
}
