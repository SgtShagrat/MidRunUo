/***************************************************************************
 *                               HeadQuarterRegion.cs
 *                            --------------------------
 *   begin                : 07 novembre, 2008
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using Server;
using Server.Regions;

namespace Midgard.Engines.OrderChaosWars
{
    public class HeadQuarterRegion : BaseRegion
    {
        public HeadQuarterRegion( Virtue virtue, HeadQuarterRegionDefinition def )
            : base( Core.GetVirtueName( virtue ), Core.WarMap, DefaultPriority, def.Area )
        {
            RegionVirtue = virtue;

            if( Config.Enabled )
            {
                Register();
                Core.Instance.RegisterHeadQuarterRegion( this );
            }
        }

        public Virtue RegionVirtue { get; set; }

        public override bool OnMoveInto( Mobile m, Direction d, Point3D newLocation, Point3D oldLocation )
        {
            if( !base.OnMoveInto( m, d, newLocation, oldLocation ) )
                return false;

            if( m.AccessLevel >= AccessLevel.Counselor || Contains( oldLocation ) )
                return true;

            return ( RegionVirtue != Virtue.None && RegionVirtue == Core.Find( m, true, true ) );
        }

        public override bool AllowHousing( Mobile from, Point3D p )
        {
            return false;
        }
    }
}