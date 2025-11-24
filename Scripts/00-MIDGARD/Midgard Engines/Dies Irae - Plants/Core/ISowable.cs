/***************************************************************************
 *                                  ISowable.cs
 *                            		-------------------
 *  begin                	: Mese, 2000
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

namespace Midgard.Engines.PlantSystem
{
    /// <summary>
    /// Interface giving properties to verify if a class is sowable
    /// </summary>
    public interface ISowable
    {
        bool CanGrowFarm { get; }
        bool CanGrowDirt { get; }
        bool CanGrowGround { get; }
        bool CanGrowSwamp { get; }
        bool CanGrowSand { get; }
        bool CanGrowGarden { get; }

        double RequiredSkillToPlant { get; }
    }
}