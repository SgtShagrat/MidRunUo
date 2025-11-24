using System;
using Server.Mobiles;
using System.Collections.Generic;

namespace Server.Misc
{
    public class FlyingCleanup
    {
        public static void Initialize()
        {
            Timer.DelayCall( TimeSpan.FromSeconds( 2.5 ), new TimerCallback( Run ) );
        }

        public static void Run()
        {
            List<Mobile> flyCleanup = new List<Mobile>();

            foreach( Mobile mobile in World.Mobiles.Values )
            {
                if( mobile is FlyingCreature )
                {
                    if( ( (FlyingCreature)mobile ).CanFly )
                        flyCleanup.Add( mobile );
                }
            }

            for( int i = 0; i < flyCleanup.Count; i++ )
            {
                if( ( (FlyingCreature)flyCleanup[ i ] ).IsFlying )
                    ( (FlyingCreature)flyCleanup[ i ] ).IsFlying = false;
            }

            if( flyCleanup.Count > 0 )
                Console.WriteLine( "Refreshed {0} flying mobiles..", flyCleanup.Count );

            flyCleanup.Clear();
        }
    }
}