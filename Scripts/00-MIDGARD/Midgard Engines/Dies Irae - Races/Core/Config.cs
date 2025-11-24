using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;

using Midgard.Engines.Packager;

using Server;
using Server.Commands;
using Server.Misc;

namespace Midgard.Engines.Races
{
    public class Config
    {
        public static bool Debug;

        internal static bool BiteEnabled;
        internal static bool BlessEnabled;
        internal static bool FireWorksEnabled;
        internal static bool StatBonusesEnabled;
        internal static bool RaceGainFactorBonusEnabled = true;
        internal static bool RaceMorphEnabled;
        internal static bool RaceResistancesBonusEnabled;
        internal static bool RaceVisionEnabled;
        internal static bool RacialLanguageEnabled;
        internal static bool SkillBonusesEnabled;
        internal static bool VampireSystemEnabled;

        public static object[] Package_Info = {
                                                  "Script Title", "Midgard Race System",
                                                  "Enabled by Default", true,
                                                  "Script Version", new Version( 1, 0, 0, 0 ),
                                                  "Author name", "Dies Irae",
                                                  "Creation Date", new DateTime( 2009, 08, 07 ),
                                                  "Author mail-contact", "tocasia@alice.it",
                                                  "Author home site", "http://www.midgardshard.it",
                                                  //"Author notes",           null,
                                                  "Script Copyrights", "(C) Midgard Shard - Dies Irae",
                                                  "Provided packages", new string[] { "Midgard.Engines.Races" },
                                                  //"Required packages",      new string[0],
                                                  //"Conflicts with packages",new string[0],
                                                  "Research tags", new string[] { "Races" }
                                              };

        public static bool Enabled { get { return Packager.Core.Singleton[ typeof( Config ) ].Enabled; } set { Packager.Core.Singleton[ typeof( Config ) ].Enabled = value; } }

        public static void Package_Configure()
        {
            Pkg = Packager.Core.Singleton[ typeof( Config ) ];

            Core.RegisterRaces();
            Core.RegisterEventSink();

            LoadSettings();
        }

        public static void Package_Initialize()
        {
            if( !Enabled )
                return;

            CommandSystem.Register( "RaceSettings", AccessLevel.Administrator, new CommandEventHandler( RaceSettings_OnCommand ) );

            SetRace.RegisterCommands();
            Mobile.RaceChangeHandler = new RaceChangeHandler( SetRace.HandleRaceChange );

            if( VampireSystemEnabled )
            {
                VampBurnTimer.RegisterCommands();
                VampBurnTimer.StartTimer();
            }

            Werewolf.RegisterCommands();

            if( FireWorksEnabled )
                FiewWorksCommand.RegisterCommands();

            if( BiteEnabled )
                BiteCommand.RegisterCommands();

            if( BlessEnabled )
                BlessCommand.RegisterCommands();

            if( StatBonusesEnabled )
                StatBonuses.RegisterSink();

            if( SkillBonusesEnabled )
                SkillBonuses.RegisterHandler();

            if( RaceVisionEnabled )
                Infravision.RegisterCommands();

            if( RaceMorphEnabled )
                Morph.RegisterCommands();

            using( StreamWriter op = new StreamWriter( Path.Combine( "Logs", "races.log" ), false ) )
            {
                foreach( Race race in Race.AllRaces )
                {
                    if( race is MidgardRace )
                        op.WriteLine( ( (MidgardRace)race ).Log() );
                }
            }

            WebCommands.RegisterCommands();
        }

        internal static Package Pkg;

        #region [settings]
        [Usage( "RaceSettings" )]
        [Description( "Display race settings gump." )]
        private static void RaceSettings_OnCommand( CommandEventArgs e )
        {
            e.Mobile.SendGump( new RaceSettingsGump() );
        }

        internal static string SettingsPath = Path.Combine( "Data", "Races" );
        internal static string SettingsFilePath = Path.Combine( SettingsPath, "Settings.xml" );

        internal static void LoadSettings()
        {
            Pkg.LogInfo( "Loading race settings..." );

            if( !Directory.Exists( SettingsPath ) )
                Directory.CreateDirectory( SettingsPath );

            if( !File.Exists( SettingsFilePath ) )
                CreateDocument();

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load( SettingsFilePath );

                XmlElement root = doc[ "Settings" ];
                if( root == null )
                    return;

                ReadNode( root, "Bite", ref BiteEnabled );
                ReadNode( root, "Bless", ref BlessEnabled );
                ReadNode( root, "FireWorks", ref FireWorksEnabled );
                ReadNode( root, "StatBonuses", ref StatBonusesEnabled );
                ReadNode( root, "RaceGainFactorBonus", ref RaceGainFactorBonusEnabled );
                ReadNode( root, "RaceMorph", ref RaceMorphEnabled );
                ReadNode( root, "RaceResistancesBonus", ref RaceResistancesBonusEnabled );
                ReadNode( root, "RaceVision", ref RaceVisionEnabled );
                ReadNode( root, "RacialLanguage", ref RacialLanguageEnabled );
                ReadNode( root, "VampireSystem", ref VampireSystemEnabled );
                ReadNode( root, "SkillBonuses", ref SkillBonusesEnabled );
            }
            catch
            {
            }

            Pkg.LogInfoLine( "Done!" );
        }

        internal static void CreateDocument()
        {
            XDocument doc = new XDocument(
                new XElement( "Settings",
                              new XElement( "Bite", new XAttribute( "active", "false" ) ),
                              new XElement( "Bless", new XAttribute( "active", "false" ) ),
                              new XElement( "FireWorks", new XAttribute( "active", "false" ) ),
                              new XElement( "StatBonuses", new XAttribute( "active", "false" ) ),
                              new XElement( "RaceGainFactorBonus", new XAttribute( "active", "false" ) ),
                              new XElement( "RaceMorph", new XAttribute( "active", "false" ) ),
                              new XElement( "RaceResistancesBonus", new XAttribute( "active", "false" ) ),
                              new XElement( "RaceVision", new XAttribute( "active", "false" ) ),
                              new XElement( "RacialLanguage", new XAttribute( "active", "false" ) ),
                              new XElement( "VampireSystem", new XAttribute( "active", "false" ) ),
                              new XElement( "SkillBonuses", new XAttribute( "active", "false" ) ) ) );

            doc.Save( SettingsFilePath );
        }

        internal static void SaveSetings()
        {
            Pkg.LogInfo( "Saving race settings..." );

            if( !Directory.Exists( SettingsPath ) )
                Directory.CreateDirectory( SettingsPath );

            if( !File.Exists( SettingsFilePath ) )
                File.Create( SettingsFilePath );

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load( SettingsFilePath );

                XmlElement root = doc[ "Settings" ];

                if( root == null )
                    return;

                UpdateNode( root, "Bite", BiteEnabled );
                UpdateNode( root, "Bless", BlessEnabled );
                UpdateNode( root, "FireWorks", FireWorksEnabled );
                UpdateNode( root, "StatBonuses", StatBonusesEnabled );
                UpdateNode( root, "RaceGainFactorBonus", RaceGainFactorBonusEnabled );
                UpdateNode( root, "RaceMorph", RaceMorphEnabled );
                UpdateNode( root, "RaceResistancesBonus", RaceResistancesBonusEnabled );
                UpdateNode( root, "RaceVision", RaceVisionEnabled );
                UpdateNode( root, "RacialLanguage", RacialLanguageEnabled );
                UpdateNode( root, "VampireSystem", VampireSystemEnabled );
                UpdateNode( root, "SkillBonuses", SkillBonusesEnabled );

                doc.Save( SettingsFilePath );
            }
            catch( Exception e )
            {
                Console.WriteLine( "Error while updating 'Settings.xml': {0}", e );
            }

            Pkg.LogInfoLine( "Done!" );
        }

        private static void ReadNode( XmlNode root, string feature, ref bool val )
        {
            if( root == null )
                return;

            foreach( XmlElement element in root.SelectNodes( feature ) )
            {
                if( element.HasAttribute( "active" ) )
                    val = XmlConvert.ToBoolean( element.GetAttribute( "active" ) );
            }
        }

        private static void UpdateNode( XmlNode root, string feature, bool val )
        {
            if( root == null )
                return;

            foreach( XmlElement element in root.SelectNodes( feature ) )
            {
                if( element.HasAttribute( "active" ) )
                    element.SetAttribute( "active", XmlConvert.ToString( val ) );
            }
        }
        #endregion
    }
}