/***************************************************************************
 *                                  .cs
 *                            		-------------------
 *  begin                	: Mese, 2000
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info
 * 
 ***************************************************************************/

using Server;
using Server.Engines.Craft;
using Server.Items;

namespace Midgard.Engines
{
	public class CraftQuestSystem
	{
		#region Alchemy
		public static int[] LesserAlchemyRecipes = new int[]
		{
		};

		public static int[] MajorAlchemyRecipes = new int[]
		{
		};

		public static Item LesserAlchemyRecipe()
		{
			return new RecipeScroll( LesserAlchemyRecipes[ Utility.Random( LesserAlchemyRecipes.Length ) ] );
		}

		public static Item MajorAlchemyRecipe()
		{
			return new RecipeScroll( MajorAlchemyRecipes[ Utility.Random( MajorAlchemyRecipes.Length ) ] );
		}
		#endregion

		#region Blacksmithy
		public static int[] LesserBlacksmithyRecipes = new int[]
		{
		};

		public static int[] MajorBlacksmithyRecipes = new int[]
		{
		};

		public static Item LesserBlacksmithyRecipe()
		{
			return new RecipeScroll( LesserBlacksmithyRecipes[ Utility.Random( LesserBlacksmithyRecipes.Length ) ] );
		}

		public static Item MajorBlacksmithyRecipe()
		{
			return new RecipeScroll( MajorBlacksmithyRecipes[ Utility.Random( MajorBlacksmithyRecipes.Length ) ] );
		}
		#endregion

		#region BowFletching
		public static int[] LesserBowFletchingRecipes = new int[]
		{
			(int)BowRecipes.BarbedLongbow,
			(int)BowRecipes.SlayerLongbow,
			(int)BowRecipes.FrozenLongbow,
			(int)BowRecipes.LongbowOfMight,
			(int)BowRecipes.RangersShortbow,
			(int)BowRecipes.LightweightShortbow,
			(int)BowRecipes.MysticalShortbow,
			(int)BowRecipes.AssassinsShortbow,
		};

		public static int[] MajorBowFletchingRecipes = new int[]
		{
			(int)BowRecipes.BlightGrippedLongbow,
			(int)BowRecipes.FaerieFire,
			(int)BowRecipes.SilvanisFeywoodBow,
			(int)BowRecipes.MischiefMaker,
			(int)BowRecipes.TheNightReaper,			
		};

		public static Item LesserBowFletchingRecipe()
		{
			return new RecipeScroll( LesserBowFletchingRecipes[ Utility.Random( LesserBowFletchingRecipes.Length ) ] );
		}

		public static Item MajorBowFletchingRecipe()
		{
			return new RecipeScroll( MajorBowFletchingRecipes[ Utility.Random( MajorBowFletchingRecipes.Length ) ] );
		}
		#endregion

		#region Carpentery
		public static int[] LesserCarpenteryRecipes = new int[]
		{
		};

		public static int[] MajorCarpenteryRecipes = new int[]
		{
		};

		public static Item LesserCarpenteryRecipe()
		{
			return new RecipeScroll( LesserCarpenteryRecipes[ Utility.Random( LesserCarpenteryRecipes.Length ) ] );
		}

		public static Item MajorCarpenteryRecipe()
		{
			return new RecipeScroll( MajorCarpenteryRecipes[ Utility.Random( MajorCarpenteryRecipes.Length ) ] );
		}
		#endregion

		#region Inscription
		public static int[] LesserInscriptionRecipes = new int[]
		{
		};

		public static int[] MajorInscriptionRecipes = new int[]
		{
		};

		public static Item LesserInscriptionRecipe()
		{
			return new RecipeScroll( LesserInscriptionRecipes[ Utility.Random( LesserInscriptionRecipes.Length ) ] );
		}

		public static Item MajorInscriptionRecipe()
		{
			return new RecipeScroll( MajorInscriptionRecipes[ Utility.Random( MajorInscriptionRecipes.Length ) ] );
		}
		#endregion

		#region Tailoring
		public static int[] LesserTailoringRecipes = new int[]
		{
		};

		public static int[] MajorTailoringRecipes = new int[]
		{
		};

		public static Item LesserTailoringRecipe()
		{
			return new RecipeScroll( LesserTailoringRecipes[ Utility.Random( LesserTailoringRecipes.Length ) ] );
		}

		public static Item MajorTailoringRecipe()
		{
			return new RecipeScroll( MajorTailoringRecipes[ Utility.Random( MajorTailoringRecipes.Length ) ] );
		}
		#endregion

		#region Tinkering
		public static int[] LesserTinkeringRecipes = new int[]
		{
		};

		public static int[] MajorTinkeringRecipes = new int[]
		{
		};

		public static Item LesserTinkeringRecipe()
		{
			return new RecipeScroll( LesserTinkeringRecipes[ Utility.Random( LesserTinkeringRecipes.Length ) ] );
		}

		public static Item MajorTinkeringRecipe()
		{
			return new RecipeScroll( MajorTinkeringRecipes[ Utility.Random( MajorTinkeringRecipes.Length ) ] );
		}
		#endregion
	}
}
