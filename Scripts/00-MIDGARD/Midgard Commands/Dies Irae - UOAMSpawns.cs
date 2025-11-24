using System;
using System.Collections;
using System.IO;
using Server;
using Server.Commands;
using Server.Mobiles;

namespace Midgard.Commands
{
    public class UOAMSpawns
    {
        public const string BritainCemetery = "Skeleton Zombie Ghoul";
        public const string CemeteryCount = "3";

        /// <summary>
        /// what z place the spawner? 0 = land. For each unit raise by 20;
        /// </summary>
        private const int Floor = 0;

        /// <summary>
        /// How far should they wander?
        /// </summary>
        private const int HomeRange = 5;

        /// <summary>
        /// direction faced the NPC
        /// </summary>
        private const int m_direction = (int)Direction.North;

        /// <summary>
        /// 2 npcs per type (so a mage spawner will spawn 2 npcs, a alchemist and herbalist spawner will spawn 4 npcs total)
        /// </summary>
        private const int NPCCount = 2;

        public const string STDCemetery = "Ghoul Shade Skeleton Spectre Wraith Zombie";
        public const string STDCemeteryInside = "Lich";

        /// <summary>
        /// "team" the npcs are on
        /// </summary>
        private const int Team = 0;

        /// <summary>
        /// Should we spawn them up right away?
        /// </summary>
        private const bool TotalRespawn = true;

        public const string YewCemetery = "Skeleton";

        /// <summary>
        /// enable the clearspawn utility.
        /// </summary>
        private static bool ClearSpawnEnable = false;

        private static int m_Count;

        /// <summary>
        /// max spawn time
        /// </summary>
        private static double m_MaxTime = 10.0;

        /// <summary>
        /// min spawn time
        /// </summary>
        private static double m_MinTime = 2.5;

        public static void Initialize()
        {
            CommandSystem.Register( "UOAMSpawns", AccessLevel.Administrator, new CommandEventHandler( UOAMSpawns_OnCommand ) );
            CommandSystem.Register( "CleanSpawns", AccessLevel.Administrator, new CommandEventHandler( CleanSpawns_OnCommand ) );
        }

        [ Usage( "UOAMSpawns" ) ]
        [ Description( "Generates world spawns by Spawns.map file (taken from UOAutoMap)" ) ]
        private static void UOAMSpawns_OnCommand( CommandEventArgs e )
        {
            Parse( e.Mobile );
        }

        [ Usage( "CleanSpawns" ) ]
        [ Description( "Delete uoam world spawns" ) ]
        private static void CleanSpawns_OnCommand( CommandEventArgs e )
        {
            e.Mobile.SendMessage( "Deleting old UOAM Spawns..." );
            ClearSpawners();
            e.Mobile.SendMessage( "Deletation complete!" );
        }

        public static void Parse( Mobile from )
        {
            string spawnPath = Path.Combine( Core.BaseDirectory, "Data/Spawns.map" );
            m_Count = 0;

            if( File.Exists( spawnPath ) )
            {
                if( File.Exists( "Data/SpawnList.cfg" ) && ClearSpawnEnable )
                {
                    from.SendMessage( "Deleting old UOAM Spawns..." );
                    ClearSpawners();
                    from.SendMessage( "Deletation complete!" );
                }

                ArrayList FileLine = new ArrayList();
                ArrayList TypeLine = new ArrayList();

                from.SendMessage( "Generating Spawns..." );

                using( StreamReader ip = new StreamReader( spawnPath ) )
                {
                    string line;

                    while( ( line = ip.ReadLine() ) != null )
                    {
                        int indexOf = line.IndexOf( ':' );

                        if( indexOf == -1 )
                            continue;

                        string type = line.Substring( 0, ++indexOf ).Trim();
                        string sub = line.Substring( indexOf ).Trim();

                        string[] split = sub.Split( ' ' );
                        TypeLine.Add( type );
                        FileLine.Add( split );
                    }
                }

                for ( int k = 0; k < FileLine.Count; k++ )
                {
                    string type = TypeLine[ k ] as string;
                    string[] split = FileLine[ k ] as string[];

                    ArrayList valori = new ArrayList();

                    if( split == null )
                        continue;
                    else
                    {
                        if( split.Length < 3 )
                            continue;

                        valori.Add( split[ 0 ] );
                        valori.Add( split[ 1 ] );
                        valori.Add( split[ 2 ] );

                        //For Monster & Treasure Chest UOAMType, x, y, map, UserType [Floor] [HomeDist] [SpawnRange] [Amount] [MinTime] [MaxTime] [Team] [Direction]
                        //For Vendors UOAMType, x, y, map, [Floor] [HomeDist] [SpawnRange] [Amount] [MinTime] [MaxTime] [Team] [Direction]
                        if( type.CompareTo( "+Monster:" ) != 0 && type.CompareTo( "+Treasure:" ) != 0 && type.CompareTo( "+Camp:" ) != 0 )
                        {
                            try
                            {
                                valori.Add( Convert.ToInt32( split[ 3 ] ).ToString() );
                            }
                            catch
                            {
                                valori.Add( Floor.ToString() );
                            }
                        }
                        else
                        {
                            try
                            {
                                valori.Add( split[ 3 ] );
                            }
                            catch
                            {
                                continue;
                            }
                        }

                        if( type.CompareTo( "+Monster:" ) != 0 && type.CompareTo( "+Treasure:" ) != 0 && type.CompareTo( "+Camp:" ) != 0 )
                        {
                            try
                            {
                                valori.Add( Convert.ToInt32( split[ 4 ] ).ToString() );
                            }
                            catch
                            {
                                valori.Add( HomeRange.ToString() );
                            }
                        }
                        else
                        {
                            try
                            {
                                valori.Add( Convert.ToInt32( split[ 4 ] ).ToString() );
                            }
                            catch
                            {
                                valori.Add( Floor.ToString() );
                            }
                        }

                        if( type.CompareTo( "+Monster:" ) != 0 && type.CompareTo( "+Treasure:" ) != 0 && type.CompareTo( "+Camp:" ) != 0 )
                        {
                            try
                            {
                                valori.Add( Convert.ToInt32( split[ 5 ] ).ToString() );
                            }
                            catch
                            {
                                valori.Add( HomeRange.ToString() );
                            }
                        }
                        else
                        {
                            try
                            {
                                valori.Add( Convert.ToInt32( split[ 5 ] ).ToString() );
                            }
                            catch
                            {
                                valori.Add( HomeRange.ToString() );
                            }
                        }

                        if( type.CompareTo( "+Monster:" ) != 0 && type.CompareTo( "+Treasure:" ) != 0 && type.CompareTo( "+Camp:" ) != 0 )
                        {
                            try
                            {
                                valori.Add( Convert.ToInt32( split[ 6 ] ).ToString() );
                            }
                            catch
                            {
                                valori.Add( NPCCount.ToString() );
                            }
                        }
                        else
                        {
                            try
                            {
                                valori.Add( Convert.ToInt32( split[ 6 ] ).ToString() );
                            }
                            catch
                            {
                                valori.Add( HomeRange.ToString() );
                            }
                        }

                        if( type.CompareTo( "+Monster:" ) != 0 && type.CompareTo( "+Treasure:" ) != 0 && type.CompareTo( "+Camp:" ) != 0 )
                        {
                            try
                            {
                                valori.Add( Convert.ToInt32( split[ 7 ] ).ToString() );
                            }
                            catch
                            {
                                valori.Add( m_MinTime.ToString() );
                            }
                        }
                        else
                        {
                            try
                            {
                                valori.Add( Convert.ToInt32( split[ 7 ] ).ToString() );
                            }
                            catch
                            {
                                valori.Add( NPCCount.ToString() );
                            }
                        }

                        if( type.CompareTo( "+Monster:" ) != 0 && type.CompareTo( "+Treasure:" ) != 0 && type.CompareTo( "+Camp:" ) != 0 )
                        {
                            try
                            {
                                valori.Add( Convert.ToInt32( split[ 8 ] ).ToString() );
                            }
                            catch
                            {
                                valori.Add( m_MaxTime.ToString() );
                            }
                        }
                        else
                        {
                            try
                            {
                                valori.Add( Convert.ToInt32( split[ 8 ] ).ToString() );
                            }
                            catch
                            {
                                valori.Add( m_MinTime.ToString() );
                            }
                        }

                        if( type.CompareTo( "+Monster:" ) != 0 && type.CompareTo( "+Treasure:" ) != 0 && type.CompareTo( "+Camp:" ) != 0 )
                        {
                            try
                            {
                                valori.Add( Convert.ToInt32( split[ 9 ] ).ToString() );
                            }
                            catch
                            {
                                valori.Add( Team.ToString() );
                            }
                        }
                        else
                        {
                            try
                            {
                                valori.Add( Convert.ToInt32( split[ 9 ] ).ToString() );
                            }
                            catch
                            {
                                valori.Add( m_MaxTime.ToString() );
                            }
                        }

                        if( type.CompareTo( "+Monster:" ) != 0 && type.CompareTo( "+Treasure:" ) != 0 && type.CompareTo( "+Camp:" ) != 0 )
                        {
                            try
                            {
                                valori.Add( Convert.ToInt32( split[ 10 ] ).ToString() );
                            }
                            catch
                            {
                                valori.Add( m_direction.ToString() );
                            }
                        }
                        else
                        {
                            try
                            {
                                valori.Add( Convert.ToInt32( split[ 10 ] ).ToString() );
                            }
                            catch
                            {
                                valori.Add( Team.ToString() );
                            }
                        }

                        if( type.CompareTo( "+Monster:" ) != 0 && type.CompareTo( "+Treasure:" ) != 0 && type.CompareTo( "+Camp:" ) != 0 )
                        {
                            try
                            {
                                valori.Add( Convert.ToInt32( split[ 11 ] ).ToString() );
                            }
                            catch
                            {
                                valori.Add( "" );
                            }
                        }
                        else
                        {
                            try
                            {
                                valori.Add( Convert.ToInt32( split[ 11 ] ).ToString() );
                            }
                            catch
                            {
                                valori.Add( m_direction.ToString() );
                            }
                        }
                    }

                    split = new string[]{ type, (string)valori[ 0 ], (string)valori[ 1 ], (string)valori[ 2 ], (string)valori[ 3 ], (string)valori[ 4 ], (string)valori[ 5 ], (string)valori[ 6 ], (string)valori[ 7 ], (string)valori[ 8 ], (string)valori[ 9 ], (string)valori[ 10 ], (string)valori[ 11 ] };

                    switch( split[ 0 ].ToLower() )
                    {
                        case "-healer:":
                            PlaceNPC( split[ 1 ], split[ 2 ], split[ 3 ], split[ 4 ], split[ 5 ], split[ 6 ], split[ 7 ], split[ 8 ], split[ 9 ], split[ 10 ], split[ 11 ], "Healer", "HealerGuildmaster" );
                            break;
                        case "-baker:":
                            PlaceNPC( split[ 1 ], split[ 2 ], split[ 3 ], split[ 4 ], split[ 5 ], split[ 6 ], split[ 7 ], split[ 8 ], split[ 9 ], split[ 10 ], split[ 11 ], "Baker" );
                            break;
                        case "-gypsymaiden:":
                            PlaceNPC( split[ 1 ], split[ 2 ], split[ 3 ], split[ 4 ], split[ 5 ], split[ 6 ], split[ 7 ], split[ 8 ], split[ 9 ], split[ 10 ], split[ 11 ], "GypsyMaiden" );
                            break;
                        case "-gypsybank:":
                            PlaceNPC( split[ 1 ], split[ 2 ], split[ 3 ], split[ 4 ], split[ 5 ], split[ 6 ], split[ 7 ], split[ 8 ], split[ 9 ], split[ 10 ], split[ 11 ], "GypsyBanker" );
                            break;
                        case "-bank:":
                            PlaceNPC( split[ 1 ], split[ 2 ], split[ 3 ], split[ 4 ], split[ 5 ], split[ 6 ], split[ 7 ], split[ 8 ], split[ 9 ], split[ 10 ], split[ 11 ], "Banker", "Minter" );
                            break;
                        case "-inn:":
                            PlaceNPC( split[ 1 ], split[ 2 ], split[ 3 ], split[ 4 ], split[ 5 ], split[ 6 ], split[ 7 ], split[ 8 ], split[ 9 ], split[ 10 ], split[ 11 ], "Innkeeper" );
                            break;
                        case "-provisioner:":
                            PlaceNPC( split[ 1 ], split[ 2 ], split[ 3 ], split[ 4 ], split[ 5 ], split[ 6 ], split[ 7 ], split[ 8 ], split[ 9 ], split[ 10 ], split[ 11 ], "Provisioner", "Cobbler" );
                            break;
                        case "-tailor:":
                            PlaceNPC( split[ 1 ], split[ 2 ], split[ 3 ], split[ 4 ], split[ 5 ], split[ 6 ], split[ 7 ], split[ 8 ], split[ 9 ], split[ 10 ], split[ 11 ], "Tailor", "Weaver", "TailorGuildmaster" );
                            break;
                        case "-tavern:":
                            PlaceNPC( split[ 1 ], split[ 2 ], split[ 3 ], split[ 4 ], split[ 5 ], split[ 6 ], split[ 7 ], split[ 8 ], split[ 9 ], split[ 10 ], split[ 11 ], "Tavernkeeper", "Waiter", "Cook", "Barkeeper" );
                            break;
                        case "-reagents:":
                            PlaceNPC( split[ 1 ], split[ 2 ], split[ 3 ], split[ 4 ], split[ 5 ], split[ 6 ], split[ 7 ], split[ 8 ], split[ 9 ], split[ 10 ], split[ 11 ], "Herbalist", "Alchemist", "CustomHairstylist" );
                            break;
                        case "-fortuneteller:":
                            PlaceNPC( split[ 1 ], split[ 2 ], split[ 3 ], split[ 4 ], split[ 5 ], split[ 6 ], split[ 7 ], split[ 8 ], split[ 9 ], split[ 10 ], split[ 11 ], "FortuneTeller" );
                            break;
                        case "-holymage:":
                            PlaceNPC( split[ 1 ], split[ 2 ], split[ 3 ], split[ 4 ], split[ 5 ], split[ 6 ], split[ 7 ], split[ 8 ], split[ 9 ], split[ 10 ], split[ 11 ], "HolyMage" );
                            break;
                        case "-chivalrykeeper:":
                            PlaceNPC( split[ 1 ], split[ 2 ], split[ 3 ], split[ 4 ], split[ 5 ], split[ 6 ], split[ 7 ], split[ 8 ], split[ 9 ], split[ 10 ], split[ 11 ], "KeeperOfChivalry" );
                            break;
                        case "-mage:":
                            PlaceNPC( split[ 1 ], split[ 2 ], split[ 3 ], split[ 4 ], split[ 5 ], split[ 6 ], split[ 7 ], split[ 8 ], split[ 9 ], split[ 10 ], split[ 11 ], "Mage", "Alchemist", "MageGuildmaster" );
                            break;
                        case "-arms:":
                            PlaceNPC( split[ 1 ], split[ 2 ], split[ 3 ], split[ 4 ], split[ 5 ], split[ 6 ], split[ 7 ], split[ 8 ], split[ 9 ], split[ 10 ], split[ 11 ], "Armorer", "Weaponsmith" );
                            break;
                        case "-tinker:":
                            PlaceNPC( split[ 1 ], split[ 2 ], split[ 3 ], split[ 4 ], split[ 5 ], split[ 6 ], split[ 7 ], split[ 8 ], split[ 9 ], split[ 10 ], split[ 11 ], "Tinker", "TinkerGuildmaster" );
                            break;
                        case "-gypsystable:":
                            PlaceNPC( split[ 1 ], split[ 2 ], split[ 3 ], split[ 4 ], split[ 5 ], split[ 6 ], split[ 7 ], split[ 8 ], split[ 9 ], split[ 10 ], split[ 11 ], "GypsyAnimalTrainer" );
                            break;
                        case "-stable:":
                            PlaceNPC( split[ 1 ], split[ 2 ], split[ 3 ], split[ 4 ], split[ 5 ], split[ 6 ], split[ 7 ], split[ 8 ], split[ 9 ], split[ 10 ], split[ 11 ], "AnimalTrainer" );
                            break;
                        case "-blacksmith:":
                            PlaceNPC( split[ 1 ], split[ 2 ], split[ 3 ], split[ 4 ], split[ 5 ], split[ 6 ], split[ 7 ], split[ 8 ], split[ 9 ], split[ 10 ], split[ 11 ], "Blacksmith", "BlacksmithGuildmaster" );
                            break;
                        case "-bowyer:":
                        case "-fletcher:":
                            PlaceNPC( split[ 1 ], split[ 2 ], split[ 3 ], split[ 4 ], split[ 5 ], split[ 6 ], split[ 7 ], split[ 8 ], split[ 9 ], split[ 10 ], split[ 11 ], "Bowyer" );
                            break;
                        case "-carpenter:":
                            PlaceNPC( split[ 1 ], split[ 2 ], split[ 3 ], split[ 4 ], split[ 5 ], split[ 6 ], split[ 7 ], split[ 8 ], split[ 9 ], split[ 10 ], split[ 11 ], "Carpenter", "Architect", "RealEstateBroker" );
                            break;
                        case "-butcher:":
                            PlaceNPC( split[ 1 ], split[ 2 ], split[ 3 ], split[ 4 ], split[ 5 ], split[ 6 ], split[ 7 ], split[ 8 ], split[ 9 ], split[ 10 ], split[ 11 ], "Butcher" );
                            break;
                        case "-jeweler:":
                            PlaceNPC( split[ 1 ], split[ 2 ], split[ 3 ], split[ 4 ], split[ 5 ], split[ 6 ], split[ 7 ], split[ 8 ], split[ 9 ], split[ 10 ], split[ 11 ], "Jeweler" );
                            break;
                        case "-tanner:":
                            PlaceNPC( split[ 1 ], split[ 2 ], split[ 3 ], split[ 4 ], split[ 5 ], split[ 6 ], split[ 7 ], split[ 8 ], split[ 9 ], split[ 10 ], split[ 11 ], "Tanner", "Furtrader" );
                            break;
                        case "-bard:":
                            PlaceNPC( split[ 1 ], split[ 2 ], split[ 3 ], split[ 4 ], split[ 5 ], split[ 6 ], split[ 7 ], split[ 8 ], split[ 9 ], split[ 10 ], split[ 11 ], "Bard", "BardGuildmaster" );
                            break;
                        case "-market:":
                            PlaceNPC( split[ 1 ], split[ 2 ], split[ 3 ], split[ 4 ], split[ 5 ], split[ 6 ], split[ 7 ], split[ 8 ], split[ 9 ], split[ 10 ], split[ 11 ], "Butcher", "Farmer" );
                            break;
                        case "-library:":
                            PlaceNPC( split[ 1 ], split[ 2 ], split[ 3 ], split[ 4 ], split[ 5 ], split[ 6 ], split[ 7 ], split[ 8 ], split[ 9 ], split[ 10 ], split[ 11 ], "Scribe" );
                            break;
                        case "-shipwright:":
                            PlaceNPC( split[ 1 ], split[ 2 ], split[ 3 ], split[ 4 ], split[ 5 ], split[ 6 ], split[ 7 ], split[ 8 ], split[ 9 ], split[ 10 ], split[ 11 ], "Shipwright", "Mapmaker" );
                            break;
                        case "-docks:":
                            PlaceNPC( split[ 1 ], split[ 2 ], split[ 3 ], split[ 4 ], split[ 5 ], split[ 6 ], split[ 7 ], split[ 8 ], split[ 9 ], split[ 10 ], split[ 11 ], "Fisherman" );
                            break;

                        case "-beekeeper:":
                            PlaceNPC( split[ 1 ], split[ 2 ], split[ 3 ], split[ 4 ], split[ 5 ], split[ 6 ], split[ 7 ], split[ 8 ], split[ 9 ], split[ 10 ], split[ 11 ], "Beekeeper" );
                            break;

                            // Guilds & Misc
                        case "-tinkers guild:":
                            PlaceNPC( split[ 1 ], split[ 2 ], split[ 3 ], split[ 4 ], split[ 5 ], split[ 6 ], split[ 7 ], split[ 8 ], split[ 9 ], split[ 10 ], split[ 11 ], "Tinker", "TinkerGuildmaster" );
                            break;
                        case "-blacksmiths guild:":
                            PlaceNPC( split[ 1 ], split[ 2 ], split[ 3 ], split[ 4 ], split[ 5 ], split[ 6 ], split[ 7 ], split[ 8 ], split[ 9 ], split[ 10 ], split[ 11 ], "Blacksmith", "BlacksmithGuildmaster" );
                            break;
                        case "-sorcerors guild:":
                            PlaceNPC( split[ 1 ], split[ 2 ], split[ 3 ], split[ 4 ], split[ 5 ], split[ 6 ], split[ 7 ], split[ 8 ], split[ 9 ], split[ 10 ], split[ 11 ], "Mage", "MageGuildmaster" );
                            break;
                        case "-customs:":
                            PlaceNPC( split[ 1 ], split[ 2 ], split[ 3 ], split[ 4 ], split[ 5 ], split[ 6 ], split[ 7 ], split[ 8 ], split[ 9 ], split[ 10 ], split[ 11 ], "Shipwright", "Mapmaker" );
                            break;
                        case "-warriors guild:":
                            PlaceNPC( split[ 1 ], split[ 2 ], split[ 3 ], split[ 4 ], split[ 5 ], split[ 6 ], split[ 7 ], split[ 8 ], split[ 9 ], split[ 10 ], split[ 11 ], "WarriorGuildmaster" );
                            break;
                        case "-archers guild:":
                            PlaceNPC( split[ 1 ], split[ 2 ], split[ 3 ], split[ 4 ], split[ 5 ], split[ 6 ], split[ 7 ], split[ 8 ], split[ 9 ], split[ 10 ], split[ 11 ], "RangerGuildmaster" );
                            break;
                        case "-miners guild:":
                            PlaceNPC( split[ 1 ], split[ 2 ], split[ 3 ], split[ 4 ], split[ 5 ], split[ 6 ], split[ 7 ], split[ 8 ], split[ 9 ], split[ 10 ], split[ 11 ], "MinerGuildmaster" );
                            break;
                        case "-fishermans guild:":
                            PlaceNPC( split[ 1 ], split[ 2 ], split[ 3 ], split[ 4 ], split[ 5 ], split[ 6 ], split[ 7 ], split[ 8 ], split[ 9 ], split[ 10 ], split[ 11 ], "Fisherman", "FisherGuildmaster" );
                            break;
                        case "-merchants guild:":
                            PlaceNPC( split[ 1 ], split[ 2 ], split[ 3 ], split[ 4 ], split[ 5 ], split[ 6 ], split[ 7 ], split[ 8 ], split[ 9 ], split[ 10 ], split[ 11 ], "MerchantGuildmaster" );
                            break;
                        case "-sorcerers guild:":
                        case "-illusionists guild:":
                        case "-mages guild:":
                            PlaceNPC( split[ 1 ], split[ 2 ], split[ 3 ], split[ 4 ], split[ 5 ], split[ 6 ], split[ 7 ], split[ 8 ], split[ 9 ], split[ 10 ], split[ 11 ], "Mage", "MageGuildmaster" );
                            break;
                        case "-armourers guild:":
                            PlaceNPC( split[ 1 ], split[ 2 ], split[ 3 ], split[ 4 ], split[ 5 ], split[ 6 ], split[ 7 ], split[ 8 ], split[ 9 ], split[ 10 ], split[ 11 ], "Armorer", "BlacksmithGuildmaster" );
                            break;
                        case "-weapons guild:":
                            PlaceNPC( split[ 1 ], split[ 2 ], split[ 3 ], split[ 4 ], split[ 5 ], split[ 6 ], split[ 7 ], split[ 8 ], split[ 9 ], split[ 10 ], split[ 11 ], "Weaponsmith", "BlacksmithGuildmaster" );
                            break;
                        case "-bardic guild:":
                            PlaceNPC( split[ 1 ], split[ 2 ], split[ 3 ], split[ 4 ], split[ 5 ], split[ 6 ], split[ 7 ], split[ 8 ], split[ 9 ], split[ 10 ], "Bard", "BardGuildmaster" );
                            break;
                        case "-rogues guild:":
                        case "-thieves guild:":
                            PlaceNPC( split[ 1 ], split[ 2 ], split[ 3 ], split[ 4 ], split[ 5 ], split[ 6 ], split[ 7 ], split[ 8 ], split[ 9 ], split[ 10 ], split[ 11 ], "ThiefGuildmaster" );
                            break;

                        case "+vendor:":
                        case "+monster:":
                        {
                            switch( split[ 4 ].ToLower() )
                            {
                                case "britaincemetery":
                                {
                                    PlaceMonster( split[ 1 ], split[ 2 ], split[ 3 ], split[ 5 ], split[ 6 ], CemeteryCount, split[ 8 ], split[ 9 ], split[ 10 ], split[ 11 ], split[ 12 ], BritainCemetery );
                                    break;
                                }
                                case "yewcemetery":
                                {
                                    PlaceMonster( split[ 1 ], split[ 2 ], split[ 3 ], split[ 5 ], split[ 6 ], CemeteryCount, split[ 8 ], split[ 9 ], split[ 10 ], split[ 11 ], split[ 12 ], YewCemetery );
                                    break;
                                }
                                case "stdcemetery":
                                {
                                    PlaceMonster( split[ 1 ], split[ 2 ], split[ 3 ], split[ 5 ], split[ 6 ], CemeteryCount, split[ 8 ], split[ 9 ], split[ 10 ], split[ 11 ], split[ 12 ], STDCemetery );
                                    break;
                                }
                                case "stdcemeteryinside":
                                {
                                    PlaceMonster( split[ 1 ], split[ 2 ], split[ 3 ], split[ 5 ], split[ 6 ], split[ 7 ], split[ 8 ], split[ 9 ], split[ 10 ], split[ 11 ], split[ 12 ], STDCemeteryInside );
                                    break;
                                }
                                default:
                                {
                                    string typpe = split[ 4 ];

                                    string[] typ = typpe.Split( '|' );

                                    if( typ.Length > 0 ) //there is | operator?
                                    {
                                        typpe = typ[ Utility.RandomMinMax( 0, typ.Length - 1 ) ]; //randomize from specified type
                                        typ = typpe.Split( '&' );
                                        if( typ.Length > 0 ) //there is & operator?
                                        {
                                            typpe = "";
                                            for ( int i = 0; i < typ.Length; i++ )
                                                typpe = typpe + " " + typ[ i ];
                                        }
                                    }
                                    else //no | operator let's try with & only
                                    {
                                        typ = typpe.Split( '&' );

                                        if( typ.Length > 0 ) //there is & operator?
                                        {
                                            typpe = "";
                                            for ( int i = 0; i < typ.Length; i++ )
                                                typpe = typpe + " " + typ[ i ];
                                        }
                                    }
                                    typpe.Trim();
                                    if( typpe.Length >= 3 )
                                        PlaceMonster( split[ 1 ], split[ 2 ], split[ 3 ], split[ 5 ], split[ 6 ], split[ 7 ], split[ 8 ], split[ 9 ], split[ 10 ], split[ 11 ], split[ 12 ], typpe );
                                    break;
                                }
                            }
                            break;
                        }

                        case "+camp:":
                        {
                            switch( split[ 4 ] )
                            {
                                case "gypsy":
                                {
                                    PlaceCamp( split[ 1 ], split[ 2 ], split[ 3 ], 1 );
                                    break;
                                }
                                case "prison":
                                {
                                    PlaceCamp( split[ 1 ], split[ 2 ], split[ 3 ], 0 );
                                    break;
                                }
                            }
                            break;
                        }

                        case "+treasure:":
                        {
                            string typpe = split[ 4 ];

                            string[] typ = typpe.Split( '|' );

                            if( typ.Length > 0 ) //there is | operator?
                            {
                                typpe = typ[ Utility.RandomMinMax( 0, typ.Length - 1 ) ]; //randomize from specified type
                                typ = typpe.Split( '&' );
                                if( typ.Length > 0 ) //there is & operator?
                                {
                                    typpe = "";
                                    for ( int i = 0; i < typ.Length; i++ )
                                        typpe = typpe + " " + typ[ i ];
                                }
                            }
                            else //no | operator let's try with & only
                            {
                                typ = typpe.Split( '&' );

                                if( typ.Length > 0 ) //there is & operator?
                                {
                                    typpe = "";
                                    for ( int i = 0; i < typ.Length; i++ )
                                        typpe = typpe + " " + typ[ i ];
                                }
                            }
                            typpe.Trim();
                            if( typpe.Length >= 3 )
                                PlaceChest( split[ 1 ], split[ 2 ], split[ 3 ], split[ 5 ], split[ 6 ], split[ 7 ], split[ 8 ], split[ 9 ], split[ 10 ], split[ 11 ], split[ 12 ], typpe );
                            break;
                        }
                            // Skip
                        case "+champion:":
                        case "-painter:":
                        case "-theater:":
                        case "+landmark:":
                        case "-point of interest:":
                        case "+shrine:":
                        case "+moongate:":
                        case "+dungeon:":
                        case "+scenic:":
                        case "-gate:":
                        case "+Body of Water:":
                        case "+ruins:":
                        case "+teleporter:":
                        case "+Terrain:":
                        case "-exit:":
                        case "-bridge:":
                        case "-other:":
                        case "-stairs:":
                        case "-guild:":
                        case "+graveyard:":
                        case "+Island:":
                        case "+town:":
                            break;
                            /*default:
                                    Console.WriteLine(split[0]);
				
                                        break;*/
                    }
                }
            }
            else
                from.SendMessage( "{0} not found!", spawnPath );

            from.SendMessage( "Done, added {0} spawners", m_Count );
        }

        //CEMETERY
        // -- END --

        public static void PlaceCamp( string sx, string sy, string sm, int type ) //Camps
        {
            int x = Utility.ToInt32( sx );
            int y = Utility.ToInt32( sy );
            int map = Utility.ToInt32( sm );
            string[] types;
            if( type == 1 )
                types = new string[]{ "BankerCamp" };
            else
                types = new string[]{ "OrcCamp", "RatCamp" };
            switch( map )
            {
                case 0: //Trammel and Felucca
                    MakeCamp( types, x, y, Map.Felucca );
                    MakeCamp( types, x, y, Map.Trammel );
                    break;
                case 1: //Felucca
                    MakeCamp( types, x, y, Map.Felucca );
                    break;
                case 2: //Trammel
                    MakeCamp( types, x, y, Map.Trammel );
                    break;
                case 3: //Ilshenar
                    MakeCamp( types, x, y, Map.Ilshenar );
                    break;
                case 4: //Malas
                    MakeCamp( types, x, y, Map.Malas );
                    break;
                case 5: //Tokuno
                    MakeCamp( types, x, y, Map.Tokuno );
                    break;
                default:
                    Console.WriteLine( "UOAM Parser: Warning, unknown map {0}", map );
                    break;
            }
        }

        //									 x,			y,		 map,		 [Floor]		[HomeDist]		[SpawnRange]		 [Amount]		[MinTime]		[MaxTime]		[Team]		  [Direction]		MonsterType
        public static void PlaceChest( string sx, string sy, string sm, string m_floor, string HomeDist, string spawnRange, string Amount, string MinTime, string MaxTime, string Team, string direction, string type ) //Treasure Chests
        {
            string[] types = type.Split( ' ' );
            if( types.Length == 0 )
                return;

            int x = Utility.ToInt32( sx );
            int y = Utility.ToInt32( sy );
            int map = Utility.ToInt32( sm );
            int Home = Utility.ToInt32( HomeDist );
            int SpawnRange = Utility.ToInt32( spawnRange );
            int amount = Utility.ToInt32( Amount );
            TimeSpan minTime = TimeSpan.FromMinutes( Convert.ToDouble( MinTime ) );
            TimeSpan maxTime = TimeSpan.FromMinutes( Convert.ToDouble( MaxTime ) );
            int team = Utility.ToInt32( Team );
            Direction direct = ( (Direction)Convert.ToInt32( direction ) );

            int floor = 0;

            switch( map )
            {
                case 0: //Trammel and Felucca

                    floor = Utility.ToInt32( m_floor );

                    MakeSpawnerTreasure( types, x, y, floor, Map.Felucca, Home, SpawnRange, amount, minTime, maxTime, team, direct );
                    MakeSpawnerTreasure( types, x, y, floor, Map.Trammel, Home, SpawnRange, amount, minTime, maxTime, team, direct );
                    break;
                case 1: //Felucca

                    floor = Utility.ToInt32( m_floor );

                    MakeSpawnerTreasure( types, x, y, floor, Map.Felucca, Home, SpawnRange, amount, minTime, maxTime, team, direct );
                    break;
                case 2: //Trammel

                    floor = Utility.ToInt32( m_floor );

                    MakeSpawnerTreasure( types, x, y, floor, Map.Trammel, Home, SpawnRange, amount, minTime, maxTime, team, direct );
                    break;
                case 3: //Ilshenar

                    floor = Utility.ToInt32( m_floor );

                    MakeSpawnerTreasure( types, x, y, floor, Map.Ilshenar, Home, SpawnRange, amount, minTime, maxTime, team, direct );
                    break;
                case 4: //Malas

                    floor = Utility.ToInt32( m_floor );

                    MakeSpawnerTreasure( types, x, y, floor, Map.Malas, Home, SpawnRange, amount, minTime, maxTime, team, direct );
                    break;
                case 5: //Tokuno

                    floor = Utility.ToInt32( m_floor );

                    MakeSpawnerTreasure( types, x, y, floor, Map.Tokuno, Home, SpawnRange, amount, minTime, maxTime, team, direct );
                    break;
                default:
                    Console.WriteLine( "UOAM Parser: Warning, unknown map {0}", map );
                    break;
            }
        }

        //									     x,			y,		 map,		 [Floor]		[HomeDist]     [SpawnRange]		 [Amount]		[MinTime]		[MaxTime]		[Team]		  [Direction]		MonsterType
        public static void PlaceMonster( string sx, string sy, string sm, string m_floor, string HomeDist, string spawnRange, string Amount, string MinTime, string MaxTime, string Team, string direction, string type ) //Monster
        {
            string[] types = type.Split( ' ' );
            if( types.Length == 0 )
                return;

            int x = Utility.ToInt32( sx );
            int y = Utility.ToInt32( sy );
            int map = Utility.ToInt32( sm );
            int Home = Utility.ToInt32( HomeDist );
            int SpawnRange = Utility.ToInt32( spawnRange );
            int amount = Utility.ToInt32( Amount );
            TimeSpan minTime = TimeSpan.FromMinutes( Convert.ToDouble( MinTime ) );
            TimeSpan maxTime = TimeSpan.FromMinutes( Convert.ToDouble( MaxTime ) );
            int team = Utility.ToInt32( Team );
            Direction direct = ( (Direction)Convert.ToInt32( direction ) );

            int floor = 0;

            switch( map )
            {
                case 0: //Trammel and Felucca

                    floor = Utility.ToInt32( m_floor );

                    MakeSpawnerMonster( types, x, y, floor, Map.Felucca, Home, SpawnRange, amount, minTime, maxTime, team, direct );
                    MakeSpawnerMonster( types, x, y, floor, Map.Trammel, Home, SpawnRange, amount, minTime, maxTime, team, direct );
                    break;
                case 1: //Felucca

                    floor = Utility.ToInt32( m_floor );

                    MakeSpawnerMonster( types, x, y, floor, Map.Felucca, Home, SpawnRange, amount, minTime, maxTime, team, direct );
                    break;
                case 2: //Trammel

                    floor = Utility.ToInt32( m_floor );

                    MakeSpawnerMonster( types, x, y, floor, Map.Trammel, Home, SpawnRange, amount, minTime, maxTime, team, direct );
                    break;
                case 3: //Ilshenar

                    floor = Utility.ToInt32( m_floor );

                    MakeSpawnerMonster( types, x, y, floor, Map.Ilshenar, Home, SpawnRange, amount, minTime, maxTime, team, direct );
                    break;
                case 4: //Malas

                    floor = Utility.ToInt32( m_floor );

                    MakeSpawnerMonster( types, x, y, floor, Map.Malas, Home, SpawnRange, amount, minTime, maxTime, team, direct );
                    break;
                case 5: //Tokuno

                    floor = Utility.ToInt32( m_floor );

                    MakeSpawnerMonster( types, x, y, floor, Map.Tokuno, Home, SpawnRange, amount, minTime, maxTime, team, direct );
                    break;
                default:
                    Console.WriteLine( "UOAM Parser: Warning, unknown map {0}", map );
                    break;
            }
        }

        //                                   x,			y,		 map,		 [Floor]		[HomeDist]    [SpawnRange]		 [Amount]		[MinTime]		[MaxTime]		[Team]		  [Direction]			  VendorType
        public static void PlaceNPC( string sx, string sy, string sm, string m_floor, string HomeDist, string spawnRange, string Amount, string MinTime, string MaxTime, string Team, string direction, params string[] types ) //Vendor
        {
            if( types.Length == 0 )
                return;

            int x = Utility.ToInt32( sx );
            int y = Utility.ToInt32( sy );
            int map = Utility.ToInt32( sm );
            int Home = Utility.ToInt32( HomeDist );
            int SpawnRange = Utility.ToInt32( spawnRange );
            int amount = Utility.ToInt32( Amount );
            TimeSpan minTime = TimeSpan.FromMinutes( Convert.ToDouble( MinTime ) );
            TimeSpan maxTime = TimeSpan.FromMinutes( Convert.ToDouble( MaxTime ) );
            int team = Utility.ToInt32( Team );
            Direction direct = ( (Direction)Convert.ToInt32( direction ) );

            int floor = 0;

            switch( map )
            {
                case 0: //Trammel and Felucca

                    floor = Utility.ToInt32( m_floor );

                    MakeSpawnerVendor( types, x, y, floor, Map.Felucca, Home, SpawnRange, amount, minTime, maxTime, team, direct );
                    MakeSpawnerVendor( types, x, y, floor, Map.Trammel, Home, SpawnRange, amount, minTime, maxTime, team, direct );
                    break;
                case 1: //Felucca

                    floor = Utility.ToInt32( m_floor );

                    MakeSpawnerVendor( types, x, y, floor, Map.Felucca, Home, SpawnRange, amount, minTime, maxTime, team, direct );
                    break;
                case 2: //Trammel

                    floor = Utility.ToInt32( m_floor );
                    MakeSpawnerVendor( types, x, y, floor, Map.Trammel, Home, SpawnRange, amount, minTime, maxTime, team, direct );
                    break;
                case 3: //Ilshenar

                    floor = Utility.ToInt32( m_floor );

                    MakeSpawnerVendor( types, x, y, floor, Map.Ilshenar, Home, SpawnRange, amount, minTime, maxTime, team, direct );
                    break;
                case 4: //Malas

                    floor = Utility.ToInt32( m_floor );

                    MakeSpawnerVendor( types, x, y, floor, Map.Malas, Home, SpawnRange, amount, minTime, maxTime, team, direct );
                    break;
                case 5: //Tokuno

                    floor = Utility.ToInt32( m_floor );

                    MakeSpawnerVendor( types, x, y, floor, Map.Tokuno, Home, SpawnRange, amount, minTime, maxTime, team, direct );
                    break;
                default:
                    Console.WriteLine( "UOAM Parser: Warning, unknown map {0}", map );
                    break;
            }
        }

        public static int GetSpawnerZ( int x, int y, Map map )
        {
            int z = map.GetAverageZ( x, y );

            if( map.CanFit( x, y, z, 16, false, false, true ) )
                return z;

            for ( int i = 1; i <= 20; ++i )
            {
                if( map.CanFit( x, y, z + i, 16, false, false, true ) )
                    return z + i;

                if( map.CanFit( x, y, z - i, 16, false, false, true ) )
                    return z - i;
            }

            return z;
        }

        public static void ClearSpawners()
        {
            if( ClearSpawnEnable )
            {
                if( File.Exists( "Data/SpawnList.cfg" ) )
                {
                    using( StreamReader ip = new StreamReader( "Data/SpawnList.cfg" ) )
                    {
                        string line;

                        while( ( line = ip.ReadLine() ) != null )
                        {
                            try
                            {
                                Item item = World.FindItem( Convert.ToInt32( line.Trim() ) );
                                item.Delete();
                            }
                            catch
                            {
                                Console.WriteLine( "Spawn not found. Ignored..." );
                            }
                        }
                    }

                    File.Delete( "Data/SpawnList.cfg" );
                }
            }
            else
                Console.WriteLine( "The cleanspawn function is disabled!" );
        }

        //										 VendorType      x,		y,	  [Floor]	  map,	  [HomeDist]    [SpawnRange]	  [Amount]		[MinTime]		[MaxTime]		[Team]	       [Direction]
        private static void MakeSpawnerVendor( string[] types, int x, int y, int floor, Map map, int HomeDist, int SpawnRange, int Amount, TimeSpan MinTime, TimeSpan MaxTime, int Team, Direction direction ) //Vendors
        {
            if( types.Length == 0 )
                return;

            int z = floor;

            for ( int i = 0; i < types.Length; ++i )
            {
                bool isGuildmaster = ( types[ i ].EndsWith( "Guildmaster" ) );

                Spawner sp = new Spawner( types[ i ] );

                if( ClearSpawnEnable )
                {
                    using( StreamWriter ip = new StreamWriter( "Data/SpawnList.cfg", true ) )
                        ip.WriteLine( Convert.ToInt32( sp.Serial ) );
                }
                if( isGuildmaster )
                    sp.Count = 1;
                else
                    sp.Count = Amount;

                sp.MinDelay = MinTime;
                sp.MaxDelay = MaxTime;
                sp.Team = Team;
                sp.HomeRange = HomeRange;
                // sp.SpawnRange = SpawnRange;
                if( HomeDist <= 0 )
                    sp.Direction = direction;

                sp.MoveToWorld( new Point3D( x, y, z ), map );

                if( TotalRespawn )
                {
                    sp.Respawn();
                    //					sp.BringToHome();
                }

                ++m_Count;
            }
        }

        //										MonsterType      x,		y,	  [Floor]	  map,	  [HomeDist]    [SpawnRange]	[Amount]		[MinTime]		[MaxTime]		[Team]	       [Direction]
        private static void MakeSpawnerMonster( string[] types, int x, int y, int floor, Map map, int HomeDist, int SpawnRange, int Amount, TimeSpan MinTime, TimeSpan MaxTime, int Team, Direction direction ) //Monster
        {
            if( types.Length == 0 )
                return;

            int z = floor;

            Spawner sp = new Spawner();

            if( ClearSpawnEnable )
            {
                using( StreamWriter ip = new StreamWriter( "Data/SpawnList.cfg", true ) )
                    ip.WriteLine( Convert.ToInt32( sp.Serial ) );
            }

            sp.Count = Amount;

            sp.MinDelay = MinTime;
            sp.MaxDelay = MaxTime;
            sp.Team = Team;
            sp.HomeRange = HomeDist;
            // sp.SpawnRange = SpawnRange;
            if( HomeDist <= 0 )
                sp.Direction = direction;

            for ( int i = 0; i < types.Length; ++i )
            {
                if( types[ i ].CompareTo( "" ) != 0 )
                    sp.CreaturesName.Add( types[ i ] );
            }

            sp.MoveToWorld( new Point3D( x, y, z ), map );

            if( TotalRespawn )
            {
                sp.Respawn();
                //				sp.BringToHome();
            }

            ++m_Count;
        }

        //											ChestType      x,		y,	  [Floor]	  map,	  [HomeDist]    [SpawnRange]	[Amount]		[MinTime]		[MaxTime]		[Team]	       [Direction]
        private static void MakeSpawnerTreasure( string[] types, int x, int y, int floor, Map map, int HomeDist, int SpawnRange, int Amount, TimeSpan MinTime, TimeSpan MaxTime, int Team, Direction direction ) //Treaure Chest
        {
            if( types.Length == 0 )
                return;

            int z = floor;

            Spawner sp = new Spawner();

            if( ClearSpawnEnable )
            {
                using( StreamWriter ip = new StreamWriter( "Data/SpawnList.cfg", true ) )
                    ip.WriteLine( Convert.ToInt32( sp.Serial ) );
            }
            sp.Count = Amount;

            sp.MinDelay = MinTime;
            sp.MaxDelay = MaxTime;
            sp.Team = Team;
            sp.HomeRange = HomeDist;
            // sp.SpawnRange = SpawnRange;
            if( HomeDist <= 0 )
                sp.Direction = direction;

            for ( int i = 0; i < types.Length; ++i )
            {
                switch( types[ i ] )
                {
                    case "fake":
                        types[ i ] = "FakeTreasureChest";
                        break;
                    case "lev1":
                        types[ i ] = "TreasureChestLevel1";
                        break;
                    case "lev1hybrid":
                        types[ i ] = "TreasureChestLevel1hybrid";
                        break;
                    case "lev2":
                        types[ i ] = "TreasureChestLevel2";
                        break;
                    case "lev3":
                        types[ i ] = "TreasureChestLevel3";
                        break;
                    case "lev4":
                        types[ i ] = "TreasureChestLevel4";
                        break;
                }
                if( types[ i ].CompareTo( "" ) != 0 )
                    sp.CreaturesName.Add( types[ i ] );
            }

            sp.MoveToWorld( new Point3D( x, y, z ), map );

            if( TotalRespawn )
                sp.Respawn();

            ++m_Count;
        }

        private static void MakeCamp( string[] types, int x, int y, Map map ) //Camps
        {
            int z = GetSpawnerZ( x, y, map );

            Spawner sp = new Spawner();

            if( ClearSpawnEnable )
            {
                using( StreamWriter ip = new StreamWriter( "Data/SpawnList.cfg", true ) )
                    ip.WriteLine( Convert.ToInt32( sp.Serial ) );
            }
            sp.Count = 1;
            sp.MinDelay = TimeSpan.FromMinutes( 45.0 );
            sp.MaxDelay = TimeSpan.FromMinutes( 60.0 );
            sp.HomeRange = 0;

            for ( int i = 0; i < types.Length; ++i )
                sp.CreaturesName.Add( types[ i ] );

            ++m_Count;
            sp.MoveToWorld( new Point3D( x, y, z ), map );

            if( TotalRespawn )
                sp.Respawn();
        }
    }
}