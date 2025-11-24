/***************************************************************************
 *                               TestWarOne.cs
 *
 *   begin                : 02 maggio 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using Server.Mobiles;

namespace Midgard.Engines.WarSystem
{
    public sealed class TestWarOne : BaseWar
    {
        public TestWarOne()
        {
            Definition = new TestWarOneDefinition();

            if( Definition.WarTeams != null )
            {
                foreach( WarTeam warTeam in Definition.WarTeams )
                    AddWarState( warTeam );

                if( Definition.ScoreRegions != null && Definition.ScoreRegions.Length > 0 )
                {
                    foreach( ScoreRegionDefinition def in Definition.ScoreRegions )
                    {
                        ScoreRegion region = new ScoreRegion( def );

                        foreach( WarState warState in WarStates )
                        {
                            WarTeam warTeam = warState.StateTeam;

                            AddObjective( new ConquerScoreRegionObjective( region, string.Format( "Conquest of {0} building.", region.Name ), 0, warTeam ) );
                            AddObjective( new SlayObjective( typeof( PlayerMobile ), "Kill chaos enemies", 0, warTeam ) );
                        }
                    }
                }
            }
        }
    }
}