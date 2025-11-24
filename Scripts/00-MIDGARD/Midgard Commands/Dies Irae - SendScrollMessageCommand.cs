/***************************************************************************
 *                               SendScrollMessageCommand.cs
 *
 *   begin                : 09 ottobre 2010
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;

using Server;
using Server.Commands;
using Server.Commands.Generic;
using Server.Network;

namespace Midgard.Commands
{
    public class SendScrollMessageCommand : BaseCommand
    {
        public static void Initialize()
        {
            TargetCommands.Register( new SendScrollMessageCommand() );
        }

        public SendScrollMessageCommand()
        {
            AccessLevel = AccessLevel.Counselor;
            Supports = CommandSupport.AllMobiles;
            Commands = new string[] { "SendScrollMessage" };
            ObjectTypes = ObjectTypes.Mobiles;
            Usage = "SendScrollMessage <type> <tip> <text>";
            Description = "Sends a scroll message to a targeted mobile.";
        }

        public override void Execute( CommandEventArgs e, object obj )
        {
            if( e.Length == 3 )
            {
                Mobile mob = (Mobile)obj;
                Mobile from = e.Mobile;

                try
                {
                    CommandLogging.WriteLine( from, "{0} {1} sending the scroll message to {2}: \"{3}\"",
                        from.AccessLevel, CommandLogging.Format( from ), CommandLogging.Format( mob ), e.GetString( 2 ) );

                    if( !String.IsNullOrEmpty( e.GetString( 2 ) ) )
                        mob.Send( new ScrollMessage( e.GetInt32( 0 ), e.GetInt32( 1 ), e.GetString( 2 ) ) );
                }
                catch( Exception ex )
                {
                    Console.WriteLine( ex.ToString() );
                    LogFailure( "Usage: SendScrollMessage <type> <tip> <text>" );
                }
            }
            else
            {
                LogFailure( "Usage: SendScrollMessage <type> <tip> <text>" );
            }
        }
    }

    public class SendLocalizedMessageCommand : BaseCommand
    {
        public static void Initialize()
        {
            TargetCommands.Register( new SendLocalizedMessageCommand() );
        }

        public SendLocalizedMessageCommand()
        {
            AccessLevel = AccessLevel.Counselor;
            Supports = CommandSupport.AllMobiles;
            Commands = new string[] { "SendLocalizedMessage" };
            ObjectTypes = ObjectTypes.Mobiles;
            Usage = "SendLocalizedMessage <num> <arg0> <arg1>";
            Description = "Sends a localized message message to a targeted mobile.";
        }

        public override void Execute( CommandEventArgs e, object obj )
        {
            if( e.Length >= 1 )
            {
                Mobile mob = (Mobile)obj;
                Mobile from = e.Mobile;

                try
                {
                    CommandLogging.WriteLine( from, "{0} {1} sending a localized message to {2}: \"{3}\"",
                        from.AccessLevel, CommandLogging.Format( from ), CommandLogging.Format( mob ), e.GetString( 0 ) );

                    int num;

                    if( e.Length == 1 )
                    {
                        if( int.TryParse( e.GetString( 0 ), out num ) )
                            mob.SendLocalizedMessage( num );
                    }
                    else if( e.Length == 2 )
                    {
                        if( int.TryParse( e.GetString( 0 ), out num ) )
                            mob.SendLocalizedMessage( num, e.GetString( 1 ) );
                    }
                    else if( e.Length == 3 )
                    {
                        if( int.TryParse( e.GetString( 0 ), out num ) )
                            mob.SendLocalizedMessage( num, string.Format( "{0}\t{1}", e.GetString( 1 ), e.GetString( 2 ) ) );
                    }
                }
                catch( Exception ex )
                {
                    Console.WriteLine( ex.ToString() );
                    LogFailure( "Usage: SendLocalizedMessage <num> <arg0> <arg1>" );
                }
            }
            else
            {
                LogFailure( "Usage: SendLocalizedMessage <type> <arg0> <arg1>" );
            }
        }
    }
}