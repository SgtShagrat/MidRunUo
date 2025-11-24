/***************************************************************************
 *                               HeadQuarterRegionDefinition.cs
 *                            ------------------------------------
 *   begin                : 07 novembre, 2008
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using Server;

namespace Midgard.Engines.OrderChaosWars
{
    public class HeadQuarterRegionDefinition
    {
        public HeadQuarterRegionDefinition( Rectangle2D[] area, Virtue virtue )
        {
            Area = area;
            HeadQuarterVirtue = virtue;
        }

        public Virtue HeadQuarterVirtue { get; private set; }

        public Rectangle2D[] Area { get; private set; }
    }
}