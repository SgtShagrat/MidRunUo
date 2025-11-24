/***************************************************************************
 *                               NaglorRegion.cs
 *                            -------------------
 *   begin                : 21 September, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Xml;

using Server;
using Server.Regions;
using Server.Network;

namespace Midgard.Regions
{
    public class NaglundRegion : GuardedRegion
    {
        public NaglundRegion( XmlElement xml, Map map, Region parent )
            : base( xml, map, parent )
        {
        }

        private class BreathTimer : Timer
        {
            public BreathTimer()
                : base( TimeSpan.FromSeconds( 5.0 ), TimeSpan.FromSeconds( 5.0 ) )
            {
                Priority = TimerPriority.FiveSeconds;
            }

            protected override void OnTick()
            {
                foreach( NetState state in NetState.Instances )
                {
                    CheckBreath( state.Mobile );
                }
            }

            private static void CheckBreath( Mobile mobile )
            {
                if( mobile == null || mobile.Deleted || mobile.Map == Map.Internal || !mobile.Alive || !mobile.Region.IsPartOf( typeof( NaglundRegion ) ) )
                    return;

                if( Utility.Random( 3 ) == 0 )
                    Effects.SendLocationEffect( new Point3D( mobile.X, mobile.Y, mobile.Z ), mobile.Map, 0x38c0, 30, 1 );
            }
        }

        public static void StartBreathTimer()
        {
            new BreathTimer().Start();
        }
    }
}