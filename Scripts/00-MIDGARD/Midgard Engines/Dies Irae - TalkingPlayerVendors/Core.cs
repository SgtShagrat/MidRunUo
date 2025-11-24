/***************************************************************************
 *                                  Core.cs
 *
 *  begin                	: Ottobre, 2007
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 * 			Leggerissimo sistema di gestione dei messaggi che un vendor
 * 			deve dire quando un mobile gli si avvicina.
 * 
 ***************************************************************************/

using System;
using Server;
using Server.Mobiles;

namespace Midgard.Engines.TalkingPlayerVendorSystem
{
    public class Core
    {
        public static void HandleShout( PlayerVendor shouter, Mobile listener, Point3D oldLocation )
        {
            if( !Config.Enabled )
                return;

            if( shouter.Map == null || shouter.Map == Map.Internal )
                return;

            if( DateTime.Now < shouter.NextMessage )
                return;

            if( !listener.Player || !listener.Alive || listener.Hidden )
                return;

            if( !shouter.InRange( listener, Config.MessageRange ) || shouter.InRange( oldLocation, Config.MessageRange ) || !shouter.InLOS( listener ) )
                return;

            if( Config.ChanceToShout <= Utility.Random( 100 ) )
                return;

            Shout( shouter );
            shouter.NextMessage = DateTime.Now + TimeSpan.FromSeconds( Utility.RandomMinMax( Config.MinDelayMessage, Config.MaxDelayMessage ) );
        }

        private static void Shout( PlayerVendor shouter )
        {
            if( shouter == null || shouter.Deleted || shouter.Map == Map.Internal )
                return;

            if( shouter.ShoutEntries == null || shouter.ShoutEntries.Count == 0 )
                return;

            int index = Utility.Random( shouter.ShoutEntries.Count );

            string toShout = shouter.ShoutEntries[ index ];

            shouter.Say( true, toShout );
        }
    }
}