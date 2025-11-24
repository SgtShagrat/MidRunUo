/***************************************************************************
 *                               SaveTimeStamper.cs
 *                            ------------------------
 *   begin                : 15 dicembre, 2008
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/
 
using System;
using System.IO;
using Server;

namespace Midgard.Misc
{
    class SaveTimeStamper
    {
        public static DateTime LastSaveTime = DateTime.MinValue;
        public static string SaveTimeStampFile = "Saves/timestamp.txt";

        public static void Initialize()
        {
            EventSink.WorldLoad += new WorldLoadEventHandler( Load );
            EventSink.WorldSave += new WorldSaveEventHandler( Save );
        }

        public static void Load()
        {
            try
            {
                if( File.Exists( SaveTimeStampFile ) )
                {
                    using( StreamReader op = new StreamReader( SaveTimeStampFile ) )
                    {
                        try
                        {
                            LastSaveTime = DateTime.Parse( op.ReadLine() );
                            Console.WriteLine( "Last save time stamp is {0}", LastSaveTime );
                        }
                        catch( Exception ex )
                        {
                            Console.WriteLine( ex.ToString() );
                        }
                    }
                }
                else
                {
                    Console.WriteLine( "Warning: save time stamp file not found." );
                }
            }
            catch
            {
            }
        }

        public static void Save( WorldSaveEventArgs e )
        {
            try
            {
                WorldSaveProfiler.Instance.StartHandlerProfile( Save );

                if( File.Exists( SaveTimeStampFile ) )
                    File.Delete( SaveTimeStampFile);

                using( StreamWriter op = new StreamWriter( SaveTimeStampFile, true ) )
                    op.WriteLine( DateTime.Now.ToString() );

                WorldSaveProfiler.Instance.EndHandlerProfile();
            }
            catch
            {
            }
        }
    }
}