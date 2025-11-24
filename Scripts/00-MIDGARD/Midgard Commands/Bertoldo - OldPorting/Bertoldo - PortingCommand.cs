#region AuthorHeader
//   Bertoldo
//	 29/07/2006
//
//  
//
#endregion AuthorHeader

using System;
using System.IO;
using Server;
using Server.Commands;
using Server.Gumps;

namespace Midgard.Engines.OldPorting
{
    public class PortingCommand
    {
        private static bool Enabled = false;

        public static void RegisterCommands()
        {
            if( Enabled )
                CommandSystem.Register( "PortingLog", AccessLevel.Administrator, new CommandEventHandler( PortingLog_OnCommand ) );
        }

        [Usage( "PortingLog <account>" )]
        public static void PortingLog_OnCommand( CommandEventArgs e )
        {
            string testogump = "";

            Mobile from = e.Mobile;

            string filePath = @"porting/" + e.GetString( 0 ) + ".xml";

            if( !File.Exists( filePath ) )
            {
                from.SendMessage( "Non esiste un porting per questo account." );

                return;
            }

            using( StreamReader sr = File.OpenText( filePath ) )
            {
                try
                {
                    String input;
                    while( ( input = sr.ReadLine() ) != null )
                    {
                        input = input.Replace( "<", "*" );
                        input = input.Replace( ">", "*" );
                        testogump += input + "<br>";
                    }

                    sr.Close();
                }
                catch { from.SendMessage( "Errore imprevisto." ); }
            }
            from.SendGump( new NoticeGump( 1060635, 30720, testogump,
                0xFFC000, 300, 400, null, null ) );//1060635

            return;
        }
    }
}