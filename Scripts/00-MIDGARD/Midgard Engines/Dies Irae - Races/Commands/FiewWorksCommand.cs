/***************************************************************************
 *                               FiewWorksCommand.cs
 *                            -------------------
 *   begin                : 21 September, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.IO;
using Server;
using Server.Commands;

namespace Midgard.Engines.Races
{
    public class FiewWorksCommand
    {
        public const double DelayFireWorks = 5.0;

        public static void RegisterCommands()
        {
            CommandSystem.Register( "Fuochi", AccessLevel.Player, new CommandEventHandler( FireWorks_OnCommand ) );
        }

        [Usage( "Fuochi" )]
        [Description( "Crea effetti visivi attorno al folletto" )]
        public static void FireWorks_OnCommand( CommandEventArgs e )
        {
            if( !Config.FireWorksEnabled )
            {
                e.Mobile.SendMessage( "Questo potere e' stato disabilitato." );
                return;
            }

            if( !e.Mobile.Alive )
            {
                e.Mobile.SendMessage( "Non puoi usare questo comando in questo stato." );
                return;
            }

            try
            {
                Mobile from = e.Mobile;

                if( from != null )
                {
                    if( e.Length == 0 )
                    {
                        if( from.Race == Core.Sprite )
                        {
                            if( from.CanBeginAction( typeof( FiewWorksCommand ) ) )
                            {
                                DoFireworks( from );
                            }
                            else
                            {
                                from.SendMessage( "Non puoi usare ancora questo potere." );
                            }
                        }
                        else
                        {
                            from.SendMessage( "Non puoi usare questo potere. Solo i folletti possono farlo." );
                        }
                    }
                    else
                    {
                        from.SendMessage( "Uso del comando: [Fuochi" );
                    }
                }
            }
            catch( Exception ex )
            {
                TextWriter tw = File.AppendText( "Logs/RaceErrors.log" );
                tw.WriteLine( "Warning: error in FireWorks_OnCommand command." );
                tw.WriteLine( ex.ToString() );
                tw.WriteLine( "" );
                tw.Close();
            }
        }

        public static void DoFireworks( Mobile from )
        {
            from.BeginAction( typeof( FiewWorksCommand ) );

            Map map = from.Map;

            if( map == null || map == Map.Internal )
                return;

            Point3D ourLoc = from.Location;

            Point3D startLoc = new Point3D( ourLoc.X, ourLoc.Y, ourLoc.Z + 10 );
            Point3D endLoc = new Point3D( startLoc.X + Utility.RandomMinMax( -2, 2 ), startLoc.Y + Utility.RandomMinMax( -2, 2 ), startLoc.Z + 32 );

            Effects.SendMovingEffect( new Entity( Serial.Zero, startLoc, map ), new Entity( Serial.Zero, endLoc, map ), 0x36E4, 5, 0, false, false );

            Timer.DelayCall( TimeSpan.FromSeconds( 1.0 ), new TimerStateCallback( FinishLaunch ), new object[] { from, endLoc, map } );
        }

        private static void FinishLaunch( object state )
        {
            object[] states = (object[])state;

            Mobile from = (Mobile)states[ 0 ];
            Point3D endLoc = (Point3D)states[ 1 ];
            Map map = (Map)states[ 2 ];

            int hue = Utility.Random( 40 );

            if( hue < 8 )
                hue = 0x66D;
            else if( hue < 10 )
                hue = 0x482;
            else if( hue < 12 )
                hue = 0x47E;
            else if( hue < 16 )
                hue = 0x480;
            else if( hue < 20 )
                hue = 0x47F;
            else
                hue = 0;

            if( Utility.RandomBool() )
                hue = Utility.RandomList( 0x47E, 0x47F, 0x480, 0x482, 0x66D );

            int renderMode = Utility.RandomList( 0, 2, 3, 4, 5, 7 );

            Effects.PlaySound( endLoc, map, Utility.Random( 0x11B, 4 ) );
            Effects.SendLocationEffect( endLoc, map, 0x373A + ( 0x10 * Utility.Random( 4 ) ), 16, 10, hue, renderMode );

            Timer.DelayCall( TimeSpan.FromSeconds( DelayFireWorks ), new TimerStateCallback( ReleaseSpecialCommandsLock ), from );
        }

        private static void ReleaseSpecialCommandsLock( object state )
        {
            ( (Mobile)state ).EndAction( typeof( FiewWorksCommand ) );
            ( (Mobile)state ).SendMessage( "Puoi usare di nuovo il tuo potere razziale." );
        }
    }
}