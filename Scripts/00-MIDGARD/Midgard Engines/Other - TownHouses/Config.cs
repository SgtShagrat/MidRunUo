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

namespace Midgard.Engines.TownHouses
{
    public class Config
    {
        public static bool Enabled
        {
            get
            {
                return Packager.Core.Singleton[ typeof( Config ) ].Enabled;
            }
            set
            {
                Packager.Core.Singleton[ typeof( Config ) ].Enabled = value;
            }
        }

        public static bool Debug = false;

        public static object[] Package_Info = {
            "Script Title",             "Town Houses System",
            "Enabled by Default",       true,
            "Script Version",           new Version(1,0,0,0),
            "Author name",              "Revised by Dies Irae", 
            "Creation Date",            new DateTime(2009, 08, 07), 
            "Author mail-contact",      "tocasia@alice.it", 
            "Author home site",         "http://www.midgardshard.it",
            //"Author notes",           null,
            "Script Copyrights",        "(C) Midgard Shard - Dies Irae",
            "Provided packages",        new string[]{"Midgard.Engines.TownHouses"},
            //"Required packages",      new string[0],
            //"Conflicts with packages",new string[0],
            "Research tags",            new string[]{"Town Houses"}
        };

        public static string Version { get { return "2.01"; } }

        public static void Package_Configure()
        {
            if( Enabled )
                Core.ConfigSystem();
        }

        public static void Package_Initialize()
        {
            if( Enabled )
            {
                Core.InitSystem();
                Errors.RegisterCommands();
                TownHousesGump.RegisterCommands();
                GumpResponse.InitSystem();
                SignHammer.InitTable();
            }
        }

        // This setting determines the suggested gold value for a single square of a home
        //  which then derives price, lockdowns and secures.
        public static int SuggestionFactor { get { return 150; } }

        // This setting determines if players need License in order to rent out their property
        public static bool RequireRenterLicense { get { return false; } }

        public static bool SpeechEventEnabled = false;
    }
}