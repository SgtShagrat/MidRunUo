/***************************************************************************
 *                               WarDefinition.cs
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
    public abstract class WarDefinition
    {
        protected WarDefinition( string mainRegionName )
        {
            MainRegionName = mainRegionName;
        }

        // public abstract Point3D[] WarStoneLocation { get; }
        // public abstract HeadQuarterRegionDefinition[] HeadQuarters { get; set; }
        
        public abstract ScoreRegionDefinition[] ScoreRegions { get; }
        public abstract BattleType WarNameEnum { get; }
        public abstract string WarName { get; }

        /// <summary>
        /// These are the main teams battling
        /// They are defined during battle setup and popupated in pre-battle period
        /// </summary>
        public abstract WarTeam[] WarTeams { get; }

        public string MainRegionName { get; private set; }
        public abstract WarGateTravelDefinition[] TravelDefinitions { get; }
        public abstract Point3D[] TravelGateLocations { get; }
    }
}