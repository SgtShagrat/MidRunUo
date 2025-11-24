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

namespace Midgard.Engines.HardLabour
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

        public static bool Debug = true;

        public static object[] Package_Info = {
            "Script Title",             "Hard Labour System",
            "Enabled by Default",       true,
            "Script Version",           new Version(1,0,0,0),
            "Author name",              "Dies Irae", 
            "Creation Date",            new DateTime(2007, 1, 1), 
            "Author mail-contact",      "tocasia@alice.it", 
            "Author home site",         "http://www.midgardshard.it",
            //"Author notes",           null,
            "Script Copyrights",        "(C) Midgard Shard - Dies Irae",
            "Provided packages",        new string[]{"Midgard.Engines.HardLabour"},
            //"Required packages",      new string[0],
            //"Conflicts with packages",new string[0],
            "Research tags",            new string[]{"Hard Labours"}
        };

        public static void Package_Initialize()
        {
            var pkg = Packager.Core.Singleton[ typeof( Config ) ];
            if( pkg.Enabled )
            {
                HardLabourCommands.RegisterCommands();

                HardLabourPersistance.RegisterCommands();
                HardLabourPersistance.EnsureExistence();

                if( Debug )
                {
                    pkg.LogInfoLine( "HardLabourColonyRegion registered. Prisoners counter is {0}.", HardLabourPersistance.HardLabourCounter );
                }
            }
        }
    }
}