/***************************************************************************
 *                               WarDefinition.cs
 *                            ----------------------
 *   begin                : 07 novembre, 2008
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using Server;

namespace Midgard.Engines.OrderChaosWars
{
    public abstract class WarDefinition
    {
        protected WarDefinition( string mainRegionName )
        {
            MainRegionName = mainRegionName;
        }

        public abstract Point3D OrderStoneLocation { get; }
        public abstract Point3D ChaosStoneLocation { get; }

        public abstract HeadQuarterRegionDefinition OrderHeadQuarter { get; }
        public abstract HeadQuarterRegionDefinition ChaosHeadQuarter { get; }
        public abstract ScoreRegionDefinition[] ScoreRegions { get; }
        public abstract BattleType Wartype { get; }
        public abstract string WarName { get; }

        public string MainRegionName { get; private set; }
    }
}