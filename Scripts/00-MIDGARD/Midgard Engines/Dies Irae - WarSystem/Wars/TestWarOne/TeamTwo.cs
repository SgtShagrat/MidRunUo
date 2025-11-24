/***************************************************************************
 *                               TeamTwo.cs
 *
 *   begin                : 02 maggio 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using Server;

namespace Midgard.Engines.WarSystem
{
    public sealed class TeamTwo : WarTeam
    {
        public TeamTwo()
            : base( "Team Two", "T2" )
        {
        }

        public override TeamHues TeamHue { get { return TeamHues.Yellow; } }

        public override Point3D WarStoneLocation
        {
            get { return new Point3D( 2477, 401, 0 ); }
        }

        public override HeadQuarterRegionDefinition HeadQuarterDefinition
        {
            get { return m_HeadQuarterDefinition; }
        }

        private static readonly HeadQuarterRegionDefinition m_HeadQuarterDefinition = new HeadQuarterRegionDefinition(
            new Rectangle2D[]
            {
                new Rectangle2D(2458, 397, 39, 8)
            } );
    }
}