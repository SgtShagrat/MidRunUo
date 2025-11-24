/***************************************************************************
 *                               Nujelm.cs
 *                            -------------------
 *   begin                : 01 dicembre, 2008
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using Server.Mobiles;

namespace Midgard.Engines.OrderChaosWars
{
    public class Nujelm : BaseWar
    {
        public Nujelm()
        {
            Definition = new NujelmWarDefinition();

            AddStage( 1 );

            foreach( ScoreRegionDefinition def in Definition.ScoreRegions )
            {
                ScoreRegion region = new ScoreRegion( def );
                AddObjective( new ConquerScoreRegionObjective( region, string.Format( "Conquest of {0} building.", region.Name ), 0, Virtue.Order ) );
                AddObjective( new ConquerScoreRegionObjective( region, string.Format( "Conquest of {0} building.", region.Name ), 0, Virtue.Chaos ) );
            }

            AddStage( 2 );

            AddObjective( new SlayObjective( typeof( PlayerMobile ), "Kill chaos enemies", 0, Virtue.Order ) );
            AddObjective( new SlayObjective( typeof( PlayerMobile ), "Kill order enemies", 0, Virtue.Chaos ) );
        }
    }
}