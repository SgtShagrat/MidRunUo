using System;
using System.IO;

namespace Midgard.Engines.PetSystem
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
            "Script Title",             "Midgard Pet System",
            "Enabled by Default",       true,
            "Script Version",           new Version(1,0,0,0),
            "Author name",              "Dies Irae", 
            "Creation Date",            new DateTime(2009, 08, 07), 
            "Author mail-contact",      "tocasia@alice.it", 
            "Author home site",         "http://www.midgardshard.it",
            //"Author notes",           null,
            "Script Copyrights",        "(C) Midgard Shard - Dies Irae",
            "Provided packages",        new string[]{"Midgard.Engines.PetSystem"},
            //"Required packages",      new string[0],
            //"Conflicts with packages",new string[0],
            "Research tags",            new string[]{"Pet System"}
        };

        public static void Package_Initialize()
        {
            if( Enabled )
            {
                BaseCreatureReflector.RegisterCommands();
                // LowPetSkill.RegisterCommands();
                // FixPets.RegisterCommands();
                MidgardRarePets.RegisterCommands();
            }
        }
    }
}