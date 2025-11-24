using System;
using Midgard.Engines.Packager;
using System.IO;
using Server.Mobiles;

namespace Midgard.Engines.PolRawPoints
{
    public class Config
    {
        #region [Initialization]
#if DEBUG
        public static bool Debug = true;
#else
		public static bool Debug = false;
#endif
        /// <summary>
        /// Default Raw Points will be added each attempt.
        /// </summary>
        public const uint DefaultRawAdvancement = 85;

        /// <summary>
        /// Cause to randomize a parrt of DefaultRawAdvancement before add.
        ///  Note: if 0, DefaultRawAdvancement will be entirely added
        /// </summary>
        public const double RandomizedAdvancementPart = 0.2;

        /// <summary>
        /// DefaultRawAdvancement * SuccessBonus will be added if skillcheck pass
        ///  Note: RandomizeAdvancement does NOT randomize this value.
        /// </summary>
        public const double SuccessBonus = 0.0;

        /// <summary>
        /// DefaultRawAdvancement * FailBonus will be added if skillcheck NOT pass
        ///  Note: RandomizeAdvancement does NOT randomize this value.
        /// </summary>
        public const double FailBonus = 0.2;
        #endregion

        #region Messages
        public const string Message_SkilltooDifficult = "Your skill in {0} cannot advance. Last action is too difficult.";
        public const string Message_SkilltooDifficult_ITA = "La skill {0} non può avanzare. L'azione era troppo difficile.";

        public const string Message_SkilltooEasy = "Your skill in {0} cannot advance. Last action is too easy.";
        public const string Message_SkilltooEasy_ITA = "La skill {0} non può avanzare. L'azione era troppo facile.";
        #endregion

        public static object[] Package_Info =
        {
            "Script Title", "PolRawPoints Engine",
            "Enabled by Default", true,
            "Script Version", new Version( 1, 0, 0, 0 ),
            "Author name", "Magius(CHE)",
            "Creation Date", new DateTime( 2011, 4, 21 ),
            "Author mail-contact", "cheghe@tiscali.it",
            "Author home site", "http://www.magius.it",
            //"Author notes",           null,
            "Script Copyrights", "(C) Midgard Shard - Magius(CHE",
            "Provided packages", new string[] { "Midgard.Engines.PolRawPoints" },
            "Required packages",       new string[]{"Midgard.Engines.SkillSystem"},
            //"Conflicts with packages",new string[0],
            "Research tags", new string[] { "PolRawPoints", "Skill", "RawPoints", "Pol"}
        };

        internal static Package Pkg;

        public static bool Enabled { get { return Pkg.Enabled; } set { Pkg.Enabled = value; } }

        public static void Package_Configure()
        {
            Pkg = Core.Singleton[ typeof( Config ) ];
        }

        public static void Package_Initialize()
        {
            if( Debug )
            {
                //Test();
                WriteCurve();
            }
        }

        public static string Message( PlayerMobile mobile, string varname, params object[] formatargs )
        {
            if( mobile.Language == "ITA" )
                varname += "_" + mobile.Language;
            varname = "Message_" + varname;
            var prop = typeof( Config ).GetField( varname, System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public );
            if( prop == null )
                return "## invalid message: " + varname + " ##";
            if( formatargs == null || formatargs.Length == 0 )
                return (string)prop.GetValue( null );
            else
                return string.Format( (string)prop.GetValue( null ), formatargs );
        }

        public static void WriteCurve()
        {
            if( Debug )
            {
                Server.ScriptCompiler.EnsureDirectory( "Logs" );
                using( TextWriter tw = new StreamWriter( "Logs/rawPointsStats.dat", false, System.Text.Encoding.UTF8 ) )
                {
                    tw.WriteLine( "{0}\t{1}\t{2}\t{3}", "Base", "Raw", "Tentativi", "Tentativi Totali" );
                    for( int i = 0; i <= 1000; i++ )
                    {
                        var less = 0;
                        if( i > 0 )
                            less = (int)MobileSkillHandler.FromBaseFixedPoints2Raw( i - 1 );
                        var act = MobileSkillHandler.FromBaseFixedPoints2Raw( i );
                        var tent = (int)( (double)( act - less ) / (double)Config.DefaultRawAdvancement );
                        var tottent = (int)( (double)act / (double)Config.DefaultRawAdvancement );
                        tw.WriteLine( "{0}\t{1}\t{2}\t{3}", i, act, ( tent == 0 ) ? "<1" : "" + tent, ( tottent == 0 ) ? "<1" : "" + tottent );
                    }
                }
            }

        }
        public static void Test()
        {
            Pkg.LogInfoLine( "Performing test:" );
            var dt = DateTime.Now;
            for( int h = 0; h < 100; h++ )
            {
                var basefixed = Server.Utility.Random( 1001 );/*from 0 to 1000*/
                var raw = MobileSkillHandler.FromBaseFixedPoints2Raw( basefixed );
                var rebase = MobileSkillHandler.FromRawPoints2BaseFixed( raw );
                var raw2 = raw + Server.Utility.Random( (int)DefaultRawAdvancement ) + 1; //from 1 to DefaultRawAdvancement
                var base2 = MobileSkillHandler.FromRawPoints2BaseFixed( (uint)raw2 );
                var reraw2 = MobileSkillHandler.FromBaseFixedPoints2Raw( base2 );
                var rebase2 = MobileSkillHandler.FromRawPoints2BaseFixed( reraw2 );
                Pkg.LogInfoIndentLine( "{0}) Base={1}, Raw={2}, RBase={3}", 1, h + 1, basefixed, raw, rebase );
                Pkg.LogInfoIndentLine( "Raw2={0}, Base2={1}, RRaw2={2}, RBase2={3}", 2, raw2, base2, reraw2, rebase2 );
            }
            Pkg.LogInfoLine( "Text completed in {0}.", TimeSpan.FromSeconds( (int)DateTime.Now.Subtract( dt ).TotalSeconds ) );
        }
    }
}
