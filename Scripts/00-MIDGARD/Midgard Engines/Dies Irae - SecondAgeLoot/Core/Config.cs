/***************************************************************************
 *                               Config.cs
 *
 *   begin                : 06 novembre 2010
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;

using Midgard.Engines.Packager;

using Server;
using Server.Commands;
using Server.Items;

namespace Midgard.Engines.SecondAgeLoot
{
    public class Config
    {
        public static bool Debug;

        public static object[] Package_Info = {
                                                  "Script Title", "Second Age Loot System",
                                                  "Enabled by Default", true,
                                                  "Script Version", new Version( 1, 0, 0, 0 ),
                                                  "Author name", "Dies Irae",
                                                  "Creation Date", new DateTime( 2010, 10, 30 ),
                                                  "Author mail-contact", "tocasia@alice.it",
                                                  "Author home site", "http://www.midgardshard.it",
                                                  //"Author notes",           null,
                                                  "Script Copyrights", "(C) Midgard Shard - Dies Irae",
                                                  "Provided packages", new string[] { "Midgard.Engines.SecondAgeLoot" },
                                                  //"Required packages",      new string[0],
                                                  //"Conflicts with packages",new string[0],
                                                  "Research tags", new string[] { "SecondAgeLoot" }
                                              };

        public static bool Enabled { get { return Packager.Core.Singleton[ typeof( Config ) ].Enabled; } set { Packager.Core.Singleton[ typeof( Config ) ].Enabled = value; } }

        public static void Package_Configure()
        {
            Pkg = Packager.Core.Singleton[ typeof( Config ) ];

            CommandSystem.Register( "FixMagicItems", AccessLevel.Developer, new CommandEventHandler( FixMagicItems_OnCommand ) );
        }

        public static void Package_Initialize()
        {
            if( !Enabled )
                return;

            if( Server.Core.Debug )
            {
                Core.LogActualMagicItems( typeof( BaseArmor ), "armors" );

                Core.LogActualMagicItems( typeof( BaseWeapon ), "weapons" );

                //Core.LogItems();

                Core.LogMagicItems();
            }
        }

        internal static Package Pkg;

        [Usage( "FixMagicItems" )]
        [Description( "Fix all magic items in the world according to second age loot system." )]
        private static void FixMagicItems_OnCommand( CommandEventArgs e )
        {
            e.Mobile.SendMessage( "Fixing items..." );

            Core.FixActualMagicItems();

            Core.LogActualMagicItems( typeof( BaseArmor ), "armors-fixed" );

            Core.LogActualMagicItems( typeof( BaseWeapon ), "weapons-fixed" );

            Core.LogActualMagicItems( typeof( BaseJewel ), "jewel-fixed" );

            Core.LogActualMagicItems( typeof( BaseClothing ), "clothing-fixed" );

            e.Mobile.SendMessage( "Done!" );
        }
    }
}