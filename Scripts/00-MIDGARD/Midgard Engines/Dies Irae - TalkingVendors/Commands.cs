using System;
using System.Collections.Generic;
using Server;
using Server.Commands;

namespace Midgard.Engines.TalkingVendors
{
    internal class Commands
    {
        public static void RegisterCommands()
        {
            CommandSystem.Register( "ShowTypeListTable", AccessLevel.Developer, new CommandEventHandler( ShowTable_OnCommand ) );
        }

        [Usage( "ShowTypeListTable" )]
        [Description( "Returns the report table for TalkingVendorsSystem" )]
        public static void ShowTable_OnCommand( CommandEventArgs e )
        {
            Console.WriteLine( "TalkingVendorsSystem.TypeList" );
            foreach( KeyValuePair<Type, string> entry in Core.TypeList )
            {
                Console.WriteLine( "{0} -> {1}", entry.Key, entry.Value );
            }
            Console.WriteLine( "" );
            Console.WriteLine( "" );

            Console.WriteLine( "TalkingVendorsSystem.SpeechTable" );
            foreach( KeyValuePair<string, Dictionary<string, string>> entry in Core.SpeechTable )
            {
                Console.WriteLine( "{0} -> #{1} entries", entry.Key, entry.Value.Count );
            }
            Console.WriteLine( "" );
            Console.WriteLine( "" );
        }
    }
}