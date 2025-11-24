/***************************************************************************
 *                               Commands.cs
 *
 *   begin                : Aprile 2012
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using Server;
using Server.Commands;
using Server.Regions;

namespace Midgard.Engines.RegionalSpawningSystem
{
    public class Commands
    {
        public static void RegisterCommands()
        {
            CommandSystem.Register( "HandleRegionalSpawn", AccessLevel.Seer, new CommandEventHandler( HandleRegionalSpawn_Callback ) );
        }

        [Usage( "HandleRegionalSpawn [<region name>]" )]
        [Description( "Start regional spawn handle system. An optional region parameter can be used." )]
        public static void HandleRegionalSpawn_Callback( CommandEventArgs e )
        {
            BaseRegion region = GetCommandData( e );
            if( region == null )
                return;

            Mobile from = e.Mobile;
            if( from == null )
                return;

            try
            {
                from.SendGump( new RegionSpawnInfoGump( region, from ) );
            }
            catch( Exception exception )
            {
                Console.WriteLine( exception );
                from.SendMessage( "An error occurred. Contact Dies Irae." );
            }
        }

        private static BaseRegion GetCommandData( CommandEventArgs args )
        {
            Mobile from = args.Mobile;

            Region reg;
            if( args.Length == 0 )
                reg = from.Region;
            else
            {
                string name = args.GetString( 0 );

                if( !from.Map.Regions.TryGetValue( name, out reg ) )
                {
                    from.SendMessage( "Could not find region '{0}'.", name );
                    return null;
                }
            }

            BaseRegion br = reg as BaseRegion;
            if( br == null )
            {
                from.SendMessage( "Invalid region '{0}'.", reg );
                return null;
            }

            return br;
        }
    }
}