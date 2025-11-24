using Server;

namespace Midgard.Engines.TownHouses
{
    public delegate void TownHouseCommandHandler( CommandInfo info );

    public class CommandInfo
    {
        public Mobile Mobile { get; private set; }
        public string Command { get; private set; }
        public string ArgString { get; private set; }
        public string[] Arguments { get; private set; }

        public CommandInfo( Mobile m, string com, string args, string[] arglist )
        {
            Mobile = m;
            Command = com;
            ArgString = args;
            Arguments = arglist;
        }

        public string GetString( int num )
        {
            if( Arguments.Length > num )
                return Arguments[ num ];

            return "";
        }
    }
}
