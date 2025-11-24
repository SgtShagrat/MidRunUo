/***************************************************************************
 *                               TeamOne.cs
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
    public sealed class TeamOne : WarTeam
    {
        public TeamOne()
            : base( "Team One", "T1" )
        {
        }

        public override bool IsPermared
        {
            get { return true; }
        }

        public override Point3D WarStoneLocation
        {
            get { return new Point3D( 2592, 617, 0 ); }
        }

        public override HeadQuarterRegionDefinition HeadQuarterDefinition
        {
            get { return m_HeadQuarterRegionDefinition; }
        }

        public override TeamHues TeamHue
        {
            get { return TeamHues.Red; }
        }

        private static readonly HeadQuarterRegionDefinition m_HeadQuarterRegionDefinition = new HeadQuarterRegionDefinition( new Rectangle2D[]
                                                                                                                            {
                                                                                                                                new Rectangle2D(2569, 586, 18, 18)
                                                                                                                            } );
    }
}