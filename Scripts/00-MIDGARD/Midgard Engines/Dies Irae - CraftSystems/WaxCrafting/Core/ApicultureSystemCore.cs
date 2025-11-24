/***************************************************************************
 *                                  ApicultureSystemCore.cs
 *                            		-----------------------
 *  begin                	: Mese, 2000
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

namespace Midgard.Engines.Apiculture
{
	public enum HiveHealth
	{
		Dying,
		Sickly,
		Healthy,
		Thriving
	}

	public enum HiveStatus
	{
		Empty   		= 0,
		Colonizing		= 1,
		Brooding    	= 3,
		Producing		= 5,

		Stage1			= 1,
		Stage2			= 2,
		Stage3			= 3,
		Stage4			= 4,
		Stage5			= 5
	}

	public enum ResourceStatus
	{
		None		= 0,  //red X
		VeryLow		= 1,  //red -
		Low			= 2,  //yellow -
		Normal		= 3,  //nothing
		High		= 4,  //green +
		VeryHigh	= 5,  //yellow +
		TooHigh		= 6   //red +
	}

	public enum HiveGrowthIndicator
	{
		None = 0,
		LowResources,
		NotHealthy,
		Grown,	
		PopulationUp,
		PopulationDown
	}
}
