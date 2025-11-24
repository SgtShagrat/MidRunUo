/***************************************************************************
 *                               Config.cs
 *
 *   begin                : 13 giugno 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;

using Midgard.Engines.MiniChampionSystem;
using Midgard.Engines.Packager;

using Server;
using Server.Commands;

namespace Midgard.Engines.Classes.VirtueChampion
{
    public class Config
    {
        internal static Package Pkg;

        public static bool Enabled
        {
            get { return Packager.Core.Singleton[ typeof( Config ) ].Enabled; }
            set { Packager.Core.Singleton[ typeof( Config ) ].Enabled = value; }
        }

        internal static bool Debug = false;

        public static object[] Package_Info = {
            "Script Title",             "Virtue Champion System",
            "Enabled by Default",       true,
            "Script Version",           new Version(1,0,0,0),
            "Author name",              "Dies Irae", 
            "Creation Date",            new DateTime(2011, 06, 13), 
            "Author mail-contact",      "tocasia@alice.it", 
            "Author home site",         "http://www.midgardshard.it",
            //"Author notes",           null,
            "Script Copyrights",        "(C) Midgard Shard - Dies Irae",
            "Provided packages",        new string[]{"Midgard.Engines.Classes.VirtueChampion"},
            //"Required packages",      new string[0],
            //"Conflicts with packages",new string[0],
            "Research tags",            new string[]{"Virtue Champion"}
        };

        public static void Package_Configure()
        {
            Pkg = Packager.Core.Singleton[ typeof( Config ) ];
        }

        public static void Package_Initialize()
        {
            CommandSystem.Register( "GenerateVirtueChampion", AccessLevel.Administrator, new CommandEventHandler( Commands.GenerateChampion_OnCommand ) );
        }

        public const int ChampionSpawnID = 0x20F8;

        public const int SpawnerSpeechRange = 5;

        public static readonly BaseVirtue Honesty = new HonestyVirtue();
        public static readonly BaseVirtue Compassion = new CompassionVirtue();
        public static readonly BaseVirtue Valor = new ValorVirtue();
        public static readonly BaseVirtue Justice = new JusticeVirtue();
        public static readonly BaseVirtue Sacrifice = new SacrificeVirtue();
        public static readonly BaseVirtue Honor = new HonorVirtue();
        public static readonly BaseVirtue Spirituality = new SpiritualityVirtue();
        public static readonly BaseVirtue Humility = new HumilityVirtue();

        public static BaseVirtue[] Virtues = new BaseVirtue[]
                                                 {
                                                     Honesty, Compassion, Valor, Justice, Sacrifice, Honor, Spirituality, Humility
                                                 };

        public static readonly BaseAntiVirtue Deceit = new DeceitAntiVirtue();
        public static readonly BaseAntiVirtue Despise = new DespiseAntiVirtue();
        public static readonly BaseAntiVirtue Destard = new DestardAntiVirtue();
        public static readonly BaseAntiVirtue Wrong = new WrongAntiVirtue();
        public static readonly BaseAntiVirtue Covetous = new CovetousAntiVirtue();
        public static readonly BaseAntiVirtue Shame = new ShameAntiVirtue();
        public static readonly BaseAntiVirtue Hythloth = new HythlothAntiVirtue();
        public static readonly BaseAntiVirtue Pride = new PrideAntiVirtue();

        public static BaseAntiVirtue[] AntiVirtues = new BaseAntiVirtue[]
                                                         {
                                                             Deceit, Despise, Destard, Wrong, Covetous, Shame, Hythloth, Pride
                                                         };

        public static string[] CorruptedHowls = new string[]
                                                    {
                                                        "You will die today, fool!",
                                                        "You have no chance to purify tour soul!",
                                                        "You cannot redeem my soul!",
                                                        "Your soul will be shattered!"
                                                    };

        public static string[] RedemptionHowls = new string[]
                                                    {
                                                        "Find your path and free my soul!",
                                                        "Do not understimate your soul power!",
                                                        "Feel the power of virtues!",
                                                        "Free my soul, mighty warrior!"
                                                    };

        internal static readonly ChampionSpawnGroup[] DefaultCorruptionSpawnInfo = new ChampionSpawnGroup[]
                                                                {
                                                                    new ChampionSpawnGroup( 
                                                                        new SpawnGroupElement[]
                                                                        {
                                                                            new SpawnGroupElement( typeof( CorruptedBogle ), 6, 40 ),
                                                                            new SpawnGroupElement( typeof( CorruptedSpectre ), 3, 20 )
                                                                        }, 100 ),
                                                                    new ChampionSpawnGroup( 
                                                                        new SpawnGroupElement[]
                                                                        {
                                                                            new SpawnGroupElement( typeof( CorruptedBogle ), 3, 30 ),
                                                                            new SpawnGroupElement( typeof( CorruptedSpectre ), 6, 30 )
                                                                        }, 50 )
                                                                };

        public static ChampionSpawnGroup[] DefaultRedeemSpawnInfo = new ChampionSpawnGroup[]
                                                                {
                                                                    new ChampionSpawnGroup( 
                                                                        new SpawnGroupElement[]
                                                                        {
                                                                            new SpawnGroupElement( typeof( RedeemedShade ), 6, 36 ),
                                                                            new SpawnGroupElement( typeof( RedeemedWraith ), 3, 12 ),
                                                                            new SpawnGroupElement( typeof( RedeemedSpectre ), 1, 4 )
                                                                        }, 36 ),
                                                                   new ChampionSpawnGroup( 
                                                                        new SpawnGroupElement[]
                                                                        {
                                                                            new SpawnGroupElement( typeof( RedeemedShade ), 3, 8 ),
                                                                            new SpawnGroupElement( typeof( RedeemedWraith ), 6, 24 ),
                                                                            new SpawnGroupElement( typeof( RedeemedSpectre ), 1, 1 )
                                                                        }, 24 ),
                                                                   new ChampionSpawnGroup( 
                                                                        new SpawnGroupElement[]
                                                                        {
                                                                            new SpawnGroupElement( typeof( RedeemedShade ), 1, 1 ),
                                                                            new SpawnGroupElement( typeof( RedeemedWraith ), 3, 4 ),
                                                                            new SpawnGroupElement( typeof( RedeemedSpectre ), 6, 12 )
                                                                        }, 12 ),
                                                                };
    }

    public abstract class BasePrinciple
    {
    }

    public abstract class BaseVirtue : BasePrinciple
    {
        public VirtueDefinition Definition { get; protected set; }
    }

    public abstract class BaseAntiVirtue : BasePrinciple
    {
        public AntiVirtueDefinition Definition { get; protected set; }
    }
}