/***************************************************************************
 *                               Commands.cs
 *
 *   begin                : 14 giugno 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;

using Server;
using Server.Commands;

namespace Midgard.Engines.Classes.VirtueChampion
{
    public class Commands
    {
        [Usage( "GenerateVirtueChampion" )]
        [Description( "Generates Mondain's Legacy world decoration." )]
        public static void GenerateChampion_OnCommand( CommandEventArgs e )
        {
            e.Mobile.SendMessage( "Generating Virtue Champion, please wait." );

            VirtueChampionSpawn spawn;

            try
            {
                if( Config.Virtues != null )
                {
                    foreach( BaseVirtue virtue in Config.Virtues )
                    {
                        // create the main spawn
                        if( virtue != null )
                        {
                            if( Core.FindItemByType( virtue.Definition.SymbolLocation, Map.Felucca, typeof( VirtueChampionSpawn ) ) == null )
                            {
                                Engines.Classes.Config.Pkg.LogInfoLine( "Generating items for virtue: {0}", virtue.Definition.Virtue );

                                spawn = new VirtueChampionSpawn( virtue.Definition.Virtue );
                                spawn.MoveToWorld( virtue.Definition.SymbolLocation, Map.Felucca );

                                Config.Pkg.LogInfoLine( "Gen spawn for {0}: ok.", virtue.Definition.Virtue );
                            }
                        }
                        else
                            Config.Pkg.LogErrorLine( "def == null" );
                    }
                }
                else
                    Config.Pkg.LogErrorLine( "Config.VirtueDefinitions == null" );
            }
            catch( Exception exception )
            {
                Console.WriteLine( exception );
            }

            /*
            AntiVirtueChampionSpawn antiSpawn;

            foreach( var def in Config.AntiVirtueDefinitions )
            {
                // create the main spawn
                if( Core.FindItemByType( def.SymbolLocation, Map.Felucca, typeof( AntiVirtueChampionSpawn ) ) == null )
                {
                    antiSpawn = new AntiVirtueChampionSpawn( def.AntiVirtue );
                    antiSpawn.MoveToWorld( def.SymbolLocation, Map.Felucca );
                    Config.Pkg.LogInfoLine( "Gen spawn for {0}: ok.", def.AntiVirtue );
                }
            }
            */
        }
    }
}
