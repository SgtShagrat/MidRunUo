#if !DONOTCOMPILEDEBUGSCRIPT_

using System;

using Midgard.Engines.Packager;

using Server;

using Core = Midgard.Engines.Packager.Core;

namespace Midgard.Engines.RazorRpvRecorder
{
    public class Config
    {
        public static bool Enabled
        {
            get { return Pkg.Enabled; }
            set { Pkg.Enabled = value; }
        }

#if DEBUG
        public static bool Debug = true;
#else
		public static bool Debug = false;
#endif

        public static object[] Package_Info = {
                                                  "Script Title", "Razor Rpv Recorder",
                                                  "Enabled by Default", true,
                                                  "Script Version", new Version(1, 1, 0, 0),
                                                  "Author name", "Dies Irae, Magius(CHE)",
                                                  "Creation Date", new DateTime(2009, 08, 07),
                                                  "Author mail-contact", "tocasia@alice.it",
                                                  "Author home site", "http://www.midgardshard.it",
                                                  //"Author notes",           null,
                                                  "Script Copyrights", "(C) Midgard Shard - Dies Irae",
                                                  "Provided packages", new string[]
                                                                       {
                                                                           "Midgard.Engines.RazorRpvRecorder"
                                                                       },
                                                  //"Required packages",      new string[0],
                                                  //"Conflicts with packages",new string[0],
                                                  "Research tags", new string[]
                                                                   {
                                                                       "RazorRpvRecorder", "Rpv", "Recorder", "Video", "Razor"
                                                                   }
                                              };

        internal static Package Pkg { get; private set; }

        public static void Package_Configure()
        {
            Pkg = Core.Singleton[ typeof( Config ) ];
        }

        public static void Package_Initialize()
        {
            Commands.RegisterCommands();
            RecorderMobile.InitSystem();
            WebCommands.Register();

            ScriptCompiler.EnsureDirectory( StoragePath );
        }

        internal static string StoragePath = "PacketPlayerVideos";

        internal static int RecordingRange = 14;

        internal static short RecorderBody = 0;

        private static void Package_OnEnabledChanged( bool previous )
        {
            Pkg.LogInfoLine( "All RecorderMobile are now {0}.", !previous ? "Enabled" : "Disabled" );
            RecorderMobile.ToggleRecordingStates( null, !previous );
        }
    }
}

#endif