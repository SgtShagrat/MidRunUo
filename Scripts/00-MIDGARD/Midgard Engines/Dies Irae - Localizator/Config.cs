using System;

using Midgard.Engines.Packager;

using Server;
using Server.Commands;
using Server.Mobiles;

namespace Midgard.Engines.Localization
{
    public class Config
    {
        public static bool Debug;

        public static object[] Package_Info = {
                                                  "Script Title", "Midgard Localization System",
                                                  "Enabled by Default", true,
                                                  "Script Version", new Version( 1, 0, 0, 0 ),
                                                  "Author name", "Dies Irae",
                                                  "Creation Date", new DateTime( 2010, 10, 10 ),
                                                  "Author mail-contact", "tocasia@alice.it",
                                                  "Author home site", "http://www.midgardshard.it",
                                                  //"Author notes",           null,
                                                  "Script Copyrights", "(C) Midgard Shard - Dies Irae",
                                                  "Provided packages", new string[] { "Midgard.Engines.Localization" },
                                                  //"Required packages",      new string[0],
                                                  //"Conflicts with packages",new string[0],
                                                  "Research tags", new string[] { "Localization" }
                                              };

        public static bool Enabled { get { return Packager.Core.Singleton[ typeof( Config ) ].Enabled; } set { Packager.Core.Singleton[ typeof( Config ) ].Enabled = value; } }

        public static void Package_Configure()
        {
            Pkg = Packager.Core.Singleton[ typeof( Config ) ];
        }

        public static void Package_Initialize()
        {
            if( !Enabled )
                return;

            CommandSystem.Register( "LinguaDiGioco", AccessLevel.Player, new CommandEventHandler( GameLanguage_OnCommand ) );
        }

        internal static Package Pkg;

        [Usage( "LinguaDiGioco" )]
        [Description( "Cambia la lingua del gioco. (Italiano-Inglese)" )]
        public static void GameLanguage_OnCommand( CommandEventArgs e )
        {
            if( e.Mobile == null || e.Length != 0 )
                return;

            Midgard2PlayerMobile from = e.Mobile as Midgard2PlayerMobile;
            if( from != null )
            {
                if( from.TrueLanguage == LanguageType.Ita )
                {
                    from.SendMessage( "You have chosen the english game language." );
                    from.TrueLanguage = LanguageType.Eng;
                    from.Language = "ENG";
                }
                else
                {
                    from.SendMessage( "Hai scelto la lingua italiana." );
                    from.TrueLanguage = LanguageType.Ita;
                    from.Language = "ITA";
                }
            }
        }
    }
}