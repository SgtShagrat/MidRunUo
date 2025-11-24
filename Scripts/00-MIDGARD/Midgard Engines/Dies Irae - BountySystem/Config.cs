/***************************************************************************
 *                               Config.cs
 *
 *   begin                : 03 October, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.IO;

using Midgard.Engines.Packager;

namespace Midgard.Engines.BountySystem
{
    public class Config
    {
        internal static Package Pkg;

        public static bool Enabled
        {
            get { return Packager.Core.Singleton[ typeof( Config ) ].Enabled; }
            set { Packager.Core.Singleton[ typeof( Config ) ].Enabled = value; }
        }

        public static object[] Package_Info = {
                                                "Script Title", "Bounty System",
                                                "Enabled by Default", true,
                                                "Script Version", new Version( 1, 0, 0, 0 ),
                                                "Author name", "Dies Irae",
                                                "Creation Date", new DateTime( 2009, 08, 07 ),
                                                "Author mail-contact", "tocasia@alice.it",
                                                "Author home site", "http://www.midgardshard.it",
                                                "Script Copyrights", "(C) Midgard Shard - Dies Irae",
                                                "Provided packages", new string[] { "Midgard.Engines.BountySystem" },
                                                "Research tags", new string[] { "BountySystem" }
                                                };

        public static void Package_Configure()
        {
            Pkg = Packager.Core.Singleton[ typeof( Config ) ];

            Core.RegisterSinks();
        }

        public static void Package_Initialize()
        {
            if( Enabled )
            {
                Commands.RegisterCommands();
                WebCommands.RegisterCommands();
            }
        }

        internal static TimeSpan DefaultDecayRate
        {
            get { return TimeSpan.FromDays( 14.0 ); }
        }

        internal static int DefaultMinBounty
        {
            get { return 1000; }
        }

        internal static readonly string XmlRoot = Path.Combine( "Data", "Bounty System" );
        internal static readonly string XmlSchema = Path.Combine( XmlRoot, "BountySchema.xsd" );
        internal static readonly string XmlFile = Path.Combine( XmlRoot, "Bounties.xml" );
        internal static readonly string XmlFileBackup = Path.Combine( XmlRoot, "Bounties.bak" );
    }
}