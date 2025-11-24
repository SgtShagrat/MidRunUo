/***************************************************************************
 *                               NameManager.cs
 *
 *   begin                : 29 December, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;

using Midgard.Items;

using Server;
using Server.Accounting;
using Server.Commands;
using Server.Gumps;
using Server.Mobiles;

namespace Midgard.Engines.NameManager
{
    public class Core
    {
        private static readonly Dictionary<string, List<NameInfo>> m_NameInfosDict = new Dictionary<string, List<NameInfo>>();

        [Usage( "ListPlayerNames" )]
        [Description( "List in a file all players name and infos." )]
        public static void ListPlayerNames_OnCommand( CommandEventArgs e )
        {
            Mobile from = e.Mobile;
            if( from == null )
                return;

            VerifyNames( false );

            List<KeyValuePair<string, List<NameInfo>>> invalids = new List<KeyValuePair<string, List<NameInfo>>>();

            foreach( KeyValuePair<string, List<NameInfo>> keyValuePair in m_NameInfosDict )
            {
                if( keyValuePair.Value != null && keyValuePair.Value.Count > 1 )
                    invalids.Add( keyValuePair );
            }

            using( StreamWriter op = new StreamWriter( "Logs/names.log" ) )
            {
                op.WriteLine( "Found {0} invalid names.", invalids.Count );
                op.WriteLine();

                foreach( var keyValuePair in invalids )
                {
                    op.WriteLine( "Invalid name: {0}", keyValuePair.Key );

                    List<NameInfo> invalidInfos = keyValuePair.Value;
                    foreach( var invalidInfo in invalidInfos )
                        op.WriteLine( "\tSerial 0x{0:X4} -  Account {1} - CreationTime {2}", invalidInfo.Owner.Serial.Value, invalidInfo.Owner.Account.Username, invalidInfo.Owner.CreationTime );
                }

                op.WriteLine();
                op.WriteLine( "Found {0} names.", m_NameInfosDict.Count );
                op.WriteLine();

                List<string> allNames = new List<string>();
                foreach( var name in m_NameInfosDict )
                    allNames.Add( name.Key );
                allNames.Sort();
                op.WriteLine( "All Names:" );
                foreach( string name in allNames )
                    op.WriteLine( "\t{0}", name );
                op.WriteLine();

                op.WriteLine( "Details:" );
                foreach( KeyValuePair<string, List<NameInfo>> keyValuePair in m_NameInfosDict )
                {
                    op.WriteLine( "Name: {0} (instances {1})", keyValuePair.Key, keyValuePair.Value.Count );
                    List<NameInfo> infos = keyValuePair.Value;
                    foreach( var info in infos )
                        op.WriteLine( "\tSerial 0x{0:X4} -  Account {1} - CreationTime {2}", info.Owner.Serial.Value, info.Owner.Account.Username, info.Owner.CreationTime );
                }

                op.WriteLine();
            }
        }

        [Usage( "VerifyPlayerNames" )]
        [Description( "Fix all players duplicated names." )]
        public static void VerifyPlayerNames_OnCommand( CommandEventArgs e )
        {
            VerifyNames( true );
        }

        public static void OnLogin( LoginEventArgs e )
        {
            Mobile mobile = e.Mobile;
            if( mobile == null )
                return;

            if( m_NameInfosDict.ContainsKey( mobile.Name ) )
                FixInvalidInfos( m_NameInfosDict[ mobile.Name ] );
        }

        internal static void VerifyNames( bool fix )
        {
            IEnumerable<Mobile> players = BuildList();

            m_NameInfosDict.Clear();

            foreach( Mobile mobile in players )
            {
                if( m_NameInfosDict.ContainsKey( mobile.Name ) )
                {
                    List<NameInfo> infos = m_NameInfosDict[ mobile.Name ];
                    infos.Add( new NameInfo( mobile ) );
                }
                else
                {
                    List<NameInfo> infos = new List<NameInfo>();
                    infos.Add( new NameInfo( mobile ) );
                    m_NameInfosDict.Add( mobile.Name, infos );
                }
            }

            if( fix )
            {
                foreach( KeyValuePair<string, List<NameInfo>> keyValuePair in m_NameInfosDict )
                {
                    if( keyValuePair.Value != null && keyValuePair.Value.Count > 1 )
                        FixInvalidInfos( keyValuePair.Value );
                }
            }
        }

        private static void FixInvalidInfos( List<NameInfo> infos )
        {
            if( infos != null && infos.Count > 1 )
            {
                infos.Sort( InternalComparer.Instance ); // sorted by creation date

                infos.RemoveAt( 0 ); // remove the first one which is valid

                foreach( NameInfo nameInfo in infos )
                {
                    Mobile player = nameInfo.Owner;

                    if( player == null || player.AccessLevel != AccessLevel.Player )
                        continue;

                    FixInvalidName( player );

                    string message = string.Format( "Your player has been renamed to \"{0}\" because the old name was not valid.<bb>" +
                        "A player older then your with a such name exists on Midgard.<br>" +
                        "Please contact any Midgard Admin if you think that this rename should be manually fixed.<br>The Midgard Third Crown Staff", player.Name );

                    if( player.NetState != null )
                        player.SendGump( new NoticeGump( 1060635, 30720, message, 0xFFC000, 420, 280, new NoticeGumpCallback( CloseNoticeGumpCallBack ), null ) );
                    else
                        player.AddToBackpack( new NoticeScroll( message, "Thou have been renamed!" ) );
                }
            }
        }

        private static void CloseNoticeGumpCallBack( Mobile from, object state )
        {
        }

        private static void FixInvalidName( Mobile mobile )
        {
            bool female = mobile.Female;

            string name = GetRandomName( female );

            while( m_NameInfosDict.ContainsKey( name ) )
                name = GetRandomName( female );

            Account a = mobile.Account as Account;
            if( a != null )
                a.Comments.Add( new AccountComment( "NameManager", string.Format( "mobile {0} renamed to {1}", mobile.Name, name ) ) );

            AppendLog( mobile.Name, name, mobile );

            mobile.Name = name;
        }

        private static string GetRandomName( bool female )
        {
            return female ? NameList.RandomName( "female" ) : NameList.RandomName( "male" );
        }

        private static void AppendLog( string oldName, string newName, Mobile mobile )
        {
            string message = String.Format( "Warning: Renamed mobile {0} (account {1}, created on {2}) to {3}.",
                                           oldName, mobile.Account.Username, mobile.CreationTime, newName );

            Console.WriteLine( message );

            try
            {
                using( StreamWriter op = new StreamWriter( "Logs/forcedRenames.log", true ) )
                {
                    op.WriteLine( "{0}\t{1}", DateTime.Now, message );
                    op.WriteLine();
                }
            }
            catch
            {
            }
        }

        private static IEnumerable<Mobile> BuildList()
        {
            List<Mobile> list = new List<Mobile>();

            try
            {
                List<Mobile> mobs = new List<Mobile>( World.Mobiles.Values );

                foreach( Mobile m in mobs )
                {
                    if( m != null && m is Midgard2PlayerMobile )
                        list.Add( m );
                }
            }
            catch( Exception ex )
            {
                Console.WriteLine( ex );
            }

            return list;
        }

        private class InternalComparer : IComparer<NameInfo>
        {
            public static readonly IComparer<NameInfo> Instance = new InternalComparer();

            #region IComparer<NameInfo> Members
            public int Compare( NameInfo x, NameInfo y )
            {
                if( x == null || y == null )
                    throw new ArgumentException();

                if( Insensitive.Compare( x.Name, y.Name ) == 0 )
                    return x.CreationTime.CompareTo( y.CreationTime );
                else
                    return Insensitive.Compare( x.Name, y.Name );
            }
            #endregion
        }

        private class NameInfo
        {
            private readonly Mobile m_Owner;

            public NameInfo( Mobile m )
            {
                m_Owner = m;
            }

            public DateTime CreationTime
            {
                get { return Owner.CreationTime; }
            }

            public string Name
            {
                get { return Owner.Name; }
            }

            public Mobile Owner
            {
                get { return m_Owner; }
            }
        }
    }
}