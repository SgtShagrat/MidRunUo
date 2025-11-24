/***************************************************************************
 *                               HeadQuarterRegion.cs
 *
 *   begin                : 20 febbraio 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using Server;
using Server.Regions;

namespace Midgard.Engines.WarSystem
{
    public class HeadQuarterRegion : BaseRegion
    {
        public HeadQuarterRegion( WarTeam team, HeadQuarterRegionDefinition def )
            : base( team.Name, BaseWar.WarMap, DefaultPriority, def.Area )
        {
            OwnerTeam = team;

            if( Config.Enabled )
            {
                Register();
                Core.Instance.RegisterHeadQuarterRegion( this );
            }
        }

        public WarTeam OwnerTeam { get; set; }

        public override bool OnMoveInto( Mobile m, Direction d, Point3D newLocation, Point3D oldLocation )
        {
            if( !base.OnMoveInto( m, d, newLocation, oldLocation ) )
                return false;

            if( m.AccessLevel >= AccessLevel.Counselor || Contains( oldLocation ) )
                return true;

            return ( OwnerTeam != null && OwnerTeam == Utility.Find( m, true, true ) );
        }

        public override bool AllowHousing( Mobile from, Point3D p )
        {
            return false;
        }
    }
}