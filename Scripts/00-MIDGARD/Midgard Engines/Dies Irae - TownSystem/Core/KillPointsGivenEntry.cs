/***************************************************************************
 *                                  KillPointsGivenEntry.cs
 *                            		--------------------
 *  begin                	: Maggio, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;
using Server;

namespace Midgard.Engines.MidgardTownSystem
{
    public class KillPointsGivenEntry
    {
        public static readonly TimeSpan ExpirePeriod = TimeSpan.FromHours( 3.0 );

        public Mobile GivenTo { get; private set; }
        public DateTime TimeOfGift { get; private set; }

        public bool IsExpired { get { return ( TimeOfGift + ExpirePeriod ) < DateTime.Now; } }

        public KillPointsGivenEntry( Mobile givenTo )
        {
            GivenTo = givenTo;
            TimeOfGift = DateTime.Now;
        }
    }
}