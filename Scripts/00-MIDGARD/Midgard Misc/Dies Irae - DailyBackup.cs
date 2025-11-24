/***************************************************************************
 *                                   BackupGiornaliero.cs
 *                            		-------------------
 *  begin                	: Gennaio, 2006
 *  version		            : 1.0
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 * 
 * 	Info:
 * 			Ad una data ora del giorno comprime la cartella Saves prima del 
 * 			primo saving utile, la zippa e la sposta nella cartella 
 * 			<Runuo>\Saves Giornalieri .
 * 			Puo' essere scelta l'opzione FastZip oppure lo Zip avanzato met-
 * 			tendo anche il rapporto di compressione voluto (0-9).
 *  
 ***************************************************************************/

using System;
using System.Diagnostics;
using System.IO;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

using Server;
using Server.Commands;
using Server.Misc;
using Midgard.Engines;
using Server.Network;

namespace Midgard.Misc
{
    public class DailyBackup
    {
        public static readonly bool Enabled = Midgard2Persistance.DailySaveBackup && !Core.Debug;
        public static readonly bool FastZipEnabled = true; // se si vuole la compressione veloce
        public static readonly int CompressionLevel = 0; // Setta il livello di compressione 0 -> store, 9 -> massima compressione

        public static readonly string ServerDailyBackupFolder = "Daily Save Backups"; // nome della cartella sul server per il posizionamento dei save
        public static readonly int Hour = 5; // Ora del giorno in cui si vuole venga fatto il backup

        public static readonly string RemoteDir = "daily_saves";

        /*public static readonly string AddressFTP = FtpService.addressRootFTP.AbsoluteUri + "/daily_saves/"; // path al sito ftp
        public static readonly string UserFTP = FtpService.UserFTP;
        public static readonly string PasswordFTP = FtpService.PasswordFTP;*/

        private static int m_Day;

        public static void DailySaveBackup( bool forced )
        {
            if( !Enabled )
                return;

            if( World.Saving || AutoRestart.Restarting )
                return;

            if( !forced && ( DateTime.Now.Day != m_Day || DateTime.Now.Hour < Hour ) )
                return;

            World.Broadcast( 0x22, true, "The server is going to backup the last save. Thanks for patience." );
            NetState.FlushAll();
            NetState.Pause();

            m_Day++;

            Console.WriteLine( "Daily backup started:" );

            Stopwatch sw = new Stopwatch(); // Timer per indicare la durata dell'operazione
            sw.Start();

            try
            {
                FileInfo zip = CompressSaves(); // Compressione della cartella saves
                string zipName = zip.Name;

                // Spostamento della cartella compressa nella repository
                if( File.Exists( zipName ) )
                    MoveArchive( zip );

                // Eventuale delete dell'archivio non spostato
                if( File.Exists( zipName ) )
                    CleanUp( zip );

                string toUploadName = Path.Combine( ServerDailyBackupFolder, zipName );
                if( File.Exists( toUploadName ) )
                    UploadFile( new FileInfo( toUploadName ) );

                Console.WriteLine( "Daily backup completed successfully in {0:F2} seconds.", ( sw.ElapsedMilliseconds / 1000 ) );
            }
            catch( Exception ex )
            {
                Console.WriteLine( "Daily backup of last save failed: {0}", ex );
            }
            finally
            {
                // Feramata cmq del timer di durata dell'operazione
                sw.Stop();
                Console.WriteLine( "Next daily backup will be on day {0}", m_Day );
            }

            NetState.Resume();
        }

        public static void Initialize()
        {
            m_Day = DateTime.Now.Day;
            CommandSystem.Register( "DoBackup", AccessLevel.Developer, new CommandEventHandler( DoBackup_OnCommand ) );
        }

        [Usage( "DoBackup" )]
        [Description( "Force a backup of last save." )]
        public static void DoBackup_OnCommand( CommandEventArgs e )
        {
            if( e.Length == 0 )
                DailySaveBackup( true );
            else
                e.Mobile.SendMessage( "Command Use: [DoBackup" );
        }

        private static FileInfo CompressSaves()
        {
            // Annuncio dell'inizio di compressione
            Console.Write( "Compressing save folder..." );

            // Creazione del nome dell'archivio
            string zipFileName = GetTimeStamp() + ".zip";

            // Creazione della path della cartella Saves
            string pathSaves = Path.Combine( Core.BaseDirectory, "Saves" );

            // FileInfo del file compresso
            FileInfo fi;

            // Compressione della directory Saves Attuale
            if( FastZipEnabled )
            {
                #region Zip Veloce
                try
                {
                    FastZip zipFile = new FastZip();
                    zipFile.CreateZip( zipFileName, pathSaves, true, null );
                    fi = new FileInfo( zipFileName );
                    Console.Write( "done.\n" );
                }
                catch( Exception ex )
                {
                    fi = null;
                    Console.Write( "Compression failed: {0}", ex );
                }
                #endregion
            }
            else
            {
                #region Zip Avanzato
                try
                {
                    // Crea la lista di files da comprimere
                    string[] listaFiles = Directory.GetFiles( pathSaves, "*.*", SearchOption.AllDirectories );

                    // Crea il buffer di bytes da comprimere
                    byte[] buffer = new byte[ 4096 ];

                    using( ZipOutputStream s = new ZipOutputStream( File.Create( zipFileName ) ) )
                    {
                        // Setta il livello di compressione 0 -> store, 9 -> massima compressione
                        s.SetLevel( CompressionLevel );

                        foreach( string file in listaFiles )
                        {
                            // Per ogni file crea la corrispondente entry
                            ZipEntry entry = new ZipEntry( file );

                            // Inserisce nello zip la entry creata
                            s.PutNextEntry( entry );

                            using( FileStream fs = File.OpenRead( file ) )
                            {
                                StreamUtils.Copy( fs, s, buffer );

                                //								Simile al seguente (che per altro non funziona)
                                //			                    using ( FileStream fs = File.OpenRead(file) )
                                //								{
                                //									int sourceBytes;
                                //									do 
                                //									{
                                //										// Legge il buffer...
                                //										sourceBytes = fs.Read(buffer, 0, buffer.Length);
                                //										// ...e lo comprime
                                //										s.Write(buffer, 0, sourceBytes);
                                //									} 
                                //									while ( sourceBytes > 0 );
                                //								}
                            }
                        }
                    }
                    fi = new FileInfo( zipFileName );
                    Console.Write( "fatto.\n" );
                }
                catch( Exception ex )
                {
                    fi = null;
                    Console.WriteLine( "Compressione fallita: {0}", ex );
                }
                #endregion
            }

            // Annuncio della fine della compressione
            return fi;
        }

        private static void MoveArchive( FileSystemInfo fileName )
        {
            Console.Write( "Moving compressed archive..." );
            try
            {
                // Se non esiste la crea
                if( !Directory.Exists( ServerDailyBackupFolder ) )
                    Directory.CreateDirectory( ServerDailyBackupFolder );

                // Crea la path della directory <RunUo2>\Saves Giornalieri\
                string path = Path.Combine( ServerDailyBackupFolder, fileName.Name );

                // Spostamento dell'archivio
                File.Move( fileName.Name, path );

                Console.Write( "done.\n" );
            }
            catch( Exception ex )
            {
                Console.WriteLine( "Compressed archive movement failed: {0}", ex );
            }
        }

        private static void UploadFile( FileSystemInfo fileName )
        {
            Console.Write( "Uploading compressed archive..." );
            try
            {
                FtpService.UploadFile( fileName.FullName, Path.Combine( RemoteDir, fileName.Name ) );
                /*Uri uri = new Uri( Path.Combine( AddressFTP, fileName.Name ) );

                WebClient client = new WebClient();
                client.Credentials = new NetworkCredential( UserFTP, PasswordFTP );
                client.UploadFileAsync( uri, fileName.FullName );*/
            }
            catch
            {
            }
            finally
            {
                Console.Write( "done.\n" );
            }
        }

        private static void CleanUp( FileSystemInfo archivio )
        {
            Console.Write( "Erasing temporary files..." );

            try
            {
                // Cancella il file compresso. Esso non dovrebbe esserci ma se c'e' tenta di cancellarlo.
                File.Delete( archivio.Name );
                Console.Write( "done.\n" );
            }
            catch( Exception ex )
            {
                Console.WriteLine( "Erasing temporary files process failed: {0}", ex );
            }
        }

        private static string GetTimeStamp()
        {
            DateTime now = DateTime.Now;

            return String.Format( "{0}-{1}-{2} {3}-{4:D2}-{5:D2}",
                                  now.Day, now.Month, now.Year, now.Hour, now.Minute, now.Second );
        }
    }
}