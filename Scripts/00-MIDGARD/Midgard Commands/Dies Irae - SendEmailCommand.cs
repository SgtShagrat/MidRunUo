using System;
using System.Net.Mail;

using Server;
using Server.Commands;
using Server.Misc;

namespace Midgard.Commands
{
    public class SendEmailCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register( "SendEmail", AccessLevel.Administrator, new CommandEventHandler( SendEmail_OnCommand ) );
        }

        [Usage( "SendEmail <to> <content>" )]
        [Description( "Sends an email to given address" )]
        public static void SendEmail_OnCommand( CommandEventArgs e )
        {
            try
            {
                string to = e.GetString( 0 );
                string content = e.GetString( 1 );
                MailMessage msg = new MailMessage( "staff@midgardshard.it", to, "Test Mail from Midgard Staff", content );
                Email.Send( msg );
                e.Mobile.SendMessage( "An email has been sent to: {0}. Content: {1}", to, content );
            }
            catch( Exception exception )
            {
                Console.WriteLine( exception );
            }
        }
    }
}