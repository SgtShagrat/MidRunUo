/***************************************************************************
 *                                   WorldForgingCommands.cs
 *                            		--------------------------
 *  begin                	: Febbraio, 2007
 *  version					: 1.0
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 * 
 * 	Info
 * 			Comandi per la generazione del file compresso dai mul attuali.
 *
 ***************************************************************************/

using System;
using System.IO;
using System.Diagnostics;
using ICSharpCode.SharpZipLib.Zip;
using Server;
using Server.Commands;

namespace Midgard.Engines.WorldForging
{
    public class WorldForgingCommands
    {
        internal static bool Enabled = false;

        private static readonly string MulsPath = Path.Combine( Core.BaseDirectory, "muls" );
        private static readonly string RepositoryPath = @"C:\MIDGARD2\Muls";

        public static void Initialize()
        {
            if( Enabled )
            {
                CommandSystem.Register( "BuildMidgard2Muls", AccessLevel.Administrator, new CommandEventHandler( BuildMidgard2Muls_OnCommand ) );
                CommandSystem.Register( "BM2M", AccessLevel.Administrator, new CommandEventHandler( BuildMidgard2Muls_OnCommand ) );
            }
        }

        public static void BuildMidgard2Muls_OnCommand( CommandEventArgs e )
        {
            Console.WriteLine( "\nCompressione dei mul modificati dall' unfreeze iniziata" );
            World.Broadcast( 0x35, true, "Compressionedei mul modificati dall' unfreeze iniziata" );

            // Timer per indicare la durata dell'operazione
            Stopwatch sw = new Stopwatch();
            sw.Start();
            try
            {
                // Compressione della cartella muls
                FileInfo zip = CompressMuls();

                // Spostamento della cartella compressa nella repository
                if( File.Exists( zip.Name ) )
                {
                    MoveArchive( zip );
                }

                // Eventuale delete dell'archivio non spostato
                if( File.Exists( zip.Name ) )
                {
                    CleanUp( zip );
                }

                Console.WriteLine( "Build del file compresso dei muls completato con successo in {0:F2} secondi.", ( sw.ElapsedMilliseconds / 1000 ) );
                World.Broadcast( 0x35, true, "Build del file compresso dei muls completato con successo in {0:F2} secondi.\n" +
                                                            "La versione attuale dei mul e' la numero: {1}.",
                                                            ( sw.ElapsedMilliseconds / 1000 ),
                                                            WorldForgingPersistance.MidgardMulsVersion );
            }
            catch( Exception ex )
            {
                Console.WriteLine( "Build del file compresso dei muls non riuscito: {0}", ex );
                World.Broadcast( 0x35, true, string.Format( "Build del file compresso dei mouls non riuscito." ) );
            }
            finally
            {
                // Feramata cmq del timer di durata dell'operazione
                sw.Stop();
            }
        }

        private static FileInfo CompressMuls()
        {
            // Annuncio dell'inizio di compressione
            Console.Write( "Compressione della cartella Muls iniziata..." );
            World.Broadcast( 0x35, true, "Compressione della cartella Muls iniziata." );

            // Creazione del nome dell'archivio
            WorldForgingPersistance.MidgardMulsVersion++;
            string zipFileName = ( WorldForgingPersistance.MidgardMulsVersion ) + ".zip";

            // FileInfo del file compresso
            FileInfo fi;

            // Compressione della directory Saves Attuale	
            try
            {
                FastZip zipFile = new FastZip();
                zipFile.CreateZip( zipFileName, MulsPath, true, null );
                fi = new FileInfo( zipFileName );
                Console.Write( "fatto.\n" );
                World.Broadcast( 0x35, true, "Compressione della cartella Muls riuscita." );
            }
            catch( Exception ex )
            {
                fi = null;
                Console.Write( "Compressione fallita: {0}", ex );
                World.Broadcast( 0x35, true, "Compressione della cartella Muls fallita." );
            }

            return fi;
        }

        private static void MoveArchive( FileSystemInfo archivio )
        {
            Console.Write( "Spostamento dell'archivio compresso..." );
            World.Broadcast( 0x35, true, "Spostamento dell'archivio compresso iniziata." );
            try
            {
                // Se non esiste la crea
                if( !Directory.Exists( RepositoryPath ) )
                {
                    Directory.CreateDirectory( RepositoryPath );
                }

                // Crea la path della directory
                string PathDestinazione = Path.Combine( RepositoryPath, archivio.Name );

                // Spostamento dell'archivio
                File.Move( archivio.Name, PathDestinazione );

                Console.Write( "fatto.\n" );
                World.Broadcast( 0x35, true, "Spostamento dell'archivio compresso riuscito." );
            }
            catch( Exception ex )
            {
                Console.WriteLine( "Spostamento dell'archivio compresso temporaneo fallito: {0}", ex );
                World.Broadcast( 0x35, true, "Spostamento dell'archivio compresso fallito." );
            }
        }

        private static void CleanUp( FileInfo Archivio )
        {
            Console.Write( "Cancellazione dell' eventuale archivio compresso temporaneo iniziata..." );
            World.Broadcast( 0x35, true, "Cancellazione dell' eventuale archivio compresso temporaneo iniziata." );

            try
            {
                // Cancella il file compresso. 
                // Esso non dovrebbe esserci ma se c'e' tenta di cancellarlo.
                File.Delete( Archivio.Name );
                Console.Write( "fatto.\n" );
                World.Broadcast( 0x35, true, "Cancellazione dell'archivio compresso temporaneo riuscita." );
            }
            catch( Exception ex )
            {
                Console.WriteLine( "Cancellazione dell'archivio compresso temporaneo fallita: {0}", ex );
                World.Broadcast( 0x35, true, "Cancellazione dell'archivio compresso temporaneo fallita." );
            }
        }
    }
}