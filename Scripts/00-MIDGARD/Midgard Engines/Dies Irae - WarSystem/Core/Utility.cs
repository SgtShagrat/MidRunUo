/***************************************************************************
 *                               Utility.cs
 *
 *   begin                : 04 marzo 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;

using Server;
using Server.Mobiles;
using Server.Network;

namespace Midgard.Engines.WarSystem
{
    public static class Utility
    {
        public static void ClearAggressors( Mobile m )
        {
            for( int i = 0; i < m.Aggressors.Count; i++ )
            {
                AggressorInfo c = m.Aggressors[ i ];
                m.RemoveAggressor( c.Attacker );
            }
        }

        public static void ClearAggressed( Mobile m )
        {
            if( m == null || m.Aggressed == null || m.Aggressed.Count == 0 )
                return;

            for( int i = 0; i < m.Aggressed.Count; i++ )
            {
                AggressorInfo info = m.Aggressed[ i ];
                if( info == null || info.Defender == null )
                    continue;

                m.RemoveAggressed( info.Defender );
            }
        }

        #region broadcast
        public static void Broadcast( WarTeam team, int hue, string format, params object[] args )
        {
            List<NetState> list = NetState.Instances;

            foreach( NetState netState in list )
            {
                if( netState != null && netState.Mobile != null )
                {
                    if( Find( netState.Mobile ) == team )
                        netState.Mobile.SendAsciiMessage( hue, format, args );
                }
            }

            Console.WriteLine( "War System: " + String.Format( format, args ) );
        }

        public static void Broadcast( string message )
        {
            World.Broadcast( 37, true, message );

            Console.WriteLine( "War System: " + message );
        }

        public static void Broadcast( string format, params object[] args )
        {
            Broadcast( String.Format( format, args ) );
        }

        public static void BroadcastToStaff( int hue, string format, params object[] args )
        {
            List<NetState> list = NetState.Instances;

            foreach( NetState netState in list )
            {
                if( netState != null && netState.Mobile != null && netState.Mobile.AccessLevel > AccessLevel.Player )
                    netState.Mobile.SendAsciiMessage( hue, format, args );
            }

            Console.WriteLine( "War System: " + String.Format( format, args ) );
        }

        public static void BroadcastToStaff( string message )
        {
            BroadcastToStaff( 0x35, message );
        }

        public static void BroadcastToStaff( string format, params object[] args )
        {
            BroadcastToStaff( 0x35, String.Format( format, args ) );
        }
        #endregion

        #region find...
        public static WarTeam Find( Mobile mob )
        {
            return Find( mob, false, false );
        }

        public static WarTeam Find( Mobile mob, bool inherit )
        {
            return Find( mob, inherit, false );
        }

        public static WarTeam Find( Mobile mob, bool inherit, bool creatureAllegiances )
        {
            if( mob is Midgard2PlayerMobile )
                return ( (Midgard2PlayerMobile)mob ).WarTeam;

            if( inherit && mob is BaseCreature )
            {
                BaseCreature bc = (BaseCreature)mob;

                if( bc.Controlled )
                    return Find( bc.ControlMaster, false );
                else if( bc.Summoned )
                    return Find( bc.SummonMaster, false );
            }

            return null;
        }
        #endregion

        public static bool FindItem( int x, int y, int z, Map map, Item test )
        {
            return FindItem( new Point3D( x, y, z ), map, test );
        }

        public static bool FindItem( Point3D p, Map map, Item test )
        {
            IPooledEnumerable eable = map.GetItemsInRange( p );

            foreach( Item item in eable )
            {
                if( item.Z == p.Z && item.ItemID == test.ItemID )
                {
                    eable.Free();
                    return true;
                }
            }

            eable.Free();
            return false;
        }
    }
}