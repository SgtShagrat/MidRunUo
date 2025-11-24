/***************************************************************************
 *                               HeadQuarterRegionDefinition.cs
 *
 *   begin                : 20 febbraio 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using Server;

namespace Midgard.Engines.WarSystem
{
    public class HeadQuarterRegionDefinition
    {
        public HeadQuarterRegionDefinition( Rectangle2D[] area, WarTeam warTeam )
        {
            Area = area;
            HeadQuarterTeam = warTeam;
        }

        public HeadQuarterRegionDefinition( Rectangle2D[] area )
        {
            Area = area;
        }

        public WarTeam HeadQuarterTeam { get; private set; }

        public Rectangle2D[] Area { get; private set; }
    }
}