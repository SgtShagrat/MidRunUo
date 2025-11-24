/***************************************************************************
 *                               Config.cs
 *
 *   begin                : 19 settembre 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;

using Midgard.Engines.Packager;

namespace Midgard.Engines.AdvancedFishing
{
    public class Config
    {
        internal static Package Pkg;

        public static bool Enabled
        {
            get { return Packager.Core.Singleton[ typeof( Config ) ].Enabled; }
            set { Packager.Core.Singleton[ typeof( Config ) ].Enabled = value; }
        }

        public static readonly bool Debug = false;

        public static object[] Package_Info = {
                                                  "Script Title", "Midgard Advanced Fishing",
                                                  "Enabled by Default", true,
                                                  "Script Version", new Version( 1, 0, 0, 0 ),
                                                  "Author name", "Dies Irae (and SnoW)",
                                                  "Creation Date", new DateTime( 2011, 09, 19 ),
                                                  "Author mail-contact", "tocasia@alice.it",
                                                  "Author home site", "http://www.midgardshard.it",
                                                  //"Author notes",           null,
                                                  "Script Copyrights", "(C) Midgard Shard - Dies Irae",
                                                  "Provided packages", new string[] { "Midgard.Engines.AdvancedFishing" },
                                                  //"Required packages",      new string[0],
                                                  //"Conflicts with packages",new string[0],
                                                  "Research tags", new string[] { "Advanced Fishing" }
                                              };

        public static void Package_Configure()
        {
            Pkg = Packager.Core.Singleton[ typeof( Config ) ];
        }

        public static void Package_Initialize()
        {
        }

        /// <summary>
        /// Duration of a single tick in seconds.
        /// </summary>
        public const double OneTickInSeconds = 0.25;

        /// <summary>
        /// Delay in ticks between fishing start and first reaction.
        /// </summary>
        public static int GetAfterQuietDelayInTicks()
        {
            return Server.Utility.RandomMinMax( 8, 16 ); // from 2 to 4 seconds
        }

        /// <summary>
        /// Delay in ticks between fisher action and fish reaction.
        /// </summary>
        public static int GetFishActionDelayInTicks()
        {
            return Server.Utility.RandomMinMax( 8, 16 ); // from 2 to 4 seconds
        }

        /// <summary>
        /// Message hues.
        /// </summary>
        public const int HueBad = 0x25;
        public const int HueGood = 0x3F;
        public const int HueFish = 0x3B6;

        /// <summary>
        /// Timer delay.
        /// </summary>
        public const double DefaultDelayInSeconds = 0.25;

        /// <summary>
        /// Minimum fishing skill to fish in deep water.
        /// </summary>
        public const double MinSkillToFishDeepWater = 40.0;

        /// <summary>
        /// Deep water fish weight scalar
        /// </summary>
        public const double DeepWaterScalar = 2.1;

        /// <summary>
        /// Low water fish weight scalar
        /// </summary>
        public const double NotDeepWaterScalar = 0.5;

        /// <summary>
        /// Minimum dexterity required to use advanced fishing
        /// </summary>
        public const int MinDexRequired = 25;

        /// <summary>
        /// Minimum strength required to use advanced fishing
        /// </summary>
        public const int MinStrRequired = 25;

        /// <summary>
        /// Minimum skill required to use advanced fishing
        /// </summary>
        public const double MinFishingRequired = 10.0;

        /// <summary>
        /// Cross radius to check deep water.
        /// </summary>
        public const int CrossSize = 30;

        /// <summary>
        /// Maximum range of the fishing pole.
        /// </summary>
        public const double MaxRange = 9.0;

        /// <summary>
        /// Maximum standard fish size.
        /// </summary>
        public static double MaxFishSize = 15000;

        /// <summary>
        /// Action required when fish got catched.
        /// </summary>
        public static Actions FisherActionOnFishCatched = Actions.Jump;

        /// <summary>
        /// Delay in seconds for a valid fisher reaction.
        /// </summary>
        public static double DefaultReflexDelay = 5.0;

        /// <summary>
        /// Chance to loose fish contest in any case.
        /// </summary>
        public static double UnluckyChance = 0.01;

        /// <summary>
        /// Difficulty step increase on a valid fisher move.
        /// </summary>
        public static double DifficultyIncreaseOnRightMove = 0.05;

        /// <summary>
        /// After this weight a fish is considered incredible.
        /// </summary>
        public static int IncredibleWeight = 25000;
    }
}