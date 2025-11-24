using System;

using Midgard.Engines.Packager;
using Midgard.Misc;
using Server;

namespace Midgard.Engines.SkillSystem
{
    public class Config : IConfig
    {
		private Config() {}
		
		private static IConfig instance;
		
		public static IConfig Instance
		{ 
			get {
				if (instance == null) {
					instance = new Config();
				}
				return instance;
			}
			set {
				instance = value;
			}
		}
		
		public static bool Enabled
		{
            get { return Instance.IsEnabled; }
            set { Instance.IsEnabled = value; }
        }
			
        public bool IsEnabled
        {
            get { return Packager.Core.Singleton[ typeof( Config ) ].Enabled; }
            set { Packager.Core.Singleton[ typeof( Config ) ].Enabled = value; }
        }

        public static bool Debug = false;

        public static object[] Package_Info = {
            "Script Title",             "Midgard Skill System",
            "Enabled by Default",       true,
            "Script Version",           new Version(1,0,0,0),
            "Author name",              "Dies Irae", 
            "Creation Date",            new DateTime(2009, 08, 07), 
            "Author mail-contact",      "tocasia@alice.it", 
            "Author home site",         "http://www.midgardshard.it",
            //"Author notes",           null,
            "Script Copyrights",        "(C) Midgard Shard - Dies Irae",
            "Provided packages",        new string[]{"Midgard.Engines.SkillSystem"},
			"Required packages",        new string[] { "Midgard.Misc.PreAoSDocHelper" },
            //"Conflicts with packages",new string[0],
            "Research tags",            new string[]{"SkillSystem"}
        };

        internal static bool SkillLogEnabled = true;
        internal static bool RateOverTimeEnabled = false;
        internal static bool GuaranteedGainSystemEnabled = false;

        internal static Package Pkg;

        public static void Package_Configure()
        {
            Pkg = Packager.Core.Singleton[ typeof( Config ) ];

            if( RateOverTimeEnabled )
                RateOverTime.ConfigSystem();

            if( GuaranteedGainSystemEnabled )
                GuaranteedGainSystem.ConfigureSystem();

            Core.InitCore();

            if (Enabled)
            {
				EventSink.WorldLoad+= HandleEventSinkWorldLoad;
				EventSink.WorldSave+= HandleEventSinkWorldSave;
            }
        }

        static void HandleEventSinkWorldSave (WorldSaveEventArgs e)
        {
		    SkillGainFactorHelper.SaveSettings();
        }

        static void HandleEventSinkWorldLoad ()
        {
			SkillGainFactorHelper.LoadSettings ();
        }

        public static void Package_Initialize()
        {
            if( Enabled )
            {
                SkillGainFactorHelper.RegisterCommmands();


                SkillSystemLog.RegisterCommands();
                SkillSystemLog.InitLogger();

                if( RateOverTimeEnabled )
                {
                    RateOverTime.RegisterCommands();
                    RateOverTime.StartTimer();
                }

                if( GuaranteedGainSystemEnabled )
                {
                    GuaranteedGainSystem.RegisterCommands();
                    GuaranteedGainSystem.StartTimer();
                }

                OldStatGainSystem.InitStatGains();

                PreAoSDocHelper.Register( new SkillSystemDocHandler() );
            }
        }
    }
}