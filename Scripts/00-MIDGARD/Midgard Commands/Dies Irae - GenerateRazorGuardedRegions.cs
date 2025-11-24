/***************************************************************************
 *                               GenerateRazorGuardedRegions.cs
 *
 *   begin                : 11 settembre 2010
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Server;
using Server.Commands;
using Server.Regions;

namespace Midgard.Commands
{
    public class GenerateRazorGuardedRegions
    {
        public static void Initialize()
        {
            CommandSystem.Register( "GenRazorGuards", AccessLevel.Developer, new CommandEventHandler( GenRazorGuards_OnCommand ) );
        }

        [Usage( "GenRazorGuards" )]
        [Description( "Generate the 'guardlines.def' file for razor." )]
        private static void GenRazorGuards_OnCommand( CommandEventArgs e )
        {
            /*
                # Papua
                5639 3095 192 223 -128 127
                5831 3237 20 30 -128 127
            */

            IOrderedEnumerable<Region> regions = from region in Region.Regions
                                                 where region is GuardedRegion && !( region is HouseRegion )
                                                 orderby region ascending
                                                 select region;

            using( StreamWriter op = new StreamWriter( "guardlines.def" ) )
            {
                op.WriteLine( "##########################################" );
                op.WriteLine( "#" );
                op.WriteLine( "#       Midgard Razor GuardedRegion Definitions" );
                op.WriteLine( "#" );
                op.WriteLine( "#       begin      : 11 settembre 2010" );
                op.WriteLine( "#       author     : Dies Irae" );
                op.WriteLine( "#       copyright  : (C) Midgard Shard" );
                op.WriteLine( "#" );
                op.WriteLine( "##########################################" );
                op.WriteLine( "" );

                foreach( Region region in regions )
                {
                    op.WriteLine( "# {0}", region.Name );

                    foreach( Rectangle3D r in region.Area )
                    {
                        int minZ = Math.Min( r.Start.Z, r.End.Z );
                        int maxZ = Math.Max( r.Start.Z, r.End.Z );

                        op.WriteLine( "{0} {1} {2} {3} {4} {5}", r.Start.X, r.Start.Y, r.Width, r.Height, minZ, maxZ );
                    }

                    op.WriteLine( "" );
                }
            }
        }
    }
}