/***************************************************************************
 *                                  CatchAliasCommand.cs
 *                            		--------------------
 *  begin                	: Marzo, 2007
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;
using Server;
using Server.Commands;
using Server.Mobiles;

namespace Midgard.Engines.AdvancedDisguise
{
    public class CatchAliasCommand
    {
        public static readonly bool Enabled = true;

        public static void Initialize()
        {
            if( Enabled )
            {
                CommandSystem.Register( "Smaschera", AccessLevel.Player, new CommandEventHandler( CatchAlias_OnCommand ) );
            }
        }

        [Usage( "Smaschera" )]
        [Description( "Con questo comando un giocatore puo' smascherare un suo alias." )]
        public static void CatchAlias_OnCommand( CommandEventArgs e )
        {
            Mobile from = e.Mobile as Midgard2PlayerMobile;
            if( from == null )
                return;

            foreach( Mobile m in from.GetMobilesInRange( 8 ) )
            {
                if( !( m is Midgard2PlayerMobile ) )
                    continue;
                Midgard2PlayerMobile m2Pm = m as Midgard2PlayerMobile;

                if( m2Pm.Alias != null && m2Pm.Alias == from )
                {
                    m2Pm.SendMessage( "Bad bad... BAD! Your identity is fired! Get away!!" );
                    m2Pm.SendMessage( "Your alias is broken. You have to make a new one." );
                    from.SendMessage( "You have notice someone that is.... YOU!" );
                    Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( SketchGump.OnDisguiseExpire ), m2Pm );
                    m2Pm.CriminalAction( false );
                    SketchBook.RevomeAliasFromBooksForMobile( from, m2Pm );
                }
            }
        }
    }
}