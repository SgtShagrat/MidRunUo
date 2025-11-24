// #define DebugAuctionItem

using System;

using Midgard.Engines.AuctionSystem.Savings;

using Server;
using Server.Items;

using Core = Midgard.Engines.Packager.Core;

namespace Midgard.Engines.AuctionSystem
{
    public class Config
    {
        public const string DecimalSeparator = ".";
        private const string ConfigFile = @"Data/AuctionConfig.xml";
        private const string ConfigName = "AuctionSystem";
        internal static bool Debug = false;

        /// <summary>
        ///     The hue used for messages in the system
        /// </summary>
        public static int MessageHue = 0x40;

        /// <summary>
        ///     The hues used for gumps in the system
        /// </summary>
        public static int AuctionLabelHue = 0x480;

        public static int AuctionGreenHue = 0x40;
        public static int AuctionRedHue = 0x20;

        /// <summary>
        ///     List of the types that can not be sold through the auction system
        /// </summary>
        public static Type[] ForbiddenTypes = {
                                                  typeof (Gold), typeof (BankCheck), typeof (DeathRobe), typeof (AuctionGoldCheck),
                                                  typeof (AuctionItemCheck)
                                              };

        /// <summary>
        ///     This is the number of days the system will wait for the buyer or seller to decide on an ambiguous situation.
        ///     This can occur whether the highest bid didn't match the auction reserve. The owner will have then X days to
        ///     accept or refuse the auction. Another case is when one or more items is deleted due to a wipe or serialization error.
        ///     The buyer will have to decide in this case.
        /// </summary>
        public static int DaysForConfirmation = 7;

        /// <summary>
        ///     This value specifies how higher the reserve can be with respect to the starting bid. This factor should limit
        ///     any possible abuse of the reserve and prevent players from using very high values to be absolutely sure they will have
        ///     to sell only if they're happy with the outcome.
        /// </summary>
        public static double MaxReserveMultiplier = 3.0d;

        /// <summary>
        ///     This is the hue used to simulate the black hue because hue #1 isn't displayed
        ///     correctly on gumps. If your shard is using custom hues, you might want to
        ///     double check this value and verify it corresponds to a pure black hue.
        /// </summary>
        public static int BlackHue = 2000;

        /// <summary>
        ///     This variable controls whether the system will sell pets as well
        /// </summary>
        public static bool AllowPetsAuction = true;

        /// <summary>
        ///     This is the Access Level required to admin an auction through its
        ///     view gump. This will allow to see the props and to delete it.
        /// </summary>
        public static AccessLevel AuctionAdminAcessLevel = AccessLevel.Administrator;

        /// <summary>
        ///     If you don't have a valid UO installation on the server, or have trouble with the system
        ///     specify the location of the cliloc.enu file here:
        /// 
        ///     Example - Make sure you use the @ before the string:
        /// 
        ///     public static string ClilocLocation = @"C:\RunUO\Misc\cliloc.enu";
        /// </summary>
        public static string ClilocLocation;

        /// <summary>
        ///     Set this to false if you don't want to the system to produce a log file in \Logs\Auction.txt
        /// </summary>
        public static bool EnableLogging = true;

        /// <summary>
        ///     When a bid is placed within 5 minutes from the auction's ending, the auction duration will be
        ///     extended by this value.
        /// </summary>
        public static TimeSpan LateBidExtention = TimeSpan.FromMinutes( 20.0 );

        /// <summary>
        ///     This value specifies how much a player will have to pay to auction an item:
        ///     - 0.0 means that auctioning is free
        ///     - A value less or equal than 1.0 represents a percentage from 0 to 100%. This percentage will be applied on
        ///     the max value between the starting bid and the reserve.
        ///     - A value higher than 1.0 represents a fixed cost for the service (rounded).
        /// </summary>
        public static double CostOfAuction = 0.01;

        /// <summary>
        ///     Savings Account configuration for daily interest paid
        /// </summary>
        public static double GoldInterestRate = .01; // Percentage paid each day for gold

        public static double TokensInterestRate = .01; // Percentage paid each day for tokens

        public static bool EnableTokens; // Enable/disable them

        public static Type TokenType;
        public static Type TokenCheckType;

        public static int InterestHour = 20;
        public static double MaximumCheckValue = 25000000;

        internal static bool SavingsAccountEnabled = false;

        public static object[] Package_Info = {
                                                  "Script Title", "Auction System",
                                                  "Enabled by Default", true,
                                                  "Script Version", new Version(1, 0, 0, 0),
                                                  "Author name", "Revised by Dies Irae",
                                                  "Creation Date", new DateTime(2009, 08, 07),
                                                  "Author mail-contact", "tocasia@alice.it",
                                                  "Author home site", "http://www.midgardshard.it",
                                                  //"Author notes",           null,
                                                  "Script Copyrights", "(C) Midgard Shard - Dies Irae",
                                                  "Provided packages", new string[]
                                                                       {
                                                                           "Midgard.Engines.AuctionSystem"
                                                                       },
                                                  //"Required packages",      new string[0],
                                                  //"Conflicts with packages",new string[0],
                                                  "Research tags", new string[]
                                                                   {
                                                                       "Auction System"
                                                                   }
                                              };

        public static bool Enabled
        {
            get { return Core.Singleton[ typeof( Config ) ].Enabled; }
            set { Core.Singleton[ typeof( Config ) ].Enabled = value; }
        }

        public static void Package_Configure()
        {
            if( Debug )
                Console.WriteLine( "AuctionConfig.Configure" );

            Element element = ConfigParser.GetConfig( ConfigFile, ConfigName );

            if( null == element || element.ChildElements.Count <= 0 )
                return;

            if( Debug )
                Console.WriteLine( "ChildElements" + element.ChildElements.Count );

            AccessLevel tempAccessLevel;
            Type[] tempTypeArray;
            double tempDouble;
            bool tempBool;
            int tempInt;

            foreach( Element child in element.ChildElements )
            {
                if( child.TagName == "MessageHue" && child.GetIntValue( out tempInt ) )
                    MessageHue = tempInt;

                else if( child.TagName == "DaysForConfirmation" && child.GetIntValue( out tempInt ) )
                    DaysForConfirmation = tempInt;

                else if( child.TagName == "MaxReserveMultiplier" && child.GetDoubleValue( out tempDouble ) )
                    MaxReserveMultiplier = tempDouble;

                else if( child.TagName == "BlackHue" && child.GetIntValue( out tempInt ) )
                    BlackHue = tempInt;

                else if( child.TagName == "AllowPetsAuction" && child.GetBoolValue( out tempBool ) )
                    AllowPetsAuction = tempBool;

                else if( child.TagName == "AuctionAdminAcessLevel" && child.GetAccessLevelValue( out tempAccessLevel ) )
                    AuctionAdminAcessLevel = tempAccessLevel;

                else if( child.TagName == "ClilocLocation" && ( !String.IsNullOrEmpty( child.Text ) ) )
                    ClilocLocation = child.Text;

                else if( child.TagName == "EnableLogging" && child.GetBoolValue( out tempBool ) )
                    EnableLogging = tempBool;

                else if( child.TagName == "LateBidExtention" && child.GetDoubleValue( out tempDouble ) )
                    LateBidExtention = TimeSpan.FromMinutes( tempDouble );

                else if( child.TagName == "CostOfAuction" && child.GetDoubleValue( out tempDouble ) )
                    CostOfAuction = tempDouble;

                else if( child.TagName == "ForbiddenTypes" && child.GetArray( out tempTypeArray ) )
                    ForbiddenTypes = tempTypeArray;

                else if( child.TagName == "InterestHour" && child.GetIntValue( out tempInt ) )
                    InterestHour = tempInt;

                else if( child.TagName == "GoldInterestRate" && child.GetDoubleValue( out tempDouble ) )
                    GoldInterestRate = tempDouble;

                else if( child.TagName == "TokensInterestRate" && child.GetDoubleValue( out tempDouble ) )
                    TokensInterestRate = tempDouble;

                else if( child.TagName == "EnableTokens" && child.GetBoolValue( out tempBool ) )
                    EnableTokens = tempBool;
            }
        }

        public static void Package_Initialize()
        {
            if( Enabled )
            {
                if( SavingsAccountEnabled )
                {
                    SavingsAccount.RegisterCommands();
                    SavingsAccount.InitSystem();
                }

                WebCommands.RegisterCommands();
            }
        }
    }
}