using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using Midgard.Engines.MidgardTownSystem;

using Server;
using Server.Mobiles;

using Utility = Midgard.Engines.MyMidgard.Utility;

namespace Midgard.Engines.MyXmlRPC
{
    public class GetPlayersStatusCommand
    {
        private static List<Mobile> m_List = new List<Mobile>();

        public static void RegisterCommands()
        {
            // http://89.97.211.236/xmlrpc?user=XmlRpcUser&pass=securexmlpass&xcmd=getStatusList
            Core.Register( "getPlayerStatus", new MyXmlEventHandler( GetStatusListOnCommand ), null );

            BuildList();
            EventSink.Login += new LoginEventHandler( OnLogin );
        }

        private static void OnLogin( LoginEventArgs e )
        {
            if( e.Mobile != null && e.Mobile.Player && !m_List.Contains( e.Mobile ) )
            {
                InvalidatePlayerList();
            }
        }

        private static string GetRgbHueFor( Mobile m )
        {
            if( m == null )
                return "";

            switch( m.AccessLevel )
            {
                case AccessLevel.Owner:
                case AccessLevel.Developer:
                case AccessLevel.Administrator: return "9999FF";
                case AccessLevel.Seer: return "00CC33";
                case AccessLevel.GameMaster: return "CC0000";
                case AccessLevel.Counselor: return "FFFF00";
                default: return "";
            }
        }

        public static void GetStatusListOnCommand( MyXmlEventArgs e )
        {
            if( Core.Debug )
            {
                Core.Pkg.LogInfoLine( "GetStatusList command called..." );
            }

            e.Exitcode = -1;

            try
            {
                e.CustomResultTree.Add( new XElement( "status", from mobile in m_List
                                                                where (mobile != null && mobile.Account != null )
                                                                select new XElement( "player",
                                                                                     new XAttribute( "name", Utility.SafeString( mobile.Name ?? "" ) ),
                                                                                     new XAttribute( "nameHue", GetRgbHueFor( mobile ) ),
                                                                                     new XAttribute( "serial", Utility.SafeString( mobile.Serial.Value.ToString() ) ),
                                                                                     new XAttribute( "account", Utility.SafeString( mobile.Account.Username ) ),
                                                                                     new XAttribute( "accessLevel", Utility.SafeString( mobile.AccessLevel.ToString() ) ),
                                                                                     new XAttribute( "guild", Utility.SafeString( GetGuildName( mobile ) ) ),
                                                                                     new XAttribute( "town", Utility.SafeString( GetTownName( mobile ) ) ),
                                                                                     new XAttribute( "karma", mobile.Karma ),
                                                                                     new XAttribute( "fame", mobile.Fame ),
                                                                                     new XAttribute( "online", mobile.NetState != null ) ) ) );
                e.Exitcode = 0;
            }
            catch( Exception warning )
            {
                e.Warnings.Add( warning );
            }
        }

        private static string GetGuildName( Mobile m )
        {
            return m.Guild == null ? string.Empty : m.Guild.Name;
        }

        private static string GetTownName( Mobile m )
        {
            TownSystem sys = TownSystem.Find( m );
            return sys != null ? (string)sys.Definition.TownName : string.Empty;
        }

        private static void BuildList()
        {
            if( m_List == null )
            {
                m_List = new List<Mobile>();
            }
            else
                m_List.Clear();

            try
            {
                var mobs = new List<Mobile>( World.Mobiles.Values );

                foreach( Mobile m in mobs )
                {
                    if( m != null && m.Account != null && m is Midgard2PlayerMobile && ( (Midgard2PlayerMobile)m ).OnlineVisible )
                    {
                        m_List.Add( m );
                    }
                }

                m_List.Sort( InternalComparer.Instance );
            }
            catch( Exception ex )
            {
                Console.WriteLine( ex );
            }
        }

        public static void InvalidatePlayerList()
        {
            BuildList();
        }

        #region Nested type: InternalComparer
        private class InternalComparer : IComparer<Mobile>
        {
            public static readonly IComparer<Mobile> Instance = new InternalComparer();

            public int Compare( Mobile x, Mobile y )
            {
                if( x == null || y == null )
                    throw new ArgumentException();

                if( x.AccessLevel > y.AccessLevel )
                    return -1;
                else if( x.AccessLevel < y.AccessLevel )
                    return 1;
                else
                    return Insensitive.Compare( x.Name, y.Name );
            }
        }
        #endregion
    }
}