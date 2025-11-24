/***************************************************************************
 *                                  KillReset.cs
 *                            		-------------------
 *  begin                	: Aprile, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using Server;
using Server.Commands;
using Server.Mobiles;

namespace Midgard.Commands
{
    public class KillReset
    {
        public static void Initialize()
        {
            CommandSystem.Register( "KillReset", AccessLevel.Developer, new CommandEventHandler( KillReset_OnCommand ) );
        }

        [Usage( "KillReset" )]
        [Description( "Reset and log all playermobile kills." )]
        public static void KillReset_OnCommand( CommandEventArgs e )
        {
            List<PlayerMobile> toReset = new List<PlayerMobile>();

            foreach( Mobile m in World.Mobiles.Values )
            {
                if( m is PlayerMobile && m.Kills > 0 && m.AccessLevel == AccessLevel.Player )
                    toReset.Add( (PlayerMobile)m );
            }

            toReset.Sort( InternalComparerPlayerMobile.Instance );

            using( StreamWriter op = new StreamWriter( "Logs/killReset.log" ) )
            {
                op.WriteLine( "# Kill reset table generated on {0}", DateTime.Now );
                op.WriteLine();
                op.WriteLine();

                for( int i = 0; i < toReset.Count; i++ )
                {
                    PlayerMobile p = toReset[ i ];
                    op.WriteLine( "Player {0} - Account {1} - Kills {2}", p.Name, p.Account.Username, p.Kills );
                    p.Kills = 0;
                }
            }

            e.Mobile.SendMessage( "Kill reset table has been generated. See the file : <runuo root>/Logs/killReset.log" );
        }

        private class InternalComparerPlayerMobile : IComparer<PlayerMobile>
        {
            public static readonly IComparer<PlayerMobile> Instance = new InternalComparerPlayerMobile();

            public InternalComparerPlayerMobile()
            {
            }

            public int Compare( PlayerMobile x, PlayerMobile y )
            {
                if( x == null || y == null )
                    throw new ArgumentException();

                if( x.Name == null || y.Name == null )
                    return 0;

                return Insensitive.Compare( x.Name, y.Name );
            }
        }
    }
}