using System.Collections;
using Server;
using Server.Commands;

namespace Midgard.Engines.TownHouses
{
    public class Commands
    {
        private static Hashtable m_Commands = new Hashtable();

        public static void AddCommand( string com, AccessLevel acc, TownHouseCommandHandler cch )
        {
            m_Commands[ com.ToLower() ] = cch;
            CommandSystem.Register( com, acc, new CommandEventHandler( OnCommand ) );
        }

        public static void OnCommand( CommandEventArgs e )
        {
            if( m_Commands[ e.Command.ToLower() ] == null )
                return;

            ( (TownHouseCommandHandler)m_Commands[ e.Command.ToLower() ] )( new CommandInfo( e.Mobile, e.Command, e.ArgString, e.Arguments ) );
        }
    }
}