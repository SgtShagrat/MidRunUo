/***************************************************************************
 *                               Core.cs
 *
 *   begin                : 19 settembre 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;

using Server;
using Server.Commands;
using Server.Engines.Harvest;
using Server.Items;
using Server.Mobiles;
using Server.Spells;
using Server.Targeting;

namespace Midgard.Engines.AdvancedFishing
{
    public static class Core
    {
        public static void LoadMidgardLocalization()
        {
            TextHelper.LoadLocalization( "AdvancedFishingSystem.cfg" );
        }

        internal static SpecialFishingPole GetRodEquipped( Mobile from, bool message )
        {
            Item item = from.FindItemOnLayer( Layer.OneHanded );

            if( item is SpecialFishingPole )
                return (SpecialFishingPole)item;
            else
            {
                if( message )
                    from.SendLangMessage( 10020001 ); // "You must equip the rod during fishing!!"
            }

            return null;
        }

        internal static bool CheckDistance( Mobile from, Point3D p, bool message )
        {
            if( from.GetDistanceToSqrt( p ) > Config.MaxRange )
            {
                from.SendLangMessage( 10020002 ); // "That's out of range."
                return false;
            }

            return true;
        }

        internal static void SpecialFishing_OnTarget( Mobile from, object targeted )
        {
            try
            {
                int tileID;
                Point3D p;

                if( targeted is StaticTarget )
                {
                    tileID = ( ( ( (StaticTarget)targeted ).ItemID & 0x3FFF ) | 0x4000 );
                    p = ( (StaticTarget)targeted ).Location;
                }
                else if( ( targeted is LandTarget ) )
                {
                    tileID = ( (LandTarget)targeted ).TileID;
                    p = ( (LandTarget)targeted ).Location;
                }
                else
                {
                    from.SendLangMessage( 10020003 ); // "You didn't choose a valid tile."
                    return;
                }

                SpecialFishingPole pole = GetRodEquipped( from, true );

                if( pole == null )
                {
                }
                else if( !CheckDistance( from, p, true ) )
                {
                }
                else if( !IsWater( tileID ) )
                {
                    from.SendLangMessage( 10020004 ); // "You didn't choose a valid tile."
                }
                else if( CheckDeepWater( from.Location, from.Map ) && from.Skills[ SkillName.Fishing ].Value < Config.MinSkillToFishDeepWater )
                {
                    from.SendLangMessage( 10020005, Config.MinSkillToFishDeepWater.ToString( "F2" ) ); // "You must have {0} in fishing to work on deed waters!",
                }
                else
                {
                    Map map = from.Map;
                    SpellHelper.Turn( from, p );
                    from.Animate( 12, 5, 1, true, false, 0 );
                    Timer.DelayCall( TimeSpan.FromSeconds( 0.9 ), delegate
                                                                      {
                                                                          Effects.SendLocationEffect( p, map, 0x352D, 16, 4 );
                                                                          Effects.PlaySound( p, map, 0x364 );
                                                                      } );


                    from.CloseGump( typeof( FishingGump ) );
                    from.SendGump( new FishingGump( pole, new FishingGumpCallback( Fishing_Callback ) ) );

                    pole.StartTimer( from, p, tileID );
                }
            }
            catch( Exception ex )
            {
                Config.Pkg.LogErrorLine( ex.ToString() );
            }
        }

        internal static void InitFishContest( Mobile from, bool deep, out int fishStr, out int fishSize )
        {
            double randomSkillValue = ( from.Skills[ SkillName.Fishing ].Value / 100.0 ) * Utility.RandomDouble();
            double waterScalar = CheckDeepWater( from.Location, from.Map ) ? Config.DeepWaterScalar : Config.NotDeepWaterScalar;

            fishSize = (int)( Config.MaxFishSize * randomSkillValue * waterScalar );
            if( fishSize <= 500 )
                fishSize = 500 + Utility.Random( 100 );

            fishStr = (int)( fishSize / 1500.0 );
            if( fishStr <= 3 )
                fishStr += 3 + DiceRoll.OneDiceThree.Roll();

            if( fishSize > 25000 && Utility.RandomDouble() < 0.2 )
            {
                fishSize = 33000 + Utility.Random( 8000 );
                fishStr = ( 25000 + Utility.Random( 4000 ) ) / 1000;
            }
        }

        internal static void SendDebugMessage( Mobile from, string message )
        {
            if( from.PlayerDebug )
                from.SendMessage( (int)MessageHues.Yellow, message );
        }

        public static void SendDebugMessage( Mobile from, string format, params object[] args )
        {
            SendDebugMessage( from, String.Format( format, args ) );
        }

        private static void Fishing_Callback( Mobile from, Actions action, SpecialFishingPole pole )
        {
            if( !from.CheckAlive() )
            {
                pole.Reset( from );
                return;
            }

            pole.FisherAction = action;
            pole.LastAction = DateTime.Now;

            SendDebugMessage( from, "Action {0} selected.", action.ToString().ToLower() );

            from.Animate( 12, 5, 1, true, false, 0 );
            from.CloseGump( typeof( FishingGump ) );
            from.SendGump( new FishingGump( pole, new FishingGumpCallback( Fishing_Callback ) ) );
        }

        /// <summary>
        /// Return true if player moved
        /// </summary>
        internal static bool CheckLocation( Mobile from, Point3D startPoint )
        {
            if( from.AccessLevel > AccessLevel.Player )
                return false;

            return ( startPoint.X != from.X ) || ( startPoint.Y != from.Y ) || ( startPoint.Z != from.Z );
        }

        /// <summary>
        /// Return true if context must end in any case
        /// </summary>
        internal static bool CheckIsUnluckyContest( Mobile from )
        {
            if( from.AccessLevel > AccessLevel.Player )
                return true;

            return Utility.RandomDouble() > Config.UnluckyChance;
        }
        internal static bool CheckDeepWater( Point3D startLocation, Map map )
        {
            const int range = Config.CrossSize;

            if( !IsWater( startLocation.X, startLocation.Y + range, map ) )
                return false;
            if( !IsWater( startLocation.X + range, startLocation.Y, map ) )
                return false;
            if( !IsWater( startLocation.X, startLocation.Y - range, map ) )
                return false;
            if( !IsWater( startLocation.X - range, startLocation.Y, map ) )
                return false;

            return true;
        }

        internal static bool IsWater( int tileID )
        {
            return Fishing.System.Definition.Validate( tileID );
        }

        internal static bool IsWater( int x, int y, Map map )
        {
            Point2D p = new Point2D( x, y );
            List<Tile> tiles = map.GetTilesAt( p, false, true, false );

            foreach( Tile curTile in tiles )
            {
                if( IsWater( curTile.ID ) )
                    return true;
            }

            return false;
        }

        internal static BaseAdvancedFish CreateFish( Mobile from, int weight )
        {
            SendDebugMessage( from, string.Format( "Trying to create a fish of weight: {0} gr.", weight ) );

            foreach( FishGroupDefinition definition in m_FishGroupDefinitions )
            {
                if( weight >= definition.MinSize && weight <= definition.MaxSize )
                {
                    SendDebugMessage( from, "Find definition: {0} (min {1} - max {2})", definition.Name,
                                      definition.MinSize, definition.MaxSize );
                    BaseAdvancedFish fish = definition.Construct( weight );
                    return fish;
                }
            }

            SendDebugMessage( from, "NOT found any valid definition!" );
            return new PiperFish( Utility.RandomMinMax( 100, 600 ) );
        }

        internal static bool CheckReflex( Mobile from, SpecialFishingPole pole, double difficulty )
        {
            bool isFromGM = from.AccessLevel > AccessLevel.Counselor;

            TimeSpan delay = pole.LastAction - pole.LastFishAction;

            using( StreamWriter op = new StreamWriter( "fishing.log", true ) )
                op.WriteLine( "ref: {0}, {1}", CommandLogging.Format( from ), delay );

            if( isFromGM )
                return true;

            if( delay < TimeSpan.Zero )
                return false;

            return delay <= TimeSpan.FromSeconds( difficulty );
        }

        #region fishing event messaging
        /// <summary>
        /// Ita	10020020	Il pesce combatte!
        /// </summary>
        public static int RandomWarMex()
        {
            return Utility.RandomMinMax( 10020020, 10020020 );
        }

        /// <summary>
        /// Ita	10020030	Il pesce si inabissa!
        /// Ita	10020031	Il  pesce  si  inabissa!
        /// Ita	10020032	Il pesce ti viene incontro!
        /// Ita	10020033	Sale in superficie!
        /// Ita	10020034	Sale  in  superficie!
        /// Ita	10020035	E' al pelo dell'acqua!
        /// Ita	10020036	Mostra segni di stanchezza!
        /// Ita	10020037	Il pesce non combatte!
        /// Ita	10020038	Sta cedendo!
        /// Ita	10020039	Il pesce punta al fondo! <----------TODO
        /// </summary>
        public static int RandomDownMex()
        {
            return Utility.RandomMinMax( 10020030, 10020039 );
        }

        /// <summary>
        /// Ita	10020040	Il pesce salta!
        /// Ita	10020041	Il pesce  salta!
        /// Ita	10020042	Il pesce Tira con molta forza!
        /// Ita	10020043	Combatte con troppa forza!
        /// Ita	10020044	Non vuole cedere!
        /// Ita	10020045	Il pesce strattona con forza!
        /// </summary>
        public static int RandomJumpMex()
        {
            return Utility.RandomMinMax( 10020040, 10020045 );
        }

        /// <summary>
        /// Ita	10020050	Per un pelo! Stava per scappare.
        /// Ita	10020051	Ottima Mossa!
        /// Ita	10020052	Combattera' ancora molto!
        /// Ita	10020053	Vendera' cara la sua pelle!
        /// Ita	10020054	Ben Fatto!
        /// Ita	10020055	Deve essere grosso! 
        /// </summary>
        public static int RandomGoodMex()
        {
            return Utility.RandomMinMax( 10020050, 10020055 );
        }

        /// <summary>
        /// Ita	10020060	L'hai fatto scappare.
        /// Ita	10020061	Pessima Mossa!
        /// Ita	10020062	Era un gran bel pesce!
        /// Ita	10020063	Accidenti!Si e' sgaciato.
        /// Ita	10020064	Non e' la tua giornata.
        /// Ita	10020065	Dovevi pensarci meglio. 
        /// </summary>
        public static int RandomWrongMoveMessage()
        {
            return Utility.RandomMinMax( 10020060, 10020065 );
        }

        /// <summary>
        /// Ita	10020070	Buona notte!
        /// Ita	10020071	Pessimi riflessi.
        /// Ita	10020072	Chi Dorme non piglia pesci!
        /// Ita	10020073	Serve maggior attenzione!
        /// Ita	10020074	Il pesce e' stato piu' lesto di te.
        /// Ita	10020075	Tardi! Il pesce e' scappato.
        /// </summary>
        public static int RandomBadReflexMessage()
        {
            return Utility.RandomMinMax( 10020070, 10020075 );
        }

        /// <summary>
        /// Ita	10020080	Ottima mossa!
        /// Ita	10020081	Ottimi riflessi.
        /// Ita	10020082	Questo pesce non ha scampo!
        /// Ita	10020083	Perfetto!
        /// Ita	10020084	Sara' una dura battaglia!
        /// Ita	10020085	Che tempismo!
        /// Ita	10020086	Non ha scampo! 
        /// </summary>
        public static int RandomGoodReflexMessage()
        {
            return Utility.RandomMinMax( 10020080, 10020086 );
        }

        /// <summary>
        /// Ita	10020090	Accidenti!Si deve essere rotto l'amo.
        /// Ita	10020091	Accidenti!Si e' sganciato.
        /// Ita	10020092	Il pesce si e' liberato!
        /// Ita	10020093	Il pesce, e' stato piu' forte.
        /// Ita	10020094	Era troppo grosso! Si e' liberato.
        /// Ita	10020095	Il filo della lenza ha ceduto!
        /// Ita	10020096	Che sfortuna...e' scappato.
        /// </summary>
        public static int RandomUnluckyMessage()
        {
            return Utility.RandomMinMax( 10020090, 10020096 );
        }
        #endregion

        #region fish tables
        private static readonly Type[] m_VerySmallFishes = new Type[]
                                                               {
                                                                   typeof(PiperFish),
                                                                   typeof(BleakFish),
                                                                   typeof(TropicalFish),
                                                                   typeof(TenchFish),
                                                                   typeof(GranchioFish),
                                                                   typeof(StandardSmallFishesOne),
                                                                   typeof(StandardSmallFishesTwo),
                                                                   typeof(StandardSmallFishesThree),
                                                                   typeof(StandardSmallFishesFour)
                                                               };

        private static readonly Type[] m_SmallFishes = new Type[]
                                                           {
                                                               typeof(PiperFish),
                                                               typeof(SalmonFish),
                                                               typeof(TropicalFish),
                                                               typeof(TenchFish),
                                                               typeof(GranchioFish),
                                                               typeof(ChubFish),
                                                               typeof(StandardSmallFishesOne),
                                                               typeof(StandardSmallFishesTwo)
                                                           };

        private static readonly Type[] m_MediuamFishes = new Type[]
                                                             {
                                                                 typeof(SalmonFish),
                                                                 typeof(TropicalFish),
                                                                 typeof(TenchFish),
                                                                 typeof(CatFish),
                                                                 typeof(SmallMouthFish),
                                                                 typeof(ChubFish),
                                                                 typeof(MantaFish),
                                                                 typeof(CarpFish),
                                                                 typeof(StandardSmallFishesThree),
                                                                 typeof(StandardFishFour)
                                                             };

        private static readonly Type[] m_LargeFishes = new Type[]
                                                           {
                                                               typeof(SalmonFish),
                                                               typeof(TropicalFish),
                                                               typeof(TenchFish),
                                                               typeof(CatFish),
                                                               typeof(SmallMouthFish),
                                                               typeof(ChubFish),
                                                               typeof(MantaFish),
                                                               typeof(CarpFish)
                                                           };

        private static readonly Type[] m_VeryLargeFishes = new Type[]
                                                               {
                                                                   typeof(SalmonFish),
                                                                   typeof(CatFish),
                                                                   typeof(SmallMouthFish),
                                                                   typeof(MantaFish),
                                                                   typeof(CarpFish)
                                                               };

        private static readonly Type[] m_UniqueFishes = new Type[]
                                                            {
                                                                typeof(MarlinFish)
                                                            };

        private static readonly FishGroupDefinition[] m_FishGroupDefinitions = new FishGroupDefinition[]
                                                                         {
                                                                             new FishGroupDefinition( "very small", -1, 600, m_VerySmallFishes ),
                                                                             new FishGroupDefinition( "small", 601, 1500, m_SmallFishes ), 
                                                                             new FishGroupDefinition( "medium", 1501, 7000, m_MediuamFishes ),
                                                                             new FishGroupDefinition( "large", 7001, 15000, m_LargeFishes ), 
                                                                             new FishGroupDefinition( "very large", 15001, 30000, m_VeryLargeFishes ),
                                                                             new FishGroupDefinition( "unique", 30001, 70000, m_UniqueFishes )
                                                                         };
        #endregion

        internal class FishGroupDefinition
        {
            public Type[] Types { get; set; }
            public int MinSize { get; set; }
            public int MaxSize { get; set; }
            public string Name { get; set; }

            public FishGroupDefinition( string name, int minSize, int maxSize, Type[] types )
            {
                Name = name;
                MinSize = minSize;
                MaxSize = maxSize;
                Types = types;
            }

            public BaseAdvancedFish Construct( int weight )
            {
                Type t = Types[ Utility.Random( Types.Length ) ];
                return (BaseAdvancedFish)Activator.CreateInstance( t, weight );
            }
        }

        #region special rewards
        private class MutateEntry
        {
            public double ReqSkill { get; private set; }
            public double MinSkill { get; private set; }
            public double MaxSkill { get; private set; }
            public bool DeepWater { get; private set; }
            public Type[] Types { get; private set; }

            public MutateEntry( double reqSkill, double minSkill, double maxSkill, bool deepWater, params Type[] types )
            {
                ReqSkill = reqSkill;
                MinSkill = minSkill;
                MaxSkill = maxSkill;
                DeepWater = deepWater;
                Types = types;
            }
        }

        private static readonly MutateEntry[] m_MutateTable = new MutateEntry[]
			{
                //                req    min     max     deep   types
				new MutateEntry(  80.0,  80.0,  4080.0,  true, typeof( SpecialFishingNet ) ),
				new MutateEntry(  80.0,  80.0,  4080.0,  true, typeof( BigFish ) ),
				new MutateEntry(  90.0,  80.0,  4080.0,  true, typeof( TreasureMap ) ),
				new MutateEntry( 100.0,  80.0,  4080.0,  true, typeof( MessageInABottle ) ),
				new MutateEntry(   0.0, 125.0, -2375.0, false, typeof( PrizedFish ), typeof( WondrousFish ), typeof( TrulyRareFish ), typeof( PeculiarFish ) ),
				new MutateEntry(   0.0, 105.0,  -420.0, false, typeof( Boots ), typeof( Shoes ), typeof( Sandals ), typeof( ThighBoots ) ),
				new MutateEntry(   0.0, 200.0,  -200.0, false, new Type[]{ null } )
			};

        internal static Item Mutate( Item item, Mobile from, Item tool, Map map, Point3D loc )
        {
            bool deepWater = CheckDeepWater( loc, map );

            double skillBase = from.Skills[ SkillName.Fishing ].Base;
            double skillValue = from.Skills[ SkillName.Fishing ].Value;

            foreach( MutateEntry entry in m_MutateTable )
            {
                if( !deepWater && entry.DeepWater )
                    continue;

                if( skillBase < entry.ReqSkill )
                    continue;

                double chance = ( skillValue - entry.MinSkill ) / ( entry.MaxSkill - entry.MinSkill );

                if( chance > Utility.RandomDouble() )
                {
                    Item mutated = ConstructByType( entry.Types[ Utility.Random( entry.Types.Length ) ] );
                    if( mutated != null )
                    {
                        item.Delete();
                        return mutated;
                    }
                    else
                        return item;
                }
            }

            return item;
        }

        private static Item ConstructByType( Type type )
        {
            try { return Activator.CreateInstance( type ) as Item; }
            catch { return null; }
        }

        private static Item GetRandomCommonItem()
        {
            Item preLoot = null;

            switch( Utility.Random( 6 ) )
            {
                case 0: // Body parts
                    {
                        int[] list = new int[]
									{
										0x1CDD, 0x1CE5, // arm
										0x1CE0, 0x1CE8, // torso
										0x1CE1, 0x1CE9, // head
										0x1CE2, 0x1CEC // leg
									};

                        preLoot = new ShipwreckedItem( Utility.RandomList( list ) );
                        break;
                    }
                case 1: // Bone parts
                    {
                        int[] list = new int[]
									{
										0x1AE0, 0x1AE1, 0x1AE2, 0x1AE3, 0x1AE4, // skulls
										0x1B09, 0x1B0A, 0x1B0B, 0x1B0C, 0x1B0D, 0x1B0E, 0x1B0F, 0x1B10, // bone piles
										0x1B15, 0x1B16 // pelvis bones
									};

                        preLoot = new ShipwreckedItem( Utility.RandomList( list ) );
                        break;
                    }
                case 2: // Paintings and portraits
                    {
                        preLoot = new ShipwreckedItem( Utility.Random( 0xE9F, 10 ) );
                        break;
                    }
                case 3: // Pillows
                    {
                        preLoot = new ShipwreckedItem( Utility.Random( 0x13A4, 11 ) );
                        break;
                    }
                case 4: // Shells
                    {
                        preLoot = new ShipwreckedItem( Utility.Random( 0xFC4, 9 ) );
                        break;
                    }
                case 5:	//Hats
                    {
                        preLoot = Utility.RandomBool() ? (Item)new SkullCap() : new TricorneHat();

                        break;
                    }
                case 6: // Misc
                    {
                        int[] list = new int[]
									{
										0x1EB5, // unfinished barrel
										0xA2A, // stool
										0xC1F, // broken clock
										0x1047, 0x1048, // globe
										0x1EB1, 0x1EB2, 0x1EB3, 0x1EB4 // barrel staves
									};

                        if( Utility.Random( list.Length + 1 ) == 0 )
                            preLoot = new Candelabra();
                        else
                            preLoot = new ShipwreckedItem( Utility.RandomList( list ) );

                        break;
                    }
            }

            return preLoot;
        }

        private static LockableContainer GetRandomChest( ref SOS sos )
        {
            LockableContainer chest = Utility.RandomBool() ? (LockableContainer)new MetalGoldenChest() : new WoodenChest();
            if( sos.IsAncient )
                chest.Hue = 0x481;

            TreasureMapChest.Fill( chest, Math.Max( 1, Math.Max( 4, sos.Level ) ) );

            chest.DropItem( sos.IsAncient ? new FabledFishingNet() : new SpecialFishingNet() );
            chest.Movable = true;
            chest.Locked = false;
            chest.TrapType = TrapType.None;
            chest.TrapPower = 0;
            chest.TrapLevel = 0;

            sos.Delete();

            return chest;
        }

        public static Item Construct( Type type, Mobile from )
        {
            if( type == typeof( TreasureMap ) )
            {
                return new TreasureMap( 1, Map.Felucca );
            }
            else if( type == typeof( MessageInABottle ) )
            {
                return new MessageInABottle( Map.Felucca );
            }
            else
            {
                Container pack = from.Backpack;
                if( pack != null )
                {
                    List<SOS> messages = pack.FindItemsByType<SOS>();

                    for( int i = 0; i < messages.Count; ++i )
                    {
                        SOS sos = messages[ i ];

                        if( ( from.Map == Map.Felucca ) && from.InRange( sos.TargetLocation, 60 ) )
                        {
                            Item preLoot = GetRandomCommonItem();
                            if( preLoot != null )
                            {
                                ( (IShipwreckedItem)preLoot ).IsShipwreckedItem = true;
                                return preLoot;
                            }

                            LockableContainer chest = GetRandomChest( ref sos );
                            if( chest != null )
                                return chest;
                        }
                    }
                }
            }

            return ConstructByType( type );
        }

        private static void SpawnSerpent( Mobile m, Item item )
        {
            BaseCreature serp = 0.25 > Utility.RandomDouble() ? (BaseCreature)new DeepSeaSerpent() : new SeaSerpent();

            int x = m.X, y = m.Y;

            Map map = m.Map;

            for( int i = 0; map != null && i < 20; ++i )
            {
                int tx = m.X - 10 + Utility.Random( 21 );
                int ty = m.Y - 10 + Utility.Random( 21 );

                Tile t = map.Tiles.GetLandTile( tx, ty );

                if( t.Z != -5 || ( ( t.ID < 0xA8 || t.ID > 0xAB ) && ( t.ID < 0x136 || t.ID > 0x137 ) ) ||
                         SpellHelper.CheckMulti( new Point3D( tx, ty, -5 ), map ) )
                    continue;
                x = tx;
                y = ty;
                break;
            }

            serp.MoveToWorld( new Point3D( x, y, -5 ), map );

            serp.Home = serp.Location;
            serp.RangeHome = 10;

            serp.PackItem( item );
        }

        public static bool Give( Mobile m, Item item, bool placeAtFeet )
        {
            if( item is TreasureMap || item is MessageInABottle || item is SpecialFishingNet )
            {
                SpawnSerpent( m, item );
                m.SendLocalizedMessage( 503170 ); // Uh oh! That doesn't look like a fish!

                return true; // we don't want to give the item to the player, it's on the serpent
            }

            if( item is BigFish || item is WoodenChest || item is MetalGoldenChest )
                placeAtFeet = true;

            if( m.PlaceInBackpack( item ) )
                return true;

            if( !placeAtFeet )
                return false;

            List<Item> atFeet = new List<Item>();

            foreach( Item obj in m.GetItemsInRange( 0 ) )
                atFeet.Add( obj );

            foreach( Item check in atFeet )
            {
                if( check.StackWith( m, item, false ) )
                    return true;
            }

            item.MoveToWorld( m.Location, m.Map );
            return true;
        }

        public static void SendSuccessTo( Mobile from, Item item )
        {
            if( item is BaseAdvancedFish )
            {
                BaseAdvancedFish fish = (BaseAdvancedFish)item;
                string text = String.Format( TextHelper.Text( 10020007, from.TrueLanguage ), fish.Name ); // "*You fished a {0}!*"
                from.PublicOverheadMessage( Server.Network.MessageType.Regular, Config.HueGood, true, text );
            }
            else if( item is WoodenChest || item is MetalGoldenChest )
            {
                from.SendLocalizedMessage( 503175 ); // You pull up a heavy chest from the depths of the ocean!
            }
            else
            {
                int number;
                string name;

                if( item is BaseMagicFish )
                {
                    number = 1008124;
                    name = "a mess of small fish";
                }
                else if( item is Fish )
                {
                    number = 1008124;
                    name = "a fish";
                }
                else if( item is BaseShoes )
                {
                    number = 1008124;
                    name = item.ItemData.Name;
                }
                else if( item is TreasureMap )
                {
                    number = 1008125;
                    name = "a sodden piece of parchment";
                }
                else if( item is MessageInABottle )
                {
                    number = 1008125;
                    name = "a bottle, with a message in it";
                }
                else if( item is SpecialFishingNet )
                {
                    number = 1008125;
                    name = "a special fishing net";
                }
                else
                {
                    number = 1043297;

                    if( ( item.ItemData.Flags & TileFlag.ArticleA ) != 0 )
                        name = "a " + item.ItemData.Name;
                    else if( ( item.ItemData.Flags & TileFlag.ArticleAn ) != 0 )
                        name = "an " + item.ItemData.Name;
                    else
                        name = item.ItemData.Name;
                }

                if( number == 1043297 )
                    from.SendLocalizedMessage( number, name );
                else
                    from.SendLocalizedMessage( number, true, name );
            }
        }
        #endregion
    }
}