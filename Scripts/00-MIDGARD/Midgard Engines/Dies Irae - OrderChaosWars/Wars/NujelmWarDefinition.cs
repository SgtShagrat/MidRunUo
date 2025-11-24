/***************************************************************************
 *                               NujelmWarDefinition.cs
 *                            ------------------
 *   begin                : 07 novembre, 2008
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using Server;

namespace Midgard.Engines.OrderChaosWars
{
    public class NujelmWarDefinition : WarDefinition
    {
        private static readonly ScoreRegionDefinition[] m_ScoreRegions = new ScoreRegionDefinition[]
             {
                 new ScoreRegionDefinition(1, "Garden", new Rectangle2D[]
                                                        {
                                                            new Rectangle2D(3634, 1297, 33, 40)
                                                        }, 2),
                 new ScoreRegionDefinition(2, "Chess", new Rectangle2D[]
                                                       {
                                                           new Rectangle2D(3719, 1351, 20, 20)
                                                       }, 1),
                 new ScoreRegionDefinition(3, "Building 1", new Rectangle2D[]
                                                            {
                                                                new Rectangle2D(3767, 1119, 27, 18)
                                                            }, 1),
                 new ScoreRegionDefinition(4, "Building 2", new Rectangle2D[]
                                                            {
                                                                new Rectangle2D(3767, 1159, 18, 18)
                                                            }, 1),
                 new ScoreRegionDefinition(5, "Building 3", new Rectangle2D[]
                                                            {
                                                                new Rectangle2D(3696, 1208, 9, 17)
                                                            }, 1),
                 new ScoreRegionDefinition(6, "Building 4", new Rectangle2D[]
                                                            {
                                                                new Rectangle2D(3728, 1216, 24, 24)
                                                            }, 1),
                 new ScoreRegionDefinition(7, "Building 5", new Rectangle2D[]
                                                            {
                                                                new Rectangle2D(3760, 1216, 24, 24)
                                                            }, 1),
                 new ScoreRegionDefinition(8, "Building 6", new Rectangle2D[]
                                                            {
                                                                new Rectangle2D(3727, 1287, 26, 58)
                                                            }, 1),
                 new ScoreRegionDefinition(9, "Building 7", new Rectangle2D[]
                                                            {
                                                                new Rectangle2D(3759, 1287, 18, 18)
                                                            }, 1),
                 new ScoreRegionDefinition(10, "Building 8", new Rectangle2D[]
                                                             {
                                                                 new Rectangle2D(3711, 1392, 37, 20)
                                                             }, 1),
             };

        private static readonly HeadQuarterRegionDefinition m_OrderHq = new HeadQuarterRegionDefinition(
            new Rectangle2D[]
            {
                new Rectangle2D(3679, 1255, 25, 49)
            },
            Virtue.Order );

        private static readonly HeadQuarterRegionDefinition m_ChaosHq = new HeadQuarterRegionDefinition(
            new Rectangle2D[]
            {
                new Rectangle2D(3759, 1183, 26, 26)
            },
            Virtue.Chaos );

        public NujelmWarDefinition()
            : base( "Nujel'm" )
        {
        }

        public override ScoreRegionDefinition[] ScoreRegions
        {
            get { return m_ScoreRegions; }
        }

        public override BattleType Wartype
        {
            get { return BattleType.Nujelm; }
        }

        public override string WarName
        {
            get { return "Nujel'm War"; }
        }

        public override HeadQuarterRegionDefinition OrderHeadQuarter
        {
            get { return m_OrderHq; }
        }

        public override HeadQuarterRegionDefinition ChaosHeadQuarter
        {
            get { return m_ChaosHq; }
        }

        public override Point3D OrderStoneLocation
        {
            get { return new Point3D( 3689, 1265, 25 ); }
        }

        public override Point3D ChaosStoneLocation
        {
            get { return new Point3D( 3765, 1189, 0 ); }
        }
    }
}