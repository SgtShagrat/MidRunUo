/***************************************************************************
 *                               Dies Irae - GetPlayerSerialsWebCommand.cs
 *
 *   begin                : 14 November, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using Server;
using Server.Accounting;

using Utility = Midgard.Engines.MyMidgard.Utility;

namespace Midgard.Engines.MyXmlRPC
{
    public class GetAccountSerialsCommand
    {
        public static void RegisterCommands()
        {
            // http://89.97.211.236/xmlrpc?user=XmlRpcUser&pass=securexmlpass&xcmd=getAccountSerials&xargacc="pippo"
            Core.Register( "getAccountSerials", new MyXmlEventHandler( GetAccountSerialsOnCommand ), null );
        }

        public static void GetAccountSerialsOnCommand( MyXmlEventArgs e )
        {
            if( Core.Debug )
                Core.Pkg.LogInfoLine( "getAccountSerials command called..." );

            string user = Utility.SafeGetKey( e.Args, "account" );
            if( string.IsNullOrEmpty( user ) )
            {
                e.Exitcode = (int)ErrorResultTypes.InvalidAccount;
                throw new ArgumentNullException( "account" );
            }

            IAccount account = Accounts.GetAccount( user );
            if( account == null )
            {
                e.Exitcode = (int)ErrorResultTypes.InvalidAccount;
                throw new ArgumentNullException( "account" );
            }

            e.Exitcode = -1;

            try
            {
                e.CustomResultTree.Add( new XElement( "notice", from mob in GetPlayers( account )
                                                                where mob != null
                                                                select new XElement( "id", mob.Serial.Value ) ) );

                e.Exitcode = 0;
            }
            catch( Exception warning )
            {
                e.Warnings.Add( warning );
            }
        }

        private static List<Mobile> GetPlayers( IAccount a )
        {
            List<Mobile> list = new List<Mobile>();
            for( int i = 0; i < a.Length; i++ )
            {
                if( a[ i ] != null && !a[ i ].Deleted )
                    list.Add( a[ i ] );
            }

            return list;
        }
    }
}