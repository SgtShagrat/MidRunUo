/***************************************************************************
 *                               NujelmWarDefinition.cs
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
    public class TestWarOneDefinition : WarDefinition
    {
        private static readonly WarTeam m_TeamOne = new TeamOne();

        private static readonly WarTeam m_TeamTwo = new TeamTwo();

        private static readonly WarTeam[] m_WarTeams = new WarTeam[]
                                                       {
                                                           m_TeamOne, m_TeamTwo
                                                       };

        private static readonly ScoreRegionDefinition[] m_ScoreRegions = new ScoreRegionDefinition[]
                                                                         {
                                                                             new ScoreRegionDefinition(1, "Garden", new Rectangle2D[]
                                                                                                                    {
                                                                                                                        new Rectangle2D(2457, 514, 17, 13)
                                                                                                                    }, 2),
                                                                         };

        public TestWarOneDefinition()
            : base( "Green Acres" )
        {
        }

        public override ScoreRegionDefinition[] ScoreRegions
        {
            get { return m_ScoreRegions; }
        }

        public override BattleType WarNameEnum
        {
            get { return BattleType.TestWarOne; }
        }

        public override string WarName
        {
            get { return "Test War One"; }
        }

        public override WarTeam[] WarTeams
        {
            get { return m_WarTeams; }
        }

        private static readonly WarGateTravelDefinition[] m_TravelDefinitions = new WarGateTravelDefinition[]
                                                                                {
                                                                                    new WarGateTravelDefinition("Team One Base", new Point3D(0, 0, 0), Map.Felucca), new WarGateTravelDefinition("Team Two Base", new Point3D(0, 0, 0), Map.Felucca)
                                                                                };

        private readonly Point3D[] m_TravelGateLocations = new Point3D[]
                                                           {
                                                               new Point3D(0, 0, 0), new Point3D(10, 10, 10)
                                                           };

        public override WarGateTravelDefinition[] TravelDefinitions
        {
            get { return m_TravelDefinitions; }
        }

        public override Point3D[] TravelGateLocations
        {
            get { return m_TravelGateLocations; }
        }
    }
}