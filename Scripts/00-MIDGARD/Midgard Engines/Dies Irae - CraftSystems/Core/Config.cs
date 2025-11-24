using System;

using Midgard.Engines.Packager;
using Midgard.Misc;

using Server.Engines.Craft;

namespace Midgard.Engines.OldCraftSystem
{
    public class Config
    {
        internal static Package Pkg;

        public static bool Enabled
        {
            get { return Core.Singleton[ typeof( Config ) ].Enabled; }
            set { Core.Singleton[ typeof( Config ) ].Enabled = value; }
        }

        public static readonly bool Debug = false;

        public static object[] Package_Info = {
                                                  "Script Title", "Old Craft System",
                                                  "Enabled by Default", true,
                                                  "Script Version", new Version( 1, 0, 0, 0 ),
                                                  "Author name", "Dies Irae",
                                                  "Creation Date", new DateTime( 2009, 08, 07 ),
                                                  "Author mail-contact", "tocasia@alice.it",
                                                  "Author home site", "http://www.midgardshard.it",
                                                  //"Author notes",           null,
                                                  "Script Copyrights", "(C) Midgard Shard - Dies Irae",
                                                  "Provided packages", new string[] { "Midgard.Engines.OldCraftSystem", "Midgard.Engines.Craft", "Midgard.Engines.AutoLoop" },
                                                  "Required packages", new string[] { "Midgard.Misc.PreAoSDocHelper" },
                                                  //"Conflicts with packages",new string[0],
                                                  "Research tags", new string[] { "OldCraftSystem" }
                                              };

        public static void Package_Configure()
        {
            Pkg = Core.Singleton[ typeof( Config ) ];
        }

        public static void Package_Initialize()
        {
            if( Enabled )
            {
                LoadDefinitions();

                PreAoSDocHelper.Register( new CraftSystemDocHandler() );

                PreAoSDocHelper.Register( new ResourcesDocHandler() );
            }
        }

        internal static void LoadDefinitions()
        {
            CraftDefinitionTree tree;
            Pkg.LogInfo( "Initializing craft definitions..." );

            tree = DefAlchemy.CraftSystem.DefinitionTree;
            tree = DefBlacksmithy.CraftSystem.DefinitionTree;
            tree = DefBowFletching.CraftSystem.DefinitionTree;
            tree = DefCarpentry.CraftSystem.DefinitionTree;
            tree = DefCartography.CraftSystem.DefinitionTree;
            tree = DefCooking.CraftSystem.DefinitionTree;
            tree = DefGlassblowing.CraftSystem.DefinitionTree;
            tree = DefInscription.CraftSystem.DefinitionTree;
            tree = DefMasonry.CraftSystem.DefinitionTree;
            tree = DefTailoring.CraftSystem.DefinitionTree;
            tree = DefTinkering.CraftSystem.DefinitionTree;

            Pkg.LogInfoLine( "Done." );
        }
    }
}