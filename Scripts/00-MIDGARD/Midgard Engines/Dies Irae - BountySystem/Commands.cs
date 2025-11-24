/***************************************************************************
 *                               Commands.cs
 *
 *   begin                : 03 October, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using Server;
using Server.Commands;

namespace Midgard.Engines.BountySystem
{
    public class Commands
    {
        internal static void RegisterCommands()
        {
            CommandSystem.Register( "Bounties", AccessLevel.GameMaster, new CommandEventHandler( Bounties_OnCommand ) );
        }

        [Usage( "Bounties" )]
        [Description( "Manages the global bounty list." )]
        public static void Bounties_OnCommand( CommandEventArgs e )
        {
            e.Mobile.SendGump( new BountyBoardGump( e.Mobile, null ) );
        }
    }
}