/***************************************************************************
 *                                  Home.cs
 *                            		-------
 *  begin                	: Settembre, 2006
 *  version					: 1.0
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 * 
 * 	Info
 *  Il comando porta alle GA.
 *  
 ***************************************************************************/

using Server;
using Server.Commands;
using Server.Misc;
using Server.Mobiles;

namespace Midgard.Commands
{
    public class Home
    {
        private static Point3D HomeAOS = new Point3D( 5434, 1104, 0 );
        private static Point3D HomePreAOS = new Point3D( 5443, 1151, 0 );
        private static Point3D HomeTestCenter = new Point3D( 5826, 2183, 0 );

        public static void Initialize()
        {
            CommandSystem.Register( "Home", TestCenter.Enabled ? AccessLevel.Player : AccessLevel.Counselor, new CommandEventHandler( Home_OnCommand ) );
        }

        [Usage( "Home" )]
        [Description( "Teleports mobile and hos pets in a safe place." )]
        public static void Home_OnCommand( CommandEventArgs e )
        {
            Mobile from = e.Mobile;

            Point3D location;
            if( from.AccessLevel > AccessLevel.Player )
                location = Core.AOS ? HomeAOS : HomePreAOS;
            else
                location = HomeTestCenter;

            BaseCreature.TeleportPets( from, location, Map.Felucca );
            from.MoveToWorld( location, Map.Felucca );
            from.SendMessage( "You are at home {0}!", from.Name ?? "" );
        }
    }
}