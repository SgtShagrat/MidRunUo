/***************************************************************************
 *                                  CheckTownBans.cs
 *                            		----------------
 *  begin                	: Aprile, 2008
 *  version					: 1.0
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 * 
 * 	Info
 *  Il comando dice a chi lo usa da quali citta' e' bannato.
 *  
 ***************************************************************************/

using System;
using System.Text;
using Midgard.Engines.MidgardTownSystem;
using Server;
using Server.Commands;
using Server.Mobiles;

namespace Midgard.Commands
{
    public class CheckTownBans
    {
        public static void Initialize()
        {
            CommandSystem.Register( "BanCittadini", AccessLevel.Player, new CommandEventHandler( CheckTownBans_OnCommand ) );
        }

        [Usage( "BanCittadini" )]
        [Description( "Il comando dice a chi lo usa da quali citta' e' esilitato." )]
        public static void CheckTownBans_OnCommand( CommandEventArgs e )
        {
            StringBuilder sb = new StringBuilder();

            Midgard2PlayerMobile m2pm = e.Mobile as Midgard2PlayerMobile;
            if( m2pm == null )
                return;

            if( m2pm.TownBans != null )
            {
                foreach( int i in Enum.GetValues( typeof( TownBanFlag ) ) )
                {
                    if( i == 0 )
                        continue;

                    if( m2pm.TownBans[ (TownBanFlag)i ] )
                        sb.AppendLine( ( (TownBanFlag)i ).ToString() );
                }
            }

            if( sb.Length == 0 )
                m2pm.SendMessage( "Non sei esiliato da alcuna citta'." );
            else
                m2pm.SendMessage( "Sei esiliato dalle seguenti citta': {0}", sb.ToString() );
        }
    }
}