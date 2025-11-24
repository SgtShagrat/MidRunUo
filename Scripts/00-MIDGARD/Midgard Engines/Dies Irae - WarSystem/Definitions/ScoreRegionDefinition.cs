/***************************************************************************
 *                               ScoreRegionDefinition.cs
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
    public class ScoreRegionDefinition
    {
        public ScoreRegionDefinition( int index, string name, Rectangle2D[] area, int pointScalar )
        {
            Index = index;
            Name = name;
            Area = area;
            PointScalar = pointScalar;
        }

        public string Name { get; private set; }

        public Rectangle2D[] Area { get; private set; }

        public int PointScalar { get; private set; }

        public int Index { get; private set; }
    }
}