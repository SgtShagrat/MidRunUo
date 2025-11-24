/***************************************************************************
 *                                  FtpService.cs
 *                            		----------
 *  begin                	: Aprile, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;
using System.IO;
using System.Net;
using Midgard.Misc;
using Midgard.Engines.Packager;

namespace Midgard.Engines
{
    public class FtpService
    {
        private static readonly bool ShouldCastMessageOnSuccess = true;
        private static readonly Uri addressRootFTP = new Uri( "ftp://ftp.midgardshard.it/midgardshard.it/" );

        private const string UserFTP = "1073174@aruba.it";
        private const string PasswordFTP = "72bf3336cf";


		#region Package
		public static object[] Package_Info =
        {
            "Script Title", "Ftp Upload Service",
            "Enabled by Default", Midgard2Persistance.FtpService,
            "Script Version", new Version( 1, 0, 0, 0 ),
            "Author name",              "Dies Irae",
            "Creation Date", new DateTime( 2010, 1, 1 ),
            "Author mail-contact",      "tocasia@alice.it",
            "Author home site",         "http://www.midgardshard.it",
            //"Author notes",           null,
			"Script Copyrights",        "(C) Midgard Shard - Dies Irae",
            "Provided packages", new string[] { "Midgard.Engines.FtpService" },
            /*"Required packages",       new string[]{"Midgard.Engines.SkillSystem"},*/
            //"Conflicts with packages",new string[0],
            "Research tags", new string[] { "Ftp", "Upload"}
        };
		
#if DEBUG
        public static bool Debug = true;
#else
		public static bool Debug = false;
#endif

		internal static Package Pkg;
		
        public static bool Enabled { get { return Pkg.Enabled; } set { Pkg.Enabled = value; } }

        public static void Package_Configure()
        {
            Pkg = Midgard.Engines.Packager.Core.Singleton[ typeof( FtpService ) ];
        }
		#endregion


        public static void UploadFile( string localFile, string remoteFile )
        {
            if( !Enabled )
                return;

            if( !File.Exists( localFile ) )
            {
                Console.WriteLine( "Warning in UploadFile. File {0} not found.", localFile );
                return;
            }

            try
            {
                WebClient midgard = new WebClient();
                midgard.Credentials = new NetworkCredential( UserFTP, PasswordFTP );

                if( ShouldCastMessageOnSuccess )
                    Console.Write( "\nUploading file status on Midgard site..." );

                midgard.UploadFileAsync( new Uri( Path.Combine( addressRootFTP.ToString(), remoteFile ) ), localFile );

                if( ShouldCastMessageOnSuccess )
                    Console.WriteLine( "done." );
            }
            catch( WebException wex )
            {
                Console.WriteLine( wex.ToString() );
            }
            catch( Exception ex )
            {
                Console.WriteLine( ex.ToString() );
            }
        }
    }
}