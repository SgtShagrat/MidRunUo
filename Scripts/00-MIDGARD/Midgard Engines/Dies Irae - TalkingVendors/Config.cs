/***************************************************************************
 *                               Config.cs
 *
 *   begin                : 06 agosto, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.IO;

using Midgard.Engines.Packager;

namespace Midgard.Engines.TalkingVendors
{
    public class Config
    {
        internal static Package Pkg;

        public static bool Enabled
        {
            get { return Packager.Core.Singleton[ typeof( Config ) ].Enabled; }
            set { Packager.Core.Singleton[ typeof( Config ) ].Enabled = value; }
        }

        public static bool Debug = false;

        public static object[] Package_Info = {
                                                  "Script Title", "Talking Vendors System",
                                                  "Enabled by Default", true,
                                                  "Script Version", new Version( 1, 0, 0, 0 ),
                                                  "Author name", "Dies Irae",
                                                  "Creation Date", new DateTime( 2009, 08, 07 ),
                                                  "Author mail-contact", "tocasia@alice.it",
                                                  "Author home site", "http://www.midgardshard.it",
                                                  //"Author notes",           null,
                                                  "Script Copyrights", "(C) Midgard Shard - Dies Irae",
                                                  "Provided packages", new string[] { "Midgard.Engines.TalkingVendors" },
                                                  //"Required packages",      new string[0],
                                                  //"Conflicts with packages",new string[0],
                                                  "Research tags", new string[] { "Talking Vendors" }
                                              };

        /// <summary>
        /// Range in tiles to activate a ITalkingMobile
        /// </summary>
        internal static int RangePerception = 4;

        /// <summary>
        /// Max loyalty that a frind can achieve
        /// </summary>
        internal static int MaxLoyalty = 9;

        /// <summary>
        /// Probability to get a new friend if other checks are successfully passed
        /// </summary>
        internal static double FrequencyOfSpeech = 0.95;

        /// <summary>
        /// Minimum interval after which a ITalkingMobile can search a new friend
        /// </summary>
        internal static TimeSpan RefreshFriendsDelay = TimeSpan.FromMinutes( 5.0 );

        /// <summary>
        /// Minimum interval between two following greetings
        /// </summary>
        internal static TimeSpan GreetingsDelay = TimeSpan.FromSeconds( 10.0 );

        /// <summary>
        /// Path to speech system directory
        /// </summary>
        internal static string SpeechPath = Path.Combine( Server.Core.BaseDirectory, Path.Combine( "Data", "SpeechSystem" ) );

        /// <summary>
        /// Special speech file that contains chat strings used between ITalkingMobile and its friend speaking
        /// </summary>
        internal static string TalkToFriendsFileName = "talkToFriends.cfg";

        /// <summary>
        /// Special speech file that contains chat strings about ITalkingMobile friends
        /// </summary>
        internal static string TalkingAboutFriendsFileName = "talkaboutfriends.cfg";

        /// <summary>
        /// Special speech file that contains default speech strings
        /// </summary>
        internal static string DefaultSpeechTableFileName = "default.cfg";

        internal static string KarmaGreetingsWorstFileName = "defaultGrumpy.cfg";
        internal static string KarmaGreetingsNormalFileName = "defaultNormal.cfg";
        internal static string KarmaGreetingsGoodFileName = "defaultHappy.cfg";
        internal static string KarmaGreetingsAweFileName = "defaultAwesome.cfg";

        public static void Package_Configure()
        {
            Pkg = Packager.Core.Singleton[ typeof( Config ) ];

            if( Enabled )
                Core.ConfigSystem();
        }

        public static void Package_Initialize()
        {
            if( Enabled )
                Commands.RegisterCommands();
        }
    }
}