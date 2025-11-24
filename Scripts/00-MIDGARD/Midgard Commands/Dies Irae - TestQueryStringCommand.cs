using System;

using Server.Commands;
using Server.StringQueries;

namespace Server.Mobiles
{
    public class TestQueryStringCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register( "StringQuery", AccessLevel.Developer, new CommandEventHandler( TestStringQuery_OnCommand ) );
        }

        [Usage( "TestStringQuery <par1> <par2> <par3> <par4>" )]
        [Description( "Sends a string query to command user" )]
        public static void TestStringQuery_OnCommand( CommandEventArgs e )
        {
            try
            {
                StringQuery sq = new StringQuery(
                    e.GetString( 0 ),                   // top
                    e.GetBoolean( 1 ),                  // (0=disable, 1=enable)
                    (StringQueryStyle)e.GetInt32( 2 ),  // (0=disable, 1=normal, 2=numerical)
                    50,                                 // (if style 1, max text len, if style2, max numeric value)
                    e.GetString( 3 ) );                 // entry

                e.Mobile.SendStringQuery( sq );
            }
            catch( Exception exception )
            {
                Console.WriteLine( exception );
            }
        }
    }
}