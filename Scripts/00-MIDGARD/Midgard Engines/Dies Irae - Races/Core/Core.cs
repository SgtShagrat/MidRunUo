using System;
using System.Collections.Generic;
using System.IO;

using Server;
using Server.Misc;

namespace Midgard.Engines.Races
{
    public class Core
    {
        public static void RegisterEventSink()
        {
            EventSink.WorldLoad += new WorldLoadEventHandler( Load );
            EventSink.WorldSave += new WorldSaveEventHandler( Save );
        }

        #region serial-deserial
        public static readonly string RaceSystemSavePath = Path.Combine( Path.Combine( "Saves", "RaceSystem" ), "RaceSystemSave.bin" );

        public static void Save( WorldSaveEventArgs e )
        {
            if( Config.Debug )
                Console.Write( "{0}: Saving...", Config.Pkg.Title );

            try
            {
                WorldSaveProfiler.Instance.StartHandlerProfile( Save );

                string dir = Path.Combine( Path.GetPathRoot( RaceSystemSavePath ), Path.GetDirectoryName( RaceSystemSavePath ) );
                if( !Directory.Exists( dir ) )
                    Directory.CreateDirectory( dir );

                BinaryFileWriter writer = new BinaryFileWriter( RaceSystemSavePath, true );

                writer.Write( 0 ); // version

                writer.Write( Instances.Length );

                foreach( MidgardRace t in Instances )
                    t.Serialize( writer );

                writer.Close();

                WorldSaveProfiler.Instance.EndHandlerProfile();
            }
            catch( Exception ex )
            {
                Console.WriteLine( "Error serializing {0}.", Config.Pkg.Title );
                Console.WriteLine( ex.ToString() );
            }

            if( Config.Debug )
                Console.WriteLine( "done." );
        }

        public static void Load()
        {
            if( Config.Debug )
                Console.Write( "{0}: Loading...", Config.Pkg.Title );

            while( !File.Exists( RaceSystemSavePath ) )
            {
                Console.WriteLine( "Warning: {0} not found.", RaceSystemSavePath );
                Console.WriteLine( " - Press return to continue, or R to try again." );
                string str = Console.ReadLine();

                if( str == null || str.ToLower() != "r" )
                    return;
            }

            try
            {
                BinaryReader bReader = new BinaryReader( File.OpenRead( RaceSystemSavePath ) );
                BinaryFileReader reader = new BinaryFileReader( bReader );

                int version = reader.ReadInt();

                switch( version )
                {
                    case 0:
                        {
                            int count = reader.ReadInt();

                            for( int i = 0; i < count; ++i )
                                Instances[ i ].Deserialize( reader );
                            break;
                        }
                    default:
                        break;
                }

                bReader.Close();
            }
            catch( Exception ex )
            {
                Console.WriteLine( "Error de-serializing {0}.", Config.Pkg.Title );
                Console.WriteLine( ex.ToString() );
            }

            if( Config.Debug )
                Console.WriteLine( "done." );
        }
        #endregion

        public static MidgardRace HighElf { get { return Races.HighElf.Instance; } }
        public static MidgardRace NorthernElf { get { return (MidgardRace)Race.Races[ 34 ]; } }
        public static MidgardRace MountainDwarf { get { return (MidgardRace)Race.Races[ 35 ]; } }
        public static MidgardRace FairyOfWood { get { return (MidgardRace)Race.Races[ 36 ]; } }
        public static MidgardRace FairyOfFire { get { return (MidgardRace)Race.Races[ 37 ]; } }
        public static MidgardRace FairyOfWater { get { return (MidgardRace)Race.Races[ 38 ]; } }
        public static MidgardRace FairyOfAir { get { return (MidgardRace)Race.Races[ 39 ]; } }
        public static MidgardRace FairyOfEarth { get { return (MidgardRace)Race.Races[ 40 ]; } }
        public static MidgardRace HighOrc { get { return (MidgardRace)Race.Races[ 41 ]; } }
        public static MidgardRace Sprite { get { return (MidgardRace)Race.Races[ 42 ]; } }
        public static MidgardRace HalfElf { get { return (MidgardRace)Race.Races[ 43 ]; } }
        public static MidgardRace HalfDrow { get { return (MidgardRace)Race.Races[ 44 ]; } }
        public static MidgardRace Vampire { get { return (MidgardRace)Race.Races[ 45 ]; } }
        public static MidgardRace Drow { get { return (MidgardRace)Race.Races[ 46 ]; } }
        public static MidgardRace NorthernHuman { get { return (MidgardRace)Race.Races[ 47 ]; } }
        public static MidgardRace Undead { get { return (MidgardRace)Race.Races[ 48 ]; } }
        public static MidgardRace Naglor { get { return (MidgardRace)Race.Races[ 49 ]; } }
        public static MidgardRace HalfDaemon { get { return (MidgardRace)Race.Races[ 50 ]; } }
        public static MidgardRace Werewolf { get { return (MidgardRace)Race.Races[ 51 ]; } }

        public static MidgardRace[] Instances;

        public static void RegisterRaces()
        {
            List<MidgardRace> list = new List<MidgardRace>();

            RaceDefinitions.RegisterRace( new HighElf( 33, 33 ) ); // Index 2 in AllRaces
            list.Add( HighElf );

            RaceDefinitions.RegisterRace( new NorthernElf( 34, 34 ) ); // Index 3 in AllRaces
            list.Add( NorthernElf );

            RaceDefinitions.RegisterRace( new MountainDwarf( 35, 35 ) ); // Index 4 in AllRaces
            list.Add( MountainDwarf );

            RaceDefinitions.RegisterRace( new FairyOfWood( 36, 36 ) ); // Index 5 in AllRaces
            list.Add( FairyOfWood );

            RaceDefinitions.RegisterRace( new FairyOfFire( 37, 37 ) ); // Index 6 in AllRaces
            list.Add( FairyOfFire );

            RaceDefinitions.RegisterRace( new FairyOfWater( 38, 38 ) ); // Index 7 in AllRaces
            list.Add( FairyOfWater );

            RaceDefinitions.RegisterRace( new FairyOfAir( 39, 39 ) ); // Index 8 in AllRaces
            list.Add( FairyOfAir );

            RaceDefinitions.RegisterRace( new FairyOfEarth( 40, 40 ) ); // Index 9 in AllRaces
            list.Add( FairyOfEarth );

            RaceDefinitions.RegisterRace( new HighOrc( 41, 41 ) ); // Index 10 in AllRaces
            list.Add( HighOrc );

            RaceDefinitions.RegisterRace( new Sprite( 42, 42 ) ); // Index 11 in AllRaces
            list.Add( Sprite );

            RaceDefinitions.RegisterRace( new HalfElf( 43, 43 ) ); // Index 12 in AllRaces
            list.Add( HalfElf );

            RaceDefinitions.RegisterRace( new HalfDrow( 44, 44 ) ); // Index 13 in AllRaces
            list.Add( HalfDrow );

            RaceDefinitions.RegisterRace( new Vampire( 45, 45 ) ); // Index 14 in AllRaces
            list.Add( Vampire );

            RaceDefinitions.RegisterRace( new Drow( 46, 46 ) ); // Index 15 in AllRaces
            list.Add( Drow );

            RaceDefinitions.RegisterRace( new NorthernHuman( 47, 47 ) ); // Index 16 in AllRaces
            list.Add( NorthernHuman );

            RaceDefinitions.RegisterRace( new Undead( 48, 48 ) ); // Index 17 in AllRaces
            list.Add( Undead );

            RaceDefinitions.RegisterRace( new Naglor( 49, 49 ) ); // Index 18 in AllRaces
            list.Add( Naglor );

            RaceDefinitions.RegisterRace( new HalfDaemon( 50, 50 ) ); // Index 19 in AllRaces
            list.Add( HalfDaemon );

            RaceDefinitions.RegisterRace( new Werewolf( 51, 51 ) ); // Index 20 in AllRaces
            list.Add( Werewolf );

            Instances = list.ToArray();
        }

        public static bool IsElfRace( Race race )
        {
            return race == HighElf || race == NorthernElf;
        }
    }
}