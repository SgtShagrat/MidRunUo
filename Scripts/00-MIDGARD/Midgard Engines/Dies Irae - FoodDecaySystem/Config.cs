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

using Midgard.Engines.Packager;

namespace Midgard.Engines.FoodDecaySystem
{
    public class Config
    {
        internal static Package Pkg;

        public static bool Enabled
        {
            get { return Core.Singleton[ typeof( Config ) ].Enabled; }
            set { Core.Singleton[ typeof( Config ) ].Enabled = value; }
        }

        public static bool Debug = false;

        public static object[] Package_Info = {
            "Script Title",             "Food Decay System",
            "Enabled by Default",       true,
            "Script Version",           new Version(1,0,0,0),
            "Author name",              "Dies Irae", 
            "Creation Date",            new DateTime(2007, 1, 1), 
            "Author mail-contact",      "tocasia@alice.it", 
            "Author home site",         "http://www.midgardshard.it",
            //"Author notes",           null,
            "Script Copyrights",        "(C) Midgard Shard - Dies Irae",
            "Provided packages",        new string[]{"Midgard.Engines.FoodDecaySystem"},
            //"Required packages",      new string[0],
            //"Conflicts with packages",new string[0],
            "Research tags",            new string[]{"Food Decay"}
        };

        public static void Package_Configure()
        {
            Pkg = Core.Singleton[ typeof( Config ) ];
        }

        public static void Package_Initialize()
        {
            if( !Enabled )
                return;

            FoodDecayTimer.StartTimer();
            // FoodGumpCommand.RegisterCommands();
        }
    }
}