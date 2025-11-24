/***************************************************************************
 *                                  ListHouses.cs
 *                            		-------------------
 *  begin                	: Novembre, 2007
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
using Server.Multis;

namespace Midgard.Commands
{
    public class ListHouses
    {
        #region registrazione
        public static void Initialize()
        {
            CommandSystem.Register( "ListHouses", AccessLevel.Developer, new CommandEventHandler( ListHouses_OnCommand ) );   
        }
        #endregion

        #region callback
        [Usage( "ListHouses" )]
        [Description( "List all house info on our shard" )]
        public static void ListHouses_OnCommand( CommandEventArgs e )
        {
            Mobile from = e.Mobile;
            if( from == null || from.Deleted )
                return;

            if( e.Length == 0 )
            {
                try
                {
                    List<BaseHouse> houses = new List<BaseHouse>();
                    foreach( Mobile m in World.Mobiles.Values )
                    {
                        if( m != null && !m.Deleted && m.Player )
                            houses.AddRange( BaseHouse.GetHouses( m ) );
                    }

                    using( StreamWriter op = new StreamWriter( "Logs/MidgardHouses.log" ) )
                    {
                        op.WriteLine( "List generated on {0} in time {1}.", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString() );
                        op.WriteLine( "Total houses processed {0}", houses.Count );
                        op.WriteLine( "" );

                        for( int i = 0; i < houses.Count; i++ )
                        {
                            BaseHouse h = houses[ i ];
                            if( h != null && !h.Deleted )
                            {
                                op.WriteLine( "{0}) Serial {1} | Owner {2} | LastRefreshed {3} | WillDecayOn {4}",
                                             i, h.Serial,
                                             ( h.Owner != null ? h.Owner.Name : "null" ),
                                             h.LastRefreshed.ToString( "dd'-'MM'-'yyyy HH':'mm':'ss" ),
                                             ( h.LastRefreshed + h.DecayPeriod ).ToString( "dd'-'MM'-'yyyy HH':'mm':'ss" ) );
                            }
                        }
                    }

                    from.SendMessage( "House table has been generated. See the file : <runuo root>/Logs/MidgardHouses.log" );
                }
                catch
                {
                }
            }
        }
        #endregion
    }
}
