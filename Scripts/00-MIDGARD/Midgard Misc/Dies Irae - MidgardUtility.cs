/***************************************************************************
 *                                  MidgardUtility.cs
 *                            		-----------------
 *  begin                	: Settembre, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 *          Classe di supporto per metodi utili
 * 
 ***************************************************************************/

using System;
using System.Reflection;
using System.Security;
using Server;
using Server.Commands;

namespace Midgard
{
    public static class MidgardUtility
    {
        public static string FormatTS( TimeSpan ts )
        {
            int totalSeconds = (int)ts.TotalSeconds;
            int seconds = totalSeconds % 60;
            int minutes = totalSeconds / 60;

            if( minutes != 0 && seconds != 0 )
                return String.Format( "{0} minute{1} and {2} second{3}", minutes, minutes == 1 ? "" : "s", seconds, seconds == 1 ? "" : "s" );
            else if( minutes != 0 )
                return String.Format( "{0} minute{1}", minutes, minutes == 1 ? "" : "s" );
            else
                return String.Format( "{0} second{1}", seconds, seconds == 1 ? "" : "s" );
        }

        public static string GetFriendlyDirectionName( Direction dir )
        {
            switch( dir )
            {
                case Direction.North:
                    return "North";
                case Direction.East:
                    return "East";
                case Direction.South:
                    return "South";
                case Direction.West:
                    return "West";
                case Direction.Up:
                    return "North West";
                case Direction.Right:
                    return "North East";
                case Direction.Down:
                    return "South East";
                case Direction.Left:
                    return "South West";
            }

            return string.Empty;
        }

        public static bool IsArtifact( Item item )
        {
            if( null == item )
                return false;

            Type t = item.GetType();
            PropertyInfo prop = null;

            try { prop = t.GetProperty( "ArtifactRarity" ); }
            catch { }

            if( null == prop || (int)( prop.GetValue( item, null ) ) <= 0 )
                return false;

            return true;
        }

        public static bool IsPlayerConstructed( Item item )
        {
            if( null == item )
                return false;

            Type t = item.GetType();
            PropertyInfo prop = null;

            try { prop = t.GetProperty( "PlayerConstructed" ); }
            catch { }

            if( null == prop || true != (bool)( prop.GetValue( item, null ) ) )
                return false;

            return true;
        }

        /// <summary>
        /// Puts spaces before type name inner-caps
        /// </summary>
        /// <param name="typeName">type name of our item</param>
        /// <returns>string with friendly name or <c>typeName</c></returns>
        public static string GetFriendlyClassName( string typeName )
        {
            string temp = typeName;

            for( int index = 1; index < temp.Length; index++ )
            {
                if( char.IsUpper( temp, index ) )
                    temp = temp.Insert( index++, " " );
            }

            return temp;
        }

        public static object InvokeParameterlessMethod( object target, string method )
        {
            object result = null;

            try
            {
                Type objectType = target.GetType();
                MethodInfo methodInfo = objectType.GetMethod( method );

                result = methodInfo.Invoke( target, null );
            }
            catch( SecurityException exc )
            {
                Console.WriteLine( "SecurityException: " + exc.Message );
            }
            return result;
        }

        public static void SendCommandDetails( Mobile player, string command )
        {
            SendCommandDescription( player, command );
            SendCommandUsage( player, command );
        }

        public static void SendCommandUsage( Mobile player, string command )
        {
            string message;
            CommandEntry entry = CommandSystem.Entries[ command ];

            if( null != entry )
            {
                MethodInfo mi = entry.Handler.Method;

                object[] attrs = mi.GetCustomAttributes( typeof( UsageAttribute ), false );

                UsageAttribute usage = attrs.Length > 0 ? attrs[ 0 ] as UsageAttribute : null;

                message = "Format: " + ( null == usage ? " - no usage" : usage.Usage );
            }
            else
                message = command + " - unknown command";

            player.SendMessage( 37, message );
        }

        public static void SendCommandDescription( Mobile player, string command )
        {
            string message;
            CommandEntry entry = CommandSystem.Entries[ command ];

            if( null != entry )
            {
                MethodInfo mi = entry.Handler.Method;

                object[] attrs = mi.GetCustomAttributes( typeof( DescriptionAttribute ), false );

                DescriptionAttribute desc = attrs.Length > 0 ? attrs[ 0 ] as DescriptionAttribute : null;

                message = command + ": " + ( null == desc ? " - no description" : desc.Description );
            }
            else
                message = command + " - unknown command";

            player.SendMessage( 37, message );
        }
    }
}